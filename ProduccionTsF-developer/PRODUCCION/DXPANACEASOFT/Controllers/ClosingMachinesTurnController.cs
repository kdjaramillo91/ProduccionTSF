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
	public class ClosingMachinesTurnController : DefaultController
	{
		private const string m_TipoDocumentoClosingMachinesTurn = "133";

		private ClosingMachinesTurnDTO GetClosingMachinesTurnDTO()
		{
			if (!(Session["ClosingMachinesTurnDTO"] is ClosingMachinesTurnDTO closingMachinesTurn))
				closingMachinesTurn = new ClosingMachinesTurnDTO();
			return closingMachinesTurn;
		}

		private List<ClosingMachinesTurnResultConsultDTO> GetClosingMachinesTurnResultConsultDTO()
		{
			if (!(Session["ClosingMachinesTurnResultConsultDTO"] is List<ClosingMachinesTurnResultConsultDTO> closingMachinesTurnResultConsult))
				closingMachinesTurnResultConsult = new List<ClosingMachinesTurnResultConsultDTO>();
			return closingMachinesTurnResultConsult;
		}

		private void SetClosingMachinesTurnDTO(ClosingMachinesTurnDTO closingMachinesTurnDTO)
		{
			Session["ClosingMachinesTurnDTO"] = closingMachinesTurnDTO;
		}

		private void SetClosingMachinesTurnResultConsultDTO(List<ClosingMachinesTurnResultConsultDTO> closingMachinesTurnResultConsult)
		{
			Session["ClosingMachinesTurnResultConsultDTO"] = closingMachinesTurnResultConsult;
		}

		// GET: ClosingMachinesTurn
		public ActionResult Index()
		{
			BuildViewDataIndex();
			return View();
		}

		[HttpPost]
		public ActionResult SearchResult(ClosingMachinesTurnConsultDTO consult)
		{
			var result = GetListsConsultDto(consult);
			SetClosingMachinesTurnResultConsultDTO(result);
			return PartialView("ConsultResult", result);
		}

		[HttpPost]
		public ActionResult GridViewClosingMachinesTurn()
		{
			return PartialView("_GridViewIndex", GetClosingMachinesTurnResultConsultDTO());
		}

		private List<ClosingMachinesTurnResultConsultDTO> GetListsConsultDto(ClosingMachinesTurnConsultDTO consulta)
		{
			using (var db = new DBContext())
			{
				var consultaAux = Session["consulta"] as ClosingMachinesTurnConsultDTO;
				if (consultaAux != null && consulta.initDate == null)
				{
					consulta = consultaAux;
				}


				var consultResult = db.ClosingMachinesTurn.Where(ClosingMachinesTurnQueryExtensions.GetRequestByFilter(consulta));
				var query = consultResult.Select(t => new ClosingMachinesTurnResultConsultDTO
				{
					id = t.id,
					number = t.Document.number,
					emissionDate = t.Document.emissionDate,
					id_machineForProd = t.MachineProdOpeningDetail.id_MachineForProd,
					machineForProd = t.MachineProdOpeningDetail.MachineForProd.name,
					plantProcess = t.MachineProdOpeningDetail.MachineForProd.Person.processPlant,
					turn = t.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
					state = t.Document.DocumentState.name,

					canEdit = t.Document.DocumentState.code.Equals("01"),
					canAproved = t.Document.DocumentState.code.Equals("01"),
					canAnnul = t.Document.DocumentState.code.Equals("01"),
					canReverse = t.Document.DocumentState.code.Equals("03")

				}).OrderBy(ob => ob.number).ToList();

				var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

				if (entityObjectPermissions != null)
				{
					var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
					if (entityPermissions != null)
					{
						var tempModel = new List<ClosingMachinesTurnResultConsultDTO>();
						foreach (var item in query)
						{
							if (entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) != null)
							{
								tempModel.Add(item);
							}
						}
						query = tempModel;
					}
				}

				Session["consulta"] = consulta;

				return query;
			}
		}

		private static List<ClosingMachinesTurnPendingNewDTO> GetClosingMachinesTurnPendingNewDto()

		{
			using (var db = new DBContext())
			{

				// Ejecución de la consulta de pendientes
				var q = db.MachineProdOpeningDetail
					.Where(r => r.MachineProdOpening.Document.DocumentState.code.Equals("03") && //03: APROBADA
								r.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") != null)//05: ANULADA
					.Select(r => new ClosingMachinesTurnPendingNewDTO
					{
						id_MachineProdOpeningDetail = r.id,
						numberMachineProdOpening = r.MachineProdOpening.Document.number,
						plantProcess = r.MachineForProd.Person.processPlant,
						machineForProd = r.MachineForProd.name,
						emissionDate = r.MachineProdOpening.Document.emissionDate,
						//emissionDateStr = r.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
						id_turn = r.MachineProdOpening.id_Turn,
						turn = r.MachineProdOpening.Turn.name,
						timeInit = r.timeInit,
						timeEnd = r.timeEnd,
						state = r.MachineProdOpening.Document.DocumentState.name
					}).ToList();

				return q;
			}
		}

		[HttpPost]
		public ActionResult PendingNew()
		{
			return View(GetClosingMachinesTurnPendingNewDto());
		}

		[ValidateInput(false)]
		public ActionResult GridViewPendingNew()
		{
			return PartialView("_GridViewPendingNew", GetClosingMachinesTurnPendingNewDto());
		}

		private ClosingMachinesTurnDTO Create(int id_MachineProdOpeningDetail)
		{
			using (var db = new DBContext())
			{
				var machineProdOpeningDetail =
					db.MachineProdOpeningDetail.FirstOrDefault(r => r.id == id_MachineProdOpeningDetail);
				if (machineProdOpeningDetail == null)
					return new ClosingMachinesTurnDTO();

				var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoClosingMachinesTurn));
				var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

				var closingMachinesTurnDTO = new ClosingMachinesTurnDTO
				{
					id_machineProdOpeningDetail = id_MachineProdOpeningDetail,
					description = "",
					number = "",//GetDocumentNumber(documentType?.id ?? 0),
					documentType = documentType?.name ?? "",
					id_documentType = documentType?.id ?? 0,
					reference = "",
					dateTimeEmision = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate,
					dateTimeEmisionStr = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
					idSate = documentState?.id ?? 0,
					state = documentState?.name ?? "",
					id_machineForProd = machineProdOpeningDetail.id_MachineForProd,
					machineForProd = machineProdOpeningDetail.MachineForProd.name,
					id_turn = machineProdOpeningDetail.MachineProdOpening.id_Turn,
					turn = machineProdOpeningDetail.MachineProdOpening.Turn.name,
					noOfPerson = (int)machineProdOpeningDetail.numPerson,
					poundsRemitted = 0.00M,
					noOfBox = 0,
					idPerson = machineProdOpeningDetail.id_Person,
					person = machineProdOpeningDetail.Person.fullname_businessName,
					poundsProcessed = 0.00M,
					poundsTailProcessed = 0.00M,
					poundsWholeProcessed = 0.00M,
					poundsCooling = 0.00M,
					poundsTailCooling = 0.00M,
					poundsWholeCooling = 0.00M,
					ClosingMachinesTurnDetails = new List<ClosingMachinesTurnDetailDTO>()
				};

				FillClosingMachinesTurnDetails(closingMachinesTurnDTO, machineProdOpeningDetail);

				return closingMachinesTurnDTO;
			}
		}

		private void FillClosingMachinesTurnDetails(ClosingMachinesTurnDTO closingMachinesTurnDTO, MachineProdOpeningDetail machineProdOpeningDetail)
		{
			var liquidationCartOnCarts = db.LiquidationCartOnCart
					.Where(r => r.id_MachineForProd == machineProdOpeningDetail.id_MachineForProd &&
								r.id_MachineProdOpening == machineProdOpeningDetail.id_MachineProdOpening &&
								r.Document.DocumentState.code != "05")
					.ToList();
			
			foreach (var liquidationCartOnCartN in liquidationCartOnCarts)
			{
				var pt = db.ProcessType.FirstOrDefault(fod => fod.id == liquidationCartOnCartN.idProccesType);
				string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
				if (loteManualParm == "SI")
				{
					var liquidationsCartOnCartDetails = db.LiquidationCartOnCartDetail
						.Where(fod => fod.id_LiquidationCartOnCart == liquidationCartOnCartN.id);

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

						var _quantityPoundsIL = detallesAgrupados.Sum(e => e.quantityPoundsIL);
						var _poundsCooling = detallesAgrupados.Sum(e => e.quantityPoundsITW);
						var _noOfBox = Decimal.ToInt32(detallesAgrupados.Sum(e => e.quatityBoxesIL));

						bool _loteManual = false;
						_loteManual = liquidationCartOnCartDetail.ProductionLot.ProductionProcess.code == "RMM";
						var _weightAux = 0m;

						if(_loteManual == false)
						{

							_weightAux = liquidationCartOnCartDetail
                                    ?.ProductionLot
                                    ?.ProductionLotDetail
                                    .Average(e => e?.ProductionLotDetailQualityControl
                                        .Where(fod => fod.QualityControl != null && (fod.QualityControl?.Document?.DocumentState?.code != "05"))
                                        .Where(x => (x?.QualityControl?.grammageReference != null))
                                        ?.FirstOrDefault()
                                        ?.QualityControl?.grammageReference ?? 0m) ?? 0m;

						}
						_weightAux = decimal.Round(_weightAux, 2);

                        var closingMachinesTurnDetailDTO = new ClosingMachinesTurnDetailDTO
                        //retorno.Add(new ClosingMachinesTurnDetailDTO() 
                        {
                            id_liquidationCartOnCart = liquidationCartOnCartDetail.id_LiquidationCartOnCart,
							numberLiquidationCartOnCart = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.number,
							provider = liquidationCartOnCartDetail.ProductionLot.Provider.Person.fullname_businessName,
							nameProviderShrimp = liquidationCartOnCartDetail.ProductionLot.ProductionUnitProvider.name,
							productionUnitProviderPool = liquidationCartOnCartDetail.ProductionLot.ProductionUnitProviderPool.name,
							weight = _weightAux,
							proccesType = pt.code.Equals("COL") ? "Cola" : "Entero",
							numberLot = liquidationCartOnCartDetail.ProductionLot.internalNumber,
							plantProcess = liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.Person.processPlant,

							poundsRemitted = liquidationCartOnCartDetail.ProductionLot.totalQuantityRecived,
							poundsProcessed = _quantityPoundsIL,
							poundsCooling = _poundsCooling,
							noOfBox = _noOfBox,
							cod_state = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.DocumentState.code,
							state = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.DocumentState.name,
							nameliquidator = (liquidationCartOnCartDetail.LiquidationCartOnCart.id_liquidator == null) ? "" : (liquidationCartOnCartDetail.LiquidationCartOnCart.Person.fullname_businessName),
						};

                        closingMachinesTurnDTO.ClosingMachinesTurnDetails.Add(closingMachinesTurnDetailDTO);

                    }
				}
				else
				{

					var weightAux = liquidationCartOnCartN
					?.ProductionLot
					?.ProductionLotDetail
					.Average(e => e?.ProductionLotDetailQualityControl
						.Where(fod => fod.QualityControl != null && (fod.QualityControl?.Document?.DocumentState?.code != "05"))
						.Where(x => (x?.QualityControl?.grammageReference != null))
						?.FirstOrDefault()
						?.QualityControl?.grammageReference ?? 0m) ?? 0m;


					weightAux = decimal.Round(weightAux,2);

					var closingMachinesTurnDetailDTO = new ClosingMachinesTurnDetailDTO
					{
						id_liquidationCartOnCart = liquidationCartOnCartN.id,
						numberLiquidationCartOnCart = liquidationCartOnCartN.Document.number,
						provider = liquidationCartOnCartN.ProductionLot.Provider.Person.fullname_businessName,
						nameProviderShrimp = liquidationCartOnCartN.ProductionLot.ProductionUnitProvider.name,
						productionUnitProviderPool = liquidationCartOnCartN.ProductionLot.ProductionUnitProviderPool.name,
						weight = weightAux,
						proccesType = pt.code.Equals("COL") ? "Cola" : "Entero",
						numberLot = liquidationCartOnCartN.ProductionLot.internalNumber,
						plantProcess = liquidationCartOnCartN.MachineForProd.Person.processPlant,
						//plantProcess = liquidationCartOnCartN.ProductionLot.Person1.processPlant,
						poundsRemitted = liquidationCartOnCartN.ProductionLot.totalQuantityRecived,
						poundsProcessed = 0.00M,
						poundsCooling = 0.00M,
						noOfBox = 0,
						cod_state = liquidationCartOnCartN.Document.DocumentState.code,
						state = liquidationCartOnCartN.Document.DocumentState.name,
						nameliquidator = (liquidationCartOnCartN.id_liquidator == null) ? "" : (liquidationCartOnCartN.Person.fullname_businessName),

					};
					if (closingMachinesTurnDTO.ClosingMachinesTurnDetails.FirstOrDefault(fod => fod.numberLot == closingMachinesTurnDetailDTO.numberLot) == null)
					{
						closingMachinesTurnDTO.poundsRemitted += closingMachinesTurnDetailDTO.poundsRemitted;
					}
					foreach (var item in liquidationCartOnCartN.LiquidationCartOnCartDetail.ToList())
					{
						closingMachinesTurnDetailDTO.poundsProcessed += item.quantityPoundsIL;
						closingMachinesTurnDTO.poundsProcessed += item.quantityPoundsIL;
						closingMachinesTurnDTO.poundsTailProcessed += pt.code.Equals("COL") ? item.quantityPoundsIL : 0.00M;
						closingMachinesTurnDTO.poundsWholeProcessed += pt.code.Equals("COL") ? 0.00M : item.quantityPoundsIL;

						closingMachinesTurnDetailDTO.poundsCooling += item.quantityPoundsITW;
						closingMachinesTurnDTO.poundsCooling += item.quantityPoundsITW;
						closingMachinesTurnDTO.poundsTailCooling += pt.code.Equals("COL") ? item.quantityPoundsITW : 0.00M;
						closingMachinesTurnDTO.poundsWholeCooling += pt.code.Equals("COL") ? 0.00M : item.quantityPoundsITW;

						var quatityBoxesILAux = Decimal.ToInt32(item.quatityBoxesIL);
						closingMachinesTurnDetailDTO.noOfBox += quatityBoxesILAux;
						closingMachinesTurnDTO.noOfBox += quatityBoxesILAux;

					}
					closingMachinesTurnDTO.ClosingMachinesTurnDetails.Add(closingMachinesTurnDetailDTO);
				}
			}

		}

		private ClosingMachinesTurnDTO ConvertToDto(ClosingMachinesTurn closingMachinesTurn)
		{
			//var reception = closingMachinesTurn.ResultProdLotClosingMachinesTurn;//.FirstOrDefault(r => r.idClosingMachinesTurn == closingMachinesTurn.id);
			//if (reception == null)
			//    return null;

			var closingMachinesTurnDto = new ClosingMachinesTurnDTO
			{
				id = closingMachinesTurn.id,
				id_machineProdOpeningDetail = closingMachinesTurn.id_MachineProdOpeningDetail,
				description = closingMachinesTurn.Document.description,
				id_documentType = closingMachinesTurn.Document.id_documentType,
				number = closingMachinesTurn.Document.number,
				state = closingMachinesTurn.Document.DocumentState.name,
				documentType = closingMachinesTurn.Document.DocumentType.name,
				dateTimeEmision = closingMachinesTurn.Document.emissionDate,//.ToString("d"),
				dateTimeEmisionStr = closingMachinesTurn.Document.emissionDate.ToString("dd-MM-yyyy"),
				idSate = closingMachinesTurn.Document.id_documentState,
				reference = closingMachinesTurn.Document.reference,
				id_machineForProd = closingMachinesTurn.MachineProdOpeningDetail.id_MachineForProd,
				machineForProd = closingMachinesTurn.MachineProdOpeningDetail.MachineForProd.name,
				id_turn = closingMachinesTurn.MachineProdOpeningDetail.MachineProdOpening.id_Turn,
				turn = closingMachinesTurn.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
				noOfPerson =(int) closingMachinesTurn.MachineProdOpeningDetail.numPerson,
				poundsRemitted = 0.00M,
				noOfBox = 0,
				idPerson = closingMachinesTurn.MachineProdOpeningDetail.id_Person,
				person = closingMachinesTurn.MachineProdOpeningDetail.Person.fullname_businessName,
				poundsProcessed = 0.00M,
				poundsTailProcessed = 0.00M,
				poundsWholeProcessed = 0.00M,
				poundsCooling = 0.00M,
				poundsTailCooling = 0.00M,
				poundsWholeCooling = 0.00M,
				ClosingMachinesTurnDetails = new List<ClosingMachinesTurnDetailDTO>()
			};

			FillClosingMachinesTurnDetails(closingMachinesTurnDto, closingMachinesTurn.MachineProdOpeningDetail);

			return closingMachinesTurnDto;
		}

		private void BuildViewDataIndex()
		{
			BuildComboBoxState();
			BuildComboBoxTurnIndex();
			BuildComboBoxMachineForProdIndex();
			BuildComboBoxPersonIndex();
			BuildComboBoxProviderIndex();
			BuildComboBoxProductionUnitProviderIndex();
		}

		private void BuildViewDataEdit()
		{
			//BuildComboBoxAnalist();
			//BuildComboBoxWeigher();
		}

		[HttpPost]
		public ActionResult Edit(int id = 0, int id_MachineProdOpeningDetail = 0, bool enabled = true)
		{
			//BuildViewDataEdit();

			var model = new ClosingMachinesTurnDTO();
			ClosingMachinesTurn closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(d => d.id == id);
			if (closingMachinesTurn == null)
			{
				model = Create(id_MachineProdOpeningDetail);
				SetClosingMachinesTurnDTO(model);

				BuilViewBag(enabled);
				return PartialView(model);
			}

			model = ConvertToDto(closingMachinesTurn);
			SetClosingMachinesTurnDTO(model);
			BuilViewBag(enabled);

			return PartialView(model);
		}

		private void BuilViewBag(bool enabled)
		{
			var closingMachinesTurnDTO = GetClosingMachinesTurnDTO();
			ViewBag.enabled = enabled;
			ViewBag.canNew = closingMachinesTurnDTO.id != 0;
			ViewBag.canEdit = !enabled &&
							  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closingMachinesTurnDTO.idSate)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closingMachinesTurnDTO.idSate)
									 ?.code.Equals("01") ?? false) && closingMachinesTurnDTO.id != 0;
			ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closingMachinesTurnDTO.idSate)
									 ?.code.Equals("03") ?? false) && !enabled;
			ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closingMachinesTurnDTO.idSate)
									  ?.code.Equals("01") ?? false) && closingMachinesTurnDTO.id != 0;
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewDetails(bool? enabled)
		{
			var closingMachinesTurnDetails = GetClosingMachinesTurnDTO().ClosingMachinesTurnDetails;

			ViewBag.enabled = enabled;

			return PartialView("_GridViewDetails", closingMachinesTurnDetails);
		}

		#region Combobox
		private void BuildComboBoxState()
		{
			ViewData["Estados"] = db.DocumentState
				.Where(e => e.isActive
					&& e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoClosingMachinesTurn))
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

		private void BuildComboBoxMachineForProdIndex()
		{
			ViewData["MachineForProdIndex"] = (DataProviderMachineForProd.MachineForProds((EntityObjectPermissions)ViewData["entityObjectPermissions"]) as List<MachineForProd>)
				.Select(s => new SelectListItem
				{
					Text = s.name,
					Value = s.id.ToString()
				}).ToList();
		}

		public ActionResult ComboBoxMachineForProdIndex()
		{
			BuildComboBoxMachineForProdIndex();
			return PartialView("_ComboBoxMachineForProdIndex");
		}

		private void BuildComboBoxPersonIndex()
		{
			ViewData["PersonIndex"] = (DataProviderPerson.RolsByCompany((int?)ViewData["id_company"], "Supervisor") as List<Person>)
				.Select(s => new SelectListItem
				{
					Text = s.fullname_businessName,
					Value = s.id.ToString()
				}).ToList();
		}

		public ActionResult ComboBoxPersonIndex()
		{
			BuildComboBoxPersonIndex();
			return PartialView("_ComboBoxPersonIndex");
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
		public JsonResult Save(string jsonClosingMachinesTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						var closingMachinesTurnDTO = GetClosingMachinesTurnDTO();

						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closingMachinesTurnDTO.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para editar y guardar el cierre por máquina turno.");
								}
							}
						}

						#endregion


						foreach (var item in closingMachinesTurnDTO.ClosingMachinesTurnDetails)
						{
							if (item.cod_state == "01")//01: Pendiente
							{
								throw new Exception("No se puede guardar el cierre de Turno de máquina, con detalle de liquidación de Carro por Carro en estado Pendiente");
							}
						}

						JToken token = JsonConvert.DeserializeObject<JToken>(jsonClosingMachinesTurn);

						//var id_machineProdOpening = closingMachinesTurnDTO.id_machineProdOpening;
						//var id_machineProdOpening = token.Value<int>("id_machineProdOpening");
						//                  var machineProdOpening = db.MachineProdOpening.FirstOrDefault(r =>
						//	r.id == id_machineProdOpening);
						//if (machineProdOpening == null)
						//	throw new Exception("Apertura de Turno no encontrada");


						var newObject = false;
						var id = token.Value<int>("id");

						var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoClosingMachinesTurn));
						var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

						var closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(d => d.id == id);
						if (closingMachinesTurn == null)
						{
							newObject = true;

							var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
								? ActiveUser.EmissionPoint.First().id
								: 0;
							if (id_emissionPoint == 0)
								throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

							closingMachinesTurn = new ClosingMachinesTurn
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
							closingMachinesTurn.id_MachineProdOpeningDetail = closingMachinesTurnDTO.id_machineProdOpeningDetail;
							closingMachinesTurn.Document.emissionDate = closingMachinesTurnDTO.dateTimeEmision;//token.Value<DateTime>("dateTimeEmision");
							closingMachinesTurn.id_userCreate = closingMachinesTurn.Document.id_userCreate;
							closingMachinesTurn.id_userUpdate = closingMachinesTurn.Document.id_userCreate;

							documentType.currentNumber++;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;



						}

						closingMachinesTurn.Document.id_documentState = documentState.id;
						closingMachinesTurn.Document.id_userUpdate = ActiveUser.id;
						closingMachinesTurn.id_userUpdate = closingMachinesTurn.Document.id_userUpdate;
						closingMachinesTurn.Document.dateUpdate = DateTime.Now;
						//closingMachinesTurn.Document.reference = token.Value<string>("reference");
						closingMachinesTurn.Document.description = token.Value<string>("description");


						if (newObject)
						{
							db.ClosingMachinesTurn.Add(closingMachinesTurn);
							db.Entry(closingMachinesTurn).State = EntityState.Added;

							var closurePendingState = db.DocumentState.FirstOrDefault(d => d.code.Equals("20"));//20: PENDIENTE DE CIERRE
							if (closurePendingState != null)
							{
								var machineProdOpeningDetail = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == closingMachinesTurnDTO.id_machineProdOpeningDetail);
								machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closurePendingState.id;
								db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
								db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;

								var liquidationCartOnCarts = db.LiquidationCartOnCart
																.Where(r => r.id_MachineForProd == machineProdOpeningDetail.id_MachineForProd &&
																			r.id_MachineProdOpening == machineProdOpeningDetail.id_MachineProdOpening &&
																			r.Document.DocumentState.code != "05")
																.ToList();
								foreach (var item in liquidationCartOnCarts)
								{
									item.Document.id_documentState = closurePendingState.id;
									db.LiquidationCartOnCart.Attach(item);
									db.Entry(item).State = EntityState.Modified;
								}

							}

						}
						else
						{
							db.ClosingMachinesTurn.Attach(closingMachinesTurn);
							db.Entry(closingMachinesTurn).State = EntityState.Modified;
						}

						db.SaveChanges();

						trans.Commit();

						result.Data = closingMachinesTurn.id.ToString();

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
						result.Data = ApproveClosingMachinesTurn(id).name;
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

		private DocumentState ApproveClosingMachinesTurn(int id_closingMachinesTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(p => p.id == id_closingMachinesTurn);
					if (closingMachinesTurn != null)
					{
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closingMachinesTurn.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para aprobar el cierre por máquina turno.");
								}
							}
						}

						#endregion

						var closureState = db.DocumentState.FirstOrDefault(d => d.code.Equals("04"));//04: CERRADA
						if (closureState != null)
						{
							var machineProdOpeningDetail = closingMachinesTurn.MachineProdOpeningDetail;
							machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closureState.id;
							db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
							db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;

							var liquidationCartOnCarts = db.LiquidationCartOnCart
															.Where(r => r.id_MachineForProd == machineProdOpeningDetail.id_MachineForProd &&
																		r.id_MachineProdOpening == machineProdOpeningDetail.id_MachineProdOpening &&
																		r.Document.DocumentState.code != "05")
															.ToList();
							foreach (var item in liquidationCartOnCarts)
							{
								item.Document.id_documentState = closureState.id;
								db.LiquidationCartOnCart.Attach(item);
								db.Entry(item).State = EntityState.Modified;
							}

						}

						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
						if (aprovedState == null)
							return null;

						closingMachinesTurn.Document.id_documentState = aprovedState.id;
						closingMachinesTurn.Document.authorizationDate = DateTime.Now;

						db.ClosingMachinesTurn.Attach(closingMachinesTurn);
						db.Entry(closingMachinesTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, closingMachinesTurn.idResultProdLotClosingMachinesTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return closingMachinesTurn.Document.DocumentState;
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
						result.Data = ReverseClosingMachinesTurn(id).name;
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

		private DocumentState ReverseClosingMachinesTurn(int id_closingMachinesTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(p => p.id == id_closingMachinesTurn);
					if (closingMachinesTurn != null)
					{
						if (db.LiquidationTurn.FirstOrDefault(fod => fod.id_personProcessPlant == closingMachinesTurn.MachineProdOpeningDetail.MachineForProd.id_personProcessPlant &&
																	fod.id_turn == closingMachinesTurn.MachineProdOpeningDetail.MachineProdOpening.id_Turn &&
																	DbFunctions.TruncateTime(fod.emissionDate) == DbFunctions.TruncateTime(closingMachinesTurn.MachineProdOpeningDetail.MachineProdOpening.Document.emissionDate) &&
																	fod.Document.DocumentState.code != "05") != null)
						{
							throw new Exception("No se puede reversar porque se encuentra en un Turno Liquidado.");
						}
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closingMachinesTurn.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para reversar el cierre por máquina turno.");
								}
							}
						}

						#endregion

						var closurePendingState = db.DocumentState.FirstOrDefault(d => d.code.Equals("20"));//20: PENDIENTE DE CIERRE
						if (closurePendingState != null)
						{
							var machineProdOpeningDetail = closingMachinesTurn.MachineProdOpeningDetail;
							machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closurePendingState.id;
							db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
							db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;

							var liquidationCartOnCarts = db.LiquidationCartOnCart
															.Where(r => r.id_MachineForProd == machineProdOpeningDetail.id_MachineForProd &&
																		r.id_MachineProdOpening == machineProdOpeningDetail.id_MachineProdOpening &&
																		r.Document.DocumentState.code != "05")
															.ToList();
							foreach (var item in liquidationCartOnCarts)
							{
								item.Document.id_documentState = closurePendingState.id;
								db.LiquidationCartOnCart.Attach(item);
								db.Entry(item).State = EntityState.Modified;
							}

						}

						var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
						if (reverseState == null)
							return

						closingMachinesTurn.Document.DocumentState;
						closingMachinesTurn.Document.id_documentState = reverseState.id;
						closingMachinesTurn.Document.authorizationDate = DateTime.Now;

						db.ClosingMachinesTurn.Attach(closingMachinesTurn);
						db.Entry(closingMachinesTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, closingMachinesTurn.idResultProdLotClosingMachinesTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return closingMachinesTurn.Document.DocumentState;
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
						result.Data = AnnulClosingMachinesTurn(id).name;
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

		private DocumentState AnnulClosingMachinesTurn(int id_closingMachinesTurn)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(p => p.id == id_closingMachinesTurn);
					if (closingMachinesTurn != null)
					{
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closingMachinesTurn.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para anular el cierre por máquina turno.");
								}
							}
						}

						#endregion

						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));//03: APROBADA
						if (aprovedState != null)
						{
							var machineProdOpeningDetail = closingMachinesTurn.MachineProdOpeningDetail;
							machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = aprovedState.id;
							db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
							db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;

							var liquidationCartOnCarts = db.LiquidationCartOnCart
															.Where(r => r.id_MachineForProd == machineProdOpeningDetail.id_MachineForProd &&
																		r.id_MachineProdOpening == machineProdOpeningDetail.id_MachineProdOpening &&
																		r.Document.DocumentState.code != "05")
															.ToList();
							foreach (var item in liquidationCartOnCarts)
							{
								item.Document.id_documentState = aprovedState.id;
								db.LiquidationCartOnCart.Attach(item);
								db.Entry(item).State = EntityState.Modified;
							}

						}

						var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
						if (annulState == null)
							return

						closingMachinesTurn.Document.DocumentState;
						closingMachinesTurn.Document.id_documentState = annulState.id;
						closingMachinesTurn.Document.authorizationDate = DateTime.Now;

						db.ClosingMachinesTurn.Attach(closingMachinesTurn);
						db.Entry(closingMachinesTurn).State = EntityState.Modified;
						db.SaveChanges();

						//this.RecalcularTotalesLiquidacion(db, closingMachinesTurn.idResultProdLotClosingMachinesTurn);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return closingMachinesTurn.Document.DocumentState;
				}
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			var index = GetClosingMachinesTurnResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

			var result = new
			{
				maximunPages = GetClosingMachinesTurnResultConsultDTO().Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			var element = GetClosingMachinesTurnResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
			var closingMachinesTurn = db.ClosingMachinesTurn.FirstOrDefault(d => d.id == element.id);
			if (closingMachinesTurn == null)
				return PartialView("Edit", new ClosingMachinesTurnDTO());

			BuildViewDataEdit();
			var model = ConvertToDto(closingMachinesTurn);
			SetClosingMachinesTurnDTO(model);
			BuilViewBag(false);

			return PartialView("Edit", model);
		}
		public JsonResult ClosingMachinesTurnReport(int id, string codeReport)
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
	}
}