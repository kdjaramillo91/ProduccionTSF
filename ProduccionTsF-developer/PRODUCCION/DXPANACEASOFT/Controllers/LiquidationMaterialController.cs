using DevExpress.Office.Utils;
using DocumentFormat.OpenXml.Bibliography;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
	public class LiquidationMaterialController : DefaultController
	{
		// GET: LiquidationMaterial
		public ActionResult Index()
		{
			BuildViewData();
			return View();
		}

		private LiquidationMaterialDTO GetLiquidationMaterialDTO()
		{
            if (!(Session["LiquidationMaterialDTO"]  is LiquidationMaterialDTO liquidationMaterialDTO))
				liquidationMaterialDTO = new LiquidationMaterialDTO();
			return liquidationMaterialDTO;
		}

		private List<LiquidationMaterialResultConsultDTO> GetLiquidationMaterialResultConsultDTO()
		{
			if (!(Session["LiquidationMaterialResultConsultDTO"] is List<LiquidationMaterialResultConsultDTO> liquidationMaterialResultConsult))
				liquidationMaterialResultConsult = new List<LiquidationMaterialResultConsultDTO>();
			return liquidationMaterialResultConsult;
		}

		private void SetLiquidationMaterialDTO(LiquidationMaterialDTO liquidationMaterialDTO)
		{
			Session["LiquidationMaterialDTO"] = liquidationMaterialDTO;
        }

		private void SetLiquidationMaterialResultConsultDTO(List<LiquidationMaterialResultConsultDTO> liquidationMaterialResultConsultDTO)
		{
			Session["LiquidationMaterialResultConsultDTO"] = liquidationMaterialResultConsultDTO;
		}

		private void Estados(string From = "")
		{
			ViewData["Estados"] = db.tbsysDocumentTypeDocumentState
					.Where(d => d.DocumentType.code.Equals("00")
					//|| d.DocumentType.code.Equals("19") 
					//|| d.DocumentType.code.Equals("20") 
					//|| d.DocumentType.code.Equals("21")
					)
					.Select(s => new SelectListItem
					{
						Text = s.DocumentState.name,
						Value = s.id_DocumenteState.ToString()
					}).Distinct().ToList();
		}

		public ActionResult ComboBoxEstados(string From, bool enabled = true, bool IsOwner = true)
		{
			using (DBContext db = new DBContext())
			{
				Estados();

				if (From.Equals("Edit"))
				{
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxEstadosEdit");
				}
				else
				{
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxEstadosIndex");
				}
			}
		}

		private void Proveedores(string From = "")
		{
			using (DBContext db = new DBContext())
			{
				if (From.Equals("Edit"))
				{
					ViewData["Proveedores"] = db.Provider.Select(s => new SelectListItem
					{
						Text = s.Person.fullname_businessName,
						Value = s.id.ToString()
					}).ToList();
				}
				else
				{
					ViewData["Proveedores"] = db.Provider.Select(s => new SelectListItem
					{
						Text = s.Person.fullname_businessName,
						Value = s.id.ToString()
					}).ToList();
				}
			}
		}

		public ActionResult ComboBoxProveedores(string From, bool enabled = true, bool IsOwner = true)
		{
			using (DBContext db = new DBContext())
			{
				if (From.Equals("Edit"))
				{
					Proveedores(From);
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxProveedoresEdit");
				}
				else
				{
					Proveedores(From);
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxProveedoresIndex");
				}
			}
		}

		private void Productos(string From = "")
		{
			//return db.Item.Where(s => s.id_company == id_company && s.InventoryLine.code == "MI" && s.ItemType.code == "MDD").Select(s => new { id = s.id, name = s.name }).ToList();
			using (DBContext db = new DBContext())
			{
				if (From.Equals("Edit"))
				{
					ViewData["Productos"] = db.Item.Select(s => new SelectListItem
					{
						Text = s.name,
						Value = s.id.ToString()
					}).ToList();
				}
				else
				{
					ViewData["Productos"] = db.Item.Select(s => new SelectListItem
					{
						Text = s.name,
						Value = s.id.ToString()
					}).ToList();
				}
			}
		}

		public ActionResult ComboBoxProductos(string From, bool enabled = true, bool IsOwner = true)
		{
			using (DBContext db = new DBContext())
			{
				if (From.Equals("Edit"))
				{
					Productos(From);
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxProductosEdit");
				}
				else
				{
					Productos(From);
					ViewBag.enabled = enabled;
					ViewBag.IsOwner = IsOwner;
					return PartialView("_ComboBoxProductosIndex");
				}
			}
		}

		private void BuildViewData(string From = "", PriceList priceList = null, bool enabled = true)
		{
			Session["consulta"] = null;
			Estados(From);
			Proveedores(From);
			Productos(From);

			if (!From.Equals("Edit"))
				return;

		}

		private List<LiquidationMaterialResultConsultDTO> GetLiquidationMaterialDTO(LiquidationMaterialConsultDTO consulta)
		{
			using (var db = new DBContext())
			{
				var consultaAux = Session["consulta"] as LiquidationMaterialConsultDTO;
				if (consultaAux != null && consulta.fechaInicioEmision == null)
				{
					consulta = consultaAux;
				}


				var consultResult = db.LiquidationMaterialSupplies.Where(LiquidationMaterialQueryExtensions.GetRequestByFilter(consulta));

				//var lstConsultResult = consultResult.ToList();
				//if (lstConsultResult != null && lstConsultResult.Count > 0 && !(consulta.numeroGuia == "" || consulta.numeroGuia == null))
				//{
				//    lstConsultResult = lstConsultResult.Where(w => w.ResultReceptionDispatchMaterial != null 
				//    && w.ResultReceptionDispatchMaterial.Count > 0 
				//    && w.ResultReceptionDispatchMaterial.Any(a => a.numberRemissionGuide.Contains(consulta.numeroGuia))).ToList();
				//}

				var query = consultResult.Select(t => new LiquidationMaterialResultConsultDTO
				{
					id = t.id,
					numberDocument = t.Document.number,
					emissionDateDocument = t.Document.emissionDate,
					id_provider = t.idProvider,
					provider = t.Provider.Person.fullname_businessName,
					id_documentState = t.Document.id_documentState,
					documentState = t.Document.DocumentState.name,

					canEdit = t.Document.DocumentState.code.Equals("01"),
					canAproved = t.Document.DocumentState.code.Equals("01"),
					canAuthorize = t.Document.DocumentState.code.Equals("03"),
					canAnnul = t.Document.DocumentState.code.Equals("01"),
					canReverse = t.Document.DocumentState.code.Equals("03") || t.Document.DocumentState.code.Equals("06")

				}).OrderByDescending(ob => ob.numberDocument).ToList();

				Session["consulta"] = consulta;

				return query;
			}
		}

		[HttpPost]
		public ActionResult SearchResult(LiquidationMaterialConsultDTO consulta)
		{
			var result = GetLiquidationMaterialDTO(consulta);

			SetLiquidationMaterialResultConsultDTO(result);

			return PartialView("ConsultResult", result);
		}

		[ValidateInput(false)]
		public ActionResult GridViewLiquidationMaterial(LiquidationMaterialConsultDTO consulta)
		{
			return PartialView("_GridViewLiquidationMaterial", GetLiquidationMaterialDTO(consulta));
		}

		private List<ResultReceptionDispatchMaterial> GetReceptionMaterialDTO()
		{
			using (var db = new DBContext())
			{
				var IdsRemissionGuideTransportation = db.RemissionGuideTransportation
										.Where(w => w.isOwn == false)
										.Select(s => s.id_remionGuide).ToList();

                var anioMin = DateTime.Now.AddYears(-4).Year;

				var preRemmisionDocument  = db.Document
												.Where(d => d.emissionDate.Year > anioMin && d.DocumentType.code == "08")
												.Select(r => new 
												{
												  idDocument = r.id,
												  secuencial = r.sequential.ToString()
                                                })
												.ToList();


				var IdsDocumentRemissionGuide = preRemmisionDocument
														.Where(r => IdsRemissionGuideTransportation.Contains(r.idDocument))
														.Select(s => s.secuencial)
														.ToArray();

                //var IdsDocumentRemissionGuide = db.Document
                //						.Where(d => IdsRemissionGuideTransportation.Contains(d.id) && d.emissionDate.Year > anioMin)
                //						.Select(s => s.sequential.ToString())
                //						.ToList();
                //
                var query = db.ResultReceptionDispatchMaterial.Where(d => d.idLiquidationMaterialSupplies == null
							&& IdsDocumentRemissionGuide.Contains(d.numberRemissionGuide)
							&& d.codeStateReceptionDispatchMaterial.Equals("03")).OrderByDescending(e => e.dateRemissionGuide).ToList();

				return query;
			}
		}

		[ValidateInput(false)]
		public ActionResult ReceptionMaterialApproved()
		{
			return PartialView("ReceptionMaterialApproved", GetReceptionMaterialDTO());
		}

		[ValidateInput(false)]
		public ActionResult GridViewReceptionMaterialApproved()
		{
			return PartialView("_GridViewReceptionMaterialApproved", GetReceptionMaterialDTO());
		}

		[HttpPost]
		public ActionResult Edit(int[] ids, int id, bool enabled = true)
		{
			var liquidationMaterialDTO = GetLiquidationMaterialDTO(ids, id);
			BuilViewBag(enabled);
			return PartialView(liquidationMaterialDTO);
		}

		private LiquidationMaterialDTO GetLiquidationMaterialDTO(int[] ids, int id)
		{
			LiquidationMaterialSupplies liquidationMaterial = db.LiquidationMaterialSupplies.FirstOrDefault(o => o.id == id);
			LiquidationMaterialDTO liquidationMaterialDTO = null;

			if (liquidationMaterial == null)
			{
				liquidationMaterialDTO = new LiquidationMaterialDTO();
				liquidationMaterialDTO.id = 0;

				liquidationMaterialDTO.emissionDateDocument = DateTime.Now;
				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
				liquidationMaterialDTO.documentState = documentState.name;
				liquidationMaterialDTO.id_documentState = documentState.id;
				liquidationMaterialDTO.subTotal = 0M;
				liquidationMaterialDTO.iva = 0M;
				liquidationMaterialDTO.total = 0M;
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("95"));//"95- Liquidación Material"
				liquidationMaterialDTO.numberDocument = GetDocumentNumber(documentType?.id ?? 0);
				liquidationMaterialDTO.documentType = documentType.name;
				liquidationMaterialDTO.id_documentType = documentType.id;
				liquidationMaterialDTO.LiquidationMaterialDetailDTO = new List<LiquidationMaterialDetailDTO>();
				liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = new List<SummaryLiquidationMaterialDetailDTO>();

				var id_providerAux = 0;

				if (ids != null)
				{
					foreach (var id_receptionMaterial in ids)
					{
						ResultReceptionDispatchMaterial receptionMaterial = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idReceptionDispatchMaterial == id_receptionMaterial);

						if (id_providerAux == 0)
						{
							id_providerAux = receptionMaterial.idProvider;
							liquidationMaterialDTO.id_provider = id_providerAux;
							liquidationMaterialDTO.provider = receptionMaterial.nameProvider;
						}

						foreach (var receptionMaterialDetail in receptionMaterial.ResultReceptionDispatchMaterialDetail)
						{
							decimal price = ItemDetailPrice(receptionMaterialDetail.idItem);
							decimal subTotal = Math.Round((receptionMaterialDetail.quantity * price), 4);
							decimal subTotaliva = PriceDetailIVA(receptionMaterialDetail.idItem, receptionMaterialDetail.quantity, price);
							decimal total = subTotal + subTotaliva;
							Item item = db.Item.FirstOrDefault(i => i.id == receptionMaterialDetail.idItem);

							var ivaAux = item?.ItemTaxation.FirstOrDefault()?.percentage ?? 0.00M;

							LiquidationMaterialDetailDTO newLiquidationMaterialDetailDTO = new LiquidationMaterialDetailDTO
							{
								id = liquidationMaterialDTO.LiquidationMaterialDetailDTO.Any() ? (liquidationMaterialDTO.LiquidationMaterialDetailDTO.Max(m => m.id) + 1) : 1,
								id_guia = id_receptionMaterial,
								numberGuia = receptionMaterial.numberRemissionGuide,
								emisionGuia = receptionMaterial.dateRemissionGuide,
								id_guiaDetail = receptionMaterialDetail.id,
								id_item = receptionMaterialDetail.idItem,
								codigo = receptionMaterialDetail.Item.masterCode,
								name = receptionMaterialDetail.Item.name,
								id_metricUnit = receptionMaterialDetail.idMetricUnit,
								metricUnit = receptionMaterialDetail.MetricUnit.code,
								quantity = receptionMaterialDetail.quantity,
								quantityOrigin = receptionMaterialDetail.quantity,
								unitCostOrigin = price,
								unitCost = price,
								subTotalOrigin = subTotal,
								subTotal = subTotal,
								iva = ivaAux,
								subTotalIvaOrigin = subTotaliva,
								subTotalIva = subTotaliva,
								totalOrigin = total,
								total = total,
								aprovedLogist = true,
								aprovedComertial = true,
								descriptionLogist = "",
								descriptionComertial = "",
								personProcessPlant = receptionMaterial.personProcessPlant
							};
							liquidationMaterialDTO.LiquidationMaterialDetailDTO.Add(newLiquidationMaterialDetailDTO);
							liquidationMaterialDTO.subTotal += newLiquidationMaterialDetailDTO.subTotal;
							liquidationMaterialDTO.iva += newLiquidationMaterialDetailDTO.iva;
							liquidationMaterialDTO.total += newLiquidationMaterialDetailDTO.total;
						}
						liquidationMaterialDTO.LiquidationMaterialDetailDTO = liquidationMaterialDTO.LiquidationMaterialDetailDTO.OrderBy(ob => ob.codigo).ThenBy(tb => tb.numberGuia).ToList();
						liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTO.LiquidationMaterialDetailDTO);
					}
				}
			}
			else
			{
				liquidationMaterialDTO = ConvertToDto(liquidationMaterial);
			}

			SetLiquidationMaterialDTO(liquidationMaterialDTO);
			return liquidationMaterialDTO;
		}
		private List<SummaryLiquidationMaterialDetailDTO> GetSummary(List<LiquidationMaterialDetailDTO> liquidationMaterialDetailDTO)
		{
			var summaryLiquidationMaterialDetailDTO = liquidationMaterialDetailDTO.GroupBy(gb => new
			{
				gb.id_item,
				gb.codigo,
				gb.name,
				gb.id_metricUnit,
				gb.metricUnit,
				gb.unitCost
			}).Select(s => new SummaryLiquidationMaterialDetailDTO
			{
				id_item = s.Key.id_item,
				codigo = s.Key.codigo,
				name = s.Key.name,
				id_metricUnit = s.Key.id_metricUnit,
				metricUnit = s.Key.metricUnit,
				quantity = s.Sum(ss => ss.quantity),
				unitCost = s.Key.unitCost,
				subTotal = s.Sum(ss => ss.subTotal),
				subTotalIva = s.Sum(ss => ss.subTotalIva),
				total = s.Sum(ss => ss.total)
			}).ToList();

			return summaryLiquidationMaterialDetailDTO;
		}

		private LiquidationMaterialDTO ConvertToDto(LiquidationMaterialSupplies liquidationMaterialSupplies)
		{
			if (liquidationMaterialSupplies == null)
				return null;

			var liquidationMaterialDTO = new LiquidationMaterialDTO();
			liquidationMaterialDTO.id = liquidationMaterialSupplies.id;
			liquidationMaterialDTO.numberDocument = liquidationMaterialSupplies.Document.number;
			liquidationMaterialDTO.emissionDateDocument = liquidationMaterialSupplies.Document.emissionDate;
			liquidationMaterialDTO.documentDescription = liquidationMaterialSupplies.Document.description;
			liquidationMaterialDTO.documentState = liquidationMaterialSupplies.Document.DocumentState.name;
			liquidationMaterialDTO.id_documentState = liquidationMaterialSupplies.Document.id_documentState;
			liquidationMaterialDTO.subTotal = liquidationMaterialSupplies.subTotal;
			liquidationMaterialDTO.iva = liquidationMaterialSupplies.subTotalTax;
			liquidationMaterialDTO.total = liquidationMaterialSupplies.Total;
			liquidationMaterialDTO.documentType = liquidationMaterialSupplies.Document.DocumentType.name;
			liquidationMaterialDTO.id_documentType = liquidationMaterialSupplies.Document.id_documentType;
			liquidationMaterialDTO.LiquidationMaterialDetailDTO = new List<LiquidationMaterialDetailDTO>();
			liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = new List<SummaryLiquidationMaterialDetailDTO>();

			liquidationMaterialDTO.id_provider = liquidationMaterialSupplies.idProvider;
			liquidationMaterialDTO.provider = liquidationMaterialSupplies.Provider.Person.fullname_businessName;

			var canAproved = (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTO.id_documentState)
								   ?.code.Equals("01") ?? false) && (liquidationMaterialDTO.id != 0);


			foreach (var resultReceptionDispatchMaterial in liquidationMaterialSupplies.ResultReceptionDispatchMaterial)
			{
				foreach (var receptionMaterialDetail in resultReceptionDispatchMaterial.ResultReceptionDispatchMaterialDetail)
				{
					LiquidationMaterialSuppliesDetail liquidationMaterialSuppliesDetail = liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.FirstOrDefault(o => o.id == receptionMaterialDetail.idLiquidationMaterialSuppliesDetail);

					decimal price = liquidationMaterialSuppliesDetail.priceUnit;
					decimal subTotal = liquidationMaterialSuppliesDetail.subTotal;
					decimal subTotaliva = liquidationMaterialSuppliesDetail.subTotalTax;
					decimal total = subTotal + subTotaliva;
					decimal quantityAux = liquidationMaterialSuppliesDetail.quantity ?? 0.00M;

					LiquidationMaterialDetailDTO newLiquidationMaterialDetailDTO = new LiquidationMaterialDetailDTO
					{
						id = liquidationMaterialSuppliesDetail.id,
						id_guia = resultReceptionDispatchMaterial.idReceptionDispatchMaterial,
						numberGuia = resultReceptionDispatchMaterial.numberRemissionGuide,
						emisionGuia = resultReceptionDispatchMaterial.dateRemissionGuide,
						id_guiaDetail = receptionMaterialDetail.id,
						id_item = receptionMaterialDetail.idItem,
						codigo = receptionMaterialDetail.Item.masterCode,
						name = receptionMaterialDetail.Item.name,
						id_metricUnit = receptionMaterialDetail.idMetricUnit,
						metricUnit = receptionMaterialDetail.MetricUnit.code,
						quantityOrigin = quantityAux,
						quantity = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? quantityAux : 0.00M) :
												(liquidationMaterialSuppliesDetail.aprovedComertial ? quantityAux : 0.00M),
						unitCostOrigin = price,
						unitCost = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? price : 0.00M) :
												(liquidationMaterialSuppliesDetail.aprovedComertial ? price : 0.00M),
						subTotalOrigin = subTotal,
						subTotal = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? subTotal : 0.00M) :
												(liquidationMaterialSuppliesDetail.aprovedComertial ? subTotal : 0.00M),
						iva = liquidationMaterialSuppliesDetail.tax,
						subTotalIvaOrigin = subTotaliva,
						subTotalIva = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? subTotaliva : 0.00M) :
												   (liquidationMaterialSuppliesDetail.aprovedComertial ? subTotaliva : 0.00M),
						totalOrigin = total,
						total = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? total : 0.00M) :
											 (liquidationMaterialSuppliesDetail.aprovedComertial ? total : 0.00M),
						descriptionLogist = liquidationMaterialSuppliesDetail.descriptionLogist,
						descriptionComertial = liquidationMaterialSuppliesDetail.descriptionComertial,
						aprovedLogist = liquidationMaterialSuppliesDetail.aprovedLogist,
						aprovedComertial = liquidationMaterialSuppliesDetail.aprovedComertial,
						personProcessPlant = resultReceptionDispatchMaterial.personProcessPlant
					};
					liquidationMaterialDTO.LiquidationMaterialDetailDTO.Add(newLiquidationMaterialDetailDTO);
				}

				liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTO.LiquidationMaterialDetailDTO);
			}

			// Get Inventory Move Information

			string _codeLXC = "", _codeLNC = "", _codeLXP = "", _codeLNP = "";


			int _idLXC = 0, _idLNC = 0, _idLXP = 0, _idLNP = 0;
			int _idDocTypeLXC = 0, _idDocTypeLNC = 0, _idDocTypeLXP = 0, _idDocTypeLNP = 0;

			List<int> lstDocTypesInvMove = new List<int>();
			#region Get Parameters

			var lstDetail = db
							.SettingDetail
							.Where(w => w.Setting.code.Equals("CMLQM")).ToList();

			var lstInventoryReasons = db.InventoryReason.Where(w => w.isActive).ToList();

			if (lstDetail != null & lstDetail.Count > 0)
			{
				_codeLXC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXC"))?.valueAux ?? "";
				_codeLNC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNC"))?.valueAux ?? "";
				_codeLXP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXP"))?.valueAux ?? "";
				_codeLNP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNP"))?.valueAux ?? "";

				_idLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id ?? 0;
				_idLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id ?? 0;
				_idLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id ?? 0;
				_idLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id ?? 0;

				_idDocTypeLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id_documentType ?? 0;
				_idDocTypeLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id_documentType ?? 0;
				_idDocTypeLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id_documentType ?? 0;
				_idDocTypeLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id_documentType ?? 0;

				if (_idLXC == 0 || _idLNC == 0 || _idLXP == 0 || _idLNP == 0)
				{
					throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
				}

				lstDocTypesInvMove.Add(_idDocTypeLXC);
				lstDocTypesInvMove.Add(_idDocTypeLNC);
				lstDocTypesInvMove.Add(_idDocTypeLXP);
				lstDocTypesInvMove.Add(_idDocTypeLNP);
			}
			else
			{
				throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
			}

			#endregion
			// Bring All Documents Generated From this Liquidation Material
			var lstDocSource = db
								.DocumentSource
								.Where(w => w.id_documentOrigin == liquidationMaterialSupplies.id);

			if (lstDocSource != null)
			{
				// Get Id Number
				var IdsLstInvMove = lstDocSource
									.Select(s => s.id_document);

				// Look Up all Document which are Generated From this Liquidation Material and come from Any Remission Guide
				var lstRgOrigin = db.DocumentSource
										.Where(w => IdsLstInvMove.Contains(w.id_document)
										&& w.Document1.DocumentType.code.Equals("08"));

				// Select Remission Guide
				var lstRemissionGuides = lstRgOrigin
										.Select(s => new
										{
											IdInventoryMove = s.id_document,
											IdRemissionGuide = s.id_documentOrigin,
											SequentialRemissionGuide = s.Document1.sequential.ToString()
										});

				// Look Up All Inventory Move Generated From Liquidation Material in Approved State
				var lstDocSourceInvMove = lstDocSource
										.Where(w => lstDocTypesInvMove.Contains(w.Document.id_documentType)
										&& IdsLstInvMove.Contains(w.id_document)
										&& w.Document.DocumentState.code.Equals("03"));

				// Get All Inventory Move Nulled
				var lstInvMove = db
									.InventoryMove
									.Where(w => !w.Document.DocumentState.code.Equals("05"))
									.Select(s => new
									{
										s.id,
										natureSequential = s.sequential.ToString(),
										natureMove = s.AdvanceParametersDetail.valueName,
										nameInventoryReason = s.InventoryReason.name,
									});

				var lstInvMoveLiquidation = (from a in lstInvMove
											 join b in lstDocSourceInvMove on a.id equals b.id_document
											 join c in lstRemissionGuides on b.id_document equals c.IdInventoryMove
											 select new
											 {
												 IdInventoryMove = a.id,
												 a.natureSequential,
												 a.natureMove,
												 a.nameInventoryReason,
												 c.SequentialRemissionGuide
											 }).ToList();

				if (lstInvMoveLiquidation != null && lstInvMoveLiquidation.Count > 0)
				{
					liquidationMaterialDTO.InventoryMoveDetailDTO = liquidationMaterialDTO.InventoryMoveDetailDTO ?? new List<InventoryMoveFromLiquidation>();
					liquidationMaterialDTO.InventoryMoveDetailDTO = lstInvMoveLiquidation
																		.Select(s => new InventoryMoveFromLiquidation
																		{
																			idDocumentIM = s.IdInventoryMove,
																			sequentialIM = s.natureSequential,
																			nameNatureMoveIM = s.natureMove,
																			nameInventoryReasonIM = s.nameInventoryReason,
																			nameRemissionGuide = s.SequentialRemissionGuide
																		}).ToList();
				}
			}

			return liquidationMaterialDTO;
		}

		private decimal ItemDetailPrice(int id_item)
		{
			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			if (item == null)
			{
				return 0.0M;
			}

			decimal price = item?.ItemSaleInformation?.salePrice ?? 0.000M;

			return price;
		}

		private decimal PriceDetailIVA(int id_item, decimal quantity, decimal price)
		{
			decimal iva = 0.0M;

			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			if (item == null)
			{
				return 0.0M;
			}

			var percentageAux = item?.ItemTaxation?.FirstOrDefault()?.percentage ?? 0.00M;

			iva = Math.Round(((quantity * price) * (percentageAux / 100)), 4);

			return iva;
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewSummary()
		{
			var model = GetLiquidationMaterialDTO().SummaryLiquidationMaterialDetailDTO;
			return PartialView("_GridViewSummary", model);
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewDetail(bool canAproved, bool canAuthorize, bool canReverse, bool? visibleCantidadCero = false)
		{
			var model = GetLiquidationMaterialDTO().LiquidationMaterialDetailDTO.Where(w => (!visibleCantidadCero.Value ? w.quantityOrigin != 0 : true)).OrderBy(ob => ob.codigo).ThenBy(tb => tb.numberGuia).ToList();
			ViewBag.canAproved = canAproved;
			ViewBag.canAuthorize = canAuthorize;
			ViewBag.canReverse = canReverse;
			ViewBag.visibleCantidadCero = visibleCantidadCero.Value;
			return PartialView("_GridViewDetail", model);
		}
		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewInventoryMove(bool canAproved, bool canAuthorize, bool canReverse)
		{
			var model = GetLiquidationMaterialDTO().InventoryMoveDetailDTO.OrderBy(o => o.idDocumentIM).ToList();
			ViewBag.canAproved = canAproved;
			ViewBag.canAuthorize = canAuthorize;
			ViewBag.canReverse = canReverse;
			return PartialView("_GridViewInventoryMove", model);
		}

		private void BuilViewBag(bool enabled)
		{
			ViewBag.enabled = enabled;
			var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
			ViewBag.canEdit = !enabled &&
							  (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								   ?.code.Equals("01") ?? false);
			ViewBag.canCreate = (liquidationMaterialDTOAux.id != 0);
			ViewBag.canSave = enabled && (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAproved = enabled &&
							  (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAuthorize = (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								  ?.code.Equals("03") ?? false);
			ViewBag.canAnnul = (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								  ?.code.Equals("01") ?? false) && (liquidationMaterialDTOAux.id != 0);
			ViewBag.canReverse = (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								  ?.code.Equals("03") ?? false) ||
								  (db.DocumentState.FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
								  ?.code.Equals("06") ?? false);
			ViewBag.canPrint = (liquidationMaterialDTOAux.id != 0);
			ViewBag.visibleCantidadCero = false;
		}

		[HttpPost]
		public JsonResult Save(LiquidationMaterialDTO liquidationMaterialDTO, bool? approved = false)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
						liquidationMaterialDTO.LiquidationMaterialDetailDTO = liquidationMaterialDTOAux.LiquidationMaterialDetailDTO;
						var newObject = false;
						var id = liquidationMaterialDTO.id;

						var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("95"));
						var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

						var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(d => d.id == id);

						if (liquidationMaterialSupplies == null)
						{
							newObject = true;

							var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
								? ActiveUser.EmissionPoint.First().id
								: 0;
							if (id_emissionPoint == 0)
								throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

							liquidationMaterialSupplies = new LiquidationMaterialSupplies
							{
								Document = new Document
								{
									number = GetDocumentNumber(documentType?.id ?? 0),
									sequential = GetDocumentSequential(documentType?.id ?? 0),
									authorizationDate = DateTime.Now,
									id_emissionPoint = id_emissionPoint,
									id_documentType = documentType.id,
									id_userCreate = ActiveUser.id,
									dateCreate = DateTime.Now,
									id_documentState = documentState.id,
									reference = ""
								},
								LiquidationMaterialSuppliesDetail = new List<LiquidationMaterialSuppliesDetail>()
							};

							documentType.currentNumber++;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;

						}

						liquidationMaterialSupplies.Document.emissionDate = liquidationMaterialDTO.emissionDateDocument;
						liquidationMaterialSupplies.Document.id_userUpdate = ActiveUser.id;
						liquidationMaterialSupplies.Document.dateUpdate = DateTime.Now;
						liquidationMaterialSupplies.Document.description = liquidationMaterialDTO.documentDescription;

						liquidationMaterialSupplies.idProvider = liquidationMaterialDTOAux.id_provider;
						liquidationMaterialSupplies.subTotal = 0.00M;
						liquidationMaterialSupplies.subTotalTax = 0.00M;
						liquidationMaterialSupplies.Total = 0.00M;

						if (newObject)
						{
							db.LiquidationMaterialSupplies.Add(liquidationMaterialSupplies);
							db.Entry(liquidationMaterialSupplies).State = EntityState.Added;
						}
						else
						{
							db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
							db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

						}

						db.SaveChanges();

						if (approved.Value)
						{
							var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
							if (aprovedState == null)
								throw new Exception("No existe el estado de aprobación en el sistema configurelo e intentelo de nuevo.");
							liquidationMaterialSupplies.Document.id_documentState = aprovedState.id;
							db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
							db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
						}

						//Details
						foreach (var detail in liquidationMaterialDTO.LiquidationMaterialDetailDTO.ToList())
						{
                            var aResultReceptionDispatchMaterial = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idLiquidationMaterialSupplies != null && 
                                                                                                                          d.idLiquidationMaterialSupplies != liquidationMaterialSupplies.id &&
                                                                                                                          d.idReceptionDispatchMaterial == detail.id_guia &&
                                                                                                                          d.LiquidationMaterialSupplies.Document.DocumentState.code != "05");
                            if (aResultReceptionDispatchMaterial != null)
                            {
                                throw new Exception("La Guía: " + detail.numberGuia + " ya se encuentra en otra liquidación de Materiales.");
                            }

                            var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == detail.id);
							if (detailAux != null && !newObject)
							{
								detailAux.aprovedLogist = detail.aprovedLogist;
								detailAux.aprovedComertial = detail.aprovedLogist;
								detailAux.descriptionLogist = detail.descriptionLogist ?? "";
								detailAux.descriptionComertial = "";

								db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
								db.Entry(detailAux).State = EntityState.Modified;
							}
							else
							{
								detailAux = new LiquidationMaterialSuppliesDetail
								{
									aprovedLogist = detail.aprovedLogist,
									aprovedComertial = detail.aprovedLogist,
									idItem = detail.id_item,
									idMetricUnit = detail.id_metricUnit,
									quantity = detail.quantityOrigin,
									priceUnit = detail.unitCostOrigin,
									subTotal = detail.subTotalOrigin,
									tax = detail.iva,
									subTotalTax = detail.subTotalIvaOrigin,
									total = detail.totalOrigin,
									descriptionLogist = detail.descriptionLogist ?? "",
									descriptionComertial = ""
								};
								liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.Add(detailAux);

							}
							db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
							db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
							db.SaveChanges();

							if (newObject)
							{
								var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial
																			.FirstOrDefault(d => d.idReceptionDispatchMaterial == detail.id_guia);
								if (resultReceptionDispatchMaterialAux != null)
								{
									resultReceptionDispatchMaterialAux.idLiquidationMaterialSupplies = liquidationMaterialSupplies.id;
									db.ResultReceptionDispatchMaterial.Attach(resultReceptionDispatchMaterialAux);
									db.Entry(resultReceptionDispatchMaterialAux).State = EntityState.Modified;
								}

								var resultReceptionDispatchMaterialDetailAux = db.ResultReceptionDispatchMaterialDetail.FirstOrDefault(d => d.id == detail.id_guiaDetail);
								if (resultReceptionDispatchMaterialDetailAux != null)
								{
									resultReceptionDispatchMaterialDetailAux.idLiquidationMaterialSuppliesDetail = detailAux.id;
									db.ResultReceptionDispatchMaterialDetail.Attach(resultReceptionDispatchMaterialDetailAux);
									db.Entry(resultReceptionDispatchMaterialDetailAux).State = EntityState.Modified;
								}

							}

							liquidationMaterialSupplies.subTotal += detailAux.aprovedComertial ? detailAux.subTotal : 0.00M;
							liquidationMaterialSupplies.subTotalTax += detailAux.aprovedComertial ? detailAux.subTotalTax : 0.00M;
							liquidationMaterialSupplies.Total += detailAux.aprovedComertial ? detailAux.total : 0.00M;

							db.SaveChanges();
						}

						foreach (var LiquidationMaterialDetailDTOAux in liquidationMaterialDTOAux.LiquidationMaterialDetailDTO.Where(w => w.quantity == 0).ToList())
						{
							var detailDTOAux = liquidationMaterialDTO.LiquidationMaterialDetailDTO.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id);
							if (detailDTOAux == null)
							{
								if (newObject)
								{
									var detailAux = new LiquidationMaterialSuppliesDetail
									{
										aprovedLogist = LiquidationMaterialDetailDTOAux.aprovedLogist,
										aprovedComertial = LiquidationMaterialDetailDTOAux.aprovedLogist,
										idItem = LiquidationMaterialDetailDTOAux.id_item,
										idMetricUnit = LiquidationMaterialDetailDTOAux.id_metricUnit,
										quantity = LiquidationMaterialDetailDTOAux.quantityOrigin,
										priceUnit = LiquidationMaterialDetailDTOAux.unitCostOrigin,
										subTotal = LiquidationMaterialDetailDTOAux.subTotalOrigin,
										tax = LiquidationMaterialDetailDTOAux.iva,
										subTotalTax = LiquidationMaterialDetailDTOAux.subTotalIvaOrigin,
										total = LiquidationMaterialDetailDTOAux.totalOrigin,
										descriptionLogist = LiquidationMaterialDetailDTOAux.descriptionLogist,
										descriptionComertial = ""
									};
									liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.Add(detailAux);

									db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
									db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
									db.SaveChanges();

									var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idReceptionDispatchMaterial == LiquidationMaterialDetailDTOAux.id_guia);
									if (resultReceptionDispatchMaterialAux != null)
									{
										resultReceptionDispatchMaterialAux.idLiquidationMaterialSupplies = liquidationMaterialSupplies.id;
										db.ResultReceptionDispatchMaterial.Attach(resultReceptionDispatchMaterialAux);
										db.Entry(resultReceptionDispatchMaterialAux).State = EntityState.Modified;
									}

									var resultReceptionDispatchMaterialDetailAux = db.ResultReceptionDispatchMaterialDetail.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id_guiaDetail);
									if (resultReceptionDispatchMaterialDetailAux != null)
									{
										resultReceptionDispatchMaterialDetailAux.idLiquidationMaterialSuppliesDetail = detailAux.id;
										db.ResultReceptionDispatchMaterialDetail.Attach(resultReceptionDispatchMaterialDetailAux);
										db.Entry(resultReceptionDispatchMaterialDetailAux).State = EntityState.Modified;
									}

								}
								else
								{
									var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id);
									detailAux.aprovedLogist = LiquidationMaterialDetailDTOAux.aprovedLogist;
									detailAux.aprovedComertial = LiquidationMaterialDetailDTOAux.aprovedLogist;
									detailAux.descriptionLogist = LiquidationMaterialDetailDTOAux.descriptionLogist ?? "";
									detailAux.descriptionComertial = "";

									db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
									db.Entry(detailAux).State = EntityState.Modified;
								}

								db.SaveChanges();

							}

						}

						trans.Commit();

						result.Data = liquidationMaterialSupplies.id.ToString();

					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}
					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
		}

		[HttpPost]
		public JsonResult Approve(int id)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						result.Data = ApproveLiquidationMaterialSupplies(id).name;
					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}

					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}

		}

		private DocumentState ApproveLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
					if (liquidationMaterialSupplies != null)
					{
						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
						if (aprovedState == null)
							return liquidationMaterialSupplies.Document.DocumentState;
						liquidationMaterialSupplies.Document.id_documentState = aprovedState.id;

						//db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
						db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
					else
					{
						trans.Rollback();
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationMaterialSupplies.Document.DocumentState;
				}
			}
		}

		[HttpPost]
		public JsonResult Authorize(int id/*, List<LiquidationMaterialDetailDTO> listAuthorize = null*/)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						result.Data = AuthorizeLiquidationMaterialSupplies(id/*, listAuthorize*/).name;

						#region Generate Inventory Move

						var _param = db.AdvanceParametersDetail
										.FirstOrDefault(fod => fod.valueCode.Trim().Equals("MILMA")
										&& fod.AdvanceParameters.code.Trim().Equals("PRSPR"))?.valueVarchar ?? "";

						var liquidationMaterialSupplies = db.LiquidationMaterialSupplies
									.FirstOrDefault(p => p.id == id);

						if (_param.Trim().Equals("Y"))
						{
							GenerateInventoryMoveForAuthorization(liquidationMaterialSupplies);
						}
						#endregion
					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}

					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
		}

		private DocumentState AuthorizeLiquidationMaterialSupplies(int id_liquidationMaterialSupplies/*, List<LiquidationMaterialDetailDTO> listAuthorize = null*/)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationMaterialSupplies = db.LiquidationMaterialSupplies
														.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
					if (liquidationMaterialSupplies != null)
					{
						var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
						var listAuthorize = liquidationMaterialDTOAux.LiquidationMaterialDetailDTO;
						if (listAuthorize != null)
						{
							liquidationMaterialSupplies.subTotal = 0.00M;
							liquidationMaterialSupplies.subTotalTax = 0.00M;
							liquidationMaterialSupplies.Total = 0.00M;

							//Details
							foreach (var detail in listAuthorize)
							{
								var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == detail.id);
								if (detailAux != null)
								{
									detailAux.aprovedComertial = detail.aprovedComertial;
									detailAux.descriptionComertial = detail.descriptionComertial ?? "";

									db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
									db.Entry(detailAux).State = EntityState.Modified;
								}

								db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
								db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
								//db.SaveChanges();

								liquidationMaterialSupplies.subTotal += detailAux.aprovedComertial ? detailAux.subTotal : 0.00M;
								liquidationMaterialSupplies.subTotalTax += detailAux.aprovedComertial ? detailAux.subTotalTax : 0.00M;
								liquidationMaterialSupplies.Total += detailAux.aprovedComertial ? detailAux.total : 0.00M;

								db.SaveChanges();
							}

							//var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();

							foreach (var LiquidationMaterialDetailDTOAux in liquidationMaterialDTOAux.LiquidationMaterialDetailDTO.Where(w => w.quantity == 0))
							{
								var detailDTOAux = listAuthorize.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id);
								if (detailDTOAux == null)
								{
									var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id);

									detailAux.aprovedComertial = LiquidationMaterialDetailDTOAux.aprovedComertial;
									detailAux.descriptionComertial = LiquidationMaterialDetailDTOAux.descriptionComertial ?? "";

									//db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
									db.Entry(detailAux).State = EntityState.Modified;

									db.SaveChanges();
								}
							}
						}
						var authorizeState = db.DocumentState.FirstOrDefault(d => d.code.Equals("06"));
						if (authorizeState == null)
							return

						liquidationMaterialSupplies.Document.DocumentState;
						liquidationMaterialSupplies.Document.id_documentState = authorizeState.id;
						liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

						//db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
						db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
					else
					{
						trans.Rollback();
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationMaterialSupplies.Document.DocumentState;
				}
			}
		}

		private bool GenerateInventoryMoveForAuthorization(LiquidationMaterialSupplies liqMaterialSupplies)
		{
			bool _answer = false;

			string _codeLXC = "", _codeLNC = "", _codeLXP = "", _codeLNP = "";


			int _idLXC = 0, _idLNC = 0, _idLXP = 0, _idLNP = 0;
			int _idDocTypeLXC = 0, _idDocTypeLNC = 0, _idDocTypeLXP = 0, _idDocTypeLNP = 0;

            #region Get Parameters

            SettingDetail[] settingDetail = Array.Empty<SettingDetail>();
            Setting[] settings = Array.Empty<Setting>();
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            EmissionPoint[] emissionPoints = Array.Empty<EmissionPoint>();
            WarehouseLocation[] warehouseLocations = Array.Empty<WarehouseLocation>();

            settings = db.Setting.ToArray();
            settingDetail = db.SettingDetail.ToArray();
            documentStates = db.DocumentState.ToArray();
            emissionPoints = db.EmissionPoint.ToArray();
            warehouseLocations = db.WarehouseLocation.ToArray();

            var lstDetail = settingDetail
							.Where(w => w.Setting.code.Equals("CMLQM")).ToList();

			var lstInventoryReasons = db.InventoryReason.Where(w => w.isActive).ToList();

			if (lstDetail != null & lstDetail.Count > 0)
			{
				_codeLXC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXC"))?.valueAux ?? "";
				_codeLNC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNC"))?.valueAux ?? "";
				_codeLXP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXP"))?.valueAux ?? "";
				_codeLNP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNP"))?.valueAux ?? "";

				_idLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id ?? 0;
				_idLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id ?? 0;
				_idLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id ?? 0;
				_idLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id ?? 0;

				_idDocTypeLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id_documentType ?? 0;
				_idDocTypeLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id_documentType ?? 0;
				_idDocTypeLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id_documentType ?? 0;
				_idDocTypeLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id_documentType ?? 0;

				if (_idLXC == 0 || _idLNC == 0 || _idLXP == 0 || _idLNP == 0)
				{
					throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
				}
			}
			else
			{
				throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
			}

			#endregion

			#region Get Information

			// Get All Remission Guide from Detail

			var lstLiquidationDetails = liqMaterialSupplies
											.LiquidationMaterialSuppliesDetail
											.Select(s => s.id)
											.ToList();

			var lstReceptionLiquidationDetail = db
												.ResultReceptionDispatchMaterialDetail
												.Where(w => w.idLiquidationMaterialSuppliesDetail != null)
												.ToList();


			var lstLiquidationReception = (from a in lstLiquidationDetails
										   join b in lstReceptionLiquidationDetail on a equals b.idLiquidationMaterialSuppliesDetail
										   select new
										   {
											   idLiquidationMatDetail = a,
											   idReceptionMatDetail = b.id,
											   idReceptionMatHeader = b.idResultReceptionDispatchMaterial
										   });
			var lstReceptions = db.ReceptionDispatchMaterials
									.Where(w => !(
									w.Document.DocumentState.code.Equals("05")));


			var lstLiquidationDetailRG = (from a in lstLiquidationReception
										  join b in lstReceptions on a.idReceptionMatHeader equals b.id
										  select new
										  {
											  a.idLiquidationMatDetail,
											  a.idReceptionMatDetail,
											  a.idReceptionMatHeader,
											  idRemissionGuide = b.id_remissionGuide
										  }).ToList();

			// Get Warehouse Provider

			WarehouseLocation warehouseLocationAux = null;
			warehouseLocationAux = ServiceInventoryMove.GetWarehouseLocationProvider(liqMaterialSupplies.idProvider, db, ActiveCompany, ActiveUser);
			Provider providerAux = db.Provider.FirstOrDefault(fod => fod.id == liqMaterialSupplies.idProvider);

			int id_WarehouseProvider = 0;

			if (warehouseLocationAux == null)
			{
				throw new Exception("No puede Aprobarse la Guía debido a no existir la bodega Virtual de Proveedores con codigo(VIRPRO) necesaria para crear la ubicación del proveedor: " +
									providerAux.Person.fullname_businessName + " y realizar la reversión. El administrador del sistema debe configurar la opción y después intentelo de nuevo");
			}
			else
			{
				id_WarehouseProvider = warehouseLocationAux.Warehouse.id;
			}

			#endregion

			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					// Look for Each Remission Guide
					var lstRgs = lstLiquidationDetailRG.Select(s => s.idRemissionGuide).Distinct().ToList();

					if (lstRgs != null && lstRgs.Count > 0)
					{
						foreach (var det in lstRgs)
						{
							var lstIdsLiquidationMaterialDetail = lstLiquidationDetailRG
																	.Where(w => w.idRemissionGuide == det)
																	.Select(s => s.idLiquidationMatDetail);

							var lstLiquidationMatSupDetail = liqMaterialSupplies
																.LiquidationMaterialSuppliesDetail
																.Where(w => lstIdsLiquidationMaterialDetail.Contains(w.id))
																.ToList();

							// Agroup By Approved Comertial

							var lstLiquidationMatSupDetailApprovedComertial = lstLiquidationMatSupDetail
																				.Where(w => w.aprovedComertial == true).ToList();

							// Aprobado por Comercial

							if (lstLiquidationMatSupDetailApprovedComertial != null && lstLiquidationMatSupDetailApprovedComertial.Count > 0)
							{
								var lstLiquidationMatSupDetailApprovedComertialQuantityMayorZero = lstLiquidationMatSupDetailApprovedComertial
																									.Where(w => w.quantity > 0).ToList();

								// Cantidad MAYOR que 0 EGRESO MOTIVO LXC
								if (lstLiquidationMatSupDetailApprovedComertialQuantityMayorZero != null
									&& lstLiquidationMatSupDetailApprovedComertialQuantityMayorZero.Count > 0)
								{
									// Execute Service Inventory Move
									ServiceInventoryMove.UpdateInventoryMoveLiquidationMaterialExit(ActiveUser
													, ActiveCompany, ActiveEmissionPoint, liqMaterialSupplies
													, db, false, liqMaterialSupplies.Document.emissionDate//DateTime.Now
													, settings, settingDetail, documentStates, emissionPoints
													, warehouseLocations
													, null, null, lstLiquidationMatSupDetailApprovedComertialQuantityMayorZero
													, det, id_WarehouseProvider, warehouseLocationAux.id, _idDocTypeLXC, _idLXC);
								}

								var lstLiquidationMatSupDetailApprovedComertialQuantityLessZero = lstLiquidationMatSupDetailApprovedComertial
																			.Where(w => w.quantity < 0).ToList();

								// Cantidad MENOR que 0 INGRESO MOTIVO LXP

								if (lstLiquidationMatSupDetailApprovedComertialQuantityLessZero != null
									&& lstLiquidationMatSupDetailApprovedComertialQuantityLessZero.Count > 0)
								{
									// Execute Service Inventory Move
									ServiceInventoryMove.UpdateInventoryMoveLiquidationMaterialEntry(ActiveUser
													, ActiveCompany, ActiveEmissionPoint, liqMaterialSupplies
													, db, false, liqMaterialSupplies.Document.emissionDate//DateTime.Now
													, settings, settingDetail, documentStates, emissionPoints
													, warehouseLocations
													, null, null, lstLiquidationMatSupDetailApprovedComertialQuantityLessZero
													, det, id_WarehouseProvider, warehouseLocationAux.id, _idDocTypeLXP, _idLXP);
								}
							}

							var lstLiquidationMatSupDetailNotApprovedComertial = lstLiquidationMatSupDetail
																				.Where(w => w.aprovedComertial == false).ToList();

							// NO Aprobado Comercial

							if (lstLiquidationMatSupDetailNotApprovedComertial != null && lstLiquidationMatSupDetailNotApprovedComertial.Count > 0)
							{
								var lstLiquidationMatSupDetailNotApprovedComertialQuantityMayorZero = lstLiquidationMatSupDetailNotApprovedComertial
																			.Where(w => w.quantity > 0).ToList();

								// Cantidad MAYOR que 0 EGRESO MOTIVO LNC
								if (lstLiquidationMatSupDetailNotApprovedComertialQuantityMayorZero != null
									&& lstLiquidationMatSupDetailNotApprovedComertialQuantityMayorZero.Count > 0)
								{
									// Execute Service Inventory Move
									ServiceInventoryMove.UpdateInventoryMoveLiquidationMaterialExit(ActiveUser
													, ActiveCompany, ActiveEmissionPoint, liqMaterialSupplies
													, db, false, liqMaterialSupplies.Document.emissionDate//DateTime.Now
													, settings, settingDetail, documentStates, emissionPoints
													, warehouseLocations
													, null, null, lstLiquidationMatSupDetailNotApprovedComertialQuantityMayorZero
													, det, id_WarehouseProvider, warehouseLocationAux.id, _idDocTypeLNC, _idLNC);
								}

								var lstLiquidationMatSupDetailNotApprovedComertialQuantityLessZero = lstLiquidationMatSupDetailNotApprovedComertial
																			.Where(w => w.quantity < 0).ToList();

								// Cantidad MENOR que 0 INGRESO MOTIVO LNP
								if (lstLiquidationMatSupDetailNotApprovedComertialQuantityLessZero != null
									&& lstLiquidationMatSupDetailNotApprovedComertialQuantityLessZero.Count > 0)
								{
									// Execute Service Inventory Move
									ServiceInventoryMove.UpdateInventoryMoveLiquidationMaterialEntry(ActiveUser
													, ActiveCompany, ActiveEmissionPoint, liqMaterialSupplies
													, db, false, liqMaterialSupplies.Document.emissionDate//DateTime.Now
													, settings, settingDetail, documentStates, emissionPoints
													, warehouseLocations
													, null, null, lstLiquidationMatSupDetailNotApprovedComertialQuantityLessZero
													, det, id_WarehouseProvider, warehouseLocationAux.id, _idDocTypeLNP, _idLNP);
								}
							}

						}
					}
					db.SaveChanges();
					trans.Commit();
				}
			}
			return _answer;
		}

		private bool ReverseInventoryMoveForAuthorization(LiquidationMaterialSupplies liqMaterialSupplies)
		{
			bool _answer = false;

			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					string _codeLXC = "", _codeLNC = "", _codeLXP = "", _codeLNP = "";


					int _idLXC = 0, _idLNC = 0, _idLXP = 0, _idLNP = 0;
					int _idDocTypeLXC = 0, _idDocTypeLNC = 0, _idDocTypeLXP = 0, _idDocTypeLNP = 0;

					List<int> lstDocTypesInvMove = new List<int>();
					#region Get Parameters

					var lstDetail = db
									.SettingDetail
									.Where(w => w.Setting.code.Equals("CMLQM")).ToList();

					var lstInventoryReasons = db.InventoryReason.Where(w => w.isActive).ToList();

					if (lstDetail != null & lstDetail.Count > 0)
					{
						_codeLXC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXC"))?.valueAux ?? "";
						_codeLNC = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNC"))?.valueAux ?? "";
						_codeLXP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LXP"))?.valueAux ?? "";
						_codeLNP = lstDetail.FirstOrDefault(fod => fod.value.Equals("LNP"))?.valueAux ?? "";

						_idLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id ?? 0;
						_idLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id ?? 0;
						_idLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id ?? 0;
						_idLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id ?? 0;

						_idDocTypeLXC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXC))?.id_documentType ?? 0;
						_idDocTypeLNC = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNC))?.id_documentType ?? 0;
						_idDocTypeLXP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLXP))?.id_documentType ?? 0;
						_idDocTypeLNP = lstInventoryReasons.FirstOrDefault(fod => fod.code.Equals(_codeLNP))?.id_documentType ?? 0;

						if (_idLXC == 0 || _idLNC == 0 || _idLXP == 0 || _idLNP == 0)
						{
							throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
						}

						lstDocTypesInvMove.Add(_idDocTypeLXC);
						lstDocTypesInvMove.Add(_idDocTypeLNC);
						lstDocTypesInvMove.Add(_idDocTypeLXP);
						lstDocTypesInvMove.Add(_idDocTypeLNP);
					}
					else
					{
						throw new Exception("No están configurados los motivos de inventario a utilizarse en la Autorización de Liquidación");
					}

					#endregion

					#region Process Reverse

					var lstDocSource = db.DocumentSource
											.Where(w => w.id_documentOrigin == liqMaterialSupplies.id).ToList();


					var lstDocSourceInvMove = lstDocSource.Where(w => lstDocTypesInvMove.Contains(w.Document.id_documentType)
												&& w.Document.DocumentState.code.Equals("03")).ToList();

					int idDocState = db.DocumentState.FirstOrDefault(fod => fod.code.Equals("05"))?.id ?? 0;

					if (idDocState == 0)
					{
						throw new Exception("No existe el estado Anular General");
					}

					DateTime dt = DateTime.Now;

					if (lstDocSourceInvMove != null && lstDocSourceInvMove.Count > 0)
					{
						foreach (var det in lstDocSourceInvMove)
						{
							Document _doc = db.Document.FirstOrDefault(fod => fod.id == det.id_document);
							InventoryMove _im = db.InventoryMove.FirstOrDefault(fod => fod.id == det.id_document);

							ServiceInventoryMove.ValidateEmissionDateInventoryMove(db, dt, false, _im.idWarehouse);

							_doc.id_documentState = idDocState;

							db.Document.Attach(_doc);
							db.Entry(_doc).State = EntityState.Modified;
						}
					}
					#endregion

					db.SaveChanges();
					trans.Commit();
				}
			}

			return _answer;
		}
		[HttpPost]
		public JsonResult Reverse(int id)
		{
			string codeState = "";
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						var liqMatSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(fod => fod.id == id);

						if (liqMatSupplies != null)
						{
							codeState = liqMatSupplies.Document.DocumentState.code;
						}
						result.Data = ReverseLiquidationMaterialSupplies(id).name;

						#region Reverse Inventory Move

						var _param = db.AdvanceParametersDetail
										.FirstOrDefault(fod => fod.valueCode.Trim().Equals("MILMA")
										&& fod.AdvanceParameters.code.Trim().Equals("PRSPR"))?.valueVarchar ?? "";

						var liquidationMaterialSupplies = db.LiquidationMaterialSupplies
									.FirstOrDefault(p => p.id == id);

						if (_param.Trim().Equals("Y") && codeState.Equals("06"))
						{
							ReverseInventoryMoveForAuthorization(liquidationMaterialSupplies);
						}
						#endregion
					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}

					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
		}

		private DocumentState ReverseLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
					if (liquidationMaterialSupplies != null)
					{
						DocumentState reverseState = null;
						if (liquidationMaterialSupplies.Document.DocumentState.code.Equals("03"))
						{
							reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
						}
						else
						{
							if (liquidationMaterialSupplies.Document.DocumentState.code.Equals("06"))
							{
								reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
							}
						}
						if (reverseState == null)
							return

						liquidationMaterialSupplies.Document.DocumentState;
						liquidationMaterialSupplies.Document.id_documentState = reverseState.id;
						liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

						db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
						db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationMaterialSupplies.Document.DocumentState;
				}
			}
		}

		[HttpPost]
		public JsonResult Annul(int id)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						result.Data = AnnulLiquidationMaterialSupplies(id).name;
					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}

					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
		}

		private DocumentState AnnulLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
					if (liquidationMaterialSupplies != null)
					{
						var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
						if (annulState == null)
							return

						liquidationMaterialSupplies.Document.DocumentState;
						liquidationMaterialSupplies.Document.id_documentState = annulState.id;
						liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

						db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
						db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;


						var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial.Where(d => d.idLiquidationMaterialSupplies == liquidationMaterialSupplies.id);
						if (resultReceptionDispatchMaterialAux != null && resultReceptionDispatchMaterialAux.Any())
						{
							foreach (var item in resultReceptionDispatchMaterialAux)
							{
								item.idLiquidationMaterialSupplies = null;
								db.ResultReceptionDispatchMaterial.Attach(item);
								db.Entry(item).State = EntityState.Modified;
								foreach (var detail in item.ResultReceptionDispatchMaterialDetail)
								{
									detail.idLiquidationMaterialSuppliesDetail = null;
									db.ResultReceptionDispatchMaterialDetail.Attach(detail);
									db.Entry(detail).State = EntityState.Modified;

								}
							}

						}

						db.SaveChanges();
						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationMaterialSupplies.Document.DocumentState;
				}
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			var index = GetLiquidationMaterialResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

			var result = new
			{
				maximunPages = GetLiquidationMaterialResultConsultDTO().Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			var element = GetLiquidationMaterialResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
			var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(d => d.id == element.id);
			if (liquidationMaterialSupplies == null)
				return PartialView("Edit", new LiquidationMaterialDTO());

			var model = ConvertToDto(liquidationMaterialSupplies);
			SetLiquidationMaterialDTO(model);
			BuilViewBag(false);
			return PartialView("Edit", model);
		}

		[HttpPost]
		public JsonResult UpdateAndRefreshGridSummary(int id, bool aproved, bool canAproved, string description)
		{
			var result = new ApiResult();

			try
			{
				var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
				var detailAux = liquidationMaterialDTOAux.LiquidationMaterialDetailDTO.FirstOrDefault(d => d.id == id);
				if (detailAux != null)
				{
					detailAux.quantity = aproved ? detailAux.quantityOrigin : 0.00M;
					detailAux.unitCost = aproved ? detailAux.unitCostOrigin : 0.00M;
					detailAux.subTotal = aproved ? detailAux.subTotalOrigin : 0.00M;
					detailAux.subTotalIva = aproved ? detailAux.subTotalIvaOrigin : 0.00M;
					detailAux.total = aproved ? detailAux.totalOrigin : 0.00M;
					detailAux.aprovedLogist = canAproved ? aproved : detailAux.aprovedLogist;
					detailAux.aprovedComertial = !canAproved ? aproved : detailAux.aprovedComertial;

					if (canAproved) detailAux.descriptionLogist = description;
					else detailAux.descriptionComertial = description;

					liquidationMaterialDTOAux.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTOAux.LiquidationMaterialDetailDTO);

					SetLiquidationMaterialDTO(liquidationMaterialDTOAux);
				}
			}
			catch (Exception e)
			{
				result.Code = e.HResult;
				result.Message = e.Message;
			}

			return Json(result, JsonRequestBehavior.AllowGet);
		}

        [HttpPost, ValidateInput(false)]
        public JsonResult ExistData()
        {
            var result = new
            {
                exist = (Session["LiquidationMaterialDTO"] is LiquidationMaterialDTO liquidationMaterialDTO)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CleanSessionData()
        {
            Session["LiquidationMaterialDTO"] = null;
            var result = new
            {
                exist = (Session["LiquidationMaterialDTO"] is LiquidationMaterialDTO liquidationMaterialDTO)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
		public JsonResult PrintInventoryMoveGenerated(int idInvMov)
		{

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = idInvMov;

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "IDGV1";
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}

		[HttpPost]
		public JsonResult PrintLiquidationMaterialReport(int id_liquidation, bool detallado = false)
		{
			var reportProdModel = new ReportProdModel
			{
				codeReport = "LMIPG",
				conex = GetObjectConnection("DBContextNE"),
				paramCRList = new List<ParamCR>()
				{
					new ParamCR()
					{
						Nombre = "@id",
						Valor = id_liquidation,
					},
					new ParamCR()
					{
						Nombre = "@detallado",
						Valor = detallado,
					},
				},
			};

			var reportParanNameModel = GetTmpDataName(20);
			reportParanNameModel.codeReport = reportProdModel.codeReport;

			TempData[reportParanNameModel.nameQS] = reportProdModel;
			TempData.Keep(reportParanNameModel.nameQS);

			return Json(reportParanNameModel, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult PrintLiquidationMaterialGeneralReport(LiquidationMaterialConsultDTO consulta)
		{
			var reportProdModel = new ReportProdModel
			{
				codeReport = "LMGPG",
				conex = GetObjectConnection("DBContextNE"),
				paramCRList = new List<ParamCR>()
				{
					new ParamCR()
					{
						Nombre = "@dateInit",
						Valor = consulta.fechaInicioEmision,
					},
					new ParamCR()
					{
						Nombre = "@dateEnd",
						Valor = consulta.fechaFinEmision,
					},
					new ParamCR()
					{
						Nombre = "@detallado",
						Valor = consulta.detallado,
					},
				},
			};

			var reportParanNameModel = GetTmpDataName(20);
			reportParanNameModel.codeReport = reportProdModel.codeReport;

			TempData[reportParanNameModel.nameQS] = reportProdModel;
			TempData.Keep(reportParanNameModel.nameQS);

			return Json(reportParanNameModel, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult PrintInventoryMoveGeneratedAll(int id_liquidation)
		{
			var reportProdModel = new ReportProdModel
			{
				codeReport = "IDGV1T",
				conex = GetObjectConnection("DBContextNE"),
				paramCRList = new List<ParamCR>()
				{
					new ParamCR()
					{
						Nombre = "@id",
						Valor = id_liquidation,
					},
				},
			};

			var reportParanNameModel = GetTmpDataName(20);
			reportParanNameModel.codeReport = reportProdModel.codeReport;

			TempData[reportParanNameModel.nameQS] = reportProdModel;
			TempData.Keep(reportParanNameModel.nameQS);

			return Json(reportParanNameModel, JsonRequestBehavior.AllowGet);
		}
	}
}
