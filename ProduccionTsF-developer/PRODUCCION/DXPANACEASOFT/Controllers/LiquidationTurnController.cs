using DevExpress.Data.ODataLinq.Helpers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
	public class LiquidationTurnController : DefaultController
	{
		private const string m_TipoDocumentoLiquidationTurn = "132";

		private LiquidationTurnDTO GetLiquidationTurnDTO()
		{
			if (!(Session["LiquidationTurnDTO"] is LiquidationTurnDTO liquidationTurn))
				liquidationTurn = new LiquidationTurnDTO();
			return liquidationTurn;
		}

		private List<LiquidationTurnResultConsultDTO> GetLiquidationTurnResultConsultDTO()
		{
			if (!(Session["LiquidationTurnResultConsultDTO"] is List<LiquidationTurnResultConsultDTO> liquidationTurnResultConsult))
				liquidationTurnResultConsult = new List<LiquidationTurnResultConsultDTO>();
			return liquidationTurnResultConsult;
		}

		private void SetLiquidationTurnDTO(LiquidationTurnDTO liquidationTurnDTO)
		{
			Session["LiquidationTurnDTO"] = liquidationTurnDTO;
		}

		private void SetLiquidationTurnResultConsultDTO(List<LiquidationTurnResultConsultDTO> liquidationTurnResultConsult)
		{
			Session["LiquidationTurnResultConsultDTO"] = liquidationTurnResultConsult;
		}

		// GET: LiquidationTurn
		public ActionResult Index()
		{
			BuildViewDataIndex();
			return View();
		}

		[HttpPost]
		public ActionResult SearchResult(LiquidationTurnConsultDTO consult)
		{
			var result = GetListsConsultDto(consult);
			SetLiquidationTurnResultConsultDTO(result);
			return PartialView("ConsultResult", result);
		}

		[HttpPost]
		public ActionResult GridViewLiquidationTurn()
		{
			return PartialView("_GridViewIndex", GetLiquidationTurnResultConsultDTO());
		}

		private List<LiquidationTurnResultConsultDTO> GetListsConsultDto(LiquidationTurnConsultDTO consulta)
		{
			using (var db = new DBContext())
			{
				var consultaAux = Session["consulta"] as LiquidationTurnConsultDTO;
				if (consultaAux != null && consulta.initDate == null)
				{
					consulta = consultaAux;
				}


				var consultResult = db.LiquidationTurn.Where(LiquidationTurnQueryExtensions.GetRequestByFilter(consulta));

				//string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";

				//IEnumerable<int> idsProductionLot = null;
				//if (loteManualParm == "SI")
				//{
				//	if (!string.IsNullOrEmpty(consultar.numberLot))
				//	{
				//		idsProductionLot = db.LiquidationCartOnCartDetail
				//			.Where(o => o.LiquidationCartOnCart.Document.DocumentState.code != "05" &&
				//					(o.ProductionLot.number.Contains(consultar.numberLot) ||
				//						o.ProductionLot.internalNumber.Contains(consultar.numberLot)))
				//			.Select(e => e.id_LiquidationCartOnCart)
				//			.Distinct()
				//			.ToArray();
				//	}
				//}
				//else
				//{
				//	if (!string.IsNullOrEmpty(consultar.numberLot))
				//	{
				//		idsProductionLot = db.LiquidationCartOnCart
				//			.Where(o => o.Document.DocumentState.code != "05" &&
				//					(o.ProductionLot.number.Contains(consultar.numberLot) ||
				//						o.ProductionLot.internalNumber.Contains(consultar.numberLot)))
				//			.Select(e => e.id)
				//			.Distinct()
				//			.ToArray();
				//	}
				//}

				if (!String.IsNullOrEmpty(consulta.numberLot))
				{
					consultResult = consultResult.Where(w => (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == w.id_personProcessPlant &&
																															   fod.MachineProdOpening.id_Turn == w.id_turn &&
																															   DbFunctions.TruncateTime(w.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
																															   fod.Document.DocumentState.code != "05" &&
																															   fod.ProductionLot.internalNumber.Contains(consulta.numberLot)) != null));
				}
				if ((consulta.id_provider != null && consulta.id_provider != 0))
				{
					consultResult = consultResult.Where(w => (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == w.id_personProcessPlant &&
																															   fod.MachineProdOpening.id_Turn == w.id_turn &&
																															   DbFunctions.TruncateTime(w.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
																															   fod.Document.DocumentState.code != "05" &&
																															   fod.ProductionLot.id_provider == consulta.id_provider) != null));
				}
				if ((consulta.id_productionUnitProvider != null && consulta.id_productionUnitProvider != 0))
				{
					consultResult = consultResult.Where(w => (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == w.id_personProcessPlant &&
																															   fod.MachineProdOpening.id_Turn == w.id_turn &&
																															   DbFunctions.TruncateTime(w.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
																															   fod.Document.DocumentState.code != "05" &&
																															   fod.ProductionLot.id_productionUnitProvider == consulta.id_productionUnitProvider) != null));
				}
				var query = consultResult.Select(t => new LiquidationTurnResultConsultDTO
				{
					id = t.id,
					number = t.Document.number,
					emissionDate = t.Document.emissionDate,
					turn = t.Turn.name,
					processPlant = t.Person.processPlant,
					state = t.Document.DocumentState.name,


					canEdit = t.Document.DocumentState.code.Equals("01"),
					canAproved = t.Document.DocumentState.code.Equals("01"),
					canAnnul = t.Document.DocumentState.code.Equals("01"),
					canReverse = t.Document.DocumentState.code.Equals("03")

				}).OrderBy(ob => ob.number).ToList();

				Session["consulta"] = consulta;

				return query;
			}
		}

		private static List<LiquidationTurnPendingNewDTO> GetLiquidationTurnPendingNewDto()
		{
			using (var db = new DBContext())
			{
                //// Verificación del parámetro de LiquidationTurn
                //var liquidationTurnHabilitado = db.Setting.AsEnumerable()
                //	.Any(s => s.code == "ROMANEO" && s.value == "1");

                //if (!liquidationTurnHabilitado)
                //{
                //	return new List<LiquidationTurnPendingNewDTO>();
                //}

                // Ejecución de la consulta de pendientes
                try
                {
					var qa = db.MachineProdOpeningDetail.ToArray();
					var q = db.MachineProdOpeningDetail
					.Where(r => (r.MachineProdOpening.Document.DocumentState.code.Equals("03") ||
								 r.MachineProdOpening.Document.DocumentState.code.Equals("04"))//03: APROBADA o 04: CERRADA
								&& (r.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") != null) //05: ANULADA
								&& (db.LiquidationTurn.FirstOrDefault(fod => (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(fod.Document.emissionDate)) &&
																			 (r.MachineProdOpening.id_Turn == fod.id_turn) &&
																			 (r.MachineForProd.id_personProcessPlant == fod.id_personProcessPlant) &&
																			 (fod.Document.DocumentState.code != "05")) == null)) //05: ANULADA
					.GroupBy(g => new {
						emissionDate = DbFunctions.TruncateTime(g.MachineProdOpening.Document.emissionDate),
						id_turn = g.MachineProdOpening.id_Turn,
						nameTurn = g.MachineProdOpening.Turn.name,
						g.MachineForProd.id_personProcessPlant,
						g.MachineForProd.Person.processPlant
					})
					.AsEnumerable()
					.Select(r => new LiquidationTurnPendingNewDTO
					{
						emissionDate = r.Key.emissionDate.Value,
						emissionDateStr = r.Key.emissionDate.Value.ToString("dd-MM-yyyy"),
						id_turn = r.Key.id_turn,
						turn = r.Key.nameTurn,
						id_personProcessPlant = r.Key.id_personProcessPlant, 
						processPlant = r.Key.processPlant
					}).ToList();

					return q;
				}
                catch (Exception e)
                {
                    throw new Exception(e.InnerException.Message);
                }


			}
		}

		[HttpPost]
		public ActionResult PendingNew()
		{
			return View(GetLiquidationTurnPendingNewDto());
		}

		[ValidateInput(false)]
		public ActionResult GridViewPendingNew()
		{
			return PartialView("_GridViewPendingNew", GetLiquidationTurnPendingNewDto());
		}

		private LiquidationTurnDTO Create(DateTime emissionDate, int id_turn, int id_personProcessPlant)
		{
			using (var db = new DBContext())
			{
				//var machineProdOpening =
				//	db.MachineProdOpening.FirstOrDefault(r => r.id == id_machineProdOpening);
				//if (machineProdOpening == null)
				//	return new LiquidationTurnDTO();

				var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoLiquidationTurn));
				var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

				var hoy = DateTime.Now;

				var liquidationTurnDTO = new LiquidationTurnDTO
				{
					id_turn = id_turn,
					id_personProcessPlant = id_personProcessPlant,
					description = "",
					number = GetDocumentNumber(documentType?.id ?? 0),
					documentType = documentType?.name ?? "",
					id_documentType = documentType?.id ?? 0,
					reference = "",
					dateTimeEmision = emissionDate,
					liquidationDate = hoy,
					liquidationTime = hoy.TimeOfDay,
					idSate = documentState?.id ?? 0,
					state = documentState?.name ?? "",
					LiquidationTurnDetails = new List<LiquidationTurnDetailDTO>()
				};

				FillLiquidationTurnDetails(liquidationTurnDTO, emissionDate, id_turn, id_personProcessPlant);

				//foreach (var item in machineProdOpening.LiquidationCartOnCart.Where(w=> w.Document.DocumentState.code != "05").ToList())
				//{
				//    var pt = db.ProcessType.FirstOrDefault(fod => fod.id == item.idProccesType);

				//    liquidationTurnDTO.LiquidationTurnDetails.Add(new LiquidationTurnDetailDTO
				//    {
				//        turn = machineProdOpening.Turn.name,
				//        numberLot = item.ProductionLot.internalNumber,
				//        process = item.ProductionLot.Person1.processPlant,
				//        provider = item.ProductionLot.Provider.Person.fullname_businessName,
				//        numberLiquidationCarOnCar = item.Document.number,
				//        machineForProd = item.MachineForProd.name,
				//        id_state = item.Document.id_documentState,
				//        state = item.Document.DocumentState.name,
				//        tail = pt.code.Equals("COL") ? item.LiquidationCartOnCartDetail.Sum(s=> s.quantityPoundsITW) : 0.00M,
				//        whole = pt.code.Equals("COL") ? 0.00M : item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsITW),
				//        total = item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsITW),
				//    });
				//}

				return liquidationTurnDTO;
			}
		}

		private void FillLiquidationTurnDetails(LiquidationTurnDTO liquidationTurnDTO, DateTime emissionDate, int id_turn, int id_personProcessPlant)
		{
			var machineProdOpenings = db.MachineProdOpening
					.Where(r => (r.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") != null) //05: ANULADA
								&& (DbFunctions.TruncateTime(r.Document.emissionDate) == DbFunctions.TruncateTime(emissionDate))
								&& (r.id_Turn == id_turn))
					.ToList();

			foreach (var machineProdOpeningN in machineProdOpenings)
			{
				foreach (var item in machineProdOpeningN.LiquidationCartOnCart.Where(w => w.MachineForProd.id_personProcessPlant == id_personProcessPlant && w.Document.DocumentState.code != "05").ToList())
				{
					string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
					if (loteManualParm == "SI")
					{
						var liquidationsCartOnCartDetails = db.LiquidationCartOnCartDetail
						.Where(fod => fod.id_LiquidationCartOnCart == item.id);

						var agrupaciones = liquidationsCartOnCartDetails
						.GroupBy(e => new
						{
							e.ProductionLot.internalNumber,
							liquidationNumber = e.LiquidationCartOnCart.Document.number,
						})
						.Select(e => e.Key);

						var retorno = new List<ClosingMachinesTurnDetailDTO>();
						foreach (var agrupacion in agrupaciones)
						{
							var detallesAgrupados = liquidationsCartOnCartDetails
								.Where(e => e.ProductionLot.internalNumber == agrupacion.internalNumber
										&& e.LiquidationCartOnCart.Document.number == agrupacion.liquidationNumber)
								.ToArray();

							var liquidationCartOnCartDetail = detallesAgrupados.First();

							var _pt = db.ProcessType.FirstOrDefault(fod => fod.id == liquidationCartOnCartDetail.LiquidationCartOnCart.idProccesType);
							var _quantityPoundsIL = detallesAgrupados.Sum(e => e.quantityPoundsIL);

							var liquidationTurnDetailDTO = new LiquidationTurnDetailDTO
							{
								id = liquidationTurnDTO.LiquidationTurnDetails.Count() > 0 ? liquidationTurnDTO.LiquidationTurnDetails.Max(pld => pld.id) + 1 : 1,
								turn = machineProdOpeningN.Turn.name,
								numberLot = liquidationCartOnCartDetail.ProductionLot.internalNumber,
								process = liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.Person.processPlant,
								provider = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.Provider.Person.fullname_businessName,
								numberLiquidationCarOnCar = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.number,
								machineForProd = liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.name,
								cod_state = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.DocumentState.code,
								state = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.DocumentState.name,
								cod_stateLote = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.ProductionLotState.code,
								stateLote = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.ProductionLotState.name,
								tail = _pt.code.Equals("COL") ? _quantityPoundsIL : 0.00M,
								whole = _pt.code.Equals("COL") ? 0.00M : _quantityPoundsIL,
								total = _quantityPoundsIL,
							};

							liquidationTurnDTO.LiquidationTurnDetails.Add(liquidationTurnDetailDTO);

						}

					}
					else
					{
						var pt = db.ProcessType.FirstOrDefault(fod => fod.id == item.idProccesType);

						liquidationTurnDTO.LiquidationTurnDetails.Add(new LiquidationTurnDetailDTO
						{
							id = liquidationTurnDTO.LiquidationTurnDetails.Count() > 0 ? liquidationTurnDTO.LiquidationTurnDetails.Max(pld => pld.id) + 1 : 1,
							turn = machineProdOpeningN.Turn.name,
							numberLot = item.ProductionLot.internalNumber,
							process = item.MachineForProd.Person.processPlant,
							provider = item.ProductionLot.Provider.Person.fullname_businessName,
							numberLiquidationCarOnCar = item.Document.number,
							machineForProd = item.MachineForProd.name,
							cod_state = item.Document.DocumentState.code,
							state = item.Document.DocumentState.name,
							cod_stateLote = item.ProductionLot.ProductionLotState.code,
							stateLote = item.ProductionLot.ProductionLotState.name,
							tail = pt.code.Equals("COL") ? item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsIL) : 0.00M,
							whole = pt.code.Equals("COL") ? 0.00M : item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsIL),
							total = item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsIL),
						});
					}
				}
			}

		}

		private LiquidationTurnDTO ConvertToDto(LiquidationTurn liquidationTurn)
		{
			//var reception = liquidationTurn.ResultProdLotLiquidationTurn;//.FirstOrDefault(r => r.idLiquidationTurn == liquidationTurn.id);
			//if (reception == null)
			//    return null;

			var liquidationTurnDto = new LiquidationTurnDTO
			{
				id_turn = liquidationTurn.id_turn,
				id_personProcessPlant = liquidationTurn.id_personProcessPlant,
				id = liquidationTurn.id,
				description = liquidationTurn.Document.description,
				id_documentType = liquidationTurn.Document.id_documentType,
				number = liquidationTurn.Document.number,
				state = liquidationTurn.Document.DocumentState.name,
				documentType = liquidationTurn.Document.DocumentType.name,
				dateTimeEmision = liquidationTurn.Document.emissionDate,//.ToString("d"),
				liquidationDate = new DateTime(liquidationTurn.liquidationDate.Value.Year, liquidationTurn.liquidationDate.Value.Month, liquidationTurn.liquidationDate.Value.Day, 
				liquidationTurn.liquidationTime.Value.Hours, liquidationTurn.liquidationTime.Value.Minutes, liquidationTurn.liquidationTime.Value.Seconds), //liquidationTurn.liquidationDate,//.ToString("d"),
				liquidationTime = (TimeSpan)liquidationTurn.liquidationTime,//.ToString("d"),
				idSate = liquidationTurn.Document.id_documentState,
				reference = liquidationTurn.Document.reference,
				LiquidationTurnDetails = new List<LiquidationTurnDetailDTO>()
			};

			FillLiquidationTurnDetails(liquidationTurnDto, liquidationTurn.emissionDate, liquidationTurn.id_turn, liquidationTurn.id_personProcessPlant);

			return liquidationTurnDto;
		}

		private void BuildViewDataIndex()
		{
			BuildComboBoxState();
			BuildComboBoxTurnIndex();
			BuildComboBoxProviderIndex();
			BuildComboBoxProductionUnitProviderIndex();
		}

		private void BuildViewDataEdit()
		{
			//BuildComboBoxAnalist();
			//BuildComboBoxWeigher();
		}

		[HttpPost]
		public ActionResult Edit(int id = 0, int id_turn = 0, DateTime? emissionDate = null, bool enabled = true, int id_personProcessPlant = 0)
		{
			//BuildViewDataEdit();

			//var codeTypeLiquidationTurn = "";
			var model = new LiquidationTurnDTO();
			LiquidationTurn liquidationTurn = db.LiquidationTurn.FirstOrDefault(d => d.id == id);
			if (liquidationTurn == null)
			{
				model = Create(emissionDate.Value, id_turn, id_personProcessPlant);
				SetLiquidationTurnDTO(model);
				//codeTypeLiquidationTurn = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeLiquidationTurn)?.code;
				//BuildComboBoxSizeLiquidationTurn(codeTypeLiquidationTurn);

				BuilViewBag(enabled);
				//BuildComboBoxTypeLiquidationTurn(model.id, model.codeProcessType);
				return PartialView(model);
			}

			model = ConvertToDto(liquidationTurn);
			SetLiquidationTurnDTO(model);
			//codeTypeLiquidationTurn = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeLiquidationTurn)?.code;
			//BuildComboBoxSizeLiquidationTurn(codeTypeLiquidationTurn);
			BuilViewBag(enabled);
			//BuildComboBoxTypeLiquidationTurn(model.id, model.codeTypeProcess);

			return PartialView(model);
		}

		private void BuilViewBag(bool enabled)
		{
			var liquidationTurnDTO = GetLiquidationTurnDTO();
			ViewBag.enabled = enabled;
			ViewBag.canNew = liquidationTurnDTO.id != 0;
			ViewBag.canEdit = !enabled &&
							  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationTurnDTO.idSate)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationTurnDTO.idSate)
									 ?.code.Equals("01") ?? false) && liquidationTurnDTO.id != 0;
			ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationTurnDTO.idSate)
									 ?.code.Equals("03") ?? false) && !enabled;
			ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationTurnDTO.idSate)
									  ?.code.Equals("01") ?? false) && liquidationTurnDTO.id != 0;
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewDetails(bool? enabled)
		{
			var liquidationTurnDetails = GetLiquidationTurnDTO().LiquidationTurnDetails;

			ViewBag.enabled = enabled;

			return PartialView("_GridViewDetails", liquidationTurnDetails);
		}

		#region Combobox
		private void BuildComboBoxState()
		{
			ViewData["Estados"] = db.DocumentState
				.Where(e => e.isActive
					&& e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoLiquidationTurn))
				.Select(s => new SelectListItem
				{
					Text = s.name,
					Value = s.id.ToString(),
				}).ToList();
		}

		public ActionResult ComboBoxState()
		{
			BuildComboBoxState();
			return PartialView("_ComboBoxState");
		}

		private void BuildComboBoxTurnIndex()
		{
			ViewData["TurnIndex"] = db.Turn.Where(e => e.isActive)
			   .Select(s => new SelectListItem
			   {
				   Text = s.name,
				   Value = s.id.ToString(),
			   }).ToList();

		}
		public ActionResult ComboBoxTurnIndex()
		{
			BuildComboBoxTurnIndex();
			return PartialView("_ComboBoxTurnIndex");
		}

		private void BuildComboBoxProviderIndex()
		{
			ViewData["ProviderIndex"] = new List<SelectListItem>();
			var providerIndexList = db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == ActiveCompany.id)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
			//(DataProviderPerson.ProviderByCompany((int?)ViewData["id_company"]) as List<Person>);
			if (providerIndexList != null)
			{
				ViewData["ProviderIndex"] = providerIndexList
				.Select(s => new SelectListItem
				{
					Text = s.name,
					Value = s.id.ToString()
				}).ToList();
			}
		}

		public ActionResult ComboBoxProviderIndex()
		{
			BuildComboBoxProviderIndex();
			return PartialView("_ComboBoxProviderIndex");
		}

		private void BuildComboBoxProductionUnitProviderIndex(int? id_provider = null)
		{
			ViewData["ProductionUnitProviderIndex"] = new List<SelectListItem>();
			var productionUnitProviderIndexList = db.ProductionUnitProvider.Where(t => t.isActive && (t.id_provider == id_provider || id_provider == null)).Select(s => new {
				s.id,
				name = s.name
			}).OrderBy(t => t.id).ToList();
			//var productionUnitProviderIndexList = (DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, id_provider) as List<ProductionUnitProvider>);
			if (productionUnitProviderIndexList != null)
			{
				ViewData["ProductionUnitProviderIndex"] = productionUnitProviderIndexList
				.Select(s => new SelectListItem
				{
					Text = s.name,
					Value = s.id.ToString()
				}).ToList();
			}
		}

		public ActionResult ComboBoxProductionUnitProviderIndex(int? id_provider)
		{
			BuildComboBoxProductionUnitProviderIndex(id_provider);
			return PartialView("_ComboBoxProductionUnitProviderIndex");
		}

		#endregion

		[HttpPost]
		public JsonResult Save(string jsonLiquidationTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						#region Validación Permiso

						//var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						//if (entityObjectPermissions != null)
						//{
						//    //var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");//Hay que definir cual es la entidad que esta asociado el permiso
						//    //if (entityPermissions != null)
						//    //{
						//    //    foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
						//    //    {
						//    //        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
						//    //        if (entityValuePermissions == null)
						//    //        {
						//    //            throw new Exception("No tiene Permiso para editar y guardar la Recepción de Materiales.");
						//    //        }
						//    //    }
						//    //    foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
						//    //    {
						//    //        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
						//    //        if (entityValuePermissions == null)
						//    //        {
						//    //            throw new Exception("No tiene Permiso para editar y guardar la Recepción de Materiales.");
						//    //        }
						//    //    }
						//    //    if (approve)
						//    //    {
						//    //        foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
						//    //        {
						//    //            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
						//    //            if (entityValuePermissions == null)
						//    //            {
						//    //                throw new Exception("No tiene Permiso para aprobar la Recepción de Materiales.");
						//    //            }
						//    //        }

						//    //        foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
						//    //        {
						//    //            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
						//    //            if (entityValuePermissions == null)
						//    //            {
						//    //                throw new Exception("No tiene Permiso para aprobar la Recepción de Materiales.");
						//    //            }
						//    //        }

						//    //    }

						//    //}
						//}

						#endregion

						var liquidationTurnDTO = GetLiquidationTurnDTO();

						foreach (var item in liquidationTurnDTO.LiquidationTurnDetails)
						{
							if (item.cod_state != "04")//04: CERRADA
							{
								throw new Exception("No se puede guardar la liquidación de Turno, con detalle de liquidación de Carro por Carro con estado diferente a Cerrada");
							}
							if (item.cod_stateLote != "03" && item.cod_stateLote != "04" && item.cod_stateLote != "05" && item.cod_stateLote != "06" && item.cod_stateLote != "07" && item.cod_stateLote != "08")
							{
								throw new Exception("No se puede guardar la liquidación de Turno, con detalle de liquidación de Recepción de Materia Prima con estado " + item.stateLote);
							}
						}

						JToken token = JsonConvert.DeserializeObject<JToken>(jsonLiquidationTurn);

						//var id_machineProdOpening = liquidationTurnDTO.id_machineProdOpening;
						//var id_machineProdOpening = token.Value<int>("id_machineProdOpening");
						//                  var machineProdOpening = db.MachineProdOpening.FirstOrDefault(r =>
						//	r.id == id_machineProdOpening);
						//if (machineProdOpening == null)
						//	throw new Exception("Apertura de Turno no encontrada");


						var newObject = false;
						var id = token.Value<int>("id");

						var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoLiquidationTurn));
						var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

						var liquidationTurn = db.LiquidationTurn.FirstOrDefault(d => d.id == id);
						if (liquidationTurn == null)
						{
							newObject = true;

							var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
								? ActiveUser.EmissionPoint.First().id
								: 0;
							if (id_emissionPoint == 0)
								throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

							liquidationTurn = new LiquidationTurn
							{
								Document = new Document
								{
									number = GetDocumentNumber(documentType?.id ?? 0),
									sequential = GetDocumentSequential(documentType?.id ?? 0),
									emissionDate = DateTime.Now,
									authorizationDate = DateTime.Now,
									id_emissionPoint = id_emissionPoint,
									id_documentType = documentType.id,
									id_userCreate = ActiveUser.id,
									dateCreate = DateTime.Now,
								}
							};
							liquidationTurn.id_turn = liquidationTurnDTO.id_turn;
							liquidationTurn.id_personProcessPlant = liquidationTurnDTO.id_personProcessPlant;
							liquidationTurn.emissionDate = liquidationTurnDTO.dateTimeEmision;
							liquidationTurn.Document.emissionDate = liquidationTurnDTO.dateTimeEmision;//token.Value<DateTime>("dateTimeEmision");
							liquidationTurn.id_userCreate = liquidationTurn.Document.id_userCreate;
							liquidationTurn.id_userUpdate = liquidationTurn.Document.id_userCreate;

							documentType.currentNumber++;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;



						}

						//var liquidationTurnAux = db.LiquidationTurn.AsEnumerable().FirstOrDefault(r => r.id != liquidationTurn.id &&
						//					   r.idResultProdLotLiquidationTurn == id_reception && r.idLiquidationTurnType == token.Value<int>("idTypeLiquidationTurn") &&
						//					   (r.Document.DocumentState.code == "01" || r.Document.DocumentState.code == "03"));
						//if (liquidationTurnAux != null)
						//	throw new Exception("Existe la Recepción del Lote: " + liquidationTurnAux.ResultProdLotLiquidationTurn.numberLot +
						//						" con el mismo tipo de LiquidationTurn: " + liquidationTurnAux.tbsysCatalogueDetail.name +
						//						" en el LiquidationTurn con número: " + liquidationTurnAux.Document.number +
						//						" con estado: " + liquidationTurnAux.Document.DocumentState.name + ".");

						liquidationTurn.Document.id_documentState = documentState.id;
						liquidationTurn.Document.id_userUpdate = ActiveUser.id;
						liquidationTurn.id_userUpdate = liquidationTurn.Document.id_userUpdate;
						liquidationTurn.Document.dateUpdate = DateTime.Now;
						liquidationTurn.Document.reference = token.Value<string>("reference");
						liquidationTurn.Document.description = token.Value<string>("description");
						liquidationTurn.liquidationDate = token.Value<DateTime>("liquidationDateTime");
						liquidationTurn.liquidationTime = token.Value<DateTime>("liquidationDateTime").TimeOfDay;


						//liquidationTurn.idWeigher = token.Value<int>("idWeigher");
						//liquidationTurn.idAnalist = token.Value<int>("idAnalist");
						//liquidationTurn.idLiquidationTurnType = token.Value<int>("idTypeLiquidationTurn");



						//liquidationTurn.PoundsGarbage = token.Value<decimal>("poundsTrash");
						//liquidationTurn.gavetaNumber = token.Value<int>("drawersNumber");
						//liquidationTurn.TotalPoundsWeigthGross = token.Value<decimal>("totalPoundsGrossWeight");
						//liquidationTurn.porcTara = token.Value<decimal>("percentTara");
						//liquidationTurn.TotalPoundsWeigthNet = token.Value<decimal>("totalPoundsNetWeight");
						//liquidationTurn.idItemSize = token.Value<int?>("idSize");
						//liquidationTurn.idResultProdLotLiquidationTurn = id_reception;

						//Details
						//var lastDetails = db.LiquidationTurnDetail.Where(d => d.idLiquidationTurn == liquidationTurn.id);
						//foreach (var detail in lastDetails)
						//{
						//	db.LiquidationTurnDetail.Remove(detail);
						//	db.LiquidationTurnDetail.Attach(detail);
						//	db.Entry(detail).State = EntityState.Deleted;
						//}

						//var newDetails = token.Value<JArray>("liquidationTurnDetails");
						//foreach (var detail in newDetails)
						//{
						//	liquidationTurn.LiquidationTurnDetail.Add(new LiquidationTurnDetail
						//	{
						//		orderDetail = detail.Value<int>("drawerNumber"),
						//		grossWeight = detail.Value<int>("grossWeight"),
						//		idMetricUnit = db.MetricUnit.AsEnumerable().FirstOrDefault(fod => fod.code == detail.Value<string>("um"))?.id ?? 0,
						//		poundsGarbage = detail.Value<int>("poundsTrash")
						//	});
						//}

						if (newObject)
						{
							db.LiquidationTurn.Add(liquidationTurn);
							db.Entry(liquidationTurn).State = EntityState.Added;
						}
						else
						{
							db.LiquidationTurn.Attach(liquidationTurn);
							db.Entry(liquidationTurn).State = EntityState.Modified;
						}

						db.SaveChanges();

						trans.Commit();

						result.Data = liquidationTurn.id.ToString();

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
						result.Data = ApproveLiquidationTurn(id).name;
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

		private DocumentState ApproveLiquidationTurn(int id_liquidationTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationTurn = db.LiquidationTurn.FirstOrDefault(p => p.id == id_liquidationTurn);
					if (liquidationTurn != null)
					{
						//if (!liquidationTurn.LiquidationTurnDetail.All(d => d.grossWeight > 0))
						//{
						//	throw new Exception("No puede aprobar si al menos un valor en la culumna Peso Bruto es menor o igual a cero");
						//}
						//if (!liquidationTurn.LiquidationTurnDetail.All(d => d.poundsGarbage >= 0))
						//{
						//	throw new Exception("No puede aprobar si al menos un valor en la culumna Libras de basura es menor a cero");
						//}
						if (db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == liquidationTurn.id_personProcessPlant &&
																			  fod.MachineProdOpening.id_Turn == liquidationTurn.id_turn &&
																	DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(liquidationTurn.emissionDate) &&
																	(fod.MachineProdOpening.Document.DocumentState.code == "01" || fod.MachineProdOpening.Document.DocumentState.code == "03")) != null)
						{
							throw new Exception("No se puede Aprobar porque existen Aperturas de Máquina en estado Pendiente o Aprobado sin liquidaciones asociadas.");
						}

						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
						if (aprovedState == null)
							return null;

						liquidationTurn.Document.id_documentState = aprovedState.id;
						liquidationTurn.Document.authorizationDate = DateTime.Now;

						db.LiquidationTurn.Attach(liquidationTurn);
						db.Entry(liquidationTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, liquidationTurn.idResultProdLotLiquidationTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationTurn.Document.DocumentState;
				}
			}
		}

		[HttpPost]
		public JsonResult Reverse(int id)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						result.Data = ReverseLiquidationTurn(id).name;
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

		private DocumentState ReverseLiquidationTurn(int id_liquidationTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationTurn = db.LiquidationTurn.FirstOrDefault(p => p.id == id_liquidationTurn);
					if (liquidationTurn != null)
					{
						//if (!(liquidationTurn.ResultProdLotLiquidationTurn.ProductionLot.ProductionLotState.code == "01" ||
						//	  liquidationTurn.ResultProdLotLiquidationTurn.ProductionLot.ProductionLotState.code == "02"))//01: PENDIENTE DE RECEPCION o 02: RECEPCIONADO
						//{
						//	throw new Exception("No se puede reversar porque la recepción del lote esta en estado: " + liquidationTurn.ResultProdLotLiquidationTurn.ProductionLot.ProductionLotState.name);
						//}

						var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
						if (reverseState == null)
							return

						liquidationTurn.Document.DocumentState;
						liquidationTurn.Document.id_documentState = reverseState.id;
						liquidationTurn.Document.authorizationDate = DateTime.Now;

						db.LiquidationTurn.Attach(liquidationTurn);
						db.Entry(liquidationTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, liquidationTurn.idResultProdLotLiquidationTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationTurn.Document.DocumentState;
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
						result.Data = AnnulLiquidationTurn(id).name;
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

		private DocumentState AnnulLiquidationTurn(int id_liquidationTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var liquidationTurn = db.LiquidationTurn.FirstOrDefault(p => p.id == id_liquidationTurn);
					if (liquidationTurn != null)
					{
						var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
						if (annulState == null)
							return

						liquidationTurn.Document.DocumentState;
						liquidationTurn.Document.id_documentState = annulState.id;
						liquidationTurn.Document.authorizationDate = DateTime.Now;

						db.LiquidationTurn.Attach(liquidationTurn);
						db.Entry(liquidationTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, liquidationTurn.idResultProdLotLiquidationTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return liquidationTurn.Document.DocumentState;
				}
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			var index = GetLiquidationTurnResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

			var result = new
			{
				maximunPages = GetLiquidationTurnResultConsultDTO().Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			var element = GetLiquidationTurnResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
			var liquidationTurn = db.LiquidationTurn.FirstOrDefault(d => d.id == element.id);
			if (liquidationTurn == null)
				return PartialView("Edit", new LiquidationTurnDTO());

			BuildViewDataEdit();
			var model = ConvertToDto(liquidationTurn);
			SetLiquidationTurnDTO(model);
			//var codeTypeLiquidationTurn = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeLiquidationTurn)?.code;
			//BuildComboBoxSizeLiquidationTurn(codeTypeLiquidationTurn);
			BuilViewBag(false);
			//BuildComboBoxTypeLiquidationTurn(model.id, model.codeTypeProcess);

			return PartialView("Edit", model);
		}

		public JsonResult ProductionLotDailyCloseReport(int id, string codeReport)
		{
			#region "Armo Parametros"

			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR
			{
				Nombre = "@id",
				Valor = id
			};
			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel
			{
				codeReport = codeReport,
				conex = objConex,
				paramCRList = paramLst
			};

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}

		public JsonResult ProductionLotDailyCloseTemporalReport(int id, DateTime FechaLiquidacion, string codeReport)
		{
			#region "Armo Parametros"

			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param;
			_param = new ParamCR
			{
				Nombre = "@id",
				Valor = id
			};
			paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@fecha",
                Valor = FechaLiquidacion
            };
            paramLst.Add(_param);

            //_param = new ParamCR
            //{
            //	Nombre = "@idTurno",
            //	Valor = idTurno ?? 0
            //};

            //paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel
			{
				codeReport = codeReport,
				conex = objConex,
				paramCRList = paramLst
			};

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}

		public JsonResult printReportLiquidacionCaja(int id, string codeReport, DateTime FechaLiquidacion, int? idTurno)
		{
			#region "Armo Parametros"

			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param;
			_param = new ParamCR
			{
				Nombre = "@id",
				Valor = id
			};
			paramLst.Add(_param);

			_param = new ParamCR
			{
				Nombre = "@Fecha",
				Valor = FechaLiquidacion
			};
			paramLst.Add(_param);

			_param = new ParamCR
			{
				Nombre = "@idTurno",
				Valor = idTurno ?? 0
			};

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel
			{
				codeReport = codeReport,
				conex = objConex,
				paramCRList = paramLst
			};

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}

	}
}