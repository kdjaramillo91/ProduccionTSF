using AccesoDatos.MSSQL;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceProviderDTO;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;
using DXPANACEASOFT.Auxiliares;

namespace DXPANACEASOFT.Controllers
{
	public class AdvanceProviderController : DefaultController
	{
		[HttpPost]
		public ActionResult Index()
		{
			return View();
		}

		#region"ADVANCE PROVIDER FILTER RESULTS"
		/// <summary>
		/// Obtiene toda la lista de Lotes de Producción que
		/// cumple con las siguientes características:
		///     -> Control de Calidad.- Cantidad Libras Recibidas mayor a 1500 (Parámetro 'NMLP')
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult APProductionLotResults()
		{
			//List<QualityControl> qcTmp = db.QualityControl
			//    .GroupBy(gb => gb.id_lot)
			//    .Where(w => w.Sum(_ => _.temperature) >= 1500 && w.Where(wv => wv.Lot.hasAdvanceProvider != true));


			return null;
		}

		/// <summary>
		/// Obtiene la lista de todos los Anticipos a Proveedor
		/// </summary>
		/// <returns></returns>
		public ActionResult APAdvanceProviderResults(AdvanceProvider advanceProvider, ProductionLot productionLot,

                                                 Document document,
												 DateTime? startEmissionDate, DateTime? endEmissionDate,
												 DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
												 int? id_provider,
												 int[] items, int? id_logicalOperator)
		{
			List<AdvanceProvider> model = db.AdvanceProvider.AsEnumerable().ToList();
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
            if (!string.IsNullOrEmpty(productionLot.internalNumber))
            {
                model = model.Where(o => o.Lot.ProductionLot.internalNumber.Contains(productionLot.internalNumber)).ToList();
            }

            #endregion

            #region PURCHASE ORDER FILTERS

            if (id_provider != 0 && id_provider != null)
			{
				model = model.Where(o => o.id_provider == id_provider).ToList();
			}

			if (items != null && items.Length > 0)
			{

			}
			#endregion

			TempData["model"] = model;
			TempData.Keep("model");

			return PartialView("_AdvanceProviderResultsPartial", model.OrderByDescending(o => o.id).ToList());
		}
		public ActionResult APAdvanceProviderResultsDirecto(Document document, ProductionLot productionLot)
		{
			List<AdvanceProvider> model = db.AdvanceProvider.AsEnumerable().ToList();

			//if (!string.IsNullOrEmpty(document.number))
			//{
			//	model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
			//}
			if (!string.IsNullOrEmpty(productionLot.internalNumber))
			{
				model = model.Where(o => o.Lot.ProductionLot.internalNumber.Contains(productionLot.internalNumber)).ToList();
			}
			int id = 0;
			if (model != null && model.Count > 0) id = model[0].id;
			//try
			//{
			//if (model.Count >= 0)
			//{
			//	throw new Exception("Ingrese Número de documento para consultar..");
			//}

			if (id > 0)
			{

				AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id == id);
				//try
				//{
				//	if (model.Count > 1)
				//	{
				//		throw new Exception("No tiene Permiso para Anular.");
				//	}
				PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList);
				List<int?> listPLDQC = null;
				if (pl != null)
				{
					advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																				pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																				(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");
				}


				int iCont;
				//Listo los Detalle de Recepción a los que pertenece cada Control de Calidad
				listPLDQC = db.ProductionLotDetailQualityControl
							.Where(w => w.IsDelete == false
										&& w.ProductionLotDetail.ProductionLot.id == advanceProvider.id_Lot 
										&& w.QualityControl.Document.DocumentState.code != "05")
							.Select(s => s.id_productionLotDetail)
							.ToList();
				iCont = 1;
				if (advanceProvider != null)
				{
					advanceProvider.aplRg = new List<AdvanceProviderPLRG>();
					if (listPLDQC != null)
					{
						foreach (var i in listPLDQC)
						{
							if (i != null)
							{
								var _pld = db.ProductionLotDetailPurchaseDetail
											.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
											.ProductionLotDetail;
								var _pod = db.ProductionLotDetailPurchaseDetail
												.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
												.PurchaseOrderDetail;
								var _rgd = db.ProductionLotDetailPurchaseDetail
												.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
												.RemissionGuideDetail;
								var _qTmp = db.ProductionLotDetailQualityControl
														.FirstOrDefault(fod => fod.IsDelete == false
																			    && fod.id_productionLotDetail == i)?.QualityControl;
								AdvanceProviderPLRG aplrgNew = new AdvanceProviderPLRG();
								aplrgNew.id = iCont;
								aplrgNew.seq_RemissionGuide = _rgd?.RemissionGuide?.Document?.sequential ?? 0;
								aplrgNew.OCnumber = _pod?.PurchaseOrder?.Document?.number ?? "";
								aplrgNew.RGnumber = _rgd?.RemissionGuide?.Document?.number ?? "";
								aplrgNew.Pnumber = _pld?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
								aplrgNew.QuantityPoundsReceived = _pld?.quantityRecived ?? 0;
								aplrgNew.QuantityPoundsScurrid = _pld?.quantitydrained ?? 0;
								aplrgNew.sGrammage = _qTmp?.grammageReference ?? 0;
								aplrgNew.Performance = _qTmp?.wholePerformance ?? 0;
								iCont++;
								advanceProvider.aplRg.Add(aplrgNew);
							}
						}
					}
					TempData["aplRg"] = advanceProvider.aplRg;
					TempData.Keep("aplRg");
				}

				TempData["AdvanceProviderPL"] = advanceProvider;
				TempData.Keep("AdvanceProviderPL");
				return PartialView("_FormEditAdvanceProviderPL", advanceProvider);

			}
			else
			{
				return View();
			}
			//}
			//catch (Exception e)
			//{
			//	ViewData["EditMessage"] = ErrorMessage(e.Message);
			//	//return Content(ViewData["EditMessage"].ToString());
			//	//return View();
			//	advanceProviderTemp.id = 0;
			//	return PartialView("_FormEditAdvanceProviderPL", advanceProviderTemp);
			//}
		}

		/// <summary>
		/// Obtiene la lista de todos los Anticipos a Proveedor
		/// </summary>
		/// <returns></returns>
		public ActionResult APAdvanceProviderPLResults(AdvanceProvider advanceProvider,
											 Document document,
											 DateTime? startEmissionDate, DateTime? endEmissionDate,
											 DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
											 int[] items, int? id_logicalOperator)
		{
			var model = GetAdvanceProviderPLs();

			#region DOCUMENT FILTERS


			#endregion

			#region PURCHASE ORDER FILTERS

			#endregion

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");

			return PartialView("_AdvanceProviderProductionLotResultsPartial", model.OrderByDescending(o => o.id).ToList());
		}

		public ActionResult APAdvanceProviderPLResultsDirecto(ProductionLot productionLot)
		{
			var model = GetAdvanceProviderPLs();

			if (!string.IsNullOrEmpty(productionLot.internalNumber))
			{
				model = model.Where(o => o.internalNumber.Contains(productionLot.internalNumber)).ToList();
			}
			int id = 0;
			var datos = model.ToList();
			if (datos != null && datos.Count > 0) id = datos[0].id;

			if (id > 0)
			{

				//Consulto Información necesario para completar los datos
				//del Anticipo
				AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id_Lot == id && !fod.Document.DocumentState.code.Equals("05"));
				List<QualityControl> qcTmp = null;
				Grammage gmgTmp = null;
				int iCont;
				string ruta = ConfigurationManager.AppSettings["rutaLog"];

				decimal tailPerformancePercentage = 0;
				string nameCalendarPriceListType = "";
				//string namePriceList = "";
				decimal quantityMinimunReceived = 0;
				decimal quantityPoundsReceived = 0;
				int id_processType = 0;
				int? id_processTypeAdvanceProvider = 0;
				int qualityControlQuantities = 0;
				string MensajeError;
				MensajeError = string.Empty;

				int? id_priceList = 0;
				int id_grammageFinal = 0;

				decimal grammageTotal = 0;
				decimal grammageAverage = 0;
				decimal grammageAverageFinal = 0;

				decimal wholePerformanceLotTotal = 0;
				decimal wholePerformanceLotAverage = 0;

				try
				{
					MensajeError = "";
					//Consulto el Lote 
					ProductionLot plTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == id);
					List<int?> listPODD = null;
					List<int?> listPLDQC = null;

					//Consulto los análisis de calidad de todos 
					CalendarPriceList cplTmp = null;
					qcTmp = db.QualityControl.Where(w => w.id_lot == id 
														 && w.ProductionLotDetailQualityControl
																		.Where(r=> r.IsDelete == false)
																		.Count() > 0).ToList();

					if (plTmp == null)
					{
						MensajeError += "No existe el lote de producción para calcular anticipo.<br />";
					}
					if (qcTmp == null)
					{
						MensajeError += "El Lote de Producción no tiene control de calidad.<br />";
					}

					if (plTmp != null && qcTmp != null && qcTmp.Count > 0)
					{
						qualityControlQuantities = qcTmp.Count;

						//Consulto los parametros necesarios para calcular el anticipo
						//Consulto % Rendimiento Cola
						tailPerformancePercentage = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "PRC")?.valueDecimal ?? 0;

						//Consulto Cantidad mínima de Libras para realizar anticipo
						quantityMinimunReceived = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "NMLP")?.valueDecimal ?? 0;

						var listQC = qcTmp.Select(s => s.id_processType).Distinct().Count();


						//Listo los Detalle de Recepción a los que pertenece cada Control de Calidad
						listPLDQC = db.ProductionLotDetailQualityControl
									.Where(w => w.IsDelete == false
												&& w.ProductionLotDetail.ProductionLot.id == id 
												&& w.QualityControl.Document.DocumentState.code != "05")
									.Select(s => s.id_productionLotDetail)
									.ToList();



						if (listPLDQC == null)
						{
							MensajeError += "No Existen detalle de Recepción por Control de Calidad.<br />";
						}

						//Relaciono los detalles de produccion que tienen control de calidad con los detalles de las Ordenes de Compra.
						if (listPLDQC != null && listPLDQC.Count > 0)
						{
							listPODD = db.ProductionLotDetailPurchaseDetail
										.Where(w => listPLDQC.Contains(w.id_productionLotDetail) && w.id_purchaseOrderDetail != null)
										.Select(s => s.id_purchaseOrderDetail)
										.ToList();
						}

						if (listPODD == null)
						{
							MensajeError += "No Existen detalle de Ordenes de Compra por Detalle de Recepción.<br />";
						}
						List<int> listPOD = null;

						//Busco los detalles de Orden de Compra distintos de la lista de detalles de Orden de Compra
						if (listPODD != null && listPODD.Count > 0)
						{
							listPOD = db.PurchaseOrderDetail.Where(w => listPODD.Contains(w.id))
											.Select(s => s.PurchaseOrder.id)
											.Distinct()
											.ToList();
						}
						if (listPOD == null)
						{
							MensajeError += "No Existe Orden de Compra que tenga relación con el Detalle de Recepción al que se le realizó Control de Calidad.<br />";
						}

						List<int?> listPL = null;

						//Busco todas las ordenes de compra y capturo sus listas de precio distintos
						if (listPOD != null && listPOD.Count > 0)
						{
							listPL = db.PurchaseOrder.Where(w => listPOD.Contains(w.id))
							.Select(s => s.id_priceList)
							.Distinct()
							.ToList();
						}

						if (listQC != 1)
						{
							MensajeError = "Existen Distintos Tipos de Proceso en el Análisis de Calidad.<br />";
						}
						if (listPL == null)
						{
							MensajeError += "No existe Lista de Precio en la Orden de Compra.<br />";
						}
						else if (listPL.Count != 1)
						{
							MensajeError += "Existen Distintas Listas de Precios.<br />";
						}

						id_priceList = db.PurchaseOrder.Where(w => listPOD.Contains(w.id)).FirstOrDefault().id_priceList;
						if (id_priceList != null && id_priceList != 0)
						{

							//Consulto Nombre Tipo Lista de Precio
							cplTmp = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList)?.CalendarPriceList;

							if (cplTmp == null)
							{
								MensajeError += "No existe la lista de precio a la que hace referencia en la orden de compra.<br />";
							}


							nameCalendarPriceListType = "";

							if (cplTmp != null && cplTmp.CalendarPriceListType != null)
							{
								nameCalendarPriceListType = cplTmp.CalendarPriceListType.name ?? "";


							}

							if (nameCalendarPriceListType == "")
							{
								MensajeError += "No tiene tipo de lista de precio.<br />";
							}

							var lstProcessType = qcTmp
								.Select(s => s.id_processType)
								.Distinct().ToList();

							if (lstProcessType == null)
							{
								MensajeError += "No Existe Tipo de Proceso en Control de Calidad del Lote.<br />";
							}

							if (lstProcessType != null && lstProcessType.Count > 1)
							{
								MensajeError += "Existen distintos tipos de proceso en el control de calidad del Lote.<br />";
							}

							id_processType = qcTmp.FirstOrDefault().id_processType;

							id_processTypeAdvanceProvider = db.AdvanceProcessTypeFanProcessType.FirstOrDefault(fod => fod.id_processType == id_processType).id_proccessTypeFan;

							//Se modifica Las libras Escurrido
							quantityPoundsReceived = (decimal)plTmp.ProductionLotDetail.Select(s => s.quantityRecived).DefaultIfEmpty(0).Sum();

							grammageTotal = qcTmp.Sum(s => s.grammageReference).Value;

							if (qualityControlQuantities != 0)
							{
								grammageAverage = grammageTotal / qualityControlQuantities;
							}


							wholePerformanceLotTotal = qcTmp.Sum(s => s.wholePerformance).Value;
							wholePerformanceLotAverage = wholePerformanceLotTotal / qualityControlQuantities;

							if (grammageAverage > 0)
							{
								gmgTmp = db.Grammage.FirstOrDefault(fod => fod.value == grammageAverage);
								if (gmgTmp != null)
								{
									grammageAverageFinal = gmgTmp.value;
								}
								else
								{
									grammageAverageFinal = GetClosestGrammage(grammageAverage, id_processType);
								}
								if (grammageAverageFinal <= 0)
								{
									//MensajeError += "El valor del gramaje es Menor que 0.<br />";
								}
							}
							else
							{
								//MensajeError += "El valor del gramaje es Menor que 0.<br />";
							}

							id_grammageFinal = db.Grammage.FirstOrDefault(fod => fod.value == grammageAverageFinal) != null ? (db.Grammage.FirstOrDefault(fod => fod.value == grammageAverageFinal).id) : 0;

							if (id_grammageFinal == 0)
							{
								//MensajeError += "El gramaje promedio del lote es 0.<br />";
                                MensajeError += "NO EXISTE GRAMAJE DEFINIDO.<br />";
                            }

							if (advanceProvider == null)
							{
								DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("68"));
								DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
								int id_company = (int)ViewData["id_company"];

								Employee employee = ActiveUser.Employee;

								advanceProvider = new AdvanceProvider
								{

									Document = new Document
									{
										id = 0,
										id_documentType = documentType?.id ?? 0,
										DocumentType = documentType,
										id_documentState = documentState?.id ?? 0,
										DocumentState = documentState,
										emissionDate = DateTime.Now
									}
								};
								PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
								advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																				   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																					(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");

								// Calcular el porcentaje de avance
								var valuePercentageAdvance = CalcularAnticipoProveedor(
									nameCalendarPriceListType, quantityPoundsReceived, quantityMinimunReceived);

								if (db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_provider) == null)
								{
									MensajeError += "No existen datos del Proveedor del Lote.<br />";
								}
								advanceProvider.Provider = db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_provider);
								advanceProvider.Lot = plTmp.Lot;

								if (db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_providerapparent) == null)
								{
									MensajeError += "No existen datos del Proveedor Amparante del Lote.<br />";
								}
								advanceProvider.Provider1 = db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_providerapparent);

								if (db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == plTmp.id_productionUnitProvider) == null)
								{
									MensajeError += "No existen datos de la Unidad de Producción del Proveedor.<br />";
								}
								advanceProvider.ProductionUnitProvider = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == plTmp.id_productionUnitProvider);

								if (db.ProcessType.FirstOrDefault(fod => fod.id == id_processTypeAdvanceProvider) == null)
								{
									MensajeError += "No existe el tipo de proceso indicado para este Lote de Producción.<br />";
								}

								//Asigno Fecha de Orden de Compra para el Anticipo
								int _idPo = listPL.Select(s => s.Value).FirstOrDefault();

								if (listPOD != null && listPOD.Count > 0)
								{
									advanceProvider.purchaseOrderDate = db.PurchaseOrder.FirstOrDefault(fod => fod.id == listPOD.FirstOrDefault())?.Document?.emissionDate ?? DateTime.Now;
								}



								advanceProvider.ProcessType = db.ProcessType.FirstOrDefault(fod => fod.id == id_processTypeAdvanceProvider);
								advanceProvider.CalendarPriceList = cplTmp;


								advanceProvider.id_CalendarPriceList = cplTmp.id;
								advanceProvider.id_CalendarPriceListType = cplTmp.id_calendarPriceListType;

								advanceProvider.id_priceList = id_priceList;
								advanceProvider.id_provider = plTmp.id_provider;
								advanceProvider.id_protectiveProvider = plTmp.id_providerapparent;
								advanceProvider.id_Lot = plTmp.id;
								advanceProvider.id_productionUnitProvider = plTmp.id_productionUnitProvider;
								advanceProvider.id_processType = id_processTypeAdvanceProvider;

								advanceProvider.QuantityPoundReceived = quantityPoundsReceived;
								advanceProvider.QuantityPoundsReceivedMinimun = quantityMinimunReceived;

								advanceProvider.wholePerformanceLot = wholePerformanceLotAverage;
								advanceProvider.grammageLot = grammageAverageFinal;
								advanceProvider.id_grammage = id_grammageFinal;

								advanceProvider.AdvanceValuePercentageUsed = valuePercentageAdvance;
								advanceProvider.TailPerformanceUsed = tailPerformancePercentage;
								advanceProvider.valueAdvance = 0;
								advanceProvider.valueAdvanceTotal = 0;
								advanceProvider.valueAverage = 0;
								advanceProvider.valueAdvanceRounded = 0;
								advanceProvider.valueAdvanceTotalRounded = 0;
							}
							iCont = 1;
							if (advanceProvider != null)
							{
								advanceProvider.aplRg = new List<AdvanceProviderPLRG>();
								if (listPLDQC != null)
								{
									foreach (var i in listPLDQC)
									{
										if (i != null)
										{
											var _pld = db.ProductionLotDetailPurchaseDetail
														.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
														.ProductionLotDetail;
											var _pod = db.ProductionLotDetailPurchaseDetail
															.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
															.PurchaseOrderDetail;
											var _rgd = db.ProductionLotDetailPurchaseDetail
															.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
															.RemissionGuideDetail;
											var _qTmp = db.ProductionLotDetailQualityControl
																	.FirstOrDefault(fod => fod.IsDelete == false
																						   && fod.id_productionLotDetail == i)?.QualityControl;
											AdvanceProviderPLRG aplrgNew = new AdvanceProviderPLRG();
											aplrgNew.id = iCont;
											aplrgNew.seq_RemissionGuide = _rgd?.RemissionGuide?.Document?.sequential ?? 0;
											aplrgNew.OCnumber = _pod?.PurchaseOrder?.Document?.number ?? "";
											aplrgNew.RGnumber = _rgd?.RemissionGuide?.Document?.number ?? "";
											aplrgNew.Pnumber = _pld?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
											aplrgNew.QuantityPoundsReceived = _pld?.quantityRecived ?? 0;
											aplrgNew.QuantityPoundsScurrid = _pld?.quantitydrained ?? 0;
											aplrgNew.sGrammage = _qTmp?.grammageReference ?? 0;
											aplrgNew.Performance = _qTmp?.wholePerformance ?? 0;
											iCont++;
											advanceProvider.aplRg.Add(aplrgNew);
										}
									}
								}
								TempData["aplRg"] = advanceProvider.aplRg;
								TempData.Keep("aplRg");
							}
						}
					}
				}
				catch (Exception ex)
				{
					MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Anticipo Proveedores", "PROD");
				}

				if (!string.IsNullOrEmpty(MensajeError))
				{
					ViewData["EditMessage"] = ErrorMessage(MensajeError);
				}

				TempData["AdvanceProviderPL"] = advanceProvider;
				TempData.Keep("AdvanceProviderPL");
				return PartialView("_FormEditAdvanceProviderPL", advanceProvider);
			}
			else
			{
				return View();
			}
		}

		#endregion

		#region"ADVANCE PROVIDER EDIT FORM"

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditAdvanceProviderPL(int? id)
		{

			//Consulto Información necesario para completar los datos
			//del Anticipo
			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id_Lot == id && !fod.Document.DocumentState.code.Equals("05"));
			List<QualityControl> qcTmp = null;
			Grammage gmgTmp = null;
			int iCont;
			string ruta = ConfigurationManager.AppSettings["rutaLog"];

			decimal tailPerformancePercentage = 0;
			string nameCalendarPriceListType = "";
			//string namePriceList = "";
			decimal quantityMinimunReceived = 0;
			decimal quantityPoundsReceived = 0;
			int id_processType = 0;
			int? id_processTypeAdvanceProvider = 0;
			int qualityControlQuantities = 0;
			string MensajeError;
			MensajeError = string.Empty;

			int? id_priceList = 0;
			int id_grammageFinal = 0;

			decimal grammageTotal = 0;
			decimal grammageAverage = 0;
			decimal grammageAverageFinal = 0;

			decimal wholePerformanceLotTotal = 0;
			decimal wholePerformanceLotAverage = 0;

			try
			{
				MensajeError = "";
				//Consulto el Lote 
				ProductionLot plTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == id);
				List<int?> listPODD = null;
				List<int?> listPLDQC = null;

				//Consulto los análisis de calidad de todos 
				CalendarPriceList cplTmp = null;
				qcTmp = db.QualityControl.Where(w => w.id_lot == id 
													 && w.ProductionLotDetailQualityControl
																.Where(r=> r.IsDelete == false)
																.Count() > 0).ToList();

				if (plTmp == null)
				{
					MensajeError += "No existe el lote de producción para calcular anticipo.<br />";
				}
				if (qcTmp == null)
				{
					MensajeError += "El Lote de Producción no tiene control de calidad.<br />";
				}

				if (plTmp != null && qcTmp != null && qcTmp.Count > 0)
				{
					qualityControlQuantities = qcTmp.Count;

					//Consulto los parametros necesarios para calcular el anticipo
					//Consulto % Rendimiento Cola
					tailPerformancePercentage = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "PRC")?.valueDecimal ?? 0;

					//Consulto Cantidad mínima de Libras para realizar anticipo
					quantityMinimunReceived = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "NMLP")?.valueDecimal ?? 0;

					var listQC = qcTmp.Select(s => s.id_processType).Distinct().Count();


					//Listo los Detalle de Recepción a los que pertenece cada Control de Calidad
					listPLDQC = db.ProductionLotDetailQualityControl
								.Where(w => w.IsDelete == false
											&& w.ProductionLotDetail.ProductionLot.id == id 
											&& w.QualityControl.Document.DocumentState.code != "05")
								.Select(s => s.id_productionLotDetail)
								.Distinct()
								.ToList();



					if (listPLDQC == null)
					{
						MensajeError += "No Existen detalle de Recepción por Control de Calidad.<br />";
					}

					//Relaciono los detalles de produccion que tienen control de calidad con los detalles de las Ordenes de Compra.
					if (listPLDQC != null && listPLDQC.Count > 0)
					{
						listPODD = db.ProductionLotDetailPurchaseDetail
									.Where(w => listPLDQC.Contains(w.id_productionLotDetail) 
												&& w.id_purchaseOrderDetail != null)
									.Select(s => s.id_purchaseOrderDetail)
									.ToList();
					}

					if (listPODD == null)
					{
						MensajeError += "No Existen detalle de Ordenes de Compra por Detalle de Recepción.<br />";
					}
					List<int> listPOD = null;

					//Busco los detalles de Orden de Compra distintos de la lista de detalles de Orden de Compra
					if (listPODD != null && listPODD.Count > 0)
					{
						listPOD = db.PurchaseOrderDetail.Where(w => listPODD.Contains(w.id))
										.Select(s => s.PurchaseOrder.id)
										.Distinct()
										.ToList();
					}
					if (listPOD == null)
					{
						MensajeError += "No Existe Orden de Compra que tenga relación con el Detalle de Recepción al que se le realizó Control de Calidad.<br />";
					}

					List<int?> listPL = null;

					//Busco todas las ordenes de compra y capturo sus listas de precio distintos
					if (listPOD != null && listPOD.Count > 0)
					{
						listPL = db.PurchaseOrder.Where(w => listPOD.Contains(w.id))
						.Select(s => s.id_priceList)
						.Distinct()
						.ToList();
					}

					if (listQC != 1)
					{
						MensajeError = "Existen Distintos Tipos de Proceso en el Análisis de Calidad.<br />";
					}
					if (listPL == null)
					{
						MensajeError += "No existe Lista de Precio en la Orden de Compra.<br />";
					}
					else if (listPL.Count != 1)
					{
						MensajeError += "Existen Distintas Listas de Precios.<br />";
					}

					id_priceList = db.PurchaseOrder.Where(w => listPOD.Contains(w.id)).FirstOrDefault().id_priceList;
					if (id_priceList != null && id_priceList != 0)
					{

						//Consulto Nombre Tipo Lista de Precio
						cplTmp = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList)?.CalendarPriceList;

						if (cplTmp == null)
						{
							MensajeError += "No existe la lista de precio a la que hace referencia en la orden de compra.<br />";
						}


						nameCalendarPriceListType = "";

						if (cplTmp != null && cplTmp.CalendarPriceListType != null)
						{
							nameCalendarPriceListType = cplTmp.CalendarPriceListType.name ?? "";


						}

						if (nameCalendarPriceListType == "")
						{
							MensajeError += "No tiene tipo de lista de precio.<br />";
						}

						var lstProcessType = qcTmp
							.Select(s => s.id_processType)
							.Distinct().ToList();

						if (lstProcessType == null)
						{
							MensajeError += "No Existe Tipo de Proceso en Control de Calidad del Lote.<br />";
						}

						if (lstProcessType != null && lstProcessType.Count > 1)
						{
							MensajeError += "Existen distintos tipos de proceso en el control de calidad del Lote.<br />";
						}

						id_processType = qcTmp.FirstOrDefault().id_processType;

						id_processTypeAdvanceProvider = db.AdvanceProcessTypeFanProcessType.FirstOrDefault(fod => fod.id_processType == id_processType).id_proccessTypeFan;

						//Se modifica Las libras Escurrido
						quantityPoundsReceived = (decimal)plTmp.ProductionLotDetail.Select(s => s.quantityRecived).DefaultIfEmpty(0).Sum();

						grammageTotal = qcTmp.Sum(s => s.grammageReference).Value;

						if (qualityControlQuantities != 0)
						{
							grammageAverage = grammageTotal / qualityControlQuantities;
						}


						wholePerformanceLotTotal = qcTmp.Sum(s => s.wholePerformance).Value;
						wholePerformanceLotAverage = wholePerformanceLotTotal / qualityControlQuantities;

						if (grammageAverage > 0)
						{
							gmgTmp = db.Grammage.FirstOrDefault(fod => fod.value == grammageAverage);
							if (gmgTmp != null)
							{
								grammageAverageFinal = gmgTmp.value;
							}
							else
							{
								grammageAverageFinal = GetClosestGrammage(grammageAverage, id_processType);
							}
							if (grammageAverageFinal <= 0)
							{
								//MensajeError += "El valor del gramaje es Menor que 0.<br />";
							}
						}
						else
						{
							//MensajeError += "El valor del gramaje es Menor que 0.<br />";
						}

						id_grammageFinal = db.Grammage.FirstOrDefault(fod => fod.value == grammageAverageFinal) != null ? (db.Grammage.FirstOrDefault(fod => fod.value == grammageAverageFinal).id) : 0;

						if (id_grammageFinal == 0)
						{
							//MensajeError += "El gramaje promedio del lote es 0.<br />";
                            MensajeError += "NO EXISTE GRAMAJE DEFINIDO.<br />";
                        }

                        if (advanceProvider == null)
						{
							DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("68"));
							DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
							int id_company = (int)ViewData["id_company"];

							Employee employee = ActiveUser.Employee;

							advanceProvider = new AdvanceProvider
							{

								Document = new Document
								{
									id = 0,
									id_documentType = documentType?.id ?? 0,
									DocumentType = documentType,
									id_documentState = documentState?.id ?? 0,
									DocumentState = documentState,
									emissionDate = DateTime.Now
								}
							};
							PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
							advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																			   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																				(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");

							// Calcular el porcentaje de avance
							var valuePercentageAdvance = CalcularAnticipoProveedor(
								nameCalendarPriceListType, quantityPoundsReceived, quantityMinimunReceived);

							if (db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_provider) == null)
							{
								MensajeError += "No existen datos del Proveedor del Lote.<br />";
							}
							advanceProvider.Provider = db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_provider);
							advanceProvider.Lot = plTmp.Lot;

							if (db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_providerapparent) == null)
							{
								MensajeError += "No existen datos del Proveedor Amparante del Lote.<br />";
							}
							advanceProvider.Provider1 = db.Provider.FirstOrDefault(fod => fod.id == plTmp.id_providerapparent);

							if (db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == plTmp.id_productionUnitProvider) == null)
							{
								MensajeError += "No existen datos de la Unidad de Producción del Proveedor.<br />";
							}
							advanceProvider.ProductionUnitProvider = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == plTmp.id_productionUnitProvider);

							if (db.ProcessType.FirstOrDefault(fod => fod.id == id_processTypeAdvanceProvider) == null)
							{
								MensajeError += "No existe el tipo de proceso indicado para este Lote de Producción.<br />";
							}

							//Asigno Fecha de Orden de Compra para el Anticipo
							int _idPo = listPL.Select(s => s.Value).FirstOrDefault();

							if (listPOD != null && listPOD.Count > 0)
							{
								advanceProvider.purchaseOrderDate = db.PurchaseOrder.FirstOrDefault(fod => fod.id == listPOD.FirstOrDefault())?.Document?.emissionDate ?? DateTime.Now;
							}



							advanceProvider.ProcessType = db.ProcessType.FirstOrDefault(fod => fod.id == id_processTypeAdvanceProvider);
							advanceProvider.CalendarPriceList = cplTmp;


							advanceProvider.id_CalendarPriceList = cplTmp.id;
							advanceProvider.id_CalendarPriceListType = cplTmp.id_calendarPriceListType;

							advanceProvider.id_priceList = id_priceList;
							advanceProvider.id_provider = plTmp.id_provider;
							advanceProvider.id_protectiveProvider = plTmp.id_providerapparent;
							advanceProvider.id_Lot = plTmp.id;
							advanceProvider.id_productionUnitProvider = plTmp.id_productionUnitProvider;
							advanceProvider.id_processType = id_processTypeAdvanceProvider;

							advanceProvider.QuantityPoundReceived = quantityPoundsReceived;
							advanceProvider.QuantityPoundsReceivedMinimun = quantityMinimunReceived;

							advanceProvider.wholePerformanceLot = wholePerformanceLotAverage;
							advanceProvider.grammageLot = grammageAverageFinal;
							advanceProvider.id_grammage = id_grammageFinal;

							advanceProvider.AdvanceValuePercentageUsed = valuePercentageAdvance;
							advanceProvider.TailPerformanceUsed = tailPerformancePercentage;
							advanceProvider.valueAdvance = 0;
							advanceProvider.valueAdvanceTotal = 0;
							advanceProvider.valueAverage = 0;
							advanceProvider.valueAdvanceRounded = 0;
							advanceProvider.valueAdvanceTotalRounded = 0;
						}
						iCont = 1;
						if (advanceProvider != null)
						{
							advanceProvider.aplRg = new List<AdvanceProviderPLRG>();
							if (listPLDQC != null)
							{
								foreach (var i in listPLDQC)
								{
									if (i != null)
									{
										var _pld = db.ProductionLotDetailPurchaseDetail
													.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
													.ProductionLotDetail;
										var _pod = db.ProductionLotDetailPurchaseDetail
														.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
														.PurchaseOrderDetail;
										var _rgd = db.ProductionLotDetailPurchaseDetail
														.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
														.RemissionGuideDetail;
										var _qTmp = db.ProductionLotDetailQualityControl
																.FirstOrDefault(fod => fod.IsDelete == false 
																					    && fod.id_productionLotDetail == i)?.QualityControl;
										AdvanceProviderPLRG aplrgNew = new AdvanceProviderPLRG();
										aplrgNew.id = iCont;
										aplrgNew.seq_RemissionGuide = _rgd?.RemissionGuide?.Document?.sequential ?? 0;
										aplrgNew.OCnumber = _pod?.PurchaseOrder?.Document?.number ?? "";
										aplrgNew.RGnumber = _rgd?.RemissionGuide?.Document?.number ?? "";
										aplrgNew.Pnumber = _pld?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
										aplrgNew.QuantityPoundsReceived = _pld?.quantityRecived ?? 0;
										aplrgNew.QuantityPoundsScurrid = _pld?.quantitydrained ?? 0;
										aplrgNew.sGrammage = _qTmp?.grammageReference ?? 0;
										aplrgNew.Performance = _qTmp?.wholePerformance ?? 0;
										iCont++;
										advanceProvider.aplRg.Add(aplrgNew);
									}
								}
							}
							TempData["aplRg"] = advanceProvider.aplRg;
							TempData.Keep("aplRg");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Anticipo Proveedores", "PROD");
			}

			if (!string.IsNullOrEmpty(MensajeError))
			{
				ViewData["EditMessage"] = ErrorMessage(MensajeError);
			}

			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");
			return PartialView("_FormEditAdvanceProviderPL", advanceProvider);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditAdvanceProvider(int? id)
		{
			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id == id);
			PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList);
			List<int?> listPLDQC = null;
			if (pl != null)
			{
				advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																		   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																			(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");
			}


			int iCont;
			//Listo los Detalle de Recepción a los que pertenece cada Control de Calidad
			listPLDQC = db.ProductionLotDetailQualityControl
						.Where(w => w.IsDelete == false
									&& w.ProductionLotDetail.ProductionLot.id == advanceProvider.id_Lot 
									&& w.QualityControl.Document.DocumentState.code != "05")
						.Select(s => s.id_productionLotDetail)
						.ToList();
			iCont = 1;
			if (advanceProvider != null)
			{
				advanceProvider.aplRg = new List<AdvanceProviderPLRG>();
				if (listPLDQC != null)
				{
					foreach (var i in listPLDQC)
					{
						if (i != null)
						{
							var _pld = db.ProductionLotDetailPurchaseDetail
										.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
										.ProductionLotDetail;
							var _pod = db.ProductionLotDetailPurchaseDetail
											.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
											.PurchaseOrderDetail;
							var _rgd = db.ProductionLotDetailPurchaseDetail
											.FirstOrDefault(fod => fod.id_productionLotDetail == i)?
											.RemissionGuideDetail;
							var _qTmp = db.ProductionLotDetailQualityControl
													.FirstOrDefault(fod => fod.IsDelete == false
																		   && fod.id_productionLotDetail == i)?.QualityControl;
							AdvanceProviderPLRG aplrgNew = new AdvanceProviderPLRG();
							aplrgNew.id = iCont;
							aplrgNew.seq_RemissionGuide = _rgd?.RemissionGuide?.Document?.sequential ?? 0;
							aplrgNew.OCnumber = _pod?.PurchaseOrder?.Document?.number ?? "";
							aplrgNew.RGnumber = _rgd?.RemissionGuide?.Document?.number ?? "";
							aplrgNew.Pnumber = _pld?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
							aplrgNew.QuantityPoundsReceived = _pld?.quantityRecived ?? 0;
							aplrgNew.QuantityPoundsScurrid = _pld?.quantitydrained ?? 0;
							aplrgNew.sGrammage = _qTmp?.grammageReference ?? 0;
							aplrgNew.Performance = _qTmp?.wholePerformance ?? 0;
							iCont++;
							advanceProvider.aplRg.Add(aplrgNew);
						}
					}
				}
				TempData["aplRg"] = advanceProvider.aplRg;
				TempData.Keep("aplRg");
			}
			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");
			return PartialView("_FormEditAdvanceProviderPL", advanceProvider);
		}

		public ActionResult GetDetailAdvanceProvider()
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider apTmp = (AdvanceProvider)TempData["AdvanceProviderPL"];
			apTmp = apTmp ?? new AdvanceProvider();

			apTmp.AdvanceProviderDetail = apTmp.AdvanceProviderDetail ?? new List<AdvanceProviderDetail>();

			TempData["AdvanceProviderPL"] = apTmp;
			TempData.Keep("AdvanceProviderPL");
			return PartialView("_AdvanceProviderPLMainFormTabAdvanceProviderPLDetail", apTmp.AdvanceProviderDetail.ToList());
		}

		#endregion

		#region ADVANCE PROVIDER HEADER

		public ActionResult AdvanceProviderPartial()
		{

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();

			TempData.Keep("model");

			return PartialView("_AdvanceProviderPartial", model.OrderByDescending(o => o.id).ToList());
		}

		public ActionResult AdvanceProviderPLPartial()
		{

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();

			TempData.Keep("modelPL");

			return PartialView("_AdvanceProviderProductionLotPartial", model.OrderByDescending(o => o.id).ToList());
		}

		#endregion

		#region"ADVANCE PROVIDER ACTIONS"

		#endregion

		#region"ADVANCE PROVIDER HEADER"

		[HttpPost, ValidateInput(false)]
		public ActionResult AdvanceProviderPartialAddNew(bool approve, AdvanceProvider advanceProvider, Document itemDoc)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			string mensaje = "";
			AdvanceProvider tempAdvanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			string nameCalendarPriceListType = "";

			tempAdvanceProvider = tempAdvanceProvider ?? new AdvanceProvider();
			tempAdvanceProvider.diasPlazo = advanceProvider.diasPlazo;

			AdvanceProvider apNew = null;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{

				if (tempAdvanceProvider != null)
				{
					if (tempAdvanceProvider.Document != null)
					{
						try
						{
							apNew = new AdvanceProvider();
							#region"Documento"
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

							Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id_employee);

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
							itemDoc.Lot = null;
							apNew.Document = itemDoc;
							#endregion

							#region Advance Provider
							apNew.id_Lot = tempAdvanceProvider.id_Lot;
							apNew.id_provider = tempAdvanceProvider.id_provider;
							apNew.id_protectiveProvider = tempAdvanceProvider.id_protectiveProvider;
							apNew.id_productionUnitProvider = tempAdvanceProvider.id_productionUnitProvider;

							nameCalendarPriceListType = db.CalendarPriceListType.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_CalendarPriceListType)?.name ?? "";

							// Calcular el porcentaje de avance
							var valuePercentageAdvance = CalcularAnticipoProveedor(nameCalendarPriceListType,
								tempAdvanceProvider.QuantityPoundReceived ?? 0m,
								tempAdvanceProvider.QuantityPoundsReceivedMinimun ?? 0m);

							apNew.AdvanceValuePercentageDefault = valuePercentageAdvance;
							apNew.AdvanceValuePercentageUsed = advanceProvider.AdvanceValuePercentageUsed;

							apNew.TailPerformanceDefault = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "PRC")?.valueDecimal ?? 0;
							apNew.TailPerformanceUsed = tempAdvanceProvider.TailPerformanceUsed;

							apNew.QuantityPoundsReceivedMinimun = tempAdvanceProvider.QuantityPoundsReceivedMinimun;

							apNew.id_CalendarPriceListType = tempAdvanceProvider.id_CalendarPriceListType;


							//Capturo la Lista de Precios
							apNew.id_priceList = advanceProvider.id_priceList;

							apNew.id_CalendarPriceList = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList)?.id_calendarPriceList;

							apNew.id_processType = tempAdvanceProvider.id_processType;
							apNew.id_processTypeFanUse = tempAdvanceProvider.id_processType;

							apNew.id_grammage = tempAdvanceProvider.id_grammage;
							apNew.grammageLot = tempAdvanceProvider.grammageLot;
							apNew.wholePerformanceLot = tempAdvanceProvider.wholePerformanceLot;
							apNew.QuantityPoundReceived = tempAdvanceProvider.QuantityPoundReceived;
							apNew.valueAdvance = tempAdvanceProvider.valueAdvance;
							apNew.valueAdvanceHead = tempAdvanceProvider.valueAdvanceHead;
							apNew.valueAdvanceTail = tempAdvanceProvider.valueAdvanceTail;
							apNew.valueAdvanceTotal = tempAdvanceProvider.valueAdvanceTotal;
							apNew.valueAverage = tempAdvanceProvider.valueAverage;

							apNew.valueAdvanceRounded = tempAdvanceProvider.valueAdvanceRounded;
							apNew.valueAdvanceTotalRounded = advanceProvider.valueAdvanceTotal;

							apNew.purchaseOrderDate = advanceProvider.purchaseOrderDate;

							apNew.Lot = null;
							apNew.diasPlazo = advanceProvider.diasPlazo;

                            var aAdvanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id_Lot == tempAdvanceProvider.id_Lot &&
                                                                                            fod.Document.DocumentState.code != "05");
                            if (aAdvanceProvider != null)
                            {
                                var aProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_Lot);
                                throw new Exception("Ya existe grabada un Anticipó de Proveedores para este Lote(No: " + aProductionLot.internalNumber + ").");
                            }

                            #region"Detalle"
                            if (tempAdvanceProvider.AdvanceProviderDetail != null && tempAdvanceProvider.AdvanceProviderDetail.Count > 0)
							{
								apNew.AdvanceProviderDetail = apNew.AdvanceProviderDetail ?? new List<AdvanceProviderDetail>();

								foreach (var detail in tempAdvanceProvider.AdvanceProviderDetail)
								{
									AdvanceProviderDetail apldTmp = new AdvanceProviderDetail();
									apldTmp.id_class = detail.id_class;
									apldTmp.id_processtype = detail.id_processtype;
									apldTmp.id_itemsize = detail.id_itemsize;
									apldTmp.valuePrice = detail.valuePrice;
									apldTmp.valueTotal = detail.valueTotal;
									apldTmp.poundsDetail = detail.poundsDetail;

									apNew.AdvanceProviderDetail.Add(apldTmp);
								}
							}
							else
							{
								trans.Rollback();
								TempData.Keep("AdvanceProviderPL");
								ViewData["EditError"] = ErrorMessage("No existe detalle de precios y tallas para el Cálculo de Anticipo.");
								return PartialView("_AdvanceProviderPLMainFormPartial", tempAdvanceProvider);
							}

							#endregion

							TempData["AdvanceProviderPL"] = tempAdvanceProvider;
							TempData.Keep("AdvanceProviderPL");
							#endregion

							#region"Actualizo Estado de Lote"
							Lot lTmp = db.Lot.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_Lot);
							if (lTmp != null)
							{
								lTmp.hasAdvanceProvider = true;
								db.Lot.Attach(lTmp);
								db.Entry(lTmp).State = EntityState.Modified;
							}
							else
							{
								trans.Rollback();
								TempData.Keep("AdvanceProviderPL");
								ViewData["EditError"] = ErrorMessage("No existe el Lote.");
								return PartialView("_AdvanceProviderPLMainFormPartial", tempAdvanceProvider);
							}
							#endregion

							db.AdvanceProvider.Add(apNew);
							db.SaveChanges();
							trans.Commit();
							TempData.Keep("AdvanceProviderPL");
						}
						catch (Exception ex)
						{
							trans.Rollback();
							TempData.Keep("AdvanceProviderPL");
							ViewData["EditError"] = ErrorMessage(ex.Message);
						}
					}
					else
					{
						mensaje += "No se pudo crear registro porque no existen Datos en el Anticipo del Documento.";
					}
				}
				else
				{
					mensaje += "No se pudo crear registro porque no existen Datos en el anticipo.";
				}
			}
			if (mensaje != "")
			{
				ViewData["EditError"] = ErrorMessage(mensaje);
			}
			else
			{
				ViewData["EditError"] = SuccessMessage("Anticipo a Proveedor: " + apNew.Document.number + " guardado exitosamente");
			}
			tempAdvanceProvider.id = apNew.id;
			tempAdvanceProvider.AdvanceValuePercentageUsed = apNew.AdvanceValuePercentageUsed;
			tempAdvanceProvider.valueAdvanceTotalRounded = apNew.valueAdvanceTotalRounded;
			return PartialView("_AdvanceProviderPLMainFormPartial", tempAdvanceProvider);
		}

		public ActionResult AdvanceProviderPartialUpdate(bool approve, AdvanceProvider advanceProvider, Document advanceProviderDoc)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			string mensaje = "";
			AdvanceProvider tempAdvanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			tempAdvanceProvider = tempAdvanceProvider ?? new AdvanceProvider();

			string nameCalendarPriceListType = "";

			//Recupero Dato a Actualizar 
			AdvanceProvider advanceProviderBase = db.AdvanceProvider.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id);


			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{

				if (tempAdvanceProvider != null && advanceProviderBase != null)
				{
					if (tempAdvanceProvider.Document != null)
					{
						try
						{
							advanceProviderBase.Document.id_userUpdate = ActiveUser.id;
							advanceProviderBase.Document.dateUpdate = ActiveUser.dateUpdate;
							advanceProviderBase.Document.description = advanceProviderDoc?.description ?? "";

							#region"Cabecera Anticipo"
							//advanceProviderBase.id_Lot = tempAdvanceProvider.id_Lot;
							advanceProviderBase.id_provider = tempAdvanceProvider.id_provider;
							advanceProviderBase.id_protectiveProvider = tempAdvanceProvider.id_protectiveProvider;
							advanceProviderBase.id_productionUnitProvider = tempAdvanceProvider.id_productionUnitProvider;

							nameCalendarPriceListType = db.CalendarPriceListType.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_CalendarPriceListType)?.name ?? "";

							// Calcular el porcentaje de avance
							var valuePercentageAdvance = CalcularAnticipoProveedor(nameCalendarPriceListType,
								tempAdvanceProvider.QuantityPoundReceived ?? 0m,
								tempAdvanceProvider.QuantityPoundsReceivedMinimun ?? 0m);

							advanceProviderBase.AdvanceValuePercentageDefault = valuePercentageAdvance;
							advanceProviderBase.AdvanceValuePercentageUsed = advanceProvider.AdvanceValuePercentageUsed;
                            tempAdvanceProvider.AdvanceValuePercentageUsed = advanceProvider.AdvanceValuePercentageUsed;

                            advanceProviderBase.TailPerformanceDefault = db.AdvanceParameters.FirstOrDefault(fod => fod.code == "PRC")?.valueDecimal ?? 0;
							advanceProviderBase.TailPerformanceUsed = tempAdvanceProvider.TailPerformanceUsed;

							advanceProviderBase.QuantityPoundsReceivedMinimun = tempAdvanceProvider.QuantityPoundsReceivedMinimun;

							advanceProviderBase.id_CalendarPriceListType = tempAdvanceProvider.id_CalendarPriceListType;

							advanceProviderBase.id_priceList = advanceProvider.id_priceList;
                            tempAdvanceProvider.id_priceList = advanceProvider.id_priceList;

                            advanceProviderBase.id_CalendarPriceList = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList)?.id_calendarPriceList;
                            tempAdvanceProvider.id_CalendarPriceList = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList)?.id_calendarPriceList;
                            //advanceProviderBase.id_CalendarPriceList = tempAdvanceProvider.id_CalendarPriceList;

                            advanceProviderBase.id_processType = tempAdvanceProvider.id_processType;
							advanceProviderBase.id_processTypeFanUse = tempAdvanceProvider.id_processType;

							advanceProviderBase.id_grammage = tempAdvanceProvider.id_grammage;
							advanceProviderBase.grammageLot = tempAdvanceProvider.grammageLot;
							advanceProviderBase.wholePerformanceLot = tempAdvanceProvider.wholePerformanceLot;
							advanceProviderBase.QuantityPoundReceived = tempAdvanceProvider.QuantityPoundReceived;
							advanceProviderBase.valueAdvance = tempAdvanceProvider.valueAdvance;
							advanceProviderBase.valueAdvanceHead = tempAdvanceProvider.valueAdvanceHead;
							advanceProviderBase.valueAdvanceTail = tempAdvanceProvider.valueAdvanceTail;
							advanceProviderBase.valueAdvanceTotal = tempAdvanceProvider.valueAdvanceTotal;
							advanceProviderBase.valueAverage = tempAdvanceProvider.valueAverage;

							advanceProviderBase.valueAdvanceTotalRounded = advanceProvider.valueAdvanceTotal;
                            tempAdvanceProvider.valueAdvanceTotalRounded = advanceProvider.valueAdvanceTotal;
                            advanceProviderBase.valueAdvanceRounded = tempAdvanceProvider.valueAdvanceRounded;

							advanceProviderBase.purchaseOrderDate = advanceProvider.purchaseOrderDate;
                            tempAdvanceProvider.purchaseOrderDate = advanceProvider.purchaseOrderDate;

                            advanceProviderBase.diasPlazo = advanceProvider.diasPlazo;
                            tempAdvanceProvider.diasPlazo = advanceProvider.diasPlazo;

                            #endregion

                            var aAdvanceProvider = db.AdvanceProvider.FirstOrDefault(fod => fod.id != advanceProviderBase.id &&
                                                                                                              fod.id_Lot == tempAdvanceProvider.id_Lot &&
                                                                                                              fod.Document.DocumentState.code != "05");
                            if (aAdvanceProvider != null)
                            {
                                var aProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_Lot);
                                throw new Exception("Ya existe grabada un Anticipó de Proveedores para este Lote(No: " + aProductionLot.internalNumber + ").");
                            }

                            #region"Detalle Anticipo"
                            if (tempAdvanceProvider.AdvanceProviderDetail != null && tempAdvanceProvider.AdvanceProviderDetail.Count > 0)
							{
								if (advanceProviderBase.AdvanceProviderDetail != null && advanceProviderBase.AdvanceProviderDetail.Count > 0)
								{
									for (int i = advanceProviderBase.AdvanceProviderDetail.Count - 1; i >= 0; i--)
									{
										AdvanceProviderDetail apdOld = advanceProviderBase.AdvanceProviderDetail.ElementAt(i);
										advanceProviderBase.AdvanceProviderDetail.Remove(apdOld);
										db.AdvanceProviderDetail.Attach(apdOld);
										db.Entry(apdOld).State = EntityState.Deleted;
									}
								}

								foreach (var detail in tempAdvanceProvider.AdvanceProviderDetail)
								{
									AdvanceProviderDetail apdNew = new AdvanceProviderDetail();
									apdNew.id_class = detail.id_class;
									apdNew.id_itemsize = detail.id_itemsize;
									apdNew.id_processtype = detail.id_processtype;
									apdNew.valuePrice = detail.valuePrice;
									apdNew.valueTotal = detail.valueTotal;
									apdNew.poundsDetail = detail.poundsDetail;

									advanceProviderBase.AdvanceProviderDetail.Add(apdNew);
								}
							}

							#endregion

							advanceProviderBase.aplRg = tempAdvanceProvider.aplRg;

							#region"Actualizo Estado de Lote"
							Lot lTmp = db.Lot.FirstOrDefault(fod => fod.id == tempAdvanceProvider.id_Lot);
							if (lTmp != null)
							{
								lTmp.hasAdvanceProvider = true;
								db.Lot.Attach(lTmp);
								db.Entry(lTmp).State = EntityState.Modified;
							}
							else
							{
								trans.Rollback();
								TempData.Keep("AdvanceProviderPL");
								ViewData["EditError"] = ErrorMessage("No existe el Lote.");
								return PartialView("_AdvanceProviderPLMainFormPartial", tempAdvanceProvider);
							}
							#endregion

							db.SaveChanges();
							trans.Commit();

							TempData["AdvanceProviderPL"] = tempAdvanceProvider;
							TempData.Keep("AdvanceProviderPL");
						}
						catch (Exception ex)
						{
                            TempData["AdvanceProviderPL"] = tempAdvanceProvider;
                            //advanceProviderBase = (AdvanceProvider)TempData["AdvanceProviderPL"];
                            TempData.Keep("AdvanceProviderPL");
							ViewData["EditError"] = ErrorMessage(ex.Message);
							trans.Rollback();
							return PartialView("_AdvanceProviderPLMainFormPartial", tempAdvanceProvider);
						}
					}
					else
					{
						mensaje += "No se pudo crear registro porque no existen Datos en el Anticipo del Documento. ";
					}
				}
				else
				{
					mensaje += "No se pudo crear registro porque no existen Datos en el anticipo. ";
				}

			}
			if (mensaje != "")
			{
				ViewData["EditError"] = ErrorMessage(mensaje);
			}
			else
			{
				ViewData["EditError"] = SuccessMessage("Anticipo a Proveedor: " + advanceProviderBase.Document.number + " guardado exitosamente");
			}
			return PartialView("_AdvanceProviderPLMainFormPartial", advanceProviderBase);
		}

		#endregion

		#region"SELECTED DOCUMENT STATE CHANGE"

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
							AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

							if (advanceProvider != null && documentState != null)
							{
								advanceProvider.Document.id_documentState = documentState.id;
								advanceProvider.Document.DocumentState = documentState;

								//foreach (var details in purchaseOrder.PurchaseOrderDetail)
								//{
								//    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;
								//    db.PurchaseOrderDetail.Attach(details);
								//    db.Entry(details).State = EntityState.Modified;
								//}

								db.AdvanceProvider.Attach(advanceProvider);
								db.Entry(advanceProvider).State = EntityState.Modified;
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

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.AdvanceProvider.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
							AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

							if (advanceProvider != null && documentState != null)
							{
								advanceProvider.Document.id_documentState = documentState.id;
								advanceProvider.Document.DocumentState = documentState;

								//foreach (var details in purchaseOrder.PurchaseOrderDetail)
								//{
								//    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;

								//    db.PurchaseOrderDetail.Attach(details);
								//    db.Entry(details).State = EntityState.Modified;
								//}

								db.AdvanceProvider.Attach(advanceProvider);
								db.Entry(advanceProvider).State = EntityState.Modified;
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

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.AdvanceProvider.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
							AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

							if (advanceProvider != null && documentState != null)
							{
								advanceProvider.Document.id_documentState = documentState.id;
								advanceProvider.Document.DocumentState = documentState;

								db.AdvanceProvider.Attach(advanceProvider);
								db.Entry(advanceProvider).State = EntityState.Modified;
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

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.AdvanceProvider.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
							AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

							if (advanceProvider != null && documentState != null)
							{
								advanceProvider.Document.id_documentState = documentState.id;
								advanceProvider.Document.DocumentState = documentState;

								db.AdvanceProvider.Attach(advanceProvider);
								db.Entry(advanceProvider).State = EntityState.Modified;
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

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.AdvanceProvider.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
							AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

							if (advanceProvider != null && documentState != null)
							{
								advanceProvider.Document.id_documentState = documentState.id;
								advanceProvider.Document.DocumentState = documentState;

								//foreach (var details in purchaseOrder.PurchaseOrderDetail)
								//{
								//    details.quantityApproved = 0.0M;

								//    db.PurchaseOrderDetail.Attach(details);
								//    db.Entry(details).State = EntityState.Modified;
								//}

								db.AdvanceProvider.Attach(advanceProvider);
								db.Entry(advanceProvider).State = EntityState.Modified;
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

			var model = (TempData["model"] as List<AdvanceProvider>);
			model = model ?? new List<AdvanceProvider>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.AdvanceProvider.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
		}

		#endregion

		#region"SELECTED DOCUMENT STATE CHANGE PL"

		[HttpPost, ValidateInput(false)]
		public void ApproveDocumentsPL(int[] ids)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			//if (ids != null)
			//{
			//    using (DbContextTransaction trans = db.Database.BeginTransaction())
			//    {
			//        try
			//        {
			//            foreach (var id in ids)
			//            {
			//                PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

			//                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

			//                if (purchaseOrder != null && documentState != null)
			//                {
			//                    purchaseOrder.Document.id_documentState = documentState.id;
			//                    purchaseOrder.Document.DocumentState = documentState;

			//                    foreach (var details in purchaseOrder.PurchaseOrderDetail)
			//                    {
			//                        details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;
			//                        db.PurchaseOrderDetail.Attach(details);
			//                        db.Entry(details).State = EntityState.Modified;
			//                    }

			//                    db.PurchaseOrder.Attach(purchaseOrder);
			//                    db.Entry(purchaseOrder).State = EntityState.Modified;
			//                }
			//            }
			//            db.SaveChanges();
			//            trans.Commit();
			//        }
			//        catch (Exception e)
			//        {
			//            ViewData["EditError"] = e.Message;
			//            trans.Rollback();
			//        }
			//    }
			//}

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");
		}

		[HttpPost, ValidateInput(false)]
		public void AutorizeDocumentsPL(int[] ids)
		{
			//if (ids != null)
			//{
			//    using (DbContextTransaction trans = db.Database.BeginTransaction())
			//    {
			//        try
			//        {
			//            foreach (var id in ids)
			//            {
			//                PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

			//                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

			//                if (purchaseOrder != null && documentState != null)
			//                {
			//                    purchaseOrder.Document.id_documentState = documentState.id;
			//                    purchaseOrder.Document.DocumentState = documentState;

			//                    foreach (var details in purchaseOrder.PurchaseOrderDetail)
			//                    {
			//                        details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;

			//                        db.PurchaseOrderDetail.Attach(details);
			//                        db.Entry(details).State = EntityState.Modified;
			//                    }

			//                    db.PurchaseOrder.Attach(purchaseOrder);
			//                    db.Entry(purchaseOrder).State = EntityState.Modified;
			//                }
			//            }
			//            db.SaveChanges();
			//            trans.Commit();
			//        }
			//        catch (Exception e)
			//        {
			//            ViewData["EditError"] = e.Message;
			//            trans.Rollback();
			//        }
			//    }
			//}

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");
		}

		[HttpPost, ValidateInput(false)]
		public void ProtectDocumentsPL(int[] ids)
		{
			//if (ids != null)
			//{
			//    using (DbContextTransaction trans = db.Database.BeginTransaction())
			//    {
			//        try
			//        {
			//            foreach (var id in ids)
			//            {
			//                PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

			//                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

			//                if (purchaseOrder != null && documentState != null)
			//                {
			//                    purchaseOrder.Document.id_documentState = documentState.id;
			//                    purchaseOrder.Document.DocumentState = documentState;

			//                    db.PurchaseOrder.Attach(purchaseOrder);
			//                    db.Entry(purchaseOrder).State = EntityState.Modified;
			//                }
			//            }
			//            db.SaveChanges();
			//            trans.Commit();
			//        }
			//        catch (Exception e)
			//        {
			//            ViewData["EditError"] = e.Message;
			//            trans.Rollback();
			//        }
			//    }
			//}

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");
		}

		[HttpPost, ValidateInput(false)]
		public void CancelDocumentsPL(int[] ids)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			//if (ids != null)
			//{
			//    using (DbContextTransaction trans = db.Database.BeginTransaction())
			//    {
			//        try
			//        {
			//            foreach (var id in ids)
			//            {
			//                PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

			//                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

			//                if (purchaseOrder != null && documentState != null)
			//                {
			//                    purchaseOrder.Document.id_documentState = documentState.id;
			//                    purchaseOrder.Document.DocumentState = documentState;

			//                    db.PurchaseOrder.Attach(purchaseOrder);
			//                    db.Entry(purchaseOrder).State = EntityState.Modified;
			//                }
			//            }
			//            db.SaveChanges();
			//            trans.Commit();
			//        }
			//        catch (Exception e)
			//        {
			//            ViewData["EditError"] = e.Message;
			//            trans.Rollback();
			//        }
			//    }
			//}

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");
		}

		[HttpPost, ValidateInput(false)]
		public void RevertDocumentsPL(int[] ids)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			//if (ids != null)
			//{
			//    using (DbContextTransaction trans = db.Database.BeginTransaction())
			//    {
			//        try
			//        {
			//            foreach (var id in ids)
			//            {
			//                PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

			//                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

			//                if (purchaseOrder != null && documentState != null)
			//                {
			//                    purchaseOrder.Document.id_documentState = documentState.id;
			//                    purchaseOrder.Document.DocumentState = documentState;

			//                    foreach (var details in purchaseOrder.PurchaseOrderDetail)
			//                    {
			//                        details.quantityApproved = 0.0M;

			//                        db.PurchaseOrderDetail.Attach(details);
			//                        db.Entry(details).State = EntityState.Modified;
			//                    }

			//                    db.PurchaseOrder.Attach(purchaseOrder);
			//                    db.Entry(purchaseOrder).State = EntityState.Modified;
			//                }
			//            }
			//            db.SaveChanges();
			//            trans.Commit();
			//        }
			//        catch (Exception e)
			//        {
			//            ViewData["EditError"] = e.Message;
			//            trans.Rollback();
			//        }
			//    }
			//}

			var model = (TempData["modelPL"] as List<AdvanceProviderPL>);
			model = model ?? new List<AdvanceProviderPL>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["modelPL"] = model;
			TempData.Keep("modelPL");
		}

		#endregion

		#region"SINGLE DOCUMENT STATE CHANGE"

		[HttpPost]
		public ActionResult Approve(int id)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

			AdvanceProvider tempAdvanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

					if (advanceProvider != null && documentState != null)
					{
						advanceProvider.Document.id_documentState = documentState.id;
						advanceProvider.Document.DocumentState = documentState;

						PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList);
						advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																		   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																			(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");


						db.AdvanceProvider.Attach(advanceProvider);
						db.Entry(advanceProvider).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditError"] = SuccessMessage("El Anticipo a Proveedor " + advanceProvider.Document.number + " ha sido aprobado.");
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}
			advanceProvider.aplRg = tempAdvanceProvider.aplRg;

			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");

			return PartialView("_AdvanceProviderPLMainFormPartial", advanceProvider);
		}

		[HttpPost]
		public ActionResult RevertDocument(int id)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

			AdvanceProvider tempAdvanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");

					if (advanceProvider != null && documentState != null)
					{
						advanceProvider.Document.id_documentState = documentState.id;
						advanceProvider.Document.DocumentState = documentState;

						PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList);
						advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																		   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																			(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");


						db.AdvanceProvider.Attach(advanceProvider);
						db.Entry(advanceProvider).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditError"] = SuccessMessage("El Anticipo a Proveedor " + advanceProvider.Document.number + " ha sido reversado.");
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}

			advanceProvider.aplRg = tempAdvanceProvider.aplRg;

			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");

			return PartialView("_AdvanceProviderPLMainFormPartial", advanceProvider);
		}

		[HttpPost]
		public ActionResult CancelDocument(int id)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);

			AdvanceProvider tempAdvanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

					if (advanceProvider != null && documentState != null)
					{
						advanceProvider.Document.id_documentState = documentState.id;
						advanceProvider.Document.DocumentState = documentState;

						PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == advanceProvider.id_priceList);
						advanceProvider.namePriList = pl.name + " (" + pl.Document.DocumentType.name + ") " + pl.CalendarPriceList.CalendarPriceListType.name + " [" + pl.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
																		   pl.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
																			(pl.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + pl.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "");


						db.AdvanceProvider.Attach(advanceProvider);
						db.Entry(advanceProvider).State = EntityState.Modified;

						Lot ltTmp = db.Lot.FirstOrDefault(fod => fod.id == advanceProvider.id_Lot);

						if (ltTmp != null)
						{
							ltTmp.hasAdvanceProvider = false;
							db.Lot.Attach(ltTmp);
							db.Entry(ltTmp).State = EntityState.Modified;
						}

						db.SaveChanges();
						trans.Commit();
						ViewData["EditError"] = SuccessMessage("El Anticipo a Proveedor " + advanceProvider.Document.number + " ha sido Anulado.");
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}

			advanceProvider.aplRg = tempAdvanceProvider.aplRg;

			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");

			return PartialView("_AdvanceProviderPLMainFormPartial", advanceProvider);
		}

		#endregion

		#region PAGINATION

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_advanceProvider)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			TempData.Keep("AdvanceProviderPL");

			int index = db.AdvanceProvider.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_advanceProvider);

			var result = new
			{
				maximunPages = db.AdvanceProvider.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider advanceProvider = db.AdvanceProvider.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (advanceProvider != null)
			{
				TempData["AdvanceProviderPL"] = advanceProvider;
				TempData.Keep("AdvanceProviderPL");
				return PartialView("_PurchaseOrderMainFormPartial", advanceProvider);
			}

			TempData.Keep("AdvanceProviderPL");

			return PartialView("_AdvanceProviderPLMainFormPartial", new AdvanceProvider());
		}

		#endregion

		#region AVANCE PROVIDER REPORT

		[HttpPost, ValidateInput(false)]
		public JsonResult PrintAdvanceProviderReport(int id_adpr)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider advanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			advanceProvider = advanceProvider ?? new AdvanceProvider();
			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = id_adpr;

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "ACCIP";
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

		#region"METODOS AUXILIARES"

		public IEnumerable<AdvanceProviderPL> GetAdvanceProviderPLs()
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			string cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
			try
			{
                List<AdvanceProviderPL> modelAux = db.Database.SqlQuery<AdvanceProviderPL>("exec pacAntProLotesParaAnticipo").ToList();
                return (modelAux);
                //return MetodosDatos.EjecutarConsultaProcedimientoAlmacenado<AdvanceProviderPL>("pacAntProLotesParaAnticipo", cadenaConexion
                //    ,
                //    reader =>
                //    {
                //        return new AdvanceProviderPL
                //        {
                //            id = (int)reader["id"],
                //            number = (string)reader["number"],
                //            internalNumber = (string)reader["internalNumber"],
                //            lotState = (string)reader["lotState"],
                //            ReceptionDate = (DateTime)reader["ReceptionDate"],
                //            id_provider = (int)reader["id_provider"],
                //            ProviderName = (string)reader["ProviderName"],
                //            id_buyer = (int)reader["id_buyer"],
                //            BuyerName = (string)reader["BuyerName"],
                //            QuantityPoundsReceived = (decimal)reader["QuantityPoundsReceived"],
                //            ZoneName = (string)reader["ZoneName"],
                //            SiteName = (string)reader["SiteName"],
                //            personProcessPlant = (string)reader["PersonProcessPlant"]
                //        };
                //    });
            }
			catch (Exception ex)
			{
				throw new Exception("Error al obtener Lista de Lotes: " + ex.Message);
			}
		}

		public ActionResult CalculateAdvanceProvider(AdvanceProvider ap)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			List<AdvanceProviderPLDetail> applDetail = null;
			AdvanceProvider apNew = ap ?? new AdvanceProvider();
			AdvanceProvider advanceProvider = (AdvanceProvider)TempData["AdvanceProviderPL"];
			advanceProvider = advanceProvider ?? new AdvanceProvider();

			advanceProvider.TailPerformanceUsed = apNew.TailPerformanceUsed;
			advanceProvider.AdvanceValuePercentageUsed = apNew.AdvanceValuePercentageUsed;
			advanceProvider.diasPlazo = apNew.diasPlazo;

			AdvanceProviderPLGeneralParameters apTmp = new AdvanceProviderPLGeneralParameters();

			apTmp.id_priceList = ap.id_priceList;
			apTmp.id_grammage = (int)advanceProvider.id_grammage;
			apTmp.id_processType = (int)advanceProvider.id_processType;
			apTmp.idProvider = advanceProvider.id_provider;
			apTmp.totalPoundsLot = (decimal)advanceProvider.QuantityPoundReceived;
			apTmp.performanceLot = (decimal)advanceProvider.wholePerformanceLot;
			apTmp.percentageTailPerformanceUsed = (decimal)advanceProvider.TailPerformanceUsed;
			apTmp.codeProcessType = db.ProcessType.FirstOrDefault(fod => fod.id == advanceProvider.id_processType).code;

			applDetail = DataProviderCalculateAdvance.CalculateAdvanceProviderFixed(apTmp);

            var aFanSeriesGrammage = db.FanSeriesGrammage
                                .FirstOrDefault(w => w.id_grammage == apTmp.id_grammage
                                            && w.id_processtype == apTmp.id_processType
                                            && (w.ProcessType1.code == "ENT" || w.ProcessType1.code == "COL"));
            if (aFanSeriesGrammage != null) {
                ViewData["EditError"] = WarningMessage("NO EXISTE GRAMAJE DEFINIDO .");
            }
            

            decimal valueAdvance = 0;
			decimal valueAdvanceTotal = 0;

			int iValueAdvance = 0;
			int iValueAdvanceTotal = 0;

			decimal valueAdvanceAverage = 0;
			decimal percentage = 100;

			if (applDetail != null && applDetail.Count > 0)
			{
				if (apTmp.codeProcessType == "COL")
				{
					valueAdvance = applDetail
									.Select(s => new { valor = Math.Round((s.valuePounds * Math.Round((decimal)s.price, 3)), 3) }).ToList().Sum(s => s.valor);
				}
				else
				{
					valueAdvance = applDetail.
										Select(s => s.value).Sum();
					//var lstHead = applDetail
					//                .Where(w => w.id_processType == apTmp.id_processType).ToList();

					//if (lstHead != null && lstHead.Count > 0)
					//{
					//    valueAdvance = lstHead
					//                    .Select(s => s.value).Sum();
					//}

					//var lstTail = applDetail
					//                .Where(w => w.id_processType != apTmp.id_processType).ToList();

					//if (lstTail != null && lstTail.Count > 0)
					//{
					//    valueAdvance += lstTail
					//                        .Select(s => new { valor = Math.Round((s.valuePounds * Math.Round((decimal)s.price, 3)), 3) }).ToList().Sum(s => s.valor);
					//}
				}
			}
			valueAdvanceTotal = (((decimal)valueAdvance * (decimal)advanceProvider.AdvanceValuePercentageUsed) / percentage);
			valueAdvanceAverage = ((decimal)valueAdvance / (decimal)advanceProvider.QuantityPoundReceived);

			advanceProvider.valueAdvance = Math.Round(valueAdvance, 3);
			advanceProvider.valueAdvanceTotal = Math.Round(valueAdvanceTotal, 3);
			advanceProvider.valueAverage = Math.Round(valueAdvanceAverage, 3);
			advanceProvider.purchaseOrderDate = ap.purchaseOrderDate;
			//Redondeo Anticipo y Porcentaje de Anticipo
			decimal _valueAdvanceTmp = advanceProvider.valueAdvance ?? 0;
			decimal _valueAdvanceTotalTmp = advanceProvider.valueAdvanceTotal ?? 0;

			decimal _valueAdvanceDPTmp = (_valueAdvanceTmp % 1);
			decimal _valueAdvanceTotalDPTmp = (_valueAdvanceTotalTmp % 1);

			if (_valueAdvanceDPTmp < (decimal)(0.5))
				advanceProvider.valueAdvanceRounded = Math.Round(_valueAdvanceTmp);
			else if (_valueAdvanceDPTmp >= (decimal)(0.5))
				advanceProvider.valueAdvanceRounded = Math.Ceiling(_valueAdvanceTmp);

			if (_valueAdvanceTotalDPTmp < (decimal)(0.5))
				advanceProvider.valueAdvanceTotalRounded = Math.Round(_valueAdvanceTotalTmp);
			else if (_valueAdvanceTotalDPTmp >= (decimal)(0.5))
				advanceProvider.valueAdvanceTotalRounded = Math.Ceiling(_valueAdvanceTotalTmp);

			iValueAdvance = Convert.ToInt32(advanceProvider.valueAdvanceRounded);
			iValueAdvanceTotal = Convert.ToInt32(advanceProvider.valueAdvanceTotalRounded);

			int rem = iValueAdvanceTotal % 10;
			iValueAdvanceTotal = (rem >= 5 ? (iValueAdvanceTotal - rem + 10) : (iValueAdvanceTotal - rem));
			advanceProvider.valueAdvanceTotalRounded = iValueAdvanceTotal;

			//Fin Redondeo

			advanceProvider.id_priceList = ap.id_priceList;
			advanceProvider.AdvanceProviderDetail = advanceProvider.AdvanceProviderDetail ?? new List<AdvanceProviderDetail>();

			advanceProvider.AdvanceProviderDetail = applDetail
														.Select(s => new AdvanceProviderDetail
														{
															id_class = s.id_class,
															id_itemsize = s.id_itemSize,
															id_processtype = s.id_processType,
															valuePrice = s.price,
															valueTotal = s.value,
															poundsDetail = s.valuePounds,
															ProcessType = db.ProcessType.FirstOrDefault(fod => fod.id == s.id_processType),
															Class = db.Class.FirstOrDefault(fod => fod.id == s.id_class),
															ItemSize = db.ItemSize.FirstOrDefault(fod => fod.id == s.id_itemSize)
														}).ToList();

			TempData["AdvanceProviderPL"] = advanceProvider;
			TempData.Keep("AdvanceProviderPL");
			return PartialView("_AdvanceProviderPLMainFormPartial", advanceProvider);
		}

		public decimal GetClosestGrammage(decimal grammageAverage, int idProcType)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}

			List<Grammage> lstGrammage = null;
			FanSeriesGrammage fsg = db.FanSeriesGrammage
									.Where(w => w.id_processtype == idProcType)
									.OrderBy(o => o.Grammage.value)
									.FirstOrDefault();

			FanSeriesGrammage fsgMax = db.FanSeriesGrammage
											.Where(w => w.id_processtype == idProcType)
											.OrderByDescending(o => o.Grammage.value)
											.FirstOrDefault();

			if (grammageAverage <= fsg.Grammage.value)
			{
                return fsg.Grammage.value;
			}
			if (grammageAverage >= fsgMax.Grammage.value)
			{
				return fsgMax.Grammage.value;
            }

			decimal dp_grammageAverage = grammageAverage - Math.Truncate(grammageAverage);

			if (dp_grammageAverage <= 0.5m)
			{
				if (dp_grammageAverage <= 0.25m)
				{
					lstGrammage = db.Grammage.Where(w => w.value <= grammageAverage).ToList();
				}
				else
				{
					lstGrammage = db.Grammage.Where(w => w.value > grammageAverage).ToList();
				}
			}
			else
			{
				if (dp_grammageAverage <= 0.75m)
				{
					lstGrammage = db.Grammage.Where(w => w.value <= grammageAverage).ToList();
				}
				else
				{
					lstGrammage = db.Grammage.Where(w => w.value > grammageAverage).ToList();
				}
			}


			decimal closestGrammage = 0;
			if (lstGrammage != null && lstGrammage.Count > 0)
			{
				closestGrammage = lstGrammage
									.Select(n => new { n, distance = Math.Abs((n.value - grammageAverage)) })
									.OrderBy(p => p.distance)
									.First().n.value;
			}


			return closestGrammage;
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GetPriceListsByProvider(int? id_provider, DateTime purchaseOrderDate)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			AdvanceProvider _advanceProvider = (TempData["AdvanceProviderPL"] as AdvanceProvider);

			_advanceProvider = _advanceProvider ?? new AdvanceProvider();

			if (id_provider == null || id_provider < 0)
			{
				if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "")
					id_provider = int.Parse(Request.Params["id_provider"]);
				else id_provider = -1;
			}

			_advanceProvider.purchaseOrderDate = purchaseOrderDate;

			_advanceProvider.id_provider = (int)id_provider;

			TempData["AdvanceProviderPL"] = _advanceProvider;
			TempData.Keep("AdvanceProviderPL");

			return PartialView("combobox/_cmbPriceListByProvider", _advanceProvider);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ValidateProcessType(int? id_priList, int? id_procType)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			PriceList pl = db.PriceList.FirstOrDefault(fod => fod.id == id_priList);
			int _id_PT = db.ProcessTypeLPTipo.FirstOrDefault(fod => fod.id_ProcessType == id_procType).id_LPTipo;
			ProcessType pt = db.ProcessType.FirstOrDefault(fod => fod.id == _id_PT);

			string _code_PL = pl?.Document?.DocumentState?.code ?? "";

			string _plProcessType = pl?.ProcessType?.code ?? "";
			string _ptProcessType = pt?.code ?? "";

			var result = new
			{
				areDiferentProc = ((_plProcessType != _ptProcessType) ? "YES" : "NO"),
				isListApproved = ((_code_PL == "04" || _code_PL == "13" || _code_PL == "15") ? "YES" : "NO")
				//isListApproved = ((_code_PL == "02" || _code_PL == "03" || _code_PL == "06" || _code_PL == "04") ? "YES" : "NO")
			};

			return Json(result, JsonRequestBehavior.AllowGet); ;
		}

		#endregion

		#region"Actions"

		[HttpPost, ValidateInput(false)]
		public JsonResult Actions(int id)
		{
			if (TempData["aplRg"] != null)
			{
				TempData.Keep("aplRg");
			}
			var actions = new
			{
				btnApprove = false,
				btnAutorize = false,
				btnProtect = false,
				btnCancel = false,
				btnRevert = false,
			};

			if (id == 0)
			{
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			AdvanceProvider advanceProvider = db.AdvanceProvider.FirstOrDefault(r => r.id == id);
			string state = advanceProvider.Document.DocumentState.code;
			IntegrationProcessDetail transmitido = db.IntegrationProcessDetail.FirstOrDefault(s => s.id_Document == id);
			string integrationstate = transmitido?.IntegrationProcess?.IntegrationState?.code;


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
			else if ((state == "03" && transmitido == null) || (state == "03" && transmitido != null && integrationstate == "08"))//|| state == 3) // APROBADA Y NO TRANSMITIDA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true,
				};
			}
			else if (state == "04" || state == "05" || (transmitido != null && integrationstate != "08")) // CERRADA O ANULADA O TRANSMITIDA
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
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region METHODS

		public ActionResult GetDetailAdvanceProviderRG()
		{
			var model = (TempData["aplRg"] as List<AdvanceProviderPLRG>);
			model = model ?? new List<AdvanceProviderPLRG>();

			TempData.Keep("aplRg");

			return PartialView("_AdvanceProviderPLMainFormTabAdvanceProviderPLDetailRG", model.OrderByDescending(o => o.seq_RemissionGuide).ToList());
		}

		#endregion

		#region Método para calcular el porcentaje de avance de proveedor

		public JsonResult CalculatePercentageAdvanceProvider(AdvanceProvider ap)
		{
			var valuePercentageAdvance = 0m;

			TempData.Keep("aplRg");
			TempData.Keep("AdvanceProviderPL");

			if (ap != null)
			{
				// Recuperamos el nombre del tipo de calendario de la lista de precios
				var id_priceList = ap.id_priceList;
				var nameCalendarPriceListType = db.PriceList
					.FirstOrDefault(fod => fod.id == id_priceList)?
					.CalendarPriceList?
					.CalendarPriceListType?
					.name;

				valuePercentageAdvance = CalcularAnticipoProveedor(nameCalendarPriceListType,
					ap?.QuantityPoundReceived ?? 0m,
					ap?.QuantityPoundsReceivedMinimun ?? 0m);
			}

			var result = new
			{
				valuePercentageAdvance,
			};

			return Json(result);
		}

		private decimal CalcularAnticipoProveedor(string nombreTipoListaPrecio,
			decimal cantidadRecibida, decimal cantidadMinima)
		{
			if (cantidadRecibida < cantidadMinima)
			{
				return 0m;
			}
			else if (nombreTipoListaPrecio == "AGUAJE")
			{
				return db.AdvanceParameters
					.FirstOrDefault(p => p.code == "ALPA")?
					.valueDecimal ?? 0m;
			}
			else if (nombreTipoListaPrecio == "QUIEBRA")
			{
				return db.AdvanceParameters
					.FirstOrDefault(p => p.code == "ALPQ")?
					.valueDecimal ?? 0m;
			}
			else
			{
				return 0m;
			}
		}

		#endregion
	}
}