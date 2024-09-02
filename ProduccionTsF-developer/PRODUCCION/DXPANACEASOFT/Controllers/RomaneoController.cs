using DevExpress.Data.ODataLinq.Helpers;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
	public class RomaneoController : DefaultController
	{
		private const string m_TipoDocumentoRomaneo = "106";

		private RomaneoDTO GetRomaneoDTO()
		{
			if (!(Session["RomaneoDTO"] is RomaneoDTO romaneo))
				romaneo = new RomaneoDTO();
			return romaneo;
		}

		private List<RomaneoResultConsultDTO> GetRomaneoResultConsultDTO()
		{
			if (!(Session["RomaneoResultConsultDTO"] is List<RomaneoResultConsultDTO> romaneoResultConsult))
				romaneoResultConsult = new List<RomaneoResultConsultDTO>();
			return romaneoResultConsult;
		}

		private void SetRomaneoDTO(RomaneoDTO romaneoDTO)
		{
			Session["RomaneoDTO"] = romaneoDTO;
		}

		private void SetRomaneoResultConsultDTO(List<RomaneoResultConsultDTO> romaneoResultConsult)
		{
			Session["RomaneoResultConsultDTO"] = romaneoResultConsult;
		}

		// GET: Romaneo
		public ActionResult Index()
		{
			BuildViewDataIndex();
			return View();
		}

		[HttpPost]
		public ActionResult SearchResult(RomaneoConsultDTO consult)
		{
			var result = GetListsConsultDto(consult);
			SetRomaneoResultConsultDTO(result);
			return PartialView("ConsultResult", result);
		}

		[HttpPost]
		public ActionResult GridViewRomaneo()
		{
			return PartialView("_GridViewIndex", GetRomaneoResultConsultDTO());
		}

		private List<RomaneoResultConsultDTO> GetListsConsultDto(RomaneoConsultDTO consulta)
		{
			using (var db = new DBContext())
			{
				var consultaAux = Session["consulta"] as RomaneoConsultDTO;
				if (consultaAux != null && consulta.initDate == null)
				{
					consulta = consultaAux;
				}


				var consultResult = db.Romaneo.Where(RomaneoQueryExtensions.GetRequestByFilter(consulta));
				var query = consultResult.Select(t => new RomaneoResultConsultDTO
				{
					id = t.id,
					number = t.Document.number,
					secTransaction = t.ResultProdLotRomaneo.numberLotSequential,
					emissionDate = t.Document.emissionDate,
					numberLot = t.ResultProdLotRomaneo.numberLot,
					provider = t.ResultProdLotRomaneo.nameProvider,
					nameProviderShrimp = t.ResultProdLotRomaneo.nameProviderShrimp,
					namePool = t.ResultProdLotRomaneo.namePool,
					nameItem = t.ResultProdLotRomaneo.nameItem,
					typeRomaneo = t.tbsysCatalogueDetail.name,
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

		private static List<RomaneoPendingNewDTO> GetRomaneoPendingNewDto()
		{
			using (var db = new DBContext())
			{
				// Verificación del parámetro de Romaneo
				var romaneoHabilitado = db.Setting.AsEnumerable()
					.Any(s => s.code == "ROMANEO" && s.value == "1");

				if (!romaneoHabilitado)
				{
					return new List<RomaneoPendingNewDTO>();
				}

				// Ejecución de la consulta de pendientes
				return db.ResultProdLotRomaneo
					.Where(r => r.ProductionLot.ProductionLotState.code.Equals("01") ||
								r.ProductionLot.ProductionLotState.code.Equals("02")) //01: PENDIENTE DE RECEPCION o 02: RECEPCIONADO
					.AsEnumerable()
					.Select(r => new RomaneoPendingNewDTO
					{
						idProductionLotReception = r.idProductionLot,
						number = r.numberLotSequential,
						numberLot = r.numberLot,
						dateReception = r.dateTimeReception.ToString("dd-MM-yyyy"),
						provider = r.nameProvider,
						shrimper = r.nameProviderShrimp,
						item = r.nameItem,
						namePool = r.namePool,
						poundsRemitted = r.quantityRemitted,
						metricUnit = r.MetricUnit.name
					}).ToList();
			}
		}

		[HttpPost]
		public ActionResult PendingNew()
		{
			return View(GetRomaneoPendingNewDto());
		}

		[ValidateInput(false)]
		public ActionResult GridViewPendingNew()
		{
			return PartialView("_GridViewPendingNew", GetRomaneoPendingNewDto());
		}

		private RomaneoDTO Create(int idProductionLotReception)
		{
			using (var db = new DBContext())
			{
				var reception =
					db.ResultProdLotRomaneo.FirstOrDefault(r => r.idProductionLot == idProductionLotReception);
				if (reception == null)
					return new RomaneoDTO();

				var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoRomaneo));
				var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

				var romaneoDTO = new RomaneoDTO
				{
					description = "",
					number = GetDocumentNumber(documentType?.id ?? 0),
					documentType = documentType?.name ?? "",
					id_documentType = documentType?.id ?? 0,
					reference = "",
					dateTimeEmision = DateTime.Now,
					idSate = documentState?.id ?? 0,
					state = documentState?.name ?? "",
					idAnalist = ActiveUser.Employee.id,
					idWeigher = ActiveUser.Employee.id,
					dateTimeReception = reception.dateTimeReception.ToString("dd/MM/yyyy hh:mm:ss"),
					numberLot = reception.numberLot,
					numberLotSequential = reception.numberLotSequential,
					nameProvider = reception.nameProvider,
					nameProviderShrimp = reception.nameProviderShrimp,
					INPnumber = reception.INPnumber,
					namePool = reception.namePool,
					nameWarehouse = reception.nameWarehouseItem,
					nameWarehouseLocation = reception.nameWarehouseLocationItem,
					nameItem = reception.nameItem,
					codeProcessType = reception.codeProcessType,
					poundsTrash = 0,
					drawersNumber = 0,
					totalPoundsGrossWeight = 0,
					poundsRemitted = reception.quantityRemitted,
					percentTara = reception.codeProcessType == "ENT" ? 0.00M : 2.85M,
					idTypeRomaneo = reception.codeProcessType == "ENT" ? db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.code == "REC")?.id : db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.code == "COL")?.id,
					totalPoundsNetWeight = 0,
					idProductionLotReception = reception.idProductionLot,
					codeTypeProcess = reception.codeProcessType,
					RomaneoDetails = new List<RomaneoDetailDTO>(),
				};

				ViewBag.drawersNumber = romaneoDTO.drawersNumber;
				ViewBag.idTypeRomaneo = romaneoDTO.idTypeRomaneo;
				ViewBag.percentTara = romaneoDTO.percentTara;
				return romaneoDTO;
			}
		}

		private RomaneoDTO ConvertToDto(Romaneo romaneo)
		{
			//var reception = romaneo.ResultProdLotRomaneo;//.FirstOrDefault(r => r.idRomaneo == romaneo.id);
			//if (reception == null)
			//    return null;

			var romaneoDto = new RomaneoDTO
			{
				id = romaneo.id,
				description = romaneo.Document.description,
				id_documentType = romaneo.Document.id_documentType,
				number = romaneo.Document.number,
				state = romaneo.Document.DocumentState.name,
				documentType = romaneo.Document.DocumentType.name,
				dateTimeEmision = romaneo.Document.emissionDate,//.ToString("d"),
				idSate = romaneo.Document.id_documentState,
				reference = romaneo.Document.reference,
				idAnalist = romaneo.idAnalist,
				idWeigher = romaneo.idWeigher,
				dateTimeReception = romaneo.ResultProdLotRomaneo.dateTimeReception.ToString("dd/MM/yyyy hh:mm:ss"),
				numberLot = romaneo.ResultProdLotRomaneo.numberLot,
				numberLotSequential = romaneo.ResultProdLotRomaneo.numberLotSequential,
				nameProvider = romaneo.ResultProdLotRomaneo.nameProvider,
				nameProviderShrimp = romaneo.ResultProdLotRomaneo.nameProviderShrimp,
				INPnumber = romaneo.ResultProdLotRomaneo.INPnumber,
				namePool = romaneo.ResultProdLotRomaneo.namePool,
				nameWarehouse = romaneo.ResultProdLotRomaneo.nameWarehouseItem,
				nameWarehouseLocation = romaneo.ResultProdLotRomaneo.nameWarehouseLocationItem,
				nameItem = romaneo.ResultProdLotRomaneo.nameItem,
				codeProcessType = romaneo.ResultProdLotRomaneo.codeProcessType,
				poundsTrash = romaneo.PoundsGarbage,
				drawersNumber = romaneo.gavetaNumber,
				totalPoundsGrossWeight = romaneo.TotalPoundsWeigthGross,
				percentTara = romaneo.porcTara,
				totalPoundsNetWeight = romaneo.TotalPoundsWeigthNet,
				poundsRemitted = romaneo.ResultProdLotRomaneo.quantityRemitted,
				idTypeRomaneo = romaneo.idRomaneoType,
				idProductionLotReception = romaneo.ResultProdLotRomaneo.idProductionLot,
				codeTypeProcess = romaneo.ResultProdLotRomaneo.codeProcessType,
				idSize = romaneo.idItemSize,
				RomaneoDetails = new List<RomaneoDetailDTO>()
			};

			foreach (var item in romaneo.RomaneoDetail.ToList())
			{
				romaneoDto.RomaneoDetails.Add(new RomaneoDetailDTO
				{
					id = item.id,
					drawerNumber = item.orderDetail,
					grossWeight = item.grossWeight,
					um = item.MetricUnit.code,
					poundsTrash = item.poundsGarbage
				});
			}
			ViewBag.drawersNumber = romaneoDto.drawersNumber;
			ViewBag.idTypeRomaneo = romaneoDto.idTypeRomaneo;
			ViewBag.percentTara = romaneoDto.percentTara;

			return romaneoDto;
		}

		private void BuildViewDataIndex()
		{
			BuildComboBoxState();
			BuildComboBoxTypeRomaneoIndex();
		}

		private void BuildViewDataEdit()
		{
			BuildComboBoxAnalist();
			BuildComboBoxWeigher();
		}

		[HttpPost]
		public ActionResult Edit(int id = 0, int idProductionLotReception = 0, bool enabled = true)
		{
			BuildViewDataEdit();

			var codeTypeRomaneo = "";
			var model = new RomaneoDTO();
			Romaneo romaneo = db.Romaneo.FirstOrDefault(d => d.id == id);
			if (romaneo == null)
			{
				model = Create(idProductionLotReception);
				SetRomaneoDTO(model);
				codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeRomaneo)?.code;
				BuildComboBoxSizeRomaneo(codeTypeRomaneo);

				BuilViewBag(enabled);
				BuildComboBoxTypeRomaneo(model.id, model.codeProcessType);
				return PartialView(model);
			}

			model = ConvertToDto(romaneo);
			SetRomaneoDTO(model);
			codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeRomaneo)?.code;
			BuildComboBoxSizeRomaneo(codeTypeRomaneo);
			BuilViewBag(enabled);
			BuildComboBoxTypeRomaneo(model.id, model.codeTypeProcess);

			return PartialView(model);
		}

		private void BuilViewBag(bool enabled)
		{
			var romaneoDTO = GetRomaneoDTO();
			ViewBag.enabled = enabled;
			ViewBag.canEdit = !enabled &&
							  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == romaneoDTO.idSate)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == romaneoDTO.idSate)
									 ?.code.Equals("01") ?? false) && romaneoDTO.id != 0;
			ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == romaneoDTO.idSate)
									 ?.code.Equals("03") ?? false) && !enabled;
			ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == romaneoDTO.idSate)
									  ?.code.Equals("01") ?? false) && romaneoDTO.id != 0;
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewDetails(int? idTypeRomaneo, bool? enabled, int? drawersNumber = 0, decimal? percentTara = 0)
		{
			var codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == idTypeRomaneo)?.code;
			var umAux = codeTypeRomaneo == "ENT" ? "Kg" : "Lbs";
			var romaneoDetails = GetRomaneoDTO().RomaneoDetails;
			var countAux = romaneoDetails.Count;
			if (countAux > drawersNumber)
			{
				for (var i = drawersNumber + 1; i <= countAux; i++)
				{
					var romaneoDetail = romaneoDetails.FirstOrDefault(fod => fod.drawerNumber == i);
					romaneoDetails.Remove(romaneoDetail);
				}
			}
			else
			{
				if (countAux < drawersNumber)
				{
					for (var i = countAux + 1; i <= drawersNumber; i++)
					{
						var romaneoDetail = romaneoDetails.FirstOrDefault(fod => fod.drawerNumber == i);
						if (romaneoDetail == null)
						{
							romaneoDetails.Add(new RomaneoDetailDTO
							{
								id = romaneoDetails == null || romaneoDetails.Count() == 0 ? 1 : romaneoDetails.Max(m => m.id) + 1,
								drawerNumber = i,
								grossWeight = 0,
								poundsTrash = 0,
								um = umAux
							});

						}


					}
				}
			}

			foreach (var item in romaneoDetails.ToList())
			{
				item.um = umAux;
			}

			ViewBag.drawersNumber = drawersNumber;
			ViewBag.idTypeRomaneo = idTypeRomaneo;
			ViewBag.codeTypeRomaneo = codeTypeRomaneo;
			ViewBag.percentTara = percentTara;
			ViewBag.enabled = enabled;


			return PartialView("_GridViewDetails", romaneoDetails);
		}

		#region Combobox
		private void BuildComboBoxState()
		{
			ViewData["Estados"] = db.DocumentState
				.Where(e => e.isActive 
					&& e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoRomaneo))
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

		private void BuildComboBoxTypeRomaneoIndex()
		{
			//var tiposRomaneoIndex = new List<SelectListItem>();
			//tiposRomaneoIndex.Add(new SelectListItem {
			//    Text = "ENTERO",
			//    Value = "1",
			//});
			//tiposRomaneoIndex.Add(new SelectListItem
			//{
			//    Text = "RECHAZO",
			//    Value = "2",
			//});
			//tiposRomaneoIndex.Add(new SelectListItem
			//{
			//    Text = "COLA",
			//    Value = "3",
			//});
			//ViewData["TiposRomaneoIndex"] = tiposRomaneoIndex;

			ViewData["TiposRomaneoIndex"] = db.tbsysCatalogueDetail.Where(e => e.tbsysCatalogue.code == "MPLTR") //MPLTR: Tipo de Romaneo
			   .Select(s => new SelectListItem
			   {
				   Text = s.name,
				   Value = s.id.ToString(),
			   }).ToList();

		}
		public ActionResult ComboBoxTypeRomaneoIndex()
		{
			BuildComboBoxTypeRomaneoIndex();
			return PartialView("_ComboBoxTypeRomaneoIndex");
		}

		private void BuildComboBoxWeigher()
		{
			ViewData["Pesadores"] = db.Employee.Where(e => e.Person.isActive == true)
				.Select(s => new SelectListItem
				{
					Text = s.Person.fullname_businessName,
					Value = s.id.ToString(),
				}).ToList();
		}

		public ActionResult ComboBoxWeigher()
		{
			BuildComboBoxWeigher();
			return PartialView("_ComboBoxWeigher");
		}

		private void BuildComboBoxAnalist()
		{
			ViewData["Analistas"] = db.Employee.Where(e => e.Person.isActive == true)
				.Select(s => new SelectListItem
				{
					Text = s.Person.fullname_businessName,
					Value = s.id.ToString(),
				}).ToList();
		}

		public ActionResult ComboBoxAnalist()
		{
			BuildComboBoxAnalist();
			return PartialView("_ComboBoxAnalist");
		}

		private void BuildComboBoxTypeRomaneo(int id = 0, string codeTypeProcess = "ENT")
		{

			var tiposRomaneoAux = db.tbsysCatalogueDetail.Where(e => e.tbsysCatalogue.code == "MPLTR" && (codeTypeProcess == "ENT" || (codeTypeProcess != "ENT" && e.code == "COL"))) //MPLTR: Tipo de Romaneo
			   .Select(s => new SelectListItem
			   {
				   Text = s.name,
				   Value = s.id.ToString(),
				   Selected = (id == 0 && ((codeTypeProcess == "ENT" && s.code == "REC") || (codeTypeProcess != "ENT" && s.code == "COL")))
			   }).ToList();
			ViewData["TiposRomaneo"] = tiposRomaneoAux;

		}
		public ActionResult ComboBoxTypeRomaneo()
		{
			var romaneoDTO = GetRomaneoDTO();
			BuildComboBoxTypeRomaneo(romaneoDTO.id, romaneoDTO.codeTypeProcess);
			ViewBag.enabled = true;
			return PartialView("_ComboBoxTypeRomaneo");
		}

		private void BuildComboBoxSizeRomaneo(string codeTypeRomaneo)
		{
			ViewBag.enabledSize = (codeTypeRomaneo == "ENT");
			if (codeTypeRomaneo == "ENT")
			{
				ViewData["Tallas"] = db.ItemSizeProcessPLOrder.Where(e => e.ProcessType.code == "ENT" && e.ItemSize.isActive == true)
				 .Select(s => new SelectListItem
				 {
					 Text = s.ItemSize.name,
					 Value = s.ItemSize.id.ToString(),
				 }).ToList();
			}
			else
			{
				ViewData["Tallas"] = new List<SelectListItem>();
			}

		}

		public ActionResult ComboBoxSize(int? idTypeRomaneo)
		{
			var codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == idTypeRomaneo)?.code;
			BuildComboBoxSizeRomaneo(codeTypeRomaneo);
			ViewBag.enabled = true;
			return PartialView("_ComboBoxSize");
		}

		#endregion

		[HttpPost]
		public JsonResult Save(string jsonRomaneo)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						JToken token = JsonConvert.DeserializeObject<JToken>(jsonRomaneo);

						var id_reception = token.Value<int>("id_reception");
						var reception = db.ResultProdLotRomaneo.FirstOrDefault(r =>
							r.idProductionLot == id_reception);
						if (reception == null)
							throw new Exception("Recepción Lote no encontrado");


						var newObject = false;
						var id = token.Value<int>("id");

						var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoRomaneo));
						var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

						var romaneo = db.Romaneo.FirstOrDefault(d => d.id == id);
						if (romaneo == null)
						{
							newObject = true;

							var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
								? ActiveUser.EmissionPoint.First().id
								: 0;
							if (id_emissionPoint == 0)
								throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

							romaneo = new Romaneo
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

							documentType.currentNumber++;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;



						}

						var romaneoAux = db.Romaneo.AsEnumerable().FirstOrDefault(r => r.id != romaneo.id &&
											   r.idResultProdLotRomaneo == id_reception && r.idRomaneoType == token.Value<int>("idTypeRomaneo") &&
											   (r.Document.DocumentState.code == "01" || r.Document.DocumentState.code == "03"));
						if (romaneoAux != null)
							throw new Exception("Existe la Recepción del Lote: " + romaneoAux.ResultProdLotRomaneo.numberLot +
												" con el mismo tipo de Romaneo: " + romaneoAux.tbsysCatalogueDetail.name +
												" en el Romaneo con número: " + romaneoAux.Document.number +
												" con estado: " + romaneoAux.Document.DocumentState.name + ".");

						romaneo.Document.id_documentState = documentState.id;
						romaneo.Document.id_userUpdate = ActiveUser.id;
						romaneo.Document.dateUpdate = DateTime.Now;
						romaneo.Document.reference = token.Value<string>("reference");
						romaneo.Document.description = token.Value<string>("description");

						romaneo.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
						romaneo.idWeigher = token.Value<int>("idWeigher");
						romaneo.idAnalist = token.Value<int>("idAnalist");
						romaneo.idRomaneoType = token.Value<int>("idTypeRomaneo");



						romaneo.PoundsGarbage = token.Value<decimal>("poundsTrash");
						romaneo.gavetaNumber = token.Value<int>("drawersNumber");
						romaneo.TotalPoundsWeigthGross = token.Value<decimal>("totalPoundsGrossWeight");
						romaneo.porcTara = token.Value<decimal>("percentTara");
						romaneo.TotalPoundsWeigthNet = token.Value<decimal>("totalPoundsNetWeight");
						romaneo.idItemSize = token.Value<int?>("idSize");
						romaneo.idResultProdLotRomaneo = id_reception;

						//Details
						var lastDetails = db.RomaneoDetail.Where(d => d.idRomaneo == romaneo.id);
						foreach (var detail in lastDetails)
						{
							db.RomaneoDetail.Remove(detail);
							db.RomaneoDetail.Attach(detail);
							db.Entry(detail).State = EntityState.Deleted;
						}

						var newDetails = token.Value<JArray>("romaneoDetails");
						foreach (var detail in newDetails)
						{
							romaneo.RomaneoDetail.Add(new RomaneoDetail
							{
								orderDetail = detail.Value<int>("drawerNumber"),
								grossWeight = detail.Value<int>("grossWeight"),
								idMetricUnit = db.MetricUnit.AsEnumerable().FirstOrDefault(fod => fod.code == detail.Value<string>("um"))?.id ?? 0,
								poundsGarbage = detail.Value<int>("poundsTrash")
							});
						}

						if (newObject)
						{
							db.Romaneo.Add(romaneo);
							db.Entry(romaneo).State = EntityState.Added;
						}
						else
						{
							db.Romaneo.Attach(romaneo);
							db.Entry(romaneo).State = EntityState.Modified;
						}

						db.SaveChanges();

						trans.Commit();

						result.Data = romaneo.id.ToString();

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
						result.Data = ApproveRomaneo(id).name;
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

		private DocumentState ApproveRomaneo(int id_romaneo)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var romaneo = db.Romaneo.FirstOrDefault(p => p.id == id_romaneo);
					if (romaneo != null)
					{
						if (!romaneo.RomaneoDetail.All(d => d.grossWeight > 0))
						{
							throw new Exception("No puede aprobar si al menos un valor en la culumna Peso Bruto es menor o igual a cero");
						}
						if (!romaneo.RomaneoDetail.All(d => d.poundsGarbage >= 0))
						{
							throw new Exception("No puede aprobar si al menos un valor en la culumna Libras de basura es menor a cero");
						}

						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
						if (aprovedState == null)
							return null;

						romaneo.Document.id_documentState = aprovedState.id;
						romaneo.Document.authorizationDate = DateTime.Now;

						db.Romaneo.Attach(romaneo);
						db.Entry(romaneo).State = EntityState.Modified;
						db.SaveChanges();

						this.RecalcularTotalesLiquidacion(db, romaneo.idResultProdLotRomaneo);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return romaneo.Document.DocumentState;
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
						result.Data = ReverseRomaneo(id).name;
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

		private DocumentState ReverseRomaneo(int id_romaneo)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var romaneo = db.Romaneo.FirstOrDefault(p => p.id == id_romaneo);
					if (romaneo != null)
					{
						if (!(romaneo.ResultProdLotRomaneo.ProductionLot.ProductionLotState.code == "01" ||
							  romaneo.ResultProdLotRomaneo.ProductionLot.ProductionLotState.code == "02"))//01: PENDIENTE DE RECEPCION o 02: RECEPCIONADO
						{
							throw new Exception("No se puede reversar porque la recepción del lote esta en estado: " + romaneo.ResultProdLotRomaneo.ProductionLot.ProductionLotState.name);
						}

						var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
						if (reverseState == null)
							return

						romaneo.Document.DocumentState;
						romaneo.Document.id_documentState = reverseState.id;
						romaneo.Document.authorizationDate = DateTime.Now;

						db.Romaneo.Attach(romaneo);
						db.Entry(romaneo).State = EntityState.Modified;
						db.SaveChanges();

						this.RecalcularTotalesLiquidacion(db, romaneo.idResultProdLotRomaneo);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return romaneo.Document.DocumentState;
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
						result.Data = AnnulRomaneo(id).name;
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

		private DocumentState AnnulRomaneo(int id_romaneo)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var romaneo = db.Romaneo.FirstOrDefault(p => p.id == id_romaneo);
					if (romaneo != null)
					{
						var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
						if (annulState == null)
							return

						romaneo.Document.DocumentState;
						romaneo.Document.id_documentState = annulState.id;
						romaneo.Document.authorizationDate = DateTime.Now;

						db.Romaneo.Attach(romaneo);
						db.Entry(romaneo).State = EntityState.Modified;
						db.SaveChanges();

						this.RecalcularTotalesLiquidacion(db, romaneo.idResultProdLotRomaneo);

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return romaneo.Document.DocumentState;
				}
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			var index = GetRomaneoResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

			var result = new
			{
				maximunPages = GetRomaneoResultConsultDTO().Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			var element = GetRomaneoResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
			var romaneo = db.Romaneo.FirstOrDefault(d => d.id == element.id);
			if (romaneo == null)
				return PartialView("Edit", new RomaneoDTO());

			BuildViewDataEdit();
			var model = ConvertToDto(romaneo);
			SetRomaneoDTO(model);
			var codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeRomaneo)?.code;
			BuildComboBoxSizeRomaneo(codeTypeRomaneo);
			BuilViewBag(false);
			BuildComboBoxTypeRomaneo(model.id, model.codeTypeProcess);

			return PartialView("Edit", model);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult UpdateRow(int drawerNumberValue, int grossWeightValue, int poundsTrashValue)
		{
			var romaneoDetail = GetRomaneoDTO().RomaneoDetails?.FirstOrDefault(fod => fod.drawerNumber == drawerNumberValue);
			if (romaneoDetail != null)
			{
				romaneoDetail.grossWeight = grossWeightValue;
				romaneoDetail.poundsTrash = poundsTrashValue;
			}

			var result = new
			{
				message = "OK",
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult GetTypeRomaneo(int? idTypeRomaneo)
		{
			var codeTypeRomaneo = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == idTypeRomaneo)?.code;

			var result = new
			{
				message = "OK",
				enabledSize = (codeTypeRomaneo == "ENT"),
				codeTypeRomaneo
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private void RecalcularTotalesLiquidacion(DBContext db, int idProductionLot)
		{
			// Recuperamos la cabecera del lote de producción
			var productionLot = db.ProductionLot
				.FirstOrDefault(p => p.id == idProductionLot);

			if (productionLot == null)
			{
				return;
			}

			// Recuperamos los registros de romaneo aprobados
			var romaneos = db.Romaneo
				.Where(r => r.idResultProdLotRomaneo == idProductionLot && r.Document.DocumentState.code == "03")
				.Include(r => r.tbsysCatalogueDetail)
				.ToList();

			if (romaneos.Count == 0)
			{
				// Sin romaneos aprobados, todo es cero...
				productionLot.wholeLeftover = 0m;
				productionLot.wholeGarbagePounds = 0m;

				productionLot.tailLeftOver = 0m;
				productionLot.poundsGarbageTail = 0m;
			}
			else
			{
				// Determinamos el tipo de proceso actual
				var processCode = db.ResultProdLotRomaneo
					.FirstOrDefault(r => r.idProductionLot == idProductionLot)?
					.codeProcessType;

				// Realizamos el cálculo según el tipo de proceso
				if (processCode == "ENT")
				{
					// Tipo de proceso es entero...
					var totalEntero = romaneos
						.Where(r => r.tbsysCatalogueDetail.code == "REC")
						.GroupBy(r => 1)
						.Select(g => new
						{
							Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
							Basura = g.Sum(r => r.PoundsGarbage),
						})
						.FirstOrDefault();

					var totalCola = romaneos
						.Where(r => r.tbsysCatalogueDetail.code == "ENT" || r.tbsysCatalogueDetail.code == "COL")
						.GroupBy(r => 1)
						.Select(g => new
						{
							Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
							Basura = g.Sum(r => r.PoundsGarbage),
						})
						.FirstOrDefault();

					productionLot.wholeLeftover = totalEntero?.Sobrante ?? 0m;
					productionLot.wholeGarbagePounds = totalEntero?.Basura ?? 0m;

					productionLot.tailLeftOver = totalCola?.Sobrante ?? 0m;
					productionLot.poundsGarbageTail = totalCola?.Basura ?? 0m;
				}
				else
				{
					// Tipo de proceso NO es ENTERO
					var totalCola = romaneos
						.Where(r => r.tbsysCatalogueDetail.code == "COL")
						.GroupBy(r => 1)
						.Select(g => new
						{
							Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
							Basura = g.Sum(r => r.PoundsGarbage),
						})
						.FirstOrDefault();

					productionLot.wholeLeftover = 0m;
					productionLot.wholeGarbagePounds = 0m;

					productionLot.tailLeftOver = totalCola?.Sobrante ?? 0m;
					productionLot.poundsGarbageTail = totalCola?.Basura ?? 0m;
				}
			}

			db.ProductionLot.Attach(productionLot);
			db.Entry(productionLot).State = EntityState.Modified;
			db.SaveChanges();
		}
	}
}