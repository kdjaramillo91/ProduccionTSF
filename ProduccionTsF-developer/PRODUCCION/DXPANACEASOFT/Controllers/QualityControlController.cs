using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Models.QualityControls;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.CorreoElectronico;
using Utilitarios.Encriptacion;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
	public class QualityControlController : DefaultController
	{
		private QualityControl GetQualityControl()
		{
			if (!(Session["qualityControl"] is QualityControl qualityControl))
				qualityControl = new QualityControl();
			return qualityControl;
		}

		private void SetQualityControl(QualityControl qualityControl)
		{
			Session["qualityControl"] = qualityControl;
		}

		public Dictionary<object, int> RowHashes
		{
			get
			{
				if (Session["GridViewRowAnalysisQuality"] == null)
					Session["GridViewRowAnalysisQuality"] = new Dictionary<object, int>();
				return (Dictionary<object, int>)Session["GridViewRowAnalysisQuality"];
			}
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult Index()
		{
			return PartialView();
		}

		#region QUALITY CONTROL FILTERS RESULTS

		[HttpPost]
		public ActionResult QualityControlResults(Document document,
												  QualityControl qualityControl,
												  string qualityControlNumber,
												  string documentReference,
												  string conforms,
												  DateTime? startEmissionDate, DateTime? endEmissionDate)
		{
			Parametros.ParametrosBusquedaQualityControl parametrosBusquedaQualityControl = new Parametros.ParametrosBusquedaQualityControl();
			parametrosBusquedaQualityControl.id_documentState = document.id_documentState;
			if (!string.IsNullOrEmpty(qualityControlNumber)) parametrosBusquedaQualityControl.qualityControlNumber = qualityControlNumber;
			if (!string.IsNullOrEmpty(documentReference)) parametrosBusquedaQualityControl.documentReference = documentReference;
			parametrosBusquedaQualityControl.startEmissionDate = startEmissionDate;
			parametrosBusquedaQualityControl.endEmissionDate = endEmissionDate;
			parametrosBusquedaQualityControl.id_qualityControlConfiguration = qualityControl.id_qualityControlConfiguration;
			parametrosBusquedaQualityControl.id_analyst = qualityControl.id_analyst;
			if (!string.IsNullOrEmpty(conforms)) parametrosBusquedaQualityControl.conforms = conforms;

			List<QualityControlResultConsultDTO> modelAux = GetQualityControl(parametrosBusquedaQualityControl);
			TempData["model"] = modelAux;
			TempData.Keep("model");

			return PartialView("_QualityControlResultsPartial", modelAux);
		}

		private List<QualityControlResultConsultDTO> GetQualityControl(Parametros.ParametrosBusquedaQualityControl parametrosBusquedaQualityControl)
		{
			var parametrosBusquedaQualityControlAux = new SqlParameter();
			parametrosBusquedaQualityControlAux.ParameterName = "@ParametrosBusquedaQualityControl";
			parametrosBusquedaQualityControlAux.Direction = ParameterDirection.Input;
			parametrosBusquedaQualityControlAux.SqlDbType = SqlDbType.NVarChar;
			var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaQualityControl);
			parametrosBusquedaQualityControlAux.Value = jsonAux;
			db.Database.CommandTimeout = 1200;
			List<QualityControlResultConsultDTO> modelAux = db.Database.SqlQuery<QualityControlResultConsultDTO>("exec qa_Consultar_QualityControl_StoredProcedure @ParametrosBusquedaQualityControl ", parametrosBusquedaQualityControlAux).ToList();
			return modelAux;
		}

		[ValidateInput(false)]
		public ActionResult QualityControlsPartial()
		{
			var model = (TempData["model"] as List<QualityControlResultConsultDTO>);
			model = model ?? new List<QualityControlResultConsultDTO>();

			TempData.Keep("model");

			return PartialView("_QualityControlsPartial", model.OrderByDescending(o => o.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionLotDetailsResults()
		{
			TempData.Keep("model");
			var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");


			var productionLotDetail =
				from e in db.ProductionLotDetail
				where e.ProductionLotDetailQualityControl.Count > 0
					&& e.ProductionLotDetailQualityControl.Any(a => !a.QualityControl.Document.DocumentState.code.Equals("05"))
				select e.id;

			List<QualityControlPendingNewDTO> pld = new List<QualityControlPendingNewDTO>();

			if (aSettingANL != null && aSettingANL.value == "NO")
			{
				pld = db.ProductionLotDetail
				.Where(X => !productionLotDetail.Contains(X.id)
					&& X.ProductionLot.ProductionProcess.name == "RECEPCION"
					&& X.ProductionLot.ProductionLotState.code == "01")
				.Distinct().Select(s => new QualityControlPendingNewDTO
				{
					id = s.id,
					number = s.ProductionLot.number,
					internalNumber = s.ProductionLot.internalNumber,
					receptionDate = s.ProductionLot.receptionDate,
					//remissionGuideNumber = DataProviderQualityControl.RemissionGuideNumber(s.id),
					remissionGuideNumber = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null) ?
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Document.number : "",
					//remissionGuideNumberExterna = DataProviderQualityControl.RemissionGuideNumberExterna(s.id),
					remissionGuideNumberExterna = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
					                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
					                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null &&
					                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null) ?
					                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Guia_Externa : "",
					//remissionGuideNumberExterna = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
					//                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
					//                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null &&
					//                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.RemissionGuideTransportationCustomizedInformation != null) ?
					//                                      s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.RemissionGuideTransportationCustomizedInformation.numberGuide : "",
					//remissionGuideProcess = DataProviderQualityControl.RemissionGuideProcess(s.id),
					remissionGuideProcess = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Person2 != null) ?
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Person2.processPlant : "",
					proveedor = s.ProductionLot.Provider.Person.fullname_businessName,
					productionUnitProvider = s.ProductionLot.ProductionUnitProvider.name,
					name_item = s.Item.name,
					quantityRecived = s.quantityRecived,
					quantitydrained = s.quantitydrained,
					um = (s.Item != null && s.Item.ItemPurchaseInformation != null && s.Item.ItemPurchaseInformation.MetricUnit != null) ? s.Item.ItemPurchaseInformation.MetricUnit.code : ""
				})
				.ToList();
			}
			else if (aSettingANL != null && aSettingANL.value == "SI")
			{
				pld = db.ProductionLotDetail
				.Where(X => !productionLotDetail.Contains(X.id)
					&& X.ProductionLot.ProductionProcess.name == "RECEPCION"
					&& X.ProductionLot.ProductionLotState.code == "01").GroupBy(X => new { X.ProductionLot.number })
					.Select(s => new QualityControlPendingNewDTO
					{
						id = s.Max(r => r.id),
						number = s.Max(r => r.ProductionLot.number),
						internalNumber = s.Max(r => r.ProductionLot.internalNumber),
						receptionDate = s.Max(r => r.ProductionLot.receptionDate),

						remissionGuideNumber = "",

						remissionGuideNumberExterna = "",

						remissionGuideProcess = s.Max(r => r.ProductionLot.Person1.processPlant ?? ""),
						proveedor = s.Max(r => r.ProductionLot.Provider.Person.fullname_businessName),
						productionUnitProvider = s.Max(r => r.ProductionLot.ProductionUnitProvider.name),
						name_item = s.Max(r => r.Item.name),
						quantityRecived = s.Max(r => r.ProductionLot.totalQuantityRecived),
						quantitydrained = s.Sum(r => r.quantitydrained),
						um = s.Max(r => r.Item.ItemPurchaseInformation.MetricUnit.code ?? "")

					})
				.ToList();
			}
			else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
			{
				//var ls1 = db.ProductionLotDetailQualityControl.Select(s => s.id_productionLotDetail).ToList();
				//pld = db.ProductionLotDetail
				//.Where(X => !productionLotDetail.Contains(X.id)
				//    && ls1.Contains(X.id) 
				//    && X.ProductionLot.ProductionProcess.name == "RECEPCION"
				//    && X.ProductionLot.ProductionLotState.code == "01").GroupBy(X => new { X.ProductionLot.number })
				//    .Select(s => new QualityControlPendingNewDTO
				//    {
				//        id = s.Max(r => r.id),
				//        number = s.Max(r => r.ProductionLot.number),
				//        internalNumber = s.Max(r => r.ProductionLot.internalNumber),
				//        receptionDate = s.Max(r => r.ProductionLot.receptionDate),

				//        remissionGuideNumber = "",

				//        remissionGuideNumberExterna = "",

				//        remissionGuideProcess = s.Max(r => r.ProductionLot.Person1.processPlant ?? ""),
				//        proveedor = s.Max(r => r.ProductionLot.Provider.Person.fullname_businessName),
				//        productionUnitProvider = s.Max(r => r.ProductionLot.ProductionUnitProvider.name),
				//        name_item = s.Max(r => r.Item.name),
				//        quantityRecived = s.Sum(r => r.ProductionLotDetailQualityControl.Sum(x => x.ProductionLotDetail.quantityRecived)),
				//        //quantityRecived = s.Sum(r => s.Sum(s => s.ProductionLotDetailQualityControl.sum)) s.Max(r => r.ProductionLot.totalQuantityRecived),
				//        quantitydrained = s.Sum(r => r.ProductionLotDetailQualityControl.Sum(x => x.ProductionLotDetail.quantitydrained)),
				//        //quantitydrained = s.Sum(r => r.quantitydrained),
				//        um = s.Max(r => r.Item.ItemPurchaseInformation.MetricUnit.code ?? "")

				//    }).Distinct()
				//    .ToList();
				pld = db.ProductionLotDetail
				.Where(X => !productionLotDetail.Contains(X.id)
					&& X.ProductionLot.ProductionProcess.name == "RECEPCION"
					&& X.ProductionLot.ProductionLotState.code == "01")
				.Distinct().Select(s => new QualityControlPendingNewDTO
				{
					id = s.id,
					number = s.ProductionLot.number,
					internalNumber = s.ProductionLot.internalNumber,
					receptionDate = s.ProductionLot.receptionDate,
					//remissionGuideNumber = DataProviderQualityControl.RemissionGuideNumber(s.id),
					remissionGuideNumber = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null) ?
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Document.number : "",
                    //remissionGuideNumberExterna = DataProviderQualityControl.RemissionGuideNumberExterna(s.id),
                    remissionGuideNumberExterna = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
                                           s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
                                           s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null &&
                                           s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null) ?
                                           s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Guia_Externa : "",
                    //remissionGuideProcess = DataProviderQualityControl.RemissionGuideProcess(s.id),
                    remissionGuideProcess = (s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id) != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide != null &&
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Person2 != null) ?
										   s.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == s.id).RemissionGuideDetail.RemissionGuide.Person2.processPlant : "",
					proveedor = s.ProductionLot.Provider.Person.fullname_businessName,
					productionUnitProvider = s.ProductionLot.ProductionUnitProvider.name,
					name_item = s.Item.name,
					quantityRecived = s.quantityRecived,
					quantitydrained = s.quantitydrained,
					um = (s.Item != null && s.Item.ItemPurchaseInformation != null && s.Item.ItemPurchaseInformation.MetricUnit != null) ? s.Item.ItemPurchaseInformation.MetricUnit.code : ""
				})
				.ToList();
			}

			TempData["pldListResults"] = pld;
			TempData.Keep("pldListResults");
			return PartialView("RMP/_ProductionLotDetailsResultsPartial", pld.OrderByDescending(d => d.id).ToList());
		}

		public ActionResult ProductionLotDetailsPartial()
		{
			List<QualityControlPendingNewDTO> pld = new List<QualityControlPendingNewDTO>();
			if (TempData["pldListResults"] != null)
			{
				pld = (List<QualityControlPendingNewDTO>)TempData["pldListResults"];
			}
			TempData["pldListResults"] = pld;
			TempData.Keep("pldListResults");
			return PartialView("RMP/_ProductionLotDetailsPartial", pld);
		}
		#endregion

		#region QUALITY CONTROL EDITFORM
		public ActionResult ValidateParamAnalisisPorLote(int[] arr_id_productionLotDetail)
		{
			var respuesta = new
			{
				tieneError = "NO",
				tieneAdvertencia = "NO",
				mensaje = ""
			};

			var sumQuantityReceived = db.ProductionLotDetail
										.Where(w => arr_id_productionLotDetail.Contains(w.id))
										.Select(s => s.quantityRecived).Sum();

			var countProductionLotNumber = db.ProductionLotDetail
							.Where(w => arr_id_productionLotDetail.Contains(w.id))
							.Select(s => s.ProductionLot.number).Distinct().ToList();

			if (countProductionLotNumber.Count > 1)
			{
				respuesta = new
				{
					tieneError = "SI",
					tieneAdvertencia = "SI",
					mensaje = "Debe seleccionar registros de un mismo lote."
				};
				return Json(respuesta, JsonRequestBehavior.AllowGet);
			};


			decimal cantLibras = 0;
			var strCantLibras = db.SettingDetail
										.FirstOrDefault(fod => fod.Setting.code == "ANALXLOT"
										&& fod.value == "CANTLIB")?.valueAux;

			decimal.TryParse(strCantLibras, out cantLibras);

			if (cantLibras != sumQuantityReceived)
			{
				respuesta = new
				{
					tieneError = "NO",
					tieneAdvertencia = "SI",
					mensaje = "La cantidad de libras seleccionadas es diferente a la cantidad parametrizada. ¿Desea Continuar?"
				};
			}

			return Json(respuesta, JsonRequestBehavior.AllowGet);
		}

		private void llenarTempDataProductionLot(int? idProductionLotDetail)
		{
			if (!idProductionLotDetail.HasValue || idProductionLotDetail == 0) return;

			var detalle = db.ProductionLotDetail.FirstOrDefault(e => e.id == idProductionLotDetail);
			TempData["productionLotQuality"] = detalle.ProductionLot;
			TempData.Keep("productionLotQuality");

        }

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditQualityControl(int id
			, int? id_productionLotDetail
			, string paramAnalisisPorLote
			, int[] arr_id_productionLotDetail
			, string[] arrayTempDataKeep, bool? hasUpdate)
		{
			UpdateArrayTempDataKeep(arrayTempDataKeep);
			var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
			if (hasUpdate == null)
			{
				hasUpdate = true;
			}
			bool originMultipleGuides = false;

			if (arr_id_productionLotDetail != null
				&& arr_id_productionLotDetail.Length > 0
				&& paramAnalisisPorLote == "SELGUIA")
			{
				originMultipleGuides = true;
			}
			originMultipleGuides = aSettingANL.value == "SELGUIA" ? true : false;
			ProductionLot tempProductionLot = new ProductionLot();
			ProductionLotDetail productionLotDetail = null;


			llenarTempDataProductionLot(id_productionLotDetail);


            if (!originMultipleGuides)
			{
				if (id_productionLotDetail != null && TempData["productionLotQuality"] != null)
				{
					tempProductionLot = (TempData["productionLotQuality"] as ProductionLot);
					tempProductionLot = tempProductionLot ?? new ProductionLot();
					productionLotDetail = tempProductionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);
					if (productionLotDetail != null)
					{
						id = productionLotDetail.ProductionLotDetailQualityControl?.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code != ("05"))?.id_qualityControl ?? 0;//"05"Anulado
						QualityControl qualityControlAux = db.QualityControl.FirstOrDefault(o => o.id == id);
						if ((qualityControlAux?.Document?.DocumentState?.code ?? "05") == "05")
						{
							id = 0;
						}
					}

				}
				else if (id_productionLotDetail != null)
				{
					tempProductionLot = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail)?.ProductionLot;
					productionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);
				}
				if (productionLotDetail == null)
				{
					productionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);
				}
			}
			else
			{
				if (TempData["productionLotQuality"] != null)
				{
					tempProductionLot = (TempData["productionLotQuality"] as ProductionLot);
					tempProductionLot = tempProductionLot ?? new ProductionLot();
					productionLotDetail = tempProductionLot?.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);
					if (productionLotDetail != null)
					{
						id = productionLotDetail.ProductionLotDetailQualityControl?.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code != ("05"))?.id_qualityControl ?? 0;//"05"Anulado
						QualityControl qualityControlAux = db.QualityControl.FirstOrDefault(o => o.id == id);
						if ((qualityControlAux?.Document?.DocumentState?.code ?? "05") == "05")
						{
							id = 0;
						}
					}

				}
			}
			QualityControl qualityControl = db.QualityControl.FirstOrDefault(o => o.id == id);
			if (qualityControl != null)
			{
				if (qualityControl.isConformityOnHeader == true)
				{
					qualityControl.QualityControlResultConformityOnHeaderValue = db.QualityControlResultConformityOnHeaderValue.FirstOrDefault(fod => fod.id_QualityControl == qualityControl.id);
				}
			}

			#region"SI ES NUEVO"
			if (qualityControl == null)
			{
				DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code.Equals("01"));

				Employee employee = ActiveUser.Employee;
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("61"));//Control de Calidad

				qualityControl = new QualityControl
				{
					Document = new Document
					{
						id_documentState = documentState?.id ?? 0,
						DocumentState = documentState,
						id_documentType = documentType?.id ?? 0,
						DocumentType = documentType,

						emissionDate = DateTime.Now
					},
					id_analyst = employee?.id ?? 0,
					Employee = employee,
					isConforms = false,
					QualityControlDetail = new List<QualityControlDetail>()
				};
				qualityControl.totalWeightSample = 0.00M;
				if (id_productionLotDetail != null && !originMultipleGuides)
				{
					QualityControlConfiguration qualityControlConfiguration;
					var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);

					if (tempProductionLot.ProductionProcess.code == ("REC"))
					{
						qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(fod => fod.code.Equals("QARMP"));//"QARMP": Tipo de Control de Calidad de Recepción  MP
					}
					else
					{
						qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(fod => fod.code.Equals("QARLI"));//"QARLI": Tipo de Control de Calidad de Recepción LI
					}
					if (qualityControl.id == 0)
					{
						if ((bool)qualityControlConfiguration.QualityControlConfigurationConformity.isConformityOnHeader)
						{
							qualityControl.QualityControlResultConformityOnHeaderValue = new QualityControlResultConformityOnHeaderValue();
							qualityControl.isConformityOnHeader = true;
						}
						else
						{
							qualityControl.isConformityOnHeader = false;
						}
					}
					else
					{
						if ((bool)qualityControl.isConformityOnHeader)
						{
							qualityControl.QualityControlResultConformityOnHeaderValue = qualityControl.QualityControlResultConformityOnHeaderValue ?? new QualityControlResultConformityOnHeaderValue();
						}
					}

					qualityControl.id_qualityControlConfiguration = qualityControlConfiguration.id;
					qualityControl.QualityControlConfiguration = qualityControlConfiguration;

					qualityControl.id_warehouse = productionLotDetail.id_warehouse;
					qualityControl.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == productionLotDetail.id_warehouse);

					qualityControl.id_warehouseLocation = productionLotDetail.id_warehouseLocation;
					qualityControl.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == productionLotDetail.id_warehouseLocation);

					qualityControl.id_item = productionLotDetail.id_item;
					qualityControl.Item = db.Item.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);

					qualityControl.id_lot = tempProductionLot.id != 0 ? tempProductionLot.id : (int?)null;
					qualityControl.Lot = db.Lot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);

					if (aSettingANL != null && aSettingANL.value == "NO")
					{
						//Agregar Libras Recibidas antes
						qualityControl.QuantityPoundsReceived = pldTmp?.quantityRecived ?? 0;
						//Ahora
						//qualityControl.QuantityPoundsReceived = pldTmp?.quantitydrained ?? 0;
						qualityControl.DrawersReceived = pldTmp?.drawersNumber ?? 0;
						//Agregar Guía de Remisión
						int id_rgdTmp = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == id_productionLotDetail)?.id_remissionGuideDetail ?? 0;

						if (id_rgdTmp > 0)
						{
							qualityControl.remissionGuideNumber = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Document.number;
							qualityControl.remissionGuideExternalGuide = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Guia_Externa;
							qualityControl.remissionGuideProcess = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Person2.processPlant;
						}

						qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

						qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
						{
							id_productionLotDetail = productionLotDetail.id,
							IsDelete = false,
							//2021 08 09
							ProductionLotDetail = productionLotDetail
						});

					}
					else if (aSettingANL != null && aSettingANL.value == "SI")
					{
						tempProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);
						qualityControl.QuantityPoundsReceived = tempProductionLot?.totalQuantityRecived ?? 0;
						qualityControl.remissionGuideNumber = "";
						qualityControl.remissionGuideExternalGuide = string.Empty;
						qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";
						qualityControl.DrawersReceived = tempProductionLot.ProductionLotDetail.Sum(r => r.drawersNumber);

						qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

						foreach (var aDetail in tempProductionLot.ProductionLotDetail)
						{
							qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
							{
								id_productionLotDetail = aDetail.id,
								IsDelete = false,
								//2021 08 09
								ProductionLotDetail = aDetail
							});
						}
					}

					//Agregar Tipo de Proceso
					if (pldTmp != null)
					{
						qualityControl.id_processType = db.ItemProcessType.FirstOrDefault(fod => fod.Id_Item == pldTmp.id_item)?.Id_ProcessType ?? 0;
					}

					//Agregar Piscina
					if (pldTmp != null)
					{
						qualityControl.poolName = pldTmp.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
					}

					SetQualityControl(qualityControl);

					RefreshQualityControlDetail(qualityControl.id_qualityControlConfiguration);
					qualityControl = GetQualityControl();
				}
				else if (originMultipleGuides)
				{
					QualityControlConfiguration qualityControlConfiguration;
					//var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetail);
					var lsPldSelected = db.ProductionLotDetail.Where(w => arr_id_productionLotDetail.Contains(w.id)).ToList();
					var pldTmp = lsPldSelected.FirstOrDefault();
					if (pldTmp?.ProductionLot?.ProductionProcess.code == ("REC"))
					{
						qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(fod => fod.code.Equals("QARMP"));//"QARMP": Tipo de Control de Calidad de Recepción  MP
					}
					else
					{
						qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(fod => fod.code.Equals("QARLI"));//"QARLI": Tipo de Control de Calidad de Recepción LI
					}
					if (qualityControl.id == 0)
					{
						if ((bool)qualityControlConfiguration.QualityControlConfigurationConformity.isConformityOnHeader)
						{
							qualityControl.QualityControlResultConformityOnHeaderValue = new QualityControlResultConformityOnHeaderValue();
							qualityControl.isConformityOnHeader = true;
						}
						else
						{
							qualityControl.isConformityOnHeader = false;
						}
					}
					else
					{
						if ((bool)qualityControl.isConformityOnHeader)
						{
							qualityControl.QualityControlResultConformityOnHeaderValue = qualityControl.QualityControlResultConformityOnHeaderValue ?? new QualityControlResultConformityOnHeaderValue();
						}
					}

					qualityControl.id_qualityControlConfiguration = qualityControlConfiguration.id;
					qualityControl.QualityControlConfiguration = qualityControlConfiguration;

					qualityControl.id_warehouse = lsPldSelected.FirstOrDefault().id_warehouse;
					qualityControl.Warehouse = lsPldSelected.FirstOrDefault().Warehouse ?? new Warehouse();

					qualityControl.id_warehouseLocation = lsPldSelected.FirstOrDefault().id_warehouseLocation;
					qualityControl.WarehouseLocation = lsPldSelected.FirstOrDefault().WarehouseLocation ?? new WarehouseLocation();

					qualityControl.id_item = lsPldSelected.FirstOrDefault().id_item;
					qualityControl.Item = lsPldSelected.FirstOrDefault().Item ?? new Item();
					//db.Item.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);

					qualityControl.id_lot = lsPldSelected.FirstOrDefault().id_productionLot;
					//tempProductionLot.id != 0 ? tempProductionLot.id : (int?)null;
					qualityControl.Lot = lsPldSelected.FirstOrDefault().ProductionLot?.Lot ?? new Lot();

					qualityControl.QuantityPoundsReceived = lsPldSelected.Sum(s => s.quantityRecived);
					//tempProductionLot?.totalQuantityRecived ?? 0;
					qualityControl.remissionGuideNumber = "";
					qualityControl.remissionGuideProcess = lsPldSelected.FirstOrDefault().ProductionLot?.Person1?.processPlant ?? "";
					qualityControl.DrawersReceived = lsPldSelected.Sum(r => r.drawersNumber);

					qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

					foreach (var aDetail in lsPldSelected)
					{
						qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
						{
							id_productionLotDetail = aDetail.id,
							IsDelete = false,
							//2021 08 09
							ProductionLotDetail = aDetail
						});
					}


					//Agregar Tipo de Proceso
					if (pldTmp != null)
					{
						qualityControl.id_processType = db.ItemProcessType.FirstOrDefault(fod => fod.Id_Item == pldTmp.id_item)?.Id_ProcessType ?? 0;
					}

					//Agregar Piscina
					if (pldTmp != null)
					{
						qualityControl.poolName = pldTmp.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
					}

					SetQualityControl(qualityControl);

					RefreshQualityControlDetail(qualityControl.id_qualityControlConfiguration);
					qualityControl = GetQualityControl();
				}
			}
			#endregion
			else
			{
				//Agregar Guía de Remission
				var idPld = db.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == qualityControl.id)?.id_productionLotDetail;
				var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idPld);
				qualityControl.remissionGuideNumber = "";
				qualityControl.remissionGuideProcess = "";

				if (aSettingANL != null && aSettingANL.value == "NO")
				{
					//Ahora
					qualityControl.QuantityPoundsReceived = pldTmp?.quantitydrained ?? 0;
					//Agregar Guía de Remisión

					int id_rgdTmp = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idPld)?.id_remissionGuideDetail ?? 0;

					if (id_rgdTmp > 0)
					{
						qualityControl.remissionGuideNumber = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Document.number;
						qualityControl.remissionGuideProcess = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Person2.processPlant;
					}

					if (qualityControl.ProductionLotDetailQualityControl == null || qualityControl.ProductionLotDetailQualityControl.Count() == 0)
					{
						qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

						qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
						{
							id_productionLotDetail = productionLotDetail.id,
							IsDelete = false,
							ProductionLotDetail = productionLotDetail
						});
					}

				}
				else if (aSettingANL != null && aSettingANL.value == "SI")
				{
					tempProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);
					qualityControl.QuantityPoundsReceived = tempProductionLot?.totalQuantityRecived ?? 0;
					qualityControl.remissionGuideNumber = "";
					qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";
					qualityControl.DrawersReceived = tempProductionLot.ProductionLotDetail.Sum(r => r.drawersNumber);

					if (qualityControl.ProductionLotDetailQualityControl == null || qualityControl.ProductionLotDetailQualityControl.Count() == 0)
					{
						qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

						foreach (var aDetail in tempProductionLot.ProductionLotDetail)
						{
							qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
							{
								id_productionLotDetail = aDetail.id,
								IsDelete = false,
								ProductionLotDetail = aDetail
							});
						}

					}
				}
				else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
				{

					tempProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);
					qualityControl.QuantityPoundsReceived = qualityControl.ProductionLotDetailQualityControl.Select(s => s.ProductionLotDetail.quantityRecived).Sum();
					//qualityControl.QuantityPoundsReceived = tempProductionLot?.totalQuantityRecived ?? 0;
					qualityControl.remissionGuideNumber = "";
					qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";
					qualityControl.DrawersReceived = qualityControl.ProductionLotDetailQualityControl.Select(s => s.ProductionLotDetail.drawersNumber).Sum();

					//qualityControl.DrawersReceived = tempProductionLot.ProductionLotDetail.Sum(r => r.drawersNumber);

					//if (qualityControl.ProductionLotDetailQualityControl == null || qualityControl.ProductionLotDetailQualityControl.Count() == 0)
					//{
					//    qualityControl.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

					//    foreach (var aDetail in tempProductionLot.ProductionLotDetail)
					//    {
					//        qualityControl.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
					//        {
					//            id_productionLotDetail = aDetail.id,
					//            IsDelete = false,
					//            ProductionLotDetail = aDetail
					//        });
					//    }

					//}
				}
				//Agregar Piscina
				qualityControl.poolName = qualityControl?.Lot?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
			}

			if (qualityControl.iceContamination != "SI")
				qualityControl.iceContamination = "NO";
			if (qualityControl.PCC != "SI")
				qualityControl.PCC = "NO";
			if (qualityControl.transportCondition != "Sucio")
				qualityControl.transportCondition = "Limpio";
			if (qualityControl.transportState != "NO")
				qualityControl.transportState = "SI";
			if (qualityControl.transportOnlyShrimp != "NO")
				qualityControl.transportOnlyShrimp = "SI";
			ViewBagEdit();
			qualityControl.hasUpd = hasUpdate;

			SetQualityControl(qualityControl);
			UpdateViewBagTotalUnit(GetQualityControl());

			TempData["id_qualityControlConfiguration"] = qualityControl.id_qualityControlConfiguration;
			TempData.Keep("id_qualityControlConfiguration");

			if (id_productionLotDetail != null && !(bool)hasUpdate)
			{
				ViewData["ModelLink"] = qualityControl;
				return PartialView("LinkBoxTemplates/_LinkBox", "_FormEditQualityControl");
			}
			else
			{
				return PartialView("_FormEditQualityControl", qualityControl);
			}
		}

		private void UpdateViewBagTotalUnit(QualityControl qualityControl)
		{
			decimal amoutTotalUnit = 0.00M;
			foreach (var aDetail in qualityControl.QualityControlDetail)
			{
				if (db.QualityControlAnalysisGroupAnalysis.FirstOrDefault(fod => fod.id_QualityAnalysis == aDetail.id_qualityAnalysis && (fod.QualityControlAnalysisGroup.code == "TOTDEF" || fod.QualityControlAnalysisGroup.code == "TALENT" || fod.QualityControlAnalysisGroup.code == "TALCOL")) != null)
				{
					decimal resultAux = 0.00M;
					decimal.TryParse((aDetail.otherResultValue ?? "").Replace('.', ','), out resultAux);
					amoutTotalUnit += resultAux;
				}
			}
			ViewBag.totalUnit = amoutTotalUnit;
		}

		private void UpdateViewBagNADCTD(QualityControl qualityControl)
		{
			decimal amoutNADCTD = 0.00M;
			var aSettingDetailNADCTD = db.SettingDetail.Where(w => w.Setting.code.Equals("NADCTD")).ToList();
			foreach (var aDetail in qualityControl.QualityControlDetail)
			{
				var nameQualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == aDetail.id_qualityAnalysis)?.name;
				if (aSettingDetailNADCTD.FirstOrDefault(fod => fod.value == nameQualityAnalysis) != null)
				{
					decimal resultAux = 0.00M;
					decimal.TryParse((aDetail.resultValue).Replace('.', ','), out resultAux);
					amoutNADCTD += resultAux;
				}
			}
			ViewBag.amoutNADCTD = amoutNADCTD;
		}

		private void ViewBagEdit()
		{
			var aSelectListItemYesOrNo = new List<SelectListItem>();
			aSelectListItemYesOrNo.Add(new SelectListItem
			{
				Text = "NO",
				Value = "NO"
			});
			aSelectListItemYesOrNo.Add(new SelectListItem
			{
				Text = "SI",
				Value = "SI"
			});
			ViewBag.OptionYesORNo = aSelectListItemYesOrNo;

			var aSelectListItemCleanORDirty = new List<SelectListItem>();
			aSelectListItemCleanORDirty.Add(new SelectListItem
			{
				Text = "Limpio",
				Value = "Limpio"
			});
			aSelectListItemCleanORDirty.Add(new SelectListItem
			{
				Text = "Sucio",
				Value = "Sucio"
			});
			ViewBag.OptionCleanORDirty = aSelectListItemCleanORDirty;
		}

		#endregion

		#region QUALITY CONTROL HEADERS

		[HttpPost, ValidateInput(false)]
		public ActionResult QualityControlPartialAddNew(string qualityControlTime, string monerTime, bool approve, string qualityControlTimeTmp, string documentReference, string documentDescription,
															QualityControl qualityControl, Document document, DateTime documentEmissionDate, ShrimpSupplierTraceability shrimpSupplierTraceability)
		{
			string ruta = ConfigurationManager.AppSettings["rutaLog"];
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			//var model = db.QualityControl;
			try
			{
				QualityControl tempQualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
																		//tempQualityControl = tempQualityControl ?? new QualityControl();

				var qualityControlError = tempQualityControl;

				qualityControl.id_qualityControlConfiguration = qualityControl.id_qualityControlConfiguration;
				qualityControl.QualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(e => e.id == qualityControl.id_qualityControlConfiguration);

				qualityControlError.id_warehouse = qualityControl.id_warehouse;
				qualityControlError.Warehouse = db.Warehouse.FirstOrDefault(e => e.id == qualityControl.id_warehouse);

				qualityControlError.id_warehouseLocation = qualityControl.id_warehouseLocation;
				qualityControlError.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(e => e.id == qualityControl.id_warehouseLocation);

				qualityControlError.id_item = qualityControl.id_item;
				qualityControlError.Item = db.Item.FirstOrDefault(e => e.id == qualityControl.id_item);

				qualityControlError.id_lot = qualityControl.id_lot == 0 ? null : qualityControl.id_lot;
				qualityControlError.Lot = db.Lot.FirstOrDefault(e => e.id == qualityControl.id_lot);
				qualityControlError.remissionGuideProcess = qualityControlError.Lot?.ProductionLot?.Person1?.processPlant ?? "";

				qualityControlError.id_analyst = qualityControl.id_analyst;
				qualityControlError.Employee = db.Employee.FirstOrDefault(e => e.id == qualityControl.id_analyst);

				qualityControlError.reference = documentReference;
				qualityControlError.Document.reference = documentReference;
				qualityControlError.description = documentDescription;
				qualityControlError.Document.description = documentDescription;
				qualityControlError.Document.emissionDate = documentEmissionDate;

				qualityControlError.id_size = qualityControl.id_size;
				qualityControlError.iceContamination = qualityControl.iceContamination;
				qualityControlError.transportCondition = qualityControl.transportCondition;
				qualityControlError.transportState = qualityControl.transportState;
				qualityControlError.transportOnlyShrimp = qualityControl.transportOnlyShrimp;
				qualityControlError.totalWeightSample = qualityControl.totalWeightSample;
				qualityControlError.PCC = qualityControl.PCC;
				qualityControlError.monerDate = qualityControl.monerDate;
				qualityControlError.monerTime = qualityControl.monerTime;
				qualityControlError.monerResultados = qualityControl.monerResultados;

				qualityControlError.ShrimpSupplierTraceability = shrimpSupplierTraceability;



				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{

						#region Document

						document.id_userCreate = ActiveUser.id;
						document.dateCreate = DateTime.Now;
						document.id_userUpdate = ActiveUser.id;
						document.dateUpdate = DateTime.Now;

						document.emissionDate = documentEmissionDate;


						document.sequential = GetDocumentSequential(tempQualityControl.Document.id_documentType);//document.id_documentType);
						document.number = GetDocumentNumber(tempQualityControl.Document.id_documentType);//document.id_documentType);

						DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == tempQualityControl.Document.id_documentType);//document.id_documentType);
						document.DocumentType = documentType;

						DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == document.id_documentState);
						document.DocumentState = documentState;

						document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
						document.id_emissionPoint = ActiveEmissionPoint.id;

						document.reference = documentReference;
						document.description = documentDescription;

						//Actualiza Secuencial
						if (documentType != null)
						{
							documentType.currentNumber = documentType.currentNumber + 1;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;
						}

						#endregion

						#region QualityControl

						qualityControl.Document = document;

						qualityControl.id_qualityControlConfiguration = qualityControl.id_qualityControlConfiguration;
						var qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(e => e.id == qualityControl.id_qualityControlConfiguration);
						qualityControl.QualityControlConfiguration = qualityControlConfiguration;

						string code = qualityControlConfiguration?.code ?? "";
						string sequential = qualityControlConfiguration?.sequential.ToString("D9") ?? "";

						qualityControl.number = code + sequential;

						//Actualizar el number del Control de Calidad

						if (qualityControlConfiguration != null)
						{
							qualityControlConfiguration.sequential++;
							db.QualityControlConfiguration.Attach(qualityControlConfiguration);
							db.Entry(qualityControlConfiguration).State = EntityState.Modified;
						}

						qualityControl.id_warehouse = qualityControl.id_warehouse;
						qualityControl.Warehouse = db.Warehouse.FirstOrDefault(e => e.id == qualityControl.id_warehouse);

						qualityControl.DrawersReceived = qualityControl.DrawersReceived ?? 0;
						qualityControl.id_warehouseLocation = qualityControl.id_warehouseLocation;
						qualityControl.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(e => e.id == qualityControl.id_warehouseLocation);

						qualityControl.id_item = qualityControl.id_item;
						qualityControl.Item = db.Item.FirstOrDefault(e => e.id == qualityControl.id_item);

						qualityControl.id_lot = qualityControl.id_lot == 0 ? null : qualityControl.id_lot;
						qualityControl.Lot = db.Lot.FirstOrDefault(e => e.id == qualityControl.id_lot);

						qualityControl.id_analyst = qualityControl.id_analyst;
						qualityControl.wholePerformance = qualityControl.wholePerformance;
						qualityControl.Employee = db.Employee.FirstOrDefault(e => e.id == qualityControl.id_analyst);

						qualityControl.reference = documentReference;



						qualityControl.QuantityPoundsReceived = qualityControl.QuantityPoundsReceived;
						qualityControl.description = documentDescription;

						qualityControl.id_company = this.ActiveCompanyId;

						qualityControl.grammageReference = qualityControl.grammageReference;
						qualityControl.id_processType = qualityControl.id_processType;
						qualityControl.qualityControlDate = qualityControl.qualityControlDate;

						qualityControl.residual = qualityControl.residual;
						qualityControl.temperature = qualityControl.temperature;

						qualityControl.id_size = qualityControl.id_size;
						qualityControl.iceContamination = qualityControl.iceContamination;
						qualityControl.transportCondition = qualityControl.transportCondition;
						qualityControl.transportState = qualityControl.transportState;
						qualityControl.transportOnlyShrimp = qualityControl.transportOnlyShrimp;
						qualityControl.PCC = qualityControl.PCC;
						qualityControl.monerDate = qualityControl.monerDate;

						if (monerTime != null)
						{
							monerTime = monerTime.Substring(11, (monerTime.Length - 11));
						}
						else
						{
							monerTime = qualityControlTime.Substring(11, (qualityControlTime.Length - 11));
						}

						if (!string.IsNullOrEmpty(monerTime)) qualityControl.monerTime = TimeSpan.Parse(monerTime);
						qualityControl.monerResultados = qualityControl.monerResultados ?? 0;

						#region ShrimpSupplierTraceability
						var aSettingTPC = db.Setting.FirstOrDefault(fod => fod.code == "TPC");
						if (aSettingTPC != null && aSettingTPC.value == "SI")
						{
							qualityControl.ShrimpSupplierTraceability = new ShrimpSupplierTraceability();
							qualityControl.ShrimpSupplierTraceability.sowingDate = shrimpSupplierTraceability.sowingDate;
							qualityControl.ShrimpSupplierTraceability.harvestDate = shrimpSupplierTraceability.harvestDate;

							qualityControl.ShrimpSupplierTraceability.id_suppliesNauplius = shrimpSupplierTraceability.id_suppliesNauplius;
							qualityControl.ShrimpSupplierTraceability.id_providerNauplius = shrimpSupplierTraceability.id_providerNauplius;
							qualityControl.ShrimpSupplierTraceability.observationNauplius = shrimpSupplierTraceability.observationNauplius;

							qualityControl.ShrimpSupplierTraceability.id_suppliesLarva = shrimpSupplierTraceability.id_suppliesLarva;
							qualityControl.ShrimpSupplierTraceability.id_providerLarva = shrimpSupplierTraceability.id_providerLarva;
							qualityControl.ShrimpSupplierTraceability.observationLarva = shrimpSupplierTraceability.observationLarva;

							qualityControl.ShrimpSupplierTraceability.id_suppliesBalanced = shrimpSupplierTraceability.id_suppliesBalanced;
							qualityControl.ShrimpSupplierTraceability.id_providerBalanced = shrimpSupplierTraceability.id_providerBalanced;
							qualityControl.ShrimpSupplierTraceability.observationBalanced = shrimpSupplierTraceability.observationBalanced;

							qualityControl.ShrimpSupplierTraceability.id_suppliesFertilizer = shrimpSupplierTraceability.id_suppliesFertilizer;
							qualityControl.ShrimpSupplierTraceability.id_providerFertilizer = shrimpSupplierTraceability.id_providerFertilizer;
							qualityControl.ShrimpSupplierTraceability.observationFertilizer = shrimpSupplierTraceability.observationFertilizer;

							qualityControl.ShrimpSupplierTraceability.id_suppliesFilesOrOthers = shrimpSupplierTraceability.id_suppliesFilesOrOthers;
							qualityControl.ShrimpSupplierTraceability.id_providerFilesOrOthers = shrimpSupplierTraceability.id_providerFilesOrOthers;
							qualityControl.ShrimpSupplierTraceability.observationFilesOrOthers = shrimpSupplierTraceability.observationFilesOrOthers;
						}

						#endregion

						qualityControlTime = qualityControlTime.Substring(11, (qualityControlTime.Length - 11));
						if (!string.IsNullOrEmpty(qualityControlTime)) qualityControl.qualityControlTime = TimeSpan.Parse(qualityControlTime);


						//var existNotConforms = tempQualityControl.QualityControlDetail.FirstOrDefault(fod=> !fod.isConforms) != null;
						var isConformityOnHeader = db.QualityControlConfigurationConformity.FirstOrDefault(fod => fod.id_qualityControlConfiguration == qualityControl.id_qualityControlConfiguration).isConformityOnHeader;
						if (isConformityOnHeader == null)
						{
							isConformityOnHeader = false;
						}
						qualityControl.isConformityOnHeader = isConformityOnHeader;
						if (isConformityOnHeader == false)
						{
							qualityControl.isConforms = tempQualityControl.QualityControlDetail.FirstOrDefault(fod => !fod.isConforms) == null;
						}
						else
						{
							if (qualityControl.QualityControlResultConformityOnHeaderValue.id_QualityControlResultConformity != 0)
							{
								qualityControl.QualityControlResultConformityOnHeaderValue = qualityControl.QualityControlResultConformityOnHeaderValue ?? new QualityControlResultConformityOnHeaderValue();

								int? id_qualityControlResultConformity = qualityControl.QualityControlResultConformityOnHeaderValue.QualityControlResultConformity.id;
								var qualityCRC = db.QualityControlResultConformity.FirstOrDefault(fod => fod.id == id_qualityControlResultConformity);
								bool isConformsResult = (qualityCRC != null && qualityCRC.isConforms != null) ? (bool)qualityCRC.isConforms : false;
								qualityControl.isConforms = isConformsResult;
								qualityControl.QualityControlResultConformityOnHeaderValue.QualityControlResultConformity = qualityControl.QualityControlResultConformityOnHeaderValue.QualityControlResultConformity = db.QualityControlResultConformity.FirstOrDefault(fod => fod.id == id_qualityControlResultConformity);
								qualityControl.QualityControlResultConformityOnHeaderValue.id_QualityControlResultConformity = id_qualityControlResultConformity;
								qualityControl.QualityControlResultConformityOnHeaderValue.id_QualityControl = qualityControl.id;
							}
							else
							{
								qualityControl.QualityControlResultConformityOnHeaderValue = null;
							}

						}




						#endregion

						#region QualityControlDetail

						if (tempQualityControl?.QualityControlDetail != null)
						{
							qualityControl.QualityControlDetail = new List<QualityControlDetail>();
							//var itemQualityControlDetail = tempQualityControl.QualityControlDetail.ToList();
							List<QualityControlDetail> tempQualityControlList = new List<QualityControlDetail>();
							tempQualityControlList = (List<QualityControlDetail>)TempData["QualityControlDetailsTmp"];
							var itemQualityControlDetail = tempQualityControlList.ToList();


							foreach (var detail in itemQualityControlDetail)
							{
								var tempItemQualityControlDetail = new QualityControlDetail
								{
									id_qualityAnalysis = detail.id_qualityAnalysis,
									QualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == detail.id_qualityAnalysis),
									result = detail?.result ?? "",
									resultValue = detail?.resultValue ?? "",
									otherResultValue = detail?.otherResultValue ?? "",
									isConforms = detail.isConforms,
									isSave = approve
								};

								qualityControl.QualityControlDetail.Add(tempItemQualityControlDetail);
							}
						}

						foreach (var detail in tempQualityControl.ProductionLotDetailQualityControl)
						{
							var detTmp = new ProductionLotDetailQualityControl
							{
								id_productionLotDetail = detail.id_productionLotDetail,
								id_qualityControl = detail.id_qualityControl,
								IsDelete = detail.IsDelete,
								ProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == detail.id_productionLotDetail),
								QualityControl = qualityControl
							};

							qualityControl.ProductionLotDetailQualityControl.Add(detTmp);
						}

						#endregion
						//string mensaje = "";
						//bool resp = ValidaExistenciaGuiaEnOtroControlCalidad(ref tempQualityControl, qualityControl.id_processType, ref mensaje);
						//if (!resp)
						//{
						//    throw new Exception(mensaje);
						//}

						if (qualityControl.QualityControlDetail.Count == 0)
						{
							//TempData.Keep("qualityControl");
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar el análisis sin detalles.");
							return PartialView("_QualityControlMainFormPartial", qualityControlError);
						}

						if (approve)
						{
							qualityControl.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
						}

						db.QualityControl.Add(qualityControl);

						var id_productionLotDetailAux = 0;
						//var tempProductionLot = (TempData["productionLotQuality"] as ProductionLot);
						var tempProductionLot = qualityControl.Lot.ProductionLot;


						var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
						if (aSettingANL != null && aSettingANL.value == "NO")
						{
							qualityControl.remissionGuideProcess = tempQualityControl.remissionGuideProcess;
							//Agregar Guía de Remisión
							//qualityControl.remissionGuideNumber = db.ProductionLotDetailPurchaseDetail
							//                                            .FirstOrDefault(fod => fod.id_productionLotDetail == id_productionLotDetailAux)?
							//                                            .RemissionGuideDetail?.RemissionGuide?.Document.number ?? "";

							if (tempQualityControl.ProductionLotDetailQualityControl != null
								&& tempQualityControl.ProductionLotDetailQualityControl.Count() > 0)
							{
								id_productionLotDetailAux = (int)tempQualityControl.ProductionLotDetailQualityControl.FirstOrDefault().id_productionLotDetail;

								tempProductionLot = tempProductionLot ?? db.ProductionLot.FirstOrDefault(fod => fod.id == tempQualityControl.id_lot);
								var tempProductionLotDetail = tempProductionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetailAux);
								//if (tempProductionLotDetail != null)
								//{
								//    tempProductionLotDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
								//    tempProductionLotDetail.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
								//    {
								//        id_qualityControl = qualityControl.id,
								//        //QualityControl = qualityControl,
								//        id_productionLotDetail = id_productionLotDetailAux
								//    });
								//}

								TempData["productionLotQuality"] = tempProductionLot;
								TempData.Keep("productionLotQuality");
								if (tempProductionLotDetail != null && tempProductionLotDetail.ProductionLotDetailQualityControl != null)
								{
									//qualityControl.ProductionLotDetailQualityControl = tempProductionLotDetail
									//                                                        .ProductionLotDetailQualityControl
									//                                                        .Where(r => !r.IsDelete)
									//                                                        .ToList();
								}
							}

						}
						else if (aSettingANL != null && aSettingANL.value == "SI")
						{
							qualityControl.remissionGuideNumber = "";
							qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";

							if (tempQualityControl.ProductionLotDetailQualityControl != null
								&& tempQualityControl.ProductionLotDetailQualityControl.Count() > 0)
							{
								qualityControl.ProductionLotDetailQualityControl = tempQualityControl
														.ProductionLotDetailQualityControl
														.Where(r => !r.IsDelete)
														.ToList();
							}
						}
						else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
						{
							qualityControl.remissionGuideNumber = "";
							qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";

						}

						//Agregar Piscina
						if (tempProductionLot != null)
						{
							qualityControl.poolName = tempProductionLot?.ProductionUnitProviderPool?.name ?? "";
						}

						//Actualizar tipo de proceso del Lote
						if (qualityControl.id_lot != null && qualityControl.id_lot > 0)
						{
							ProductionLot plotTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);
							if (qualityControl.id_processType != 0)
							{
								int id_ptf = db.ConfProcTypeQualityControlProductionLot.FirstOrDefault(fod => fod.id_ProcessTypeStart == qualityControl.id_processType)?.id_ProcessTypeEnd ?? 0;
								if (id_ptf > 0)
								{
									plotTmp.id_processtype = id_ptf;
								}
							}
							db.ProductionLot.Attach(plotTmp);
							db.Entry(plotTmp).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						if (approve)
						{
							EnviarCorreoNotificacionCambioProceso(qualityControl.id);
						}

						TempData["qualityControlDetailsTmp"] = qualityControl.QualityControlDetail;
						TempData.Keep("qualityControlDetailsTmp");
						SetQualityControl(qualityControl);
						//TempData["qualityControl"] = qualityControl;
						//TempData.Keep("qualityControl");

						ViewData["EditMessage"] = SuccessMessage("El análisis: " + qualityControl.QualityControlConfiguration.name + ", con número: " + qualityControl.number + " guardado exitosamente");
					}
					catch (Exception e)
					{
						SetQualityControl(qualityControlError);

						ViewData["EditMessage"] = ErrorMessage(e.Message);
						MetodosEscrituraLogs.EscribeExcepcionLog(e, ruta, "QualityControl", "PROD");
						trans.Rollback();
						UpdateViewBagTotalUnit(GetQualityControl());
						return PartialView("_QualityControlMainFormPartial", qualityControlError);

					}

					UpdateViewBagTotalUnit(GetQualityControl());

					return PartialView("_QualityControlMainFormPartial", qualityControl);
				}
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "QualityControl", "PROD");
			}
			return null;
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult QualityControlPartialUpdate(string qualityControlTime, string monerTime, bool approve, string qualityControlTimeTmp, string documentReference, string documentDescription,
															QualityControl qualityControl, Document document, DateTime documentEmissionDate, ShrimpSupplierTraceability shrimpSupplierTraceability)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			var model = db.QualityControl;
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}

			QualityControl tempQualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);

			var qualityControlError = tempQualityControl;

			qualityControlError.id_warehouse = qualityControl.id_warehouse;
			qualityControlError.Warehouse = db.Warehouse.FirstOrDefault(e => e.id == qualityControl.id_warehouse);

			qualityControlError.id_warehouseLocation = qualityControl.id_warehouseLocation;
			qualityControlError.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(e => e.id == qualityControl.id_warehouseLocation);

			qualityControlError.id_item = qualityControl.id_item;
			qualityControlError.Item = db.Item.FirstOrDefault(e => e.id == qualityControl.id_item);

			qualityControlError.id_lot = qualityControl.id_lot == 0 ? null : qualityControl.id_lot;
			qualityControlError.Lot = db.Lot.FirstOrDefault(e => e.id == qualityControl.id_lot);
			qualityControlError.remissionGuideProcess = qualityControlError.Lot?.ProductionLot?.Person1?.processPlant ?? "";

			qualityControlError.id_analyst = qualityControl.id_analyst;
			qualityControlError.Employee = db.Employee.FirstOrDefault(e => e.id == qualityControl.id_analyst);

			qualityControlError.reference = documentReference;
			qualityControlError.Document.reference = documentReference;
			qualityControlError.description = documentDescription;
			qualityControlError.Document.description = documentDescription;
			qualityControlError.Document.emissionDate = documentEmissionDate;

			qualityControlError.id_size = qualityControl.id_size;
			qualityControlError.iceContamination = qualityControl.iceContamination;
			qualityControlError.transportCondition = qualityControl.transportCondition;
			qualityControlError.transportState = qualityControl.transportState;
			qualityControlError.transportOnlyShrimp = qualityControl.transportOnlyShrimp;

			qualityControlError.totalWeightSample = qualityControl.totalWeightSample;
			qualityControlError.PCC = qualityControl.PCC;
			qualityControlError.monerDate = qualityControl.monerDate;
			qualityControlError.monerTime = qualityControl.monerTime;
			qualityControlError.monerResultados = qualityControl.monerResultados;


			qualityControlError.ShrimpSupplierTraceability = shrimpSupplierTraceability;

			QualityControl modelItem = model.FirstOrDefault(it => it.id == qualityControl.id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					if (modelItem != null)
					{

						var approvedModelItem = modelItem.Document.DocumentState.code.Equals("03"); //03: APROBADO
						if (!approvedModelItem)
						{

							#region Document

							modelItem.Document.id_userUpdate = ActiveUser.id;
							modelItem.Document.dateUpdate = DateTime.Now;

							modelItem.Document.emissionDate = document.emissionDate;

							modelItem.Document.reference = documentReference;
							modelItem.Document.description = documentDescription;
							modelItem.Document.emissionDate = documentEmissionDate;

							#endregion

							#region QualityControl

							modelItem.id_warehouse = qualityControl.id_warehouse;
							modelItem.Warehouse = db.Warehouse.FirstOrDefault(e => e.id == qualityControl.id_warehouse);

							modelItem.id_warehouseLocation = qualityControl.id_warehouseLocation;
							modelItem.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(e => e.id == qualityControl.id_warehouseLocation);

							modelItem.id_item = qualityControl.id_item;
							modelItem.Item = db.Item.FirstOrDefault(e => e.id == qualityControl.id_item);

							modelItem.id_lot = qualityControl.id_lot == 0 ? null : qualityControl.id_lot;
							modelItem.Lot = db.Lot.FirstOrDefault(e => e.id == qualityControl.id_lot);
							modelItem.remissionGuideProcess = modelItem.Lot?.ProductionLot?.Person1?.processPlant ?? "";

							modelItem.id_analyst = qualityControl.id_analyst;
							modelItem.Employee = db.Employee.FirstOrDefault(e => e.id == qualityControl.id_analyst);

							modelItem.reference = documentReference;

							modelItem.description = documentDescription;
							modelItem.QuantityPoundsReceived = qualityControl.QuantityPoundsReceived;

							modelItem.totalWeightSample = qualityControl.totalWeightSample;
							modelItem.grammageReference = qualityControl.grammageReference;
							modelItem.qualityControlDate = qualityControl.qualityControlDate;

							qualityControl.DrawersReceived = qualityControl.DrawersReceived ?? 0;
							modelItem.DrawersReceived = qualityControl.DrawersReceived;
							modelItem.residual = qualityControl.residual;
							modelItem.temperature = qualityControl.temperature;
							modelItem.wholePerformance = qualityControl.wholePerformance;
							modelItem.id_processType = qualityControl.id_processType;

							modelItem.id_size = qualityControl.id_size;
							modelItem.iceContamination = qualityControl.iceContamination;
							modelItem.transportCondition = qualityControl.transportCondition;
							modelItem.transportState = qualityControl.transportState;
							modelItem.transportOnlyShrimp = qualityControl.transportOnlyShrimp;
							modelItem.PCC = qualityControl.PCC;
							modelItem.monerDate = qualityControl.monerDate;
							monerTime = monerTime.Substring(11, (monerTime.Length - 11));
							if (!string.IsNullOrEmpty(monerTime)) modelItem.monerTime = TimeSpan.Parse(monerTime);


							modelItem.monerResultados = qualityControl.monerResultados;


							#region ShrimpSupplierTraceability

							var aSettingTPC = db.Setting.FirstOrDefault(fod => fod.code == "TPC");
							if (aSettingTPC != null && aSettingTPC.value == "SI")
							{
								if (modelItem.ShrimpSupplierTraceability == null)
								{
									modelItem.ShrimpSupplierTraceability = new ShrimpSupplierTraceability();
									modelItem.ShrimpSupplierTraceability.id = modelItem.id;
								}
								modelItem.ShrimpSupplierTraceability.sowingDate = shrimpSupplierTraceability.sowingDate;
								modelItem.ShrimpSupplierTraceability.harvestDate = shrimpSupplierTraceability.harvestDate;

								modelItem.ShrimpSupplierTraceability.id_suppliesNauplius = shrimpSupplierTraceability.id_suppliesNauplius;
								modelItem.ShrimpSupplierTraceability.id_providerNauplius = shrimpSupplierTraceability.id_providerNauplius;
								modelItem.ShrimpSupplierTraceability.observationNauplius = shrimpSupplierTraceability.observationNauplius;

								modelItem.ShrimpSupplierTraceability.id_suppliesLarva = shrimpSupplierTraceability.id_suppliesLarva;
								modelItem.ShrimpSupplierTraceability.id_providerLarva = shrimpSupplierTraceability.id_providerLarva;
								modelItem.ShrimpSupplierTraceability.observationLarva = shrimpSupplierTraceability.observationLarva;

								modelItem.ShrimpSupplierTraceability.id_suppliesBalanced = shrimpSupplierTraceability.id_suppliesBalanced;
								modelItem.ShrimpSupplierTraceability.id_providerBalanced = shrimpSupplierTraceability.id_providerBalanced;
								modelItem.ShrimpSupplierTraceability.observationBalanced = shrimpSupplierTraceability.observationBalanced;

								modelItem.ShrimpSupplierTraceability.id_suppliesFertilizer = shrimpSupplierTraceability.id_suppliesFertilizer;
								modelItem.ShrimpSupplierTraceability.id_providerFertilizer = shrimpSupplierTraceability.id_providerFertilizer;
								modelItem.ShrimpSupplierTraceability.observationFertilizer = shrimpSupplierTraceability.observationFertilizer;

								modelItem.ShrimpSupplierTraceability.id_suppliesFilesOrOthers = shrimpSupplierTraceability.id_suppliesFilesOrOthers;
								modelItem.ShrimpSupplierTraceability.id_providerFilesOrOthers = shrimpSupplierTraceability.id_providerFilesOrOthers;
								modelItem.ShrimpSupplierTraceability.observationFilesOrOthers = shrimpSupplierTraceability.observationFilesOrOthers;
							}

							#endregion


							qualityControlTime = qualityControlTime.Substring(11, (qualityControlTime.Length - 11));
							if (!string.IsNullOrEmpty(qualityControlTime)) modelItem.qualityControlTime = TimeSpan.Parse(qualityControlTime);

							modelItem.isConformityOnHeader = qualityControl.isConformityOnHeader;
							if ((bool)qualityControl.isConformityOnHeader)
							{
								modelItem.QualityControlResultConformityOnHeaderValue = modelItem.QualityControlResultConformityOnHeaderValue ?? new QualityControlResultConformityOnHeaderValue();

								int? id_qualityControlResultConformity = qualityControl.QualityControlResultConformityOnHeaderValue.QualityControlResultConformity.id;
								modelItem.QualityControlResultConformityOnHeaderValue = modelItem.QualityControlResultConformityOnHeaderValue ?? new QualityControlResultConformityOnHeaderValue();
								modelItem.QualityControlResultConformityOnHeaderValue.id_QualityControlResultConformity = id_qualityControlResultConformity;
								//modelItem.QualityControlResultConformityOnHeaderValue.id_QualityControlResultConformity = qualityControl.QualityControlResultConformityOnHeaderValue.id_QualityControlResultConformity;
								modelItem.isConforms = (bool)db.QualityControlResultConformity.FirstOrDefault(fod => fod.id == id_qualityControlResultConformity).isConforms;
							}
							#endregion

						}
						if (!(bool)modelItem.isConformityOnHeader)
						{
							modelItem.isConforms = tempQualityControl.QualityControlDetail.FirstOrDefault(fod => !fod.isConforms && fod.QualityCorrectiveAction == null) == null;
						}


						#region QualityControlDetail

						if (tempQualityControl?.QualityControlDetail != null)
						{
							for (int i = modelItem.QualityControlDetail.Count - 1; i >= 0; i--)
							{
								var detail = modelItem.QualityControlDetail.ElementAt(i);

								if (tempQualityControl.QualityControlDetail.FirstOrDefault(fod => fod.id == detail.id && fod.id_qualityAnalysis == detail.id_qualityAnalysis) == null)
								{
									modelItem.QualityControlDetail.Remove(detail);
									db.Entry(detail).State = EntityState.Deleted;
								}
							}

							List<QualityControlDetail> tempQualityControlList = new List<QualityControlDetail>();
							tempQualityControlList = (List<QualityControlDetail>)TempData["qualityControlDetailsTmp"];

							var itemQualityControlDetail = tempQualityControlList.ToList();

							if (itemQualityControlDetail == null)
							{
								itemQualityControlDetail = tempQualityControl.QualityControlDetail.ToList();
							}
							foreach (var detail in itemQualityControlDetail)
							{
								var tempItemQualityControlDetail = modelItem.QualityControlDetail.FirstOrDefault(fod => fod.id == detail.id && fod.id_qualityAnalysis == detail.id_qualityAnalysis);
								if (approvedModelItem)
								{
									if (tempItemQualityControlDetail == null)
									{
										tempItemQualityControlDetail = new QualityControlDetail
										{
											id_qualityAnalysis = detail.id_qualityAnalysis,
											QualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == detail.id_qualityAnalysis),
											result = detail.result,
											resultValue = detail.resultValue ?? "",
											otherResultValue = detail.otherResultValue ?? "",
											isConforms = detail.isConforms,
											isSave = approvedModelItem
										};
										modelItem.QualityControlDetail.Add(tempItemQualityControlDetail);
									}
									else
									{
										if (detail.QualityCorrectiveAction != null && !detail.QualityCorrectiveAction.isCorrected && !detail.isConforms)
										{
											tempItemQualityControlDetail.QualityCorrectiveAction = new QualityCorrectiveAction
											{
												id_correctingPerson = detail.QualityCorrectiveAction.id_correctingPerson,
												Employee = db.Employee.FirstOrDefault(fod => fod.id == detail.QualityCorrectiveAction.id_correctingPerson),
												reference = detail.QualityCorrectiveAction.reference,
												description = detail.QualityCorrectiveAction.description,
												isCorrected = true
											};
										}

										db.QualityControlDetail.Attach(tempItemQualityControlDetail);
										db.Entry(tempItemQualityControlDetail).State = EntityState.Modified;

									}
								}
								else
								{
									if (tempItemQualityControlDetail == null)
									{
										tempItemQualityControlDetail = new QualityControlDetail
										{
											id_qualityAnalysis = detail.id_qualityAnalysis,
											QualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == detail.id_qualityAnalysis),
											result = detail.result ?? "",
											resultValue = detail.resultValue ?? "",
											otherResultValue = detail.otherResultValue ?? "",
											isConforms = detail.isConforms,
											isSave = approve
										};
										modelItem.QualityControlDetail.Add(tempItemQualityControlDetail);
									}
									else
									{
										tempItemQualityControlDetail.result = detail.result ?? "";
										tempItemQualityControlDetail.resultValue = detail.resultValue ?? "";
										tempItemQualityControlDetail.otherResultValue = detail.otherResultValue ?? "";
										tempItemQualityControlDetail.isConforms = detail.isConforms;
										tempItemQualityControlDetail.isSave = approve;

										db.QualityControlDetail.Attach(tempItemQualityControlDetail);
										db.Entry(tempItemQualityControlDetail).State = EntityState.Modified;
									}
								}
							}
						}

						#endregion

						#region Production Lot Detail rel Quality Control

						var lsDetailRelPldQc = db.ProductionLotDetailQualityControl.Where(w => w.id_qualityControl == modelItem.id).ToList();
						if (lsDetailRelPldQc != null && lsDetailRelPldQc.Count > 0)
						{
							foreach (var det in lsDetailRelPldQc)
							{
								db.ProductionLotDetailQualityControl.Remove(det);
								db.ProductionLotDetailQualityControl.Attach(det);
								db.Entry(det).State = EntityState.Deleted;
							}
						}
						foreach (var detail in tempQualityControl.ProductionLotDetailQualityControl)
						{
							var detTmp = new ProductionLotDetailQualityControl
							{
								id_productionLotDetail = detail.id_productionLotDetail,
								id_qualityControl = detail.id_qualityControl,
								IsDelete = detail.IsDelete,
								ProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == detail.id_productionLotDetail),
								QualityControl = modelItem
							};

							modelItem.ProductionLotDetailQualityControl.Add(detTmp);
						}
						//string mensaje = "";
						//bool resp = ValidaExistenciaGuiaEnOtroControlCalidad(ref tempQualityControl, qualityControl.id_processType, ref mensaje);
						//if (!resp)
						//{
						//    throw new Exception(mensaje);
						//}
						#endregion

						if (modelItem.QualityControlDetail.Count == 0)
						{
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar el análisis sin detalles.");
							return PartialView("_QualityControlMainFormPartial", qualityControlError);
						}


						if (approve)
						{
							modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
						}

						db.QualityControl.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;

						var id_productionLotDetailAux = 0;
						var tempProductionLot = (TempData["productionLotQuality"] as ProductionLot);
						var id_p = db.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == qualityControl.id)?.id_productionLotDetail ?? 0;
						var idpld = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_p)?.id ?? 0;
						var pldpd = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idpld);

						if (tempProductionLot == null)
						{
							tempProductionLot = modelItem?.Lot?.ProductionLot ?? new ProductionLot();
						}
						//if (tempQualityControl.ProductionLotDetailQualityControl != null && tempQualityControl.ProductionLotDetailQualityControl.Count() > 0)
						//{
						//    id_productionLotDetailAux = tempQualityControl.ProductionLotDetailQualityControl.FirstOrDefault().id_productionLotDetail ?? 0;

						//    if (tempProductionLot != null)
						//    {
						//        var tempProductionLotDetail = tempProductionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id_productionLotDetailAux);

						//        var productionLotDetailQualityControlAux = tempProductionLotDetail.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == modelItem.id);
						//        productionLotDetailQualityControlAux.QualityControl = modelItem;

						//        TempData["productionLotQuality"] = tempProductionLot;
						//        TempData.Keep("productionLotQuality");
						//    }

						//}

						var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
						if (aSettingANL != null && aSettingANL.value == "NO")
						{
							modelItem.remissionGuideProcess = tempQualityControl.remissionGuideProcess;

							//Agregar Guía de Remisión
							qualityControl.remissionGuideNumber = db.ProductionLotDetailPurchaseDetail
																		.FirstOrDefault(fod => fod.id_productionLotDetail == id_productionLotDetailAux)?
																		.RemissionGuideDetail?.RemissionGuide?.Document.number ?? "";

							if (pldpd != null)
							{
								qualityControl.remissionGuideNumber = pldpd.RemissionGuideDetail?.RemissionGuide?.Document?.number ?? "";
							}
						}
						else if (aSettingANL != null && aSettingANL.value == "SI")
						{
							qualityControl.remissionGuideNumber = "";
							qualityControl.remissionGuideProcess = tempProductionLot?.Person1?.processPlant ?? "";
						}
						else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
						{
							qualityControl.remissionGuideNumber = "";
							qualityControl.remissionGuideProcess = tempQualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
						}
						//Agregar Piscina
						if (tempProductionLot != null)
						{
							qualityControl.poolName = tempProductionLot?.ProductionUnitProviderPool?.name ?? "";
						}

						if (tempQualityControl != null)
						{
							qualityControl.remissionGuideNumber = tempQualityControl.remissionGuideNumber;
							qualityControl.poolName = tempQualityControl.poolName;
							modelItem.poolName = tempQualityControl.poolName;
							modelItem.remissionGuideNumber = tempQualityControl.remissionGuideNumber;
						}

						if (idpld != 0)
						{
							qualityControl.poolName = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idpld)?.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
						}
						//Actualizar tipo de proceso del Lote
						if (modelItem.id_lot != null && modelItem.id_lot > 0)
						{
							ProductionLot plotTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == modelItem.id_lot);
							if (qualityControl.id_processType != 0)
							{
								int id_ptf = db.ConfProcTypeQualityControlProductionLot.FirstOrDefault(fod => fod.id_ProcessTypeStart == qualityControl.id_processType)?.id_ProcessTypeEnd ?? 0;
								if (id_ptf > 0)
								{
									plotTmp.id_processtype = id_ptf;
								}
							}
							db.ProductionLot.Attach(plotTmp);
							db.Entry(plotTmp).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						if (approve)
						{
							EnviarCorreoNotificacionCambioProceso(qualityControl.id);
						}
						SetQualityControl(modelItem);

						ViewData["EditMessage"] = SuccessMessage("El análisis: " + modelItem.QualityControlConfiguration.name + ", con número: " + modelItem.number + " guardado exitosamente");
					}
				}
				catch (Exception e)
				{

					SetQualityControl(qualityControlError);
					//TempData["qualityControl"] = qualityControlError;
					//TempData.Keep("qualityControl");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
					//UpdateViewBagNADCTD(qualityControlError);
					UpdateViewBagTotalUnit(GetQualityControl());

					return PartialView("_QualityControlMainFormPartial", qualityControlError);

				}
			}

			//SetViewData();
			//UpdateViewBagNADCTD(modelItem);
			UpdateViewBagTotalUnit(GetQualityControl());

			return PartialView("_QualityControlMainFormPartial", modelItem);
		}
		#endregion

		#region QUALITY CONTROL DETAILS

		[ValidateInput(false)]
		public ActionResult QualityControlDetailsViewPartial(int? id_qualityControl)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			ViewData["id_qualityControl"] = id_qualityControl;
			var qualityControl = db.QualityControl.FirstOrDefault(p => p.id == id_qualityControl);
			var model = qualityControl?.QualityControlDetail.ToList() ?? new List<QualityControlDetail>();


			return PartialView("_QualityControlDetailsViewPartial", model.OrderBy(od => od.id_qualityAnalysis).ThenBy(tb => tb.id).ToList());
		}

		[ValidateInput(false)]
		public ActionResult QualityControlDetailsPartial()
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			QualityControl qualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
																//qualityControl = qualityControl ?? new QualityControl();
			qualityControl.QualityControlDetail = qualityControl.QualityControlDetail ?? new List<QualityControlDetail>();

			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			var model = qualityControl.QualityControlDetail;
			//TempData.Keep("qualityControl");

			return PartialView("_QualityControlDetailsPartial", model.OrderBy(od => od.id_qualityAnalysis).ThenBy(tb => tb.id).ToList());

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult QualityControlDetailsPartialUpdate(DXPANACEASOFT.Models.QualityControlDetail qualityControlDetail,
															   int? qualityCorrectiveActionIdCorrectingPerson, string qualityCorrectiveActionReference,
															   string descriptionQualityCorrectiveAction)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			QualityControl qualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
																//qualityControl = qualityControl ?? new QualityControl();
			qualityControl.QualityControlDetail = qualityControl.QualityControlDetail ?? new List<QualityControlDetail>();
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = qualityControl.QualityControlDetail.FirstOrDefault(i => i.id == qualityControlDetail.id);
					if (modelItem != null)
					{
						var qualityControlConfigurationAnalysisDataTypeValidate = modelItem.QualityControl.QualityControlConfiguration.QualityControlConfigurationAnalysisDataTypeValidate.FirstOrDefault(fod => fod.id_qualityAnalysis == modelItem.id_qualityAnalysis);
						var codeDataType = qualityControlConfigurationAnalysisDataTypeValidate.QualityDataType.code;
						var codeValidate = qualityControlConfigurationAnalysisDataTypeValidate.QualityValidate.code;
						var isConforms = false;

						if (codeDataType.Equals("LTXT") ||
						codeDataType.Equals("LENT") ||
						codeDataType.Equals("LDEC"))
						{
							var resultInt = Convert.ToInt32(qualityControlDetail.result);
							var resultValueAux = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.FirstOrDefault(fod => fod.id == resultInt);

							if (codeValidate.Equals("VAL"))
							{
								isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
							}
							else
							{
								if (codeValidate.Equals("RAN"))
								{
									var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
									var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
									isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
								}
								else
								{
									var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
									var valueAux = listValue.FirstOrDefault(fod => fod.id == resultInt);
									isConforms = (valueAux != null);
								}
							}
							modelItem.resultValue = resultValueAux.value;
							modelItem.result = qualityControlDetail.result;
						}
						else
						{
							if (codeDataType.Equals("TXT"))
							{
								if (codeValidate.Equals("VAL"))
								{
									isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
								}
								else
								{
									if (codeValidate.Equals("RAN"))
									{
										isConforms = (qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin ||
													  qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
									}
									else
									{
										var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
										var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
										isConforms = (valueAux != null);
									}
								}
								modelItem.resultValue = qualityControlDetail.result;
								modelItem.result = qualityControlDetail.result;
							}
							else
							{
								if (codeDataType.Equals("ENT"))
								{
									var resultInt = Convert.ToInt32(qualityControlDetail.result);

									if (codeValidate.Equals("VAL"))
									{
										var valueValidateInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
										isConforms = (resultInt == valueValidateInt);
									}
									else
									{
										if (codeValidate.Equals("RAN"))
										{
											var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
											var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
											isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
										}
										else
										{
											var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
											var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
											isConforms = (valueAux != null);
										}
									}
									modelItem.resultValue = qualityControlDetail.result;
									modelItem.result = qualityControlDetail.result;
								}
								else
								{
									if (codeDataType.Equals("DEC"))
									{
										var resultWithComa = qualityControlDetail.result.Replace('.', ',');
										if (qualityControlDetail.otherResultValue != null)
										{
											var otherResultWithComa = qualityControlDetail.otherResultValue.Replace('.', ',');
											qualityControlDetail.otherResultValue = otherResultWithComa;
										}
										var resultDecimal = Convert.ToDecimal(resultWithComa);

										if (codeValidate.Equals("VAL"))
										{
											var valueValidateDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
											isConforms = (resultDecimal == valueValidateDecimal);
										}
										else
										{
											if (codeValidate.Equals("RAN"))
											{
												var valueValidateMinDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
												var valueValidateMaxDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
												isConforms = (resultDecimal >= valueValidateMinDecimal && resultDecimal <= valueValidateMaxDecimal);
											}
											else
											{
												var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
												var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
												isConforms = (valueAux != null);
											}
										}
										modelItem.resultValue = resultWithComa;
										modelItem.result = resultWithComa;
									}
								}
							}
						}


						modelItem.isConforms = isConforms;
						//modelItem.isSave = false;

						if (qualityCorrectiveActionIdCorrectingPerson != null && qualityCorrectiveActionIdCorrectingPerson != 0)
						{
							if (modelItem.QualityCorrectiveAction == null)
							{
								modelItem.QualityCorrectiveAction = new QualityCorrectiveAction
								{
									id = modelItem.id,
									QualityControlDetail = modelItem,
									id_correctingPerson = qualityCorrectiveActionIdCorrectingPerson.Value,
									Employee = db.Employee.FirstOrDefault(fod => fod.id == qualityCorrectiveActionIdCorrectingPerson),
									isCorrected = false,
									reference = qualityCorrectiveActionReference,
									description = descriptionQualityCorrectiveAction
								};
								var modelItemAux = new QualityControlDetail();
								modelItemAux.id = qualityControl.QualityControlDetail.Count() > 0 ? qualityControl.QualityControlDetail.Max(pld => pld.id) + 1 : 1;
								modelItemAux.QualityControl = qualityControl;
								modelItemAux.id_qualityAnalysis = modelItem.id_qualityAnalysis;
								modelItemAux.QualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == modelItem.id_qualityAnalysis);
								modelItemAux.result = modelItem.result;
								modelItemAux.resultValue = modelItem.resultValue;
								modelItemAux.isConforms = modelItem.isConforms;
								modelItemAux.isSave = false;

								qualityControl.QualityControlDetail.Add(modelItemAux);
							}
							else
							{

								//modelItem.QualityCorrectiveAction.id = modelItem.id,
								//    modelItem.QualityCorrectiveAction.QualityControlDetail = modelItem,
								modelItem.QualityCorrectiveAction.id_correctingPerson = qualityCorrectiveActionIdCorrectingPerson.Value;
								modelItem.QualityCorrectiveAction.Employee = db.Employee.FirstOrDefault(fod => fod.id == qualityCorrectiveActionIdCorrectingPerson);
								//modelItem.QualityCorrectiveAction.isCorrected = false,
								modelItem.QualityCorrectiveAction.reference = qualityCorrectiveActionReference;
								modelItem.QualityCorrectiveAction.description = descriptionQualityCorrectiveAction;
							}



						}

						this.UpdateModel(modelItem);
						//db.SaveChanges();
					}
					SetQualityControl(qualityControl);
					//TempData["qualityControl"] = qualityControl;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			//TempData.Keep("qualityControl");
			//TempData.Keep("businessOportunityPhaseAttachment");
			//TempData.Keep("businessOportunityPhaseActivity");
			var model = qualityControl.QualityControlDetail;
			//SetViewData();

			return PartialView("_QualityControlDetailsPartial", model.OrderBy(od => od.id_qualityAnalysis).ThenBy(tb => tb.id).ToList());
		}

		#endregion

		#region QUALITY CONTROL DETAILS NEW

		public ActionResult QualityControlDetailsMasterViewPartial(int? id_qualityControl)
		{

			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			ViewData["id_qualityControl"] = id_qualityControl;

			var qualityControlAnalysisGroup = db.QualityControlAnalysisGroup.ToList();
			var model = qualityControlAnalysisGroup ?? new List<QualityControlAnalysisGroup>();

			return PartialView("_QualityControlDetailsMasterViewPartial", model.OrderBy(od => od.id).ToList());

		}

		public ActionResult QualityControlDetailsMasterPartial()
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			var qualityControlAnalysisGroup = db.QualityControlAnalysisGroup.ToList();
			var model = qualityControlAnalysisGroup ?? new List<QualityControlAnalysisGroup>();

			return PartialView("_QualityControlDetailsMasterPartial", model.OrderBy(od => od.id).ToList());
		}
		public ActionResult GetDetailQualityControl()
		{
			decimal LibrasCola = 0;
			decimal LibrasEntero = 0;
			QualityControl tempQualityControl = GetQualityControl();

			List<QualityControlTallaDto> plDetTallas = new List<QualityControlTallaDto>();
			var setting = db.Setting.Where(X => X.code == "PRENTC").FirstOrDefault()?.value;

			var _Grammage = db.Grammage.Where(w => w.value >= tempQualityControl.grammageReference)
											.OrderBy(o => o.value)
											.ToList()
											.FirstOrDefault();

			if (tempQualityControl.ProcessType.code == "ENT")
			{

				if (setting != null)
				{
					LibrasEntero = tempQualityControl.QuantityPoundsReceived.Value * (decimal.Parse(setting) / 100);
					LibrasCola = (tempQualityControl.QuantityPoundsReceived.Value - LibrasEntero) * (decimal.Parse(setting) / 100);
				}
				else
				{
					LibrasEntero = tempQualityControl.QuantityPoundsReceived.Value;
				}


				plDetTallas = db.FanSeriesGrammage
										   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == tempQualityControl.id_processType)
										   .Select(s => new QualityControlTallaDto
										   {
											   id_itemsize = s.id_itemsize,
											   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
											   Order = s.ItemSize.orderSize,
											   id_Class = s.id_class,
											   codeClass = s.Class.code,
											   nameClass = s.Class.description,
											   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.id_ProcessType.Value : 0,
											   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.ProcessType.name : "",
											   poundsDetail = s.percentage * (s.ProcessType1.code == "ENT" ? LibrasEntero : LibrasCola),
											   porcentaje = s.percentage,
										   }).ToList();

			}
			if (tempQualityControl.ProcessType.code == "COL")
			{

				plDetTallas = db.FanSeriesGrammage
										   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == tempQualityControl.id_processType)
										   .Select(s => new QualityControlTallaDto
										   {
											   id_itemsize = s.id_itemsize,
											   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
											   Order = s.ItemSize.orderSize,
											   id_Class = s.id_class,
											   codeClass = s.Class.code,
											   nameClass = s.Class.description,
											   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.id_ProcessType.Value : 0,
											   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.ProcessType.name : "",
											   poundsDetail = s.percentage * tempQualityControl.QuantityPoundsReceived.Value,
											   porcentaje = s.percentage,
										   }).ToList();

			}
			if (tempQualityControl.ProcessType.code == "ENC")
			{

				LibrasCola = tempQualityControl.QuantityPoundsReceived.Value * (decimal.Parse("66") / 100);


				plDetTallas = db.FanSeriesGrammage
										   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == tempQualityControl.id_processType)
										   .Select(s => new QualityControlTallaDto
										   {
											   id_itemsize = s.id_itemsize,
											   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
											   Order = s.ItemSize.orderSize,
											   id_Class = s.id_class,
											   codeClass = s.Class.code,
											   nameClass = s.Class.description,
											   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.id_ProcessType.Value : 0,
											   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																.ProcessType.name : "",
											   poundsDetail = s.percentage * LibrasCola,
											   porcentaje = s.percentage,
										   }).ToList();

			}

			//TempData["QualityControl"] = apTmp;
			//TempData.Keep("QualityControl");
			return PartialView("_QualityControlMainFormTabDetailTallas", plDetTallas.OrderBy(o => o.sProcessType).ThenBy(o => o.nameClass).ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult PopupControlTallaListDetail(decimal QuantityPoundsReceived, decimal totalWeightSample, int idQualityControl, int idProcessType, int totalUnit)
		{
			decimal LibrasCola = 0;
			decimal LibrasEntero = 0;

			List<QualityControlTallaDto> plDetTallas = new List<QualityControlTallaDto>();
			var grammage = Math.Truncate(totalWeightSample / totalUnit * 100) / 100;

			var setting = db.Setting.Where(X => X.code == "PRENTC").FirstOrDefault()?.value;
			ProcessType processtype = db.ProcessType.Where(x => x.id == idProcessType).FirstOrDefault();

			var _Grammage = db.Grammage.Where(w => w.value >= grammage)
											.OrderBy(o => o.value)
											.ToList()
											.FirstOrDefault();
			try
			{

				if (processtype.code == "ENT")
				{

					if (setting != null)
					{
						LibrasEntero = QuantityPoundsReceived * (decimal.Parse(setting) / 100);
						LibrasCola = (QuantityPoundsReceived - LibrasEntero) * (decimal.Parse(setting) / 100);
					}
					else
					{
						LibrasEntero = QuantityPoundsReceived;
					}


					plDetTallas = db.FanSeriesGrammage
											   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == idProcessType)
											   .Select(s => new QualityControlTallaDto
											   {
												   id_itemsize = s.id_itemsize,
												   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
												   Order = s.ItemSize.orderSize,
												   id_Class = s.id_class,
												   codeClass = s.Class.code,
												   nameClass = s.Class.description,
												   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.id_ProcessType.Value : 0,
												   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.ProcessType.name : "",
												   poundsDetail = s.percentage * (s.ProcessType1.code == "ENT" ? LibrasEntero : LibrasCola),
												   porcentaje = s.percentage,
											   }).ToList();

				}
				if (processtype.code == "COL")
				{

					plDetTallas = db.FanSeriesGrammage
											   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == idProcessType)
											   .Select(s => new QualityControlTallaDto
											   {
												   id_itemsize = s.id_itemsize,
												   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
												   Order = s.ItemSize.orderSize,
												   id_Class = s.id_class,
												   codeClass = s.Class.code,
												   nameClass = s.Class.description,
												   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.id_ProcessType.Value : 0,
												   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.ProcessType.name : "",
												   poundsDetail = s.percentage * QuantityPoundsReceived,
												   porcentaje = s.percentage,
											   }).ToList();

				}
				if (processtype.code == "ENC")
				{

					LibrasCola = QuantityPoundsReceived * (decimal.Parse("66") / 100);


					plDetTallas = db.FanSeriesGrammage
											   .Where(w => w.id_grammage == _Grammage.id && w.percentage > 0 && w.id_processtype == idProcessType)
											   .Select(s => new QualityControlTallaDto
											   {
												   id_itemsize = s.id_itemsize,
												   sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
												   Order = s.ItemSize.orderSize,
												   id_Class = s.id_class,
												   codeClass = s.Class.code,
												   nameClass = s.Class.description,
												   id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.id_ProcessType.Value : 0,
												   sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.id_itemsize)
																	? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.id_itemsize)
																	.ProcessType.name : "",
												   poundsDetail = s.percentage * LibrasCola,
												   porcentaje = s.percentage,
											   }).ToList();

				}
				return PartialView("_QualityControlMainFormTabDetailTallas", plDetTallas.OrderBy(o => o.sProcessType).ThenBy(o => o.nameClass).ToList());

			}
			catch (Exception e)
			{
				throw e;
			}
		}
		public ActionResult QualityControlDetailsDetailPartial(int id_analysisGroup)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			ViewData["id_analysisGroup"] = id_analysisGroup;
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			List<QualityControlDetail> qualityControlDetail = (List<QualityControlDetail>)TempData["qualityControlDetailsTmp"];


			List<QualityControlAnalysisGroupAnalysis> qualityControlAnalysisGroupAnalysis = db.QualityControlAnalysisGroupAnalysis.Where(w => w.id_QualityControlAnalysisGroup == id_analysisGroup).ToList();

			var qualityControlAnalysisGroupTmp = db.QualityControlAnalysisGroup.FirstOrDefault(w => w.id == id_analysisGroup);

			var visualizationTypeData = db.VisualizationTypeData.FirstOrDefault(fod => fod.id == qualityControlAnalysisGroupTmp.id_VisualizationTypeData);


			List<QualityControlDetail> qualityControlDetailFilter = new List<QualityControlDetail>();
			try
			{
				qualityControlDetailFilter = qualityControlDetail.Join(qualityControlAnalysisGroupAnalysis,
							it => it.id_qualityAnalysis,
							tt => tt.id_QualityAnalysis,
							(it, tt) => it).ToList();
			}
			catch //(Exception e)
			{

			}
			ViewData["RowHashes"] = RowHashes;

			//QualityControl tempQualityControl = (TempData["qualityControl"] as QualityControl);
			//tempQualityControl = tempQualityControl ?? new QualityControl();
			UpdateViewBagNADCTD(GetQualityControl());
			var aSettingDetailLNACVMC = db.SettingDetail.Where(fod => fod.Setting.code == "LNACVMC").ToList();
			var aKeysQualityAnalysisMayor0 = new List<int>();
			foreach (var aItem in aSettingDetailLNACVMC)
			{
				var aQualityControlDetailFilter = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == aItem.value);
				if (aQualityControlDetailFilter != null)
				{
					aKeysQualityAnalysisMayor0.Add(aQualityControlDetailFilter.id);
				}
			}
			ViewBag.keysQualityAnalysisMayor0 = aKeysQualityAnalysisMayor0;
			//ViewBag.keyFRESCOYSANO = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "FRESCO Y SANO")?.id;
			//ViewBag.keyBASURAGR = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "BASURA GR")?.id;
			//ViewBag.keyBASURAGR = aKeyBASURAGR != null ? aKeyBASURAGR - 1 : aKeyBASURAGR;

			return PartialView("_QualityControlDetailsMasterDetailPartial", qualityControlDetailFilter);

		}

		public ActionResult BatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<QualityControlDetail, int> updateValues, int id_analysisGroup)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			ViewData["id_analysisGroup"] = id_analysisGroup;

			//QualityControl tempQualityControl = (TempData["qualityControl"] as QualityControl);
			//tempQualityControl = tempQualityControl ?? new QualityControl();

			//decimal amoutNADCTD = 0.00M;
			//var aSettingDetailNADCTD = db.SettingDetail.Where(w=> w.Setting.code.Equals("NADCTD")).ToList();
			foreach (var qualityControl in updateValues.Update)
			{
				//var id_qualityAnalysisAux = tempQualityControl.QualityControlDetail.FirstOrDefault(fod => fod.id == qualityControl.id)?.id_qualityAnalysis;
				//var nameQualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod=> fod.id == id_qualityAnalysisAux)?.name;
				//if (aSettingDetailNADCTD.FirstOrDefault(fod=> fod.value == nameQualityAnalysis) != null) {
				//    decimal resultAux = 0.00M;
				//    decimal.TryParse((qualityControl.resultValue).Replace('.', ','), out resultAux);
				//    amoutNADCTD += resultAux;
				//}
				if (updateValues.IsValid(qualityControl))
				{

					UpdateQualityControlBatchDetail(qualityControl, updateValues, id_analysisGroup, RowHashes[qualityControl.id]);
				}
			}
			//ViewBag.amoutNADCTD = amoutNADCTD;
			List<QualityControlDetail> qualityControlDetail = (List<QualityControlDetail>)TempData["qualityControlDetailsTmp"];

			List<QualityControlAnalysisGroupAnalysis> qualityControlAnalysisGroupAnalysis = db.QualityControlAnalysisGroupAnalysis.Where(w => w.id_QualityControlAnalysisGroup == id_analysisGroup).ToList();

			var qualityControlAnalysisGroupTmp = db.QualityControlAnalysisGroup.FirstOrDefault(w => w.id == id_analysisGroup);

			var visualizationTypeData = db.VisualizationTypeData.FirstOrDefault(fod => fod.id == qualityControlAnalysisGroupTmp.id_VisualizationTypeData);


			List<QualityControlDetail> qualityControlDetailFilter = new List<QualityControlDetail>();
			try
			{
				qualityControlDetailFilter = qualityControlDetail.Join(qualityControlAnalysisGroupAnalysis,
							it => it.id_qualityAnalysis,
							tt => tt.id_QualityAnalysis,
							(it, tt) => it).ToList();
			}
			catch //(Exception e)
			{
			}
			ViewData["RowHashes"] = RowHashes;

			//QualityControl tempQualityControl = (TempData["qualityControl"] as QualityControl);
			//tempQualityControl = tempQualityControl ?? new QualityControl();
			UpdateViewBagNADCTD(GetQualityControl());
			var aSettingDetailLNACVMC = db.SettingDetail.Where(fod => fod.Setting.code == "LNACVMC").ToList();
			var aKeysQualityAnalysisMayor0 = new List<int>();
			foreach (var aItem in aSettingDetailLNACVMC)
			{
				var aQualityControlDetailFilter = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == aItem.value);
				if (aQualityControlDetailFilter != null)
				{
					aKeysQualityAnalysisMayor0.Add(aQualityControlDetailFilter.id);
				}
			}
			ViewBag.keysQualityAnalysisMayor0 = aKeysQualityAnalysisMayor0;
			//ViewBag.keyFRESCOYSANO = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "FRESCO Y SANO")?.id;
			//ViewBag.keyBASURAGR = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "BASURA GR")?.id;
			//ViewBag.keyBASURAGR = aKeyBASURAGR != null ? aKeyBASURAGR - 1 : aKeyBASURAGR;

			return PartialView("_QualityControlDetailsMasterDetailPartial", qualityControlDetailFilter);
		}

		private void UpdateQualityControlBatchDetail(QualityControlDetail qualityControlDetail, MVCxGridViewBatchUpdateValues<QualityControlDetail, int> updateValues, int id_analysisGroup, int id_qualityAnalysis)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			List<QualityControlDetail> lstQualityControlDetail = (List<QualityControlDetail>)TempData["qualityControlDetailsTmp"];
			List<QualityControlDetail> qualityControlDetailFilter = new List<QualityControlDetail>();
			QualityControlDetail qualityControlDetailTmp = lstQualityControlDetail.FirstOrDefault(fod => fod.id_qualityAnalysis == id_qualityAnalysis);
			List<QualityControlAnalysisGroupAnalysis> qualityControlAnalysisGroupAnalysis = db.QualityControlAnalysisGroupAnalysis.Where(w => w.id_QualityControlAnalysisGroup == id_analysisGroup).ToList();
			if (ModelState.IsValid && qualityControlDetailTmp != null)
			{
				try
				{
					var qualityControlConfigurationAnalysisDataTypeValidate = qualityControlDetailTmp.QualityControl.QualityControlConfiguration.QualityControlConfigurationAnalysisDataTypeValidate.FirstOrDefault(fod => fod.id_qualityAnalysis == id_qualityAnalysis);
					var codeDataType = qualityControlConfigurationAnalysisDataTypeValidate.QualityDataType.code;
					var codeValidate = qualityControlConfigurationAnalysisDataTypeValidate.QualityValidate.code;
					var isConforms = false;
					if (codeDataType.Equals("LTXT") ||
						codeDataType.Equals("LENT") ||
						codeDataType.Equals("LDEC"))
					{
						var resultInt = Convert.ToInt32(qualityControlDetail.result);
						var resultValueAux = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.FirstOrDefault(fod => fod.id == resultInt);

						if (codeValidate.Equals("VAL"))
						{
							isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
						}
						else
						{
							if (codeValidate.Equals("RAN"))
							{
								var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
								var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
								isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
							}
							else
							{
								var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
								var valueAux = listValue.FirstOrDefault(fod => fod.id == resultInt);
								isConforms = (valueAux != null);
							}
						}
						qualityControlDetailTmp.resultValue = resultValueAux.value;
						qualityControlDetailTmp.result = qualityControlDetail.result;
					}
					else
					{
						if (codeDataType.Equals("TXT"))
						{
							if (codeValidate.Equals("VAL"))
							{
								isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
							}
							else
							{
								if (codeValidate.Equals("RAN"))
								{
									isConforms = (qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin ||
												  qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
								}
								else
								{
									var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
									var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
									isConforms = (valueAux != null);
								}
							}
							qualityControlDetailTmp.resultValue = qualityControlDetail.resultValue;
							qualityControlDetailTmp.result = qualityControlDetail.result;
						}
						else
						{
							if (codeDataType.Equals("ENT"))
							{
								var resultInt = Convert.ToInt32(qualityControlDetail.resultValue);

								if (codeValidate.Equals("VAL"))
								{
									var valueValidateInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
									isConforms = (resultInt == valueValidateInt);
								}
								else
								{
									if (codeValidate.Equals("RAN"))
									{
										var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
										var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
										isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
									}
									else
									{
										var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
										var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
										isConforms = (valueAux != null);
									}
								}
								qualityControlDetailTmp.resultValue = qualityControlDetail.resultValue;
								qualityControlDetailTmp.result = qualityControlDetail.result;
							}
							else
							{
								if (codeDataType.Equals("DEC"))
								{
									var resultWithComa = qualityControlDetail.resultValue.Replace('.', ',');
									if (qualityControlDetail.otherResultValue != null)
									{
										var otherResultWithComa = qualityControlDetail.otherResultValue.Replace('.', ',');
										qualityControlDetail.otherResultValue = otherResultWithComa;
									}

									var resultDecimal = Convert.ToDecimal(resultWithComa);

									if (codeValidate.Equals("VAL"))
									{
										var valueValidateDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
										if (valueValidateDecimal != 0)
										{
											isConforms = (resultDecimal == valueValidateDecimal);
										}

									}
									else
									{
										if (codeValidate.Equals("RAN"))
										{
											var valueValidateMinDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
											var valueValidateMaxDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
											isConforms = (resultDecimal >= valueValidateMinDecimal && resultDecimal <= valueValidateMaxDecimal);
										}
										else
										{
											var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
											var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
											isConforms = (valueAux != null);
										}
									}
									qualityControlDetailTmp.resultValue = resultWithComa;
									qualityControlDetailTmp.result = resultWithComa;
									string aValueSettingGRAMCLA = DataProviderSetting.ValueSetting("GRAMCLA");
									bool aValueSettingGRAMCLASI = aValueSettingGRAMCLA == "SI";
									if (aValueSettingGRAMCLASI)
									{
										qualityControlDetailTmp.otherResultValue = qualityControlDetail.otherResultValue;
									}
								}
							}
						}
						this.UpdateModel(qualityControlDetailTmp);
					}
				}
				catch //(Exception ex)
				{

				}

			}
			//TempData.Keep("qualityControl");
			TempData["qualityControlDetailsTmp"] = lstQualityControlDetail;
			TempData.Keep("qualityControlDetailsTmp");
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult QualityControlDetailsDetailUpdatePartial(QualityControlDetail qualityControlDetail, int id_analysisGroup)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}

			ViewData["id_analysisGroup"] = id_analysisGroup;
			List<QualityControlDetail> lstQualityControlDetail = (List<QualityControlDetail>)TempData["qualityControlDetailsTmp"];
			List<QualityControlDetail> qualityControlDetailFilter = new List<QualityControlDetail>();
			QualityControlDetail qualityControlDetailTmp = lstQualityControlDetail.FirstOrDefault(fod => fod.id_qualityAnalysis == qualityControlDetail.id_qualityAnalysis);
			List<QualityControlAnalysisGroupAnalysis> qualityControlAnalysisGroupAnalysis = db.QualityControlAnalysisGroupAnalysis.Where(w => w.id_QualityControlAnalysisGroup == id_analysisGroup).ToList();
			if (ModelState.IsValid && qualityControlDetailTmp != null)
			{
				try
				{
					var qualityControlConfigurationAnalysisDataTypeValidate = qualityControlDetailTmp.QualityControl.QualityControlConfiguration.QualityControlConfigurationAnalysisDataTypeValidate.FirstOrDefault(fod => fod.id_qualityAnalysis == qualityControlDetailTmp.id_qualityAnalysis);
					var codeDataType = qualityControlConfigurationAnalysisDataTypeValidate.QualityDataType.code;
					var codeValidate = qualityControlConfigurationAnalysisDataTypeValidate.QualityValidate.code;
					var isConforms = false;
					if (codeDataType.Equals("LTXT") ||
						codeDataType.Equals("LENT") ||
						codeDataType.Equals("LDEC"))
					{
						var resultInt = Convert.ToInt32(qualityControlDetail.result);
						var resultValueAux = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.FirstOrDefault(fod => fod.id == resultInt);

						if (codeValidate.Equals("VAL"))
						{
							isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
						}
						else
						{
							if (codeValidate.Equals("RAN"))
							{
								var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
								var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
								isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
							}
							else
							{
								var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
								var valueAux = listValue.FirstOrDefault(fod => fod.id == resultInt);
								isConforms = (valueAux != null);
							}
						}
						qualityControlDetailTmp.resultValue = resultValueAux.value;
						qualityControlDetailTmp.result = qualityControlDetail.result;
						qualityControlDetailTmp.otherResultValue = qualityControlDetail.otherResultValue;
					}
					else
					{
						if (codeDataType.Equals("TXT"))
						{
							if (codeValidate.Equals("VAL"))
							{
								isConforms = qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidate;
							}
							else
							{
								if (codeValidate.Equals("RAN"))
								{
									isConforms = (qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin ||
												  qualityControlDetail.result == qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
								}
								else
								{
									var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
									var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
									isConforms = (valueAux != null);
								}
							}
							qualityControlDetailTmp.resultValue = qualityControlDetail.result;
							qualityControlDetailTmp.result = qualityControlDetail.result;
							qualityControlDetailTmp.otherResultValue = qualityControlDetail.otherResultValue;
						}
						else
						{
							if (codeDataType.Equals("ENT"))
							{
								var resultInt = Convert.ToInt32(qualityControlDetail.resultValue);

								if (codeValidate.Equals("VAL"))
								{
									var valueValidateInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
									isConforms = (resultInt == valueValidateInt);
								}
								else
								{
									if (codeValidate.Equals("RAN"))
									{
										var valueValidateMinInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
										var valueValidateMaxInt = Convert.ToInt32(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
										isConforms = (resultInt >= valueValidateMinInt && resultInt <= valueValidateMaxInt);
									}
									else
									{
										var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
										var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
										isConforms = (valueAux != null);
									}
								}
								qualityControlDetailTmp.resultValue = qualityControlDetail.result;
								qualityControlDetailTmp.result = qualityControlDetail.result;
								qualityControlDetailTmp.otherResultValue = qualityControlDetail.otherResultValue;
							}
							else
							{
								if (codeDataType.Equals("DEC"))
								{
									var resultWithComa = qualityControlDetail.resultValue.Replace('.', ',');
									if (qualityControlDetail.otherResultValue != null)
									{
										var otherResultWithComa = qualityControlDetail.otherResultValue.Replace('.', ',');
										qualityControlDetail.otherResultValue = otherResultWithComa;
									}
									var resultDecimal = Convert.ToDecimal(resultWithComa);

									if (codeValidate.Equals("VAL"))
									{
										var valueValidateDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidate);
										isConforms = (resultDecimal == valueValidateDecimal);
									}
									else
									{
										if (codeValidate.Equals("RAN"))
										{
											var valueValidateMinDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMin);
											var valueValidateMaxDecimal = Convert.ToDecimal(qualityControlConfigurationAnalysisDataTypeValidate.valueValidateMax);
											isConforms = (resultDecimal >= valueValidateMinDecimal && resultDecimal <= valueValidateMaxDecimal);
										}
										else
										{
											var listValue = qualityControlConfigurationAnalysisDataTypeValidate.QualityControlConfigurationAnalysisDataTypeValidateDetailValueValidate.Select(s => new { id = s.id_qualityControlConfigurationAnalysisDataTypeValidateDetailValue, value = s.QualityControlConfigurationAnalysisDataTypeValidateDetailValue.value });
											var valueAux = listValue.FirstOrDefault(fod => fod.value == qualityControlDetail.result);
											isConforms = (valueAux != null);
										}
									}
									qualityControlDetailTmp.resultValue = resultWithComa;
									qualityControlDetailTmp.result = resultWithComa;
									qualityControlDetailTmp.otherResultValue = qualityControlDetail.otherResultValue;
								}
							}
						}
						this.UpdateModel(qualityControlDetailTmp);
					}
				}
				catch //(Exception ex)
				{

				}

			}
			//TempData.Keep("qualityControl");
			TempData["qualityControlDetailsTmp"] = lstQualityControlDetail;
			TempData.Keep("qualityControlDetailsTmp");
			var qualityControlAnalysisGroupTmp = db.QualityControlAnalysisGroup.FirstOrDefault(w => w.id == id_analysisGroup);

			var visualizationTypeData = db.VisualizationTypeData.FirstOrDefault(fod => fod.id == qualityControlAnalysisGroupTmp.id_VisualizationTypeData);

			try
			{
				qualityControlDetailFilter = lstQualityControlDetail.Join(qualityControlAnalysisGroupAnalysis,
							it => it.id_qualityAnalysis,
							tt => tt.id_QualityAnalysis,
							(it, tt) => it).ToList();
			}
			catch //(Exception e)
			{

			}
			var aSettingDetailLNACVMC = db.SettingDetail.Where(fod => fod.Setting.code == "LNACVMC").ToList();
			var aKeysQualityAnalysisMayor0 = new List<int>();
			foreach (var aItem in aSettingDetailLNACVMC)
			{
				var aQualityControlDetailFilter = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == aItem.value);
				if (aQualityControlDetailFilter != null)
				{
					aKeysQualityAnalysisMayor0.Add(aQualityControlDetailFilter.id);
				}
			}
			ViewBag.keysQualityAnalysisMayor0 = aKeysQualityAnalysisMayor0;
			//ViewBag.keyFRESCOYSANO = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "FRESCO Y SANO")?.id;
			//ViewBag.keyBASURAGR = qualityControlDetailFilter.FirstOrDefault(fod => fod.QualityAnalysis.name == "BASURA GR")?.id;
			//ViewBag.keyBASURAGR = aKeyBASURAGR != null ? aKeyBASURAGR : aKeyBASURAGR;
			return PartialView("_QualityControlDetailsMasterDetailPartial", qualityControlDetailFilter.OrderBy(od => od.id).ToList());
		}

		//COMBOBOX
		[HttpPost]
		public ActionResult GetQualityControlConformityResult()
		{
			QualityControl qualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
																//qualityControl = qualityControl ?? new QualityControl();
																//TempData.Keep("qualityControl");
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			return PartialView("_QualityControlConformityResult", qualityControl);

		}

		#endregion

		#region SINGLE CHANGE DOCUMENT STATE

		[HttpPost]
		public ActionResult Cancel(int id)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			QualityControl qualityControl = db.QualityControl.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

					if (qualityControl.Document.DocumentState.code != "01")//Pendiente
					{
						//TempData.Keep("qualityControl");
						ViewData["EditMessage"] = ErrorMessage("No se puede anular el análisis debido a tener estado aprobado, solo se le pueden aplicar acciones correctivas en los detalles que aún no se le haya aplicado la misma");
						return PartialView("_QualityControlMainFormPartial", qualityControl);
					}

					if (qualityControl != null && documentState != null)
					{

						qualityControl.Document.id_documentState = documentState.id;
						qualityControl.Document.DocumentState = documentState;

						if ((qualityControl?.ProductionLotDetailQualityControl?.Count ?? 0) > 0)
						{
							foreach (var lotdetq in qualityControl?.ProductionLotDetailQualityControl)
							{
								lotdetq.IsDelete = true;
							}
						}

						db.QualityControl.Attach(qualityControl);
						db.Entry(qualityControl).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						SetQualityControl(qualityControl);
						//TempData["qualityControl"] = qualityControl;
						//TempData.Keep("qualityControl");

						ViewData["EditMessage"] = SuccessMessage("El análisis: " + qualityControl.QualityControlConfiguration.name + ", con número: " + qualityControl.number + " guardado exitosamente");

					}
				}
				catch (Exception e)
				{
					//TempData.Keep("qualityControl");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}

			//UpdateViewBagNADCTD(qualityControl);
			UpdateViewBagTotalUnit(GetQualityControl());

			//Agregar Guía de Remisión
			var idPld = db.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == qualityControl.id)?.id_productionLotDetail;
			int id_rgdTmp = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idPld)?.id_remissionGuideDetail ?? 0;

			var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
			if (aSettingANL != null && aSettingANL.value == "NO")
			{
				if (id_rgdTmp > 0)
				{
					qualityControl.remissionGuideNumber = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Document.number;
					qualityControl.remissionGuideProcess = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Person2.processPlant;
				}

			}
			else if (aSettingANL != null && aSettingANL.value == "SI")
			{
				qualityControl.remissionGuideNumber = "";
				qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
			}
			else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
			{
				qualityControl.remissionGuideNumber = "";
				qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
			}
			var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idPld);
			//Agregar Piscina
			if (pldTmp != null)
			{
				qualityControl.poolName = pldTmp.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
			}

			return PartialView("_QualityControlMainFormPartial", qualityControl);

		}

		[HttpPost]
		public ActionResult Revert(int id)
		{
			QualityControl qcTmp = new QualityControl();
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			qcTmp = GetQualityControl();
			//if (TempData["qualityControl"] != null)
			//{

			//    qcTmp = (QualityControl)TempData["qualityControl"];
			//    TempData.Keep("qualityControl");
			//}

			QualityControl qualityControl = db.QualityControl.FirstOrDefault(r => r.id == id);
			//qualityControl.poolName = qcTmp.poolName;
			//qualityControl.remissionGuideNumber = qcTmp.remissionGuideNumber;
			//qualityControl.remissionGuideProcess = qcTmp.remissionGuideProcess;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					if (qualityControl != null && qualityControl.id_lot != null
						&& qualityControl.id_lot > 0 && qualityControl.QualityControlConfiguration.code == "QARMP")
					{
						// Validar estado de Recepción
						string validCodeStateProductionLotReception = "01";
						int? idValidCodeStateProductionLotReception = db.ProductionLotState
																		.FirstOrDefault(r => r.code == validCodeStateProductionLotReception)?.id;
						int? idProductionLot = db.ProductionLot
													.FirstOrDefault(r => r.id == qualityControl.id_lot && r.Lot.Document.DocumentState.code != "05")?.id;

						int? idStatusProductionLot = db.ProductionLot
															.FirstOrDefault(r => r.id == idProductionLot)?.id_ProductionLotState;

						if (idStatusProductionLot != null && idStatusProductionLot != idValidCodeStateProductionLotReception)
						{
							throw new Exception(
							$"No se puede reversar el control de calidad debido a que el lote se encuentra en estado diferente a PENDIENTE DE RECEPCIÓN.");

						}

						var existAdvancedProvider = db.AdvanceProvider
							.FirstOrDefault(e => e.id_Lot == qualityControl.id_lot && e.Document.DocumentState.code != "05");

						if (existAdvancedProvider != null)
						{
							throw new Exception(
								$"No se puede reversar el control de calidad debido a que está relacionado con un anticipo de proveedor. " +
								$"Número: {existAdvancedProvider.Document.number}. Estado: {existAdvancedProvider.Document.DocumentState.name}");
						}

						if (qualityControl.Lot.ProductionLot.ProductionLotState.code == "01")
						{
							DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01");
							qualityControl.Document.DocumentState = documentStatePendiente;

							db.QualityControl.Attach(qualityControl);
							db.Entry(qualityControl).State = EntityState.Modified;
							db.SaveChanges();
							trans.Commit();
						}
						else
						{
							//TempData.Keep("qualityControl");
							ViewData["EditMessage"] = ErrorMessage("No se puede reversar el control de calidad debido a que pertenece a un Lote de Recepción cuyo estado es : " + qualityControl.Lot.ProductionLot.ProductionLotState.name);
							return PartialView("_QualityControlMainFormPartial", qualityControl);
						}
					}

					//if (businessOportunity.BusinessOportunityResults.BusinessOportunityState.code != "01")//ABIERTA
					//{
					//    TempData.Keep("businessOportunity");
					//    ViewData["EditMessage"] = ErrorMessage("No se puede reversar la oportunidad debido a tener estado de oportunidad: " + businessOportunity.BusinessOportunityResults.BusinessOportunityState.name);
					//    return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);
					//}

					//DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

					//if (businessOportunity != null && documentStatePendiente != null)
					//{
					//    businessOportunity.Document.id_documentState = documentStatePendiente.id;
					//    businessOportunity.Document.DocumentState = documentStatePendiente;

					//    db.BusinessOportunity.Attach(businessOportunity);
					//    db.Entry(businessOportunity).State = EntityState.Modified;

					//    db.SaveChanges();
					//    trans.Commit();

					//    TempData["businessOportunity"] = businessOportunity;
					//    TempData.Keep("businessOportunity");

					//    ViewData["EditMessage"] = SuccessMessage("Oportunidad: " + businessOportunity.name + ", con número: " + businessOportunity.Document.number + " reversada exitosamente");

					//}
				}
				catch (Exception e)
				{
					//TempData.Keep("qualityControl");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}

			//UpdateViewBagNADCTD(qualityControl);
			UpdateViewBagTotalUnit(GetQualityControl());

			//Agregar Guía de Remisión
			var idPld = db.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == qualityControl.id)?.id_productionLotDetail;
			int id_rgdTmp = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idPld)?.id_remissionGuideDetail ?? 0;

			var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
			if (aSettingANL != null && aSettingANL.value == "NO")
			{
				if (id_rgdTmp > 0)
				{
					qualityControl.remissionGuideNumber = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Document.number;
					qualityControl.remissionGuideProcess = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Person2.processPlant;
				}
			}
			else if (aSettingANL != null && aSettingANL.value == "SI")
			{
				qualityControl.DrawersReceived = qcTmp.DrawersReceived;
				qualityControl.remissionGuideNumber = "";
				qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
			}
			else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
			{
				qualityControl.DrawersReceived = qcTmp.DrawersReceived;
				qualityControl.remissionGuideNumber = "";
				qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
			}
			var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idPld);
			//Agregar Piscina
			if (pldTmp != null)
			{
				qualityControl.poolName = pldTmp.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
			}

			return PartialView("_QualityControlMainFormPartial", qualityControl);
		}

		#endregion

		#region ACTIONS

		[HttpPost, ValidateInput(false)]
		public JsonResult Actions(int id)
		{
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

			QualityControl qualityControl = db.QualityControl.FirstOrDefault(r => r.id == id);
			ProductionLot pl = db.ProductionLot.FirstOrDefault(fod => fod.id == qualityControl.id_lot);
			//int state = businessOportunity.Document.DocumentState.id;
			string state = qualityControl.Document.DocumentState.code;

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
				if (pl != null)
				{
					if (!(pl.ProductionLotState.code == "01"))
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
					else
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
				}
				else
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
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PAGINATION

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_qualityControl)
		{
			//TempData.Keep("qualityControl");

			int index = db.QualityControl.OrderByDescending(r => r.id).Select(r => r.id).ToList().FindIndex(id => id == id_qualityControl);

			var result = new
			{
				maximunPages = db.QualityControl.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			QualityControl qualityControl = db.QualityControl.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (qualityControl != null)
			{
				SetQualityControl(qualityControl);
				//TempData["qualityControl"] = qualityControl;
				//TempData.Keep("qualityControl");
				UpdateViewBagTotalUnit(GetQualityControl());
				//Agregar Guía de Remisión
				var idPld = db.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.id_qualityControl == qualityControl.id)?.id_productionLotDetail;
				int id_rgdTmp = db.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idPld)?.id_remissionGuideDetail ?? 0;

				var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
				if (aSettingANL != null && aSettingANL.value == "NO")
				{
					if (id_rgdTmp > 0)
					{
						qualityControl.remissionGuideNumber = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Document.number;
						qualityControl.remissionGuideProcess = db.RemissionGuideDetail.FirstOrDefault(fod => fod.id == id_rgdTmp).RemissionGuide.Person2.processPlant;
					}
				}
				else if (aSettingANL != null && aSettingANL.value == "SI")
				{
					qualityControl.remissionGuideNumber = "";
					qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
				}
				else if (aSettingANL != null && aSettingANL.value == "SELGUIA")
				{
					qualityControl.remissionGuideNumber = "";
					qualityControl.remissionGuideProcess = qualityControl?.Lot?.ProductionLot?.Person1?.processPlant ?? "";
				}
				var pldTmp = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idPld);
				//Agregar Piscina
				if (pldTmp != null)
				{
					qualityControl.poolName = pldTmp.ProductionLot?.ProductionUnitProviderPool?.name ?? "";
				}

				return PartialView("_QualityControlMainFormPartial", qualityControl);
			}

			//TempData.Keep("qualityControl");
			//SetViewData();

			return PartialView("_QualityControlMainFormPartial", new QualityControl());
		}

		#endregion

		#region AXILIAR FUNCTIONS

		private bool ValidaExistenciaGuiaEnOtroControlCalidad(ref QualityControl qc, int id_processType, ref string mensaje)
		{
			bool respuesta = true;

			var id_pt = id_processType;
			var idProductionLotDetail = qc.ProductionLotDetailQualityControl.Select(s => s.id_productionLotDetail).FirstOrDefault();

			var lotNumber = db.ProductionLotDetail.FirstOrDefault(fod => fod.id == idProductionLotDetail)?.ProductionLot?.number ?? "";

			var lsTmpExist = db.ProductionLotDetailQualityControl
								.Where(w => w.ProductionLotDetail.ProductionLot.number == lotNumber
								&& w.QualityControl.Document.DocumentState.code != "05")
								.Select(s => new { s.QualityControl.id_processType }).ToList();

			if (lsTmpExist != null && lsTmpExist.Count > 0)
			{
				var existDiferente = lsTmpExist.FirstOrDefault(fod => fod.id_processType != id_pt);
				if (existDiferente != null)
				{
					mensaje = "Ya existe un registro grabado con un tipo de proceso diferente al indicado.";
					respuesta = false;
					return respuesta;
				}
			}

			return respuesta;
		}

		[HttpPost]
		public ActionResult ComboBoxSize(int? id_size, int? id_processType)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			//TempData.Keep("qualityControl");

			return GridViewExtension.GetComboBoxCallbackResult(p =>
			{
				p.Width = Unit.Percentage(100);

				p.ValueField = "id";
				p.TextField = "name";
				//p.DataSource = DataProviderCopackingTariff.CopackingTariffByCompanyWithCurrentAndProviderForLiquidation((int?)ViewData["id_company"], productionLot.id_provider, dtNow);
				p.ValueType = typeof(int);
				p.ClientInstanceName = "id_size";
				p.CallbackRouteValues = new { Controller = "QualityControl", Action = "ComboBoxSize" };
				p.DropDownStyle = DropDownStyle.DropDownList;
				p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				p.EnableSynchronization = DefaultBoolean.False;
				p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
				p.ClientSideEvents.BeginCallback = "Size_OnBeginCallback";
				p.ClientSideEvents.EndCallback = "Size_OnEndCallback";
				p.ClientSideEvents.Init = "Size_OnInit";
				//p.ReadOnly = (codeState != "01");
				//Es de Sistema el Tipo de Analisis
				//p.ShowModelErrors = true;

				//p.ValidationSettings.RequiredField.IsRequired = true;
				//p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
				//p.ValidationSettings.CausesValidation = true;
				//p.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
				//p.ValidationSettings.ValidateOnLeave = true;
				//p.ValidationSettings.SetFocusOnError = true;
				//p.ValidationSettings.ErrorText = "Valor Incorrecto";

				//p.ClientSideEvents.BeginCallback = "ProductionLotPriceListPayment_BeginCallback";

				//p.ValidationSettings.EnableCustomValidation = true;
				//p.DropDownStyle = DropDownStyle.DropDown;
				//p.ClientSideEvents.Validation = "OnPriceListValidation";
				//p.ClientSideEvents.SelectedIndexChanged = "ComboCopackingTariff_SelectedIndexChanged";
				p.BindList(DataProviderItemSize.ItemSizebyProcessTypeAndCurrentCOL(id_processType, id_size));

			});
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult RefreshQualityControlDetail(int? id_qualityControlConfiguration)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			QualityControl qualityControl = GetQualityControl();// (TempData["qualityControl"] as QualityControl);

			QualityControlConfiguration qualityControlConfiguration = db.QualityControlConfiguration.FirstOrDefault(i => i.id == id_qualityControlConfiguration);

			for (int i = qualityControl.QualityControlDetail.Count - 1; i >= 0; i--)
			{
				var detail = qualityControl.QualityControlDetail.ElementAt(i);

				qualityControl.QualityControlDetail.Remove(detail);
			}

			if (qualityControlConfiguration != null)
			{
				foreach (var detailAnalysis in qualityControlConfiguration.QualityControlConfigurationAnalysisDataTypeValidate)
				{
					qualityControl.id_qualityControlConfiguration = qualityControlConfiguration.id;
					qualityControl.QualityControlConfiguration = qualityControlConfiguration;
					qualityControl.QualityControlDetail.Add(
						new QualityControlDetail
						{
							id = qualityControl.QualityControlDetail.Count() > 0 ? qualityControl.QualityControlDetail.Max(pld => pld.id) + 1 : 1,
							id_qualityAnalysis = detailAnalysis.id_qualityAnalysis,
							QualityAnalysis = db.QualityAnalysis.FirstOrDefault(fod => fod.id == detailAnalysis.id_qualityAnalysis),
							QualityControl = qualityControl,
							result = string.Empty,
							resultValue = string.Empty,
							isConforms = false,
							isSave = false
						});

				}
			}

			var result = new
			{
				Message = "OK"
			};

			SetQualityControl(qualityControl);
			//TempData["qualityControl"] = qualityControl;
			//TempData.Keep("qualityControl");
			//SetViewData();
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult WarehouseChangeData(int id_warehouse)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
									   .Select(s => new
									   {
										   id = s.id,
										   name = s.name
									   });

			var result = new
			{
				warehouseLocations = warehouseLocations
									.Select(s => new
									{
										id = s.id,
										name = s.name
									})
			};

			//TempData.Keep("qualityControl");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult WarehouseLocationChangeData(int id_warehouse, int id_warehouseLocation)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_warehouse == id_warehouse &&
																		 i.id_warehouseLocation == id_warehouseLocation &&
																		 i.id_inventoryMoveDetailNext == null &&
																		 i.balance != 0);

			var items = new List<Item>();

			foreach (var detail in inventoryMoveDetails)
			{
				if (!items.Contains(detail.Item))
				{
					items.Add(detail.Item);
				}
			}

			var result = new
			{
				items = items.Select(s => new
				{
					id = s.id,
					name = s.name
				})
			};

			//TempData["inventoryMove"] = inventoryMove;
			//TempData.Keep("qualityControl");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ItemChangeData(int id_warehouse, int id_warehouseLocation, int id_item)
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}

			var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_warehouse == id_warehouse &&
																		 i.id_warehouseLocation == id_warehouseLocation &&
																		 i.id_item == id_item &&
																		 i.id_inventoryMoveDetailNext == null &&
																		 i.balance != 0);
			var lotZero = new Lot { id = 0, number = "(Sin Lote)" };

			var lots = new List<Lot>();

			foreach (var detail in inventoryMoveDetails)
			{
				if (!lots.Contains(detail.Lot ?? lotZero))
				{
					lots.Add(detail.Lot ?? lotZero);
				}
			}

			var result = new
			{
				lots = lots.Select(s => new
				{
					id = s.id,
					number = s.number
				})
			};

			//TempData["inventoryMove"] = inventoryMove;
			//TempData.Keep("qualityControl");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult AnyResultEqualsNull()
		{
			if (TempData["productionLotQuality"] != null)
			{
				TempData.Keep("productionLotQuality");
			}
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			QualityControl qualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
			List<QualityControlDetail> qualityControlDetail = new List<QualityControlDetail>();

			qualityControl = qualityControl ?? new QualityControl();
			var result = new
			{
				itsNull = 0,
				Error = ""

			};

			var itsNullqualityControlResultAux = qualityControl.
													  QualityControlDetail.
													  FirstOrDefault(fod => fod.result == null ||
																	 fod.resultValue == null);
			if (itsNullqualityControlResultAux != null)
			{
				result = new
				{
					itsNull = 1,
					Error = "No puede quedar ningun resultado Nulo, por favor regístrelos todos."

				};

			}


			//TempData.Keep("qualityControl");
			return Json(result, JsonRequestBehavior.AllowGet);

		}

		#endregion

		#region "Envío de correo de notificación de cambio de proceso"

		private string EnviarCorreoNotificacionCambioProceso(int idQualityControl)
		{
			const string codigoCamaronEnteroMateriaPrima = "MP000001";
			const string codigoPorDescabezarProceso = "ENC";

			try
			{
				using (var db = new DBContext())
				{
					// Verificamos si se cumplen las condiciones para enviar el correo electrónico...
					var qualityControl = db.QualityControl
						.FirstOrDefault(qc => qc.id == idQualityControl);

					if (qualityControl.Item.masterCode != codigoCamaronEnteroMateriaPrima)
					{
						// No es CAMARÓN ENTERO
						return "ERR: No es CAMARÓN ENTERO";
					}
					if (qualityControl.ProcessType.code != codigoPorDescabezarProceso)
					{
						// No es POR DESCABEZAR
						return "ERR: Proceso no es POR DESCABEZAR";
					}

					// Recuperamos la lista de destinatarios del correo
					var mailDestination = db.Setting
						.FirstOrDefault(s => s.code == "LDANACAL")?
						.value;

					if (String.IsNullOrEmpty(mailDestination))
					{
						// No hay destinatarios del correo
						return "ERR: No existen destinatarios configurados para el correo electrónico";
					}

					// Recuperamos la configuración de envío de correo electrónico...
					var mailFrom = ConfigurationManager.AppSettings["correoDefaultDesde"];
					var passwordMailFrom = ConfigurationManager.AppSettings["contrasenaCorreoDefault"];
					var passwordMailFromUncrypted = clsEncriptacion1.LeadnjirSimple.Desencriptar(passwordMailFrom);
					var smtpHost = ConfigurationManager.AppSettings["smtpHost"];
					var puertoHost = Int32.Parse(ConfigurationManager.AppSettings["puertoHost"]);
					var mensajePrueba = ConfigurationManager.AppSettings["Pruebas"];

					// Recuperamos el contenido para el correo
					var nombreProveedor = qualityControl.Lot?
						.ProductionLot?
						.Provider?
						.Person?.fullname_businessName;

					var nombreCamaronera = qualityControl.Lot?
						.ProductionLot?
						.ProductionUnitProvider?
						.name;

					var direccionCamaronera = qualityControl.Lot?
						.ProductionLot?
						.ProductionUnitProvider?
						.address;

					var nombrePiscinaCamaronera = qualityControl.Lot?
						.ProductionLot?
						.ProductionUnitProviderPool?
						.name;

					var numeroLote = qualityControl.Lot?
						.ProductionLot?
						.internalNumber;

					var ordenCompra = qualityControl.Lot?
						.ProductionLot?
						.ProductionLotDetail?
						.FirstOrDefault()?
						.ProductionLotDetailPurchaseDetail?
						.FirstOrDefault()?
						.PurchaseOrderDetail?
						.PurchaseOrder;

					var numeroOrdenCompra = ordenCompra?
						.Document?
						.number;
					var fechaOrdenCompra = ordenCompra?
						.Document?
						.emissionDate;

					var comprador = (ordenCompra != null)
						? db.Person
							.FirstOrDefault(p => p.id == ordenCompra.id_buyer)
						: null;

					var nombreComprador = comprador?.fullname_businessName;
					var correoElectronicoComprador = comprador?.email;
					if (!String.IsNullOrEmpty(correoElectronicoComprador))
					{
						mailDestination = correoElectronicoComprador + ";" + mailDestination;
					}

					var cantidadRemitida = qualityControl.Lot?
						.ProductionLot?
						.totalQuantityRecived;

					var fechaRecepcion = qualityControl.Lot?
						.ProductionLot?
						.receptionDate;

					var tipoProceso = qualityControl.ProcessType.name;
					var productoProceso = qualityControl.Item.name;


					var asuntoMensaje = "CAMBIO DE PROCESO DE MATERIA PRIMA";

					var cuerpoMensaje = $"<b>Proveedor:</b> {nombreProveedor}"
						+ "<br />"
						+ $"<b>Camaronera:</b> {nombreCamaronera}"
						+ "<br />"
						+ $"<b>Piscina:</b> {nombrePiscinaCamaronera}"
						+ "<br />"
						+ $"<b>Dirección:</b> {direccionCamaronera}"
						+ "<br />"
						+ $"<b>Número de Lote:</b> {numeroLote}"
						+ "<br />"
						+ $"<b>Fecha de Emisión de O/C:</b> {fechaOrdenCompra:dd/MM/yyyy}"
						+ "<br />"
						+ $"<b>Número de O/C:</b> {numeroOrdenCompra}"
						+ "<br />"
						+ $"<b>Cantidad Remitida:</b> {cantidadRemitida:#,0}"
						+ "<br />"
						+ $"<b>Fecha de Recepción:</b> {fechaRecepcion:dd/MM/yyyy}"
						+ "<br />"
						+ $"<b>Comercial:</b> {nombreComprador}"
						+ "<br />"
						+ $"<b>Proceso de:</b> {productoProceso}"
						+ "<br />"
						+ $"<b>Tipo de Proceso:</b> {tipoProceso}";

					if (mensajePrueba == "SI")
					{
						asuntoMensaje = "MENSAJE DE PRUEBA - " + asuntoMensaje;
						cuerpoMensaje = "<b>MENSAJE DE PRUEBA</b>"
							+ "<br />"
							+ cuerpoMensaje;
					}

					var respuestaCorreo = clsCorreoElectronico.EnviarCorreoElectronico(
						mailDestination, mailFrom, asuntoMensaje, smtpHost, puertoHost,
						passwordMailFromUncrypted, cuerpoMensaje, ';');

					if (respuestaCorreo == "OK")
					{
						using (var transaction = db.Database.BeginTransaction())
						{
							try
							{
								qualityControl.hasSentEmail = true;
								db.QualityControl.Attach(qualityControl);
								db.Entry(qualityControl).State = EntityState.Modified;
								db.SaveChanges();

								transaction.Commit();
							}
							catch
							{
								transaction.Rollback();
								throw;
							}
						}
					}

					return respuestaCorreo;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Error enviando correo de Análisis de Calidad:");
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return $"ERROR: {e.Message}";
			}
		}

		#endregion

		#region PURCHASE ORDER REPORTS CRYSTAL
		public JsonResult PrintQualityCOntrolReport(int id_Quality)
		{
			var qualityControl = GetQualityControl();//(TempData["qualityControl"] as QualityControl);
			SetQualityControl(qualityControl);
			//TempData["qualityControl"] = qualityControl;
			//TempData.Keep("qualityControl");

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = id_Quality;

			paramLst.Add(_param);


			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "QUALIT";
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


	}
}