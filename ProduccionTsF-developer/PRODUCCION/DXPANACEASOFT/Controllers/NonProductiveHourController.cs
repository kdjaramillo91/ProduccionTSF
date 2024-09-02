using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
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
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
	public class NonProductiveHourController : DefaultController
	{
		private const string m_TipoDocumentoNonProductiveHour = "134";

		private NonProductiveHourDTO GetNonProductiveHourDTO()
		{
			if (!(Session["NonProductiveHourDTO"] is NonProductiveHourDTO nonProductiveHour))
				nonProductiveHour = new NonProductiveHourDTO();
			return nonProductiveHour;
		}

		private List<NonProductiveHourResultConsultDTO> GetNonProductiveHourResultConsultDTO()
		{
			if (!(Session["NonProductiveHourResultConsultDTO"] is List<NonProductiveHourResultConsultDTO> nonProductiveHourResultConsult))
				nonProductiveHourResultConsult = new List<NonProductiveHourResultConsultDTO>();
			return nonProductiveHourResultConsult;
		}

		private void SetNonProductiveHourDTO(NonProductiveHourDTO nonProductiveHourDTO)
		{
			Session["NonProductiveHourDTO"] = nonProductiveHourDTO;
		}

		private void SetNonProductiveHourResultConsultDTO(List<NonProductiveHourResultConsultDTO> nonProductiveHourResultConsult)
		{
			Session["NonProductiveHourResultConsultDTO"] = nonProductiveHourResultConsult;
		}

		// GET: NonProductiveHour
		public ActionResult Index()
		{
			BuildViewDataIndex();
			return View();
		}

		[HttpPost]
		public ActionResult SearchResult(NonProductiveHourConsultDTO consult)
		{
			var result = GetListsConsultDto(consult);
			SetNonProductiveHourResultConsultDTO(result);
			return PartialView("ConsultResult", result);
		}

		[HttpPost]
		public ActionResult GridViewNonProductiveHour()
		{
			return PartialView("_GridViewIndex", GetNonProductiveHourResultConsultDTO());
		}

		private List<NonProductiveHourResultConsultDTO> GetListsConsultDto(NonProductiveHourConsultDTO consulta)
		{
			using (var db = new DBContext())
			{
				var consultaAux = Session["consulta"] as NonProductiveHourConsultDTO;
				if (consultaAux != null && consulta.initDate == null)
				{
					consulta = consultaAux;
				}


				var consultResult = db.NonProductiveHour.Where(NonProductiveHourQueryExtensions.GetRequestByFilter(consulta));
				var query = consultResult.Select(t => new NonProductiveHourResultConsultDTO
				{
					id = t.id,
					number = t.Document.number,
					emissionDate = t.Document.emissionDate,
					turn = t.Turn.name,
					processPlant = t.MachineForProd.Person.processPlant,
					machineForProd = t.MachineForProd.name,
					id_machineForProd = t.id_machineForProd,
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
						var tempModel = new List<NonProductiveHourResultConsultDTO>();
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

		private List<NonProductiveHourPendingNewDTO> GetNonProductiveHourPendingNewDto()
		{
			//using (var db = new DBContext())
			//{
			//	//// Verificación del parámetro de NonProductiveHour
			//	//var nonProductiveHourHabilitado = db.Setting.AsEnumerable()
			//	//	.Any(s => s.code == "ROMANEO" && s.value == "1");

			//	//if (!nonProductiveHourHabilitado)
			//	//{
			//	//	return new List<NonProductiveHourPendingNewDTO>();
			//	//}

			//	// Ejecución de la consulta de pendientes
			//	var idPerson = db.User.FirstOrDefault(u => u.id == ActiveUser.id).id_employee;
			//	var idsRol = db.Person.FirstOrDefault(p => p.id == idPerson).Rol.Select(r => r.id).Distinct().ToList();
			//	var maquinas = db.MachineForProd.Where(z => idsRol.Contains(z.tbsysTypeMachineForProd.id_Rol)).Select(e => e.id).Distinct().ToList(); //seleccciono solo las ids de los machine for prod
			//	db.Database.CommandTimeout = 1200;
			//	var q = db.MachineProdOpeningDetail
			//		.Where(r => (r.MachineProdOpening.Document.DocumentState.code != ("05") &&
			//					 r.MachineProdOpening.Document.DocumentState.code != ("01"))//05: ANULADA y 01: PENDIENTE
			//																				//&& (r.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod=> fod.Document.DocumentState.code != "05") != null) //05: ANULADA
			//					&& (db.NonProductiveHour.FirstOrDefault(fod => (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(fod.Document.emissionDate)) &&
			//																 (r.MachineProdOpening.id_Turn == fod.id_turn) &&
			//																 (r.id_MachineForProd == fod.id_machineForProd) &&
			//																 (fod.Document.DocumentState.code != "05")) == null))

			//																 //) //05: ANULADA
			//		.Where(e => maquinas.Contains(e.id_MachineForProd)) //verifico que el id de la maquina se cumpla
			//		.GroupBy(g => new
			//		{
			//			emissionDate = DbFunctions.TruncateTime(g.MachineProdOpening.Document.emissionDate),
			//			id_turn = g.MachineProdOpening.id_Turn,
			//			nameTurn = g.MachineProdOpening.Turn.name,
			//			g.id_MachineForProd,
			//			machineForProd = g.MachineForProd.name,
			//			g.MachineForProd.Person.processPlant,
			//			g.Person.fullname_businessName,
			//			g.MachineProdOpening.Document.DocumentState.name,
			//			g.MachineProdOpening.Document.number

			//		})
			//		.AsEnumerable()
			//		.Select(r => new NonProductiveHourPendingNewDTO
			//		{
			//			numberMachineProdOpening = r.Key.number,
			//			emissionDate = r.Key.emissionDate.Value,
			//			emissionDateStr = r.Key.emissionDate.Value.ToString("dd-MM-yyyy"),
			//			id_turn = r.Key.id_turn,
			//			turn = r.Key.nameTurn,
			//			id_machineForProd = r.Key.id_MachineForProd,
			//			machineForProd = r.Key.machineForProd,
			//			processPlant = r.Key.processPlant,
			//			personRequire = r.Key.fullname_businessName,
			//			state = r.Key.name
			//		}).ToList();

			//	return q;

			//}

			using (var db = new DBContext())
			{
				Parametros.ParametrosNonProductiveHourPending parametros = new Parametros.ParametrosNonProductiveHourPending();
				parametros.idUsuario = ActiveUser.id;

				var parametrosBusquedaAux = new SqlParameter();
				parametrosBusquedaAux.ParameterName = "@Parametros";
				parametrosBusquedaAux.Direction = ParameterDirection.Input;
				parametrosBusquedaAux.SqlDbType = SqlDbType.NVarChar;
				var jsonAux = JsonConvert.SerializeObject(parametros);
				parametrosBusquedaAux.Value = jsonAux;

				List<NonProductiveHourPendingNewDTO> modelAux = db.Database.SqlQuery<NonProductiveHourPendingNewDTO>("exec pro_GetNonProductiveHourPendingNewDto_StoredProcedure @Parametros ", parametrosBusquedaAux).ToList();

				return modelAux;
			}
		}

		[HttpPost]
		public ActionResult PendingNew()
		{
			return View(GetNonProductiveHourPendingNewDto());
		}

		[ValidateInput(false)]
		public ActionResult GridViewPendingNew()
		{
			return PartialView("_GridViewPendingNew", GetNonProductiveHourPendingNewDto());
		}

		private NonProductiveHourDTO Create(DateTime emissionDate, int id_turn, int id_machineForProd)
		{
			using (var db = new DBContext())
			{
				var machineForProd =
					db.MachineForProd.FirstOrDefault(r => r.id == id_machineForProd);
				var turn =
					db.Turn.FirstOrDefault(r => r.id == id_turn);
				//if (machineProdOpening == null)
				//	return new NonProductiveHourDTO();

				var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoNonProductiveHour));
				var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

				var nonProductiveHourDTO = new NonProductiveHourDTO
				{
					id_turn = id_turn,
					turn = turn.name,
					timeInitTurn = turn.timeInit.Hours + ":" + turn.timeInit.Minutes,
					timeEndTurn = turn.timeEnd.Hours + ":" + turn.timeEnd.Minutes,
					id_machineForProd = id_machineForProd,
					machineForProd = machineForProd.name,
					id_personProcessPlant = machineForProd.id_personProcessPlant,
					personProcessPlant = machineForProd.Person.processPlant,
					description = "",
					number = "",//GetDocumentNumber(documentType?.id ?? 0),
					documentType = documentType?.name ?? "",
					id_documentType = documentType?.id ?? 0,
					reference = "",
					dateTimeEmision = emissionDate,
					dateTimeEmisionStr = emissionDate.ToString("dd-MM-yyyy"),
					idSate = documentState?.id ?? 0,
					state = documentState?.name ?? "",
					hoursStop = "00:00",
					hoursProduction = "00:00",
					totalHours = "00:00",
                    numPerson = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.MachineProdOpening.Document.DocumentState.code != ("05") &&
                                                                                  fod.MachineProdOpening.Document.DocumentState.code != ("01") &&
                                                                                  fod.id_MachineForProd == id_machineForProd &&
                                                                                  fod.MachineProdOpening.id_Turn == id_turn &&
                                                                             DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(emissionDate))?.numPerson ?? 0,
                    NonProductiveHourDetails = new List<NonProductiveHourDetailDTO>()
				};

				//FillNonProductiveHourDetails(nonProductiveHourDTO, emissionDate, id_turn, id_machineForProd);

				return nonProductiveHourDTO;
			}
		}

		//private void FillNonProductiveHourDetails(NonProductiveHourDTO nonProductiveHourDTO, DateTime emissionDate, int id_turn, int id_machineForProd)
		//{
		//    var machineProdOpenings = db.MachineProdOpening
		//            .Where(r => (r.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") != null) //05: ANULADA
		//                        && (DbFunctions.TruncateTime(r.Document.emissionDate) == DbFunctions.TruncateTime(emissionDate)) 
		//                        && (r.id_Turn == id_turn))
		//            .ToList();

		//    foreach (var machineProdOpeningN in machineProdOpenings)
		//    {
		//        foreach (var item in machineProdOpeningN.LiquidationCartOnCart.Where(w=> w.MachineForProd.id_personProcessPlant == id_personProcessPlant).ToList())
		//        {
		//            var pt = db.ProcessType.FirstOrDefault(fod => fod.id == item.idProccesType);

		//            nonProductiveHourDTO.NonProductiveHourDetails.Add(new NonProductiveHourDetailDTO
		//            {
		//                id = nonProductiveHourDTO.NonProductiveHourDetails.Count() > 0 ? nonProductiveHourDTO.NonProductiveHourDetails.Max(pld => pld.id) + 1 : 1,
		//                turn = machineProdOpeningN.Turn.name,
		//                numberLot = item.ProductionLot.internalNumber,
		//                process = item.MachineForProd.Person.processPlant,
		//                provider = item.ProductionLot.Provider.Person.fullname_businessName,
		//                numberLiquidationCarOnCar = item.Document.number,
		//                machineForProd = item.MachineForProd.name,
		//                cod_state = item.Document.DocumentState.code,
		//                state = item.Document.DocumentState.name,
		//                tail = pt.code.Equals("COL") ? item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsITW) : 0.00M,
		//                whole = pt.code.Equals("COL") ? 0.00M : item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsITW),
		//                total = item.LiquidationCartOnCartDetail.Sum(s => s.quantityPoundsITW),
		//            });
		//        }
		//    }

		//}

		private NonProductiveHourDTO ConvertToDto(NonProductiveHour nonProductiveHour)
		{
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            //var reception = nonProductiveHour.ResultProdLotNonProductiveHour;//.FirstOrDefault(r => r.idNonProductiveHour == nonProductiveHour.id);
            //if (reception == null)
            //    return null;

            var nonProductiveHourDto = new NonProductiveHourDTO
			{
				id = nonProductiveHour.id,
				id_documentType = nonProductiveHour.Document.id_documentType,
				documentType = nonProductiveHour.Document.DocumentType.name,
				number = nonProductiveHour.Document.number,
				reference = nonProductiveHour.Document.reference,
				description = nonProductiveHour.Document.description,
				idSate = nonProductiveHour.Document.id_documentState,
				state = nonProductiveHour.Document.DocumentState.name,
				dateTimeEmisionStr = nonProductiveHour.Document.emissionDate.ToString("dd-MM-yyyy"),
				dateTimeEmision = nonProductiveHour.Document.emissionDate,
				id_machineForProd = nonProductiveHour.id_machineForProd,
				machineForProd = nonProductiveHour.MachineForProd.name,
				id_personProcessPlant = nonProductiveHour.MachineForProd.id_personProcessPlant,
				personProcessPlant = nonProductiveHour.MachineForProd.Person.processPlant,
				id_turn = nonProductiveHour.id_turn,
				turn = nonProductiveHour.Turn.name,
				timeInitTurn = nonProductiveHour.Turn.timeInit.Hours + ":" + nonProductiveHour.Turn.timeInit.Minutes,
				timeEndTurn = nonProductiveHour.Turn.timeEnd.Hours + ":" + nonProductiveHour.Turn.timeEnd.Minutes,
				hoursStop = nonProductiveHour.hoursStop,
				hoursProduction = nonProductiveHour.hoursProduction,
				totalHours = nonProductiveHour.totalHours,
                numPerson = db.MachineProdOpeningDetail.FirstOrDefault(fod=> fod.MachineProdOpening.Document.DocumentState.code != ("05") &&
                                                                             fod.MachineProdOpening.Document.DocumentState.code != ("01") &&
                                                                             fod.id_MachineForProd == nonProductiveHour.id_machineForProd &&
                                                                             fod.MachineProdOpening.id_Turn == nonProductiveHour.id_turn &&
                                                                             DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.Document.emissionDate))?.numPerson ?? 0,
                NonProductiveHourDetails = new List<NonProductiveHourDetailDTO>()
			};

			var listaDetalles = new List<NonProductiveHourDetailDTO>();
			foreach (var r in nonProductiveHour.NonProductiveHourDetail)
			{
				string motiveLot;
				if (r.stop)
				{
                    motiveLot = db.productiveHoursReason.FirstOrDefault(fod => fod.id == r.id_motiveLot)?.name ?? "";
                }
				else
				{
					var productionLot = db.ProductionLot.FirstOrDefault(e => e.id == r.id_motiveLot);
					var requestliquidationmachine = productionLot != null
						? productionLot.ProductionProcess?.requestliquidationmachine ?? false
						: false;


                    motiveLot = solicitaMaquina == "SI" && requestliquidationmachine
                        ? ((db.ProductionLot.FirstOrDefault(fod => fod.id == r.id_motiveLot)?.number ?? "") + " | " +
							(db.ProductionLot.FirstOrDefault(fod => fod.id == r.id_motiveLot)?.internalNumber ?? ""))
                        : ((db.ProductionLot.FirstOrDefault(fod => fod.id == r.id_motiveLot)?.internalNumber ?? "") + " | " +
                            ((db.ProcessType.FirstOrDefault(fod => fod.id == r.id_processType)?.code ?? "") == "ENT" ? "ENTERO" : "COLA")) + " | " +
                            (db.Document.FirstOrDefault(fod => fod.id == r.id_RemisionGuide && fod.DocumentType.code == "08")?.number ?? "");

                }

				listaDetalles.Add(new NonProductiveHourDetailDTO
                {
                    id = r.id,
                    stop = r.stop,
                    id_motiveLotProcessTypeGeneral = r.stop ? r.id_motiveLot.ToString() : $"{r.id_motiveLot}|{r.id_processType}|{r.id_RemisionGuide}",

                    id_motiveLot = r.id_motiveLot,
                    motiveLot = motiveLot,
                    id_processType = r.id_processType,
                    startDate = r.startDate,
                    startTime = r.startTime,
                    endDate = r.endDate,
                    endTime = r.endTime,
                    totalHours = r.totalHours,
                    numPerson = r.numPerson,
                    observation = r.observation,
                    id_RemisionGuide = r.id_RemisionGuide
                });
            }
			nonProductiveHourDto.NonProductiveHourDetails = listaDetalles;
            return nonProductiveHourDto;
		}

		private void BuildViewDataIndex()
		{
			BuildComboBoxState();
			BuildComboBoxTurnIndex();
			BuildComboBoxMachineForProdIndex();
		}

		private void BuildViewDataEdit()
		{
			//BuildComboBoxAnalist();
			//BuildComboBoxWeigher();
		}

		[HttpPost]
		public ActionResult Edit(int id = 0, int id_turn = 0, DateTime? emissionDate = null, bool enabled = true, int id_machineForProd = 0)
		{
			//BuildViewDataEdit();

			var model = new NonProductiveHourDTO();
			NonProductiveHour nonProductiveHour = db.NonProductiveHour.FirstOrDefault(d => d.id == id);
			if (nonProductiveHour == null)
			{
				model = Create(emissionDate.Value, id_turn, id_machineForProd);
				SetNonProductiveHourDTO(model);

				BuilViewBag(enabled);
				return PartialView(model);
			}

			model = ConvertToDto(nonProductiveHour);
			SetNonProductiveHourDTO(model);
			BuilViewBag(enabled);

			return PartialView(model);
		}

		private void BuilViewBag(bool enabled)
		{
			var nonProductiveHourDTO = GetNonProductiveHourDTO();
			ViewBag.enabled = enabled;
            ViewBag.numPerson = nonProductiveHourDTO.numPerson;
			ViewBag.canNew = nonProductiveHourDTO.id != 0;
            ViewBag.canEdit = !enabled &&
							  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == nonProductiveHourDTO.idSate)
								   ?.code.Equals("01") ?? false);
			ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == nonProductiveHourDTO.idSate)
									 ?.code.Equals("01") ?? false) && nonProductiveHourDTO.id != 0;
			ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == nonProductiveHourDTO.idSate)
									 ?.code.Equals("03") ?? false) && !enabled;
			ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == nonProductiveHourDTO.idSate)
									  ?.code.Equals("01") ?? false) && nonProductiveHourDTO.id != 0;

			ViewBag.dateTimeEmision = nonProductiveHourDTO.dateTimeEmision;
		}

		#region GridViewDetails

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult GridViewDetails(bool? enabled)
		{
			var nonProductiveHour = GetNonProductiveHourDTO();

			ViewBag.enabled = enabled;
			ViewBag.dateTimeEmision = nonProductiveHour.dateTimeEmision;
            ViewBag.numPerson = nonProductiveHour.numPerson;

            return PartialView("_GridViewDetails", nonProductiveHour.NonProductiveHourDetails.OrderBy(ob => ob.startDate).ThenBy(tb => tb.startTime));
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailsAddNew(NonProductiveHourDetailDTO item, bool? enabled)
		{
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            var nonProductiveHour = GetNonProductiveHourDTO();
			var motiveLot = 0;
			var processType = 0;
			var guiaRemission = 0;
            var productionLot = new ProductionLot();

            var ids = item.id_motiveLotProcessTypeGeneral.Split('|');
            if (ids.Length > 1)//(item.id_motiveLotProcessTypeGeneral.Length > 1)
            {
                motiveLot = Convert.ToInt32(ids[0]);
				processType = Convert.ToInt32(ids[1]);
				guiaRemission = Convert.ToInt32(ids[2]);
                productionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == motiveLot);
            }
            else
            {
                motiveLot = Convert.ToInt32(ids[0]);
                productionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == motiveLot);
            }
            var requestliquidationmachine = productionLot != null
                        ? productionLot.ProductionProcess?.requestliquidationmachine ?? false
                        : false;
            //ModelState.IsValid = true;
            if (ModelState.IsValid)
			{
                if (solicitaMaquina == "NO" || (solicitaMaquina == "SI" && !requestliquidationmachine))
                {
                    item.id = nonProductiveHour.NonProductiveHourDetails.Count() > 0 ? nonProductiveHour.NonProductiveHourDetails.Max(ppd => ppd.id) + 1 : 1;
					//item.id_motiveLot = item.stop ? item.id_motiveLotProcessType : item.id_motiveLotProcessType / 10;
					//item.id_processType = item.stop ? null : (int?)item.id_motiveLotProcessType % 10;
					item.id_motiveLot = item.stop ? Convert.ToInt32(item.id_motiveLotProcessTypeGeneral) :  motiveLot;
					item.id_processType = item.stop ? null : (int?)processType;
					item.id_RemisionGuide = item.stop ? null : (int?)guiaRemission;
					item.motiveLot = item.stop ? db.productiveHoursReason.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.name ?? ""
										   : ((db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.internalNumber ?? "") + " | " +
										 ((db.ProcessType.FirstOrDefault(fod => fod.id == item.id_processType)?.code ?? "") == "ENT" ? "ENTERO" : "COLA")) + " | " +
										 (db.Document.FirstOrDefault(fod => fod.id == item.id_RemisionGuide && fod.DocumentType.code == "08")?.number ?? "");
					item.startTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["startTimeHours"]) ? "0" : Request.Params["startTimeHours"]),
												  int.Parse(string.IsNullOrEmpty(Request.Params["startTimeMinutes"]) ? "0" : Request.Params["startTimeMinutes"]), 0);
					item.endTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["endTimeHours"]) ? "0" : Request.Params["endTimeHours"]),
												  int.Parse(string.IsNullOrEmpty(Request.Params["endTimeMinutes"]) ? "0" : Request.Params["endTimeMinutes"]), 0);
					item.totalHours = string.IsNullOrEmpty(Request.Params["totalHoursDetail"]) ? "00:00" : Request.Params["totalHoursDetail"];
					//item.totalHours = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursHours"]) ? "0" : Request.Params["totalHoursHours"]),
					//                              int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursMinutes"]) ? "0" : Request.Params["totalHoursMinutes"]), 0);
					//item.observation = item.stop ? item.observation : item.motiveLot;
					nonProductiveHour.NonProductiveHourDetails.Add(item);

					UpdateTotals();
                }
				else
				{
                    item.id = nonProductiveHour.NonProductiveHourDetails.Count() > 0 ? nonProductiveHour.NonProductiveHourDetails.Max(ppd => ppd.id) + 1 : 1;
                    item.id_motiveLot = item.stop ? Convert.ToInt32(item.id_motiveLotProcessTypeGeneral) : motiveLot;
                    item.id_processType = null;
					item.id_RemisionGuide = null;
                    item.motiveLot = item.stop ? db.productiveHoursReason.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.name ?? ""
                                                   : ((db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.number ?? "") + " | " +
                                                 ((db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.internalNumber ?? "")));
                    item.startTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["startTimeHours"]) ? "0" : Request.Params["startTimeHours"]),
                                                  int.Parse(string.IsNullOrEmpty(Request.Params["startTimeMinutes"]) ? "0" : Request.Params["startTimeMinutes"]), 0);
                    item.endTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["endTimeHours"]) ? "0" : Request.Params["endTimeHours"]),
                                                  int.Parse(string.IsNullOrEmpty(Request.Params["endTimeMinutes"]) ? "0" : Request.Params["endTimeMinutes"]), 0);
                    item.totalHours = string.IsNullOrEmpty(Request.Params["totalHoursDetail"]) ? "00:00" : Request.Params["totalHoursDetail"];
                    nonProductiveHour.NonProductiveHourDetails.Add(item);

                    UpdateTotals();
                }
            }
			else
			{
				foreach (var modelState in this.ModelState.Values)
				{
					if (modelState.Errors.Count > 0)
					{
						ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
						break;
					}
				}
			}

			ViewBag.enabled = bool.Parse(string.IsNullOrEmpty(Request.Params["enabledCurrent"]) ? "true" : Request.Params["enabledCurrent"]);
			ViewBag.dateTimeEmision = nonProductiveHour.dateTimeEmision;
            ViewBag.numPerson = nonProductiveHour.numPerson;

            return PartialView("_GridViewDetails", nonProductiveHour.NonProductiveHourDetails.OrderBy(ob => ob.startDate).ThenBy(tb => tb.startTime));
		}

		private void UpdateTotals()
		{
			var nonProductiveHour = GetNonProductiveHourDTO();
			var hoursStopH = 0;
			var hoursStopM = 0;
			var hoursProductionH = 0;
			var hoursProductionM = 0;
			var totalHoursH = 0;
			var totalHoursM = 0;

			string[] words;

			foreach (var item in nonProductiveHour.NonProductiveHourDetails)
			{
				words = item.totalHours.Split(':');
				var hoursAux = int.Parse(words[0]);
				var minutesAux = int.Parse(words[1]);

				if (item.stop)
				{
					hoursStopH += hoursAux;//item.totalHours.Hours;
					hoursStopM += minutesAux;//item.totalHours.Minutes;
				}
				else
				{
					hoursProductionH += hoursAux;
					hoursProductionM += minutesAux;
				}
				totalHoursH += hoursAux;
				totalHoursM += minutesAux;
			}

			nonProductiveHour.hoursStop = (hoursStopH + (hoursStopM / 60)).ToString("#00") + ":" + (hoursStopM % 60).ToString("00");//new TimeSpan(hoursStopH + (hoursStopM / 60), hoursStopM % 60, 0);
			nonProductiveHour.hoursProduction = (hoursProductionH + (hoursProductionM / 60)).ToString("#00") + ":" + (hoursProductionM % 60).ToString("00");//new TimeSpan(hoursProductionH + (hoursProductionM / 60), hoursProductionM % 60, 0);
			nonProductiveHour.totalHours = (totalHoursH + (totalHoursM / 60)).ToString("#00") + ":" + (totalHoursM % 60).ToString("00"); //new TimeSpan(totalHoursH + (totalHoursM / 60), totalHoursM % 60, 0);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailsUpdate(NonProductiveHourDetailDTO item, bool? enabled)
		{
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            var nonProductiveHour = GetNonProductiveHourDTO();

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = nonProductiveHour.NonProductiveHourDetails.FirstOrDefault(it => it.id == item.id);
		
					var motiveLot = 0;
					var processType = 0;
					var guiaRemission = 0;
					var productionLot = new ProductionLot();

                    var ids = item.id_motiveLotProcessTypeGeneral.Split('|');
					if (ids.Length > 1)//(item.id_motiveLotProcessTypeGeneral.Length > 1)
					{
						motiveLot = Convert.ToInt32(ids[0]);
						processType = Convert.ToInt32(ids[1]);
						guiaRemission = Convert.ToInt32(ids[2]);
                        productionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == motiveLot);
                    }
					else
					{
                        motiveLot = Convert.ToInt32(ids[0]);
                        productionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == motiveLot);
                    }

                    var requestliquidationmachine = productionLot != null
                        ? productionLot.ProductionProcess?.requestliquidationmachine ?? false
                        : false;

                    if (modelItem != null)
					{
                        if (solicitaMaquina == "NO" || (solicitaMaquina == "SI" && !requestliquidationmachine))
                        {
                            modelItem.stop = item.stop;
							//modelItem.id_motiveLot = item.stop ? item.id_motiveLotProcessType : item.id_motiveLotProcessType / 10;
							modelItem.id_motiveLot = item.stop ? Convert.ToInt32(item.id_motiveLotProcessTypeGeneral) :  motiveLot;
							modelItem.id_processType = item.stop ? null : (int?)processType;
							modelItem.id_RemisionGuide = item.stop ? null : (int?)guiaRemission;
							//modelItem.id_processType = item.stop ? null : (int?)item.id_motiveLotProcessType % 10;
							modelItem.motiveLot = item.stop ? db.productiveHoursReason.FirstOrDefault(fod => fod.id == modelItem.id_motiveLot)?.name ?? ""
												   : ((db.ProductionLot.FirstOrDefault(fod => fod.id == modelItem.id_motiveLot)?.internalNumber ?? "") + " | " +
												 ((db.ProcessType.FirstOrDefault(fod => fod.id == modelItem.id_processType)?.code ?? "") == "ENT" ? "ENTERO" : "COLA")) + " | " +
												 (db.Document.FirstOrDefault(fod => fod.id == modelItem.id_RemisionGuide && fod.DocumentType.code == "08")?.number ?? "");
							//modelItem.id_motiveLot = item.id_motiveLot;
							//modelItem.motiveLot = item.stop ? db.productiveHoursReason.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.name ?? ""
							//                                : db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_motiveLot)?.internalNumber ?? "";
							modelItem.startDate = item.startDate;
							//modelItem.startTime = item.startTime;
							modelItem.endDate = item.endDate;
							//modelItem.endTime = item.endTime;
							//modelItem.totalHours = item.totalHours;
							modelItem.startTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["startTimeHours"]) ? "0" : Request.Params["startTimeHours"]),
												  int.Parse(string.IsNullOrEmpty(Request.Params["startTimeMinutes"]) ? "0" : Request.Params["startTimeMinutes"]), 0);
							modelItem.endTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["endTimeHours"]) ? "0" : Request.Params["endTimeHours"]),
														  int.Parse(string.IsNullOrEmpty(Request.Params["endTimeMinutes"]) ? "0" : Request.Params["endTimeMinutes"]), 0);
							modelItem.totalHours = string.IsNullOrEmpty(Request.Params["totalHoursDetail"]) ? "00:00" : Request.Params["totalHoursDetail"];
							//modelItem.totalHours = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursHours"]) ? "0" : Request.Params["totalHoursHours"]),
							//                              int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursMinutes"]) ? "0" : Request.Params["totalHoursMinutes"]), 0);
							modelItem.numPerson = item.numPerson;// item.stop ? item.observation ?? ""
							modelItem.observation = item.observation;// item.stop ? item.observation ?? ""
																	 //: modelItem.motiveLot;
							this.UpdateModel(modelItem);
							UpdateTotals();
                        }
						else 
						{
                            modelItem.stop = item.stop;
                            modelItem.id_motiveLot = item.stop ? Convert.ToInt32(item.id_motiveLotProcessTypeGeneral) : motiveLot;
                            modelItem.id_processType = null;
                            modelItem.id_RemisionGuide = null;

                            modelItem.motiveLot = item.stop ? db.productiveHoursReason.FirstOrDefault(fod => fod.id == modelItem.id_motiveLot)?.name ?? ""
                                                   : ((db.ProductionLot.FirstOrDefault(fod => fod.id == modelItem.id_motiveLot)?.number ?? "") + " | " +
                                                 ((db.ProductionLot.FirstOrDefault(fod => fod.id == modelItem.id_motiveLot)?.internalNumber ?? "")));
                            modelItem.startDate = item.startDate;
                            modelItem.endDate = item.endDate;
                            modelItem.startTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["startTimeHours"]) ? "0" : Request.Params["startTimeHours"]),
                                                  int.Parse(string.IsNullOrEmpty(Request.Params["startTimeMinutes"]) ? "0" : Request.Params["startTimeMinutes"]), 0);
                            modelItem.endTime = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["endTimeHours"]) ? "0" : Request.Params["endTimeHours"]),
                                                          int.Parse(string.IsNullOrEmpty(Request.Params["endTimeMinutes"]) ? "0" : Request.Params["endTimeMinutes"]), 0);
                            modelItem.totalHours = string.IsNullOrEmpty(Request.Params["totalHoursDetail"]) ? "00:00" : Request.Params["totalHoursDetail"];

                            modelItem.numPerson = item.numPerson;// item.stop ? item.observation ?? ""
                            modelItem.observation = item.observation;// item.stop ? item.observation ?? ""

                            this.UpdateModel(modelItem);
                            UpdateTotals();
                        }
                    }
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
			{
				foreach (var modelState in this.ModelState.Values)
				{
					if (modelState.Errors.Count > 0)
					{
						ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
						break;
					}
				}
			}

			ViewBag.enabled = bool.Parse(string.IsNullOrEmpty(Request.Params["enabledCurrent"]) ? "true" : Request.Params["enabledCurrent"]);
			ViewBag.dateTimeEmision = nonProductiveHour.dateTimeEmision;
            ViewBag.numPerson = nonProductiveHour.numPerson;

            return PartialView("_GridViewDetails", nonProductiveHour.NonProductiveHourDetails.OrderBy(ob => ob.startDate).ThenBy(tb => tb.startTime));
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailsDelete(System.Int32 id)
		{
			var nonProductiveHour = GetNonProductiveHourDTO();

			try
			{
				var modelItem = nonProductiveHour.NonProductiveHourDetails.FirstOrDefault(it => it.id == id);
				if (modelItem != null)
					nonProductiveHour.NonProductiveHourDetails.Remove(modelItem);
				UpdateTotals();


			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			//ViewBag.enabled = bool.Parse(string.IsNullOrEmpty(Request.Params["enabledCurrent"]) ? "true" : Request.Params["enabledCurrent"]);
			ViewBag.dateTimeEmision = nonProductiveHour.dateTimeEmision;

			return PartialView("_GridViewDetails", nonProductiveHour.NonProductiveHourDetails.OrderBy(ob => ob.startDate).ThenBy(tb => tb.startTime));
		}

		#endregion

		#region Combobox
		private void BuildComboBoxState()
		{
			ViewData["Estados"] = db.DocumentState
				.Where(e => e.isActive
					&& e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoNonProductiveHour))
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

		[HttpPost]
		public ActionResult ComboBoxMotivoLoteEdit(bool? stopCurrent)
		{
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            List<SelectListItem> model = new List<SelectListItem>();
			if (stopCurrent.Value)
			{
				model = db.productiveHoursReason.Where(w => w.isActive)
					.Select(s => new SelectListItem
					{
						Text = s.name,
						Value = s.id.ToString()
					}).ToList();
			}
			else
			{
				var nonProductiveHour = GetNonProductiveHourDTO();

				//var model2 = db.LiquidationCartOnCart
				//    .Where(r => (r.Document.DocumentState.code != ("05")) &&//05: ANULADA
				//                                                            //(DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.dateTimeEmision)) &&
				//                (r.MachineProdOpening.id_Turn == nonProductiveHour.id_turn) &&
				//                (r.id_MachineForProd == nonProductiveHour.id_machineForProd)).ToList();

				var NonProductiveH = db.LiquidationCartOnCart
					.Where(r => (r.Document.DocumentState.code != ("05")) &&//05: ANULADA
								(DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.dateTimeEmision)) &&
								(r.MachineProdOpening.id_Turn == nonProductiveHour.id_turn) &&
								(r.id_MachineForProd == nonProductiveHour.id_machineForProd)).ToList();


                foreach (var modelAux in NonProductiveH) {
					var numeroLote = modelAux.ProductionLot.internalNumber;
					var idLote = modelAux.ProductionLot.id;
					var idProcessType = modelAux.idProccesType;
					var processType = (db.ProcessType.FirstOrDefault(fod => fod.id == modelAux.idProccesType).code == "ENT" ? "ENTERO" : "COLA");
					var productionLotDetails = modelAux.ProductionLot.ProductionLotDetail;
					foreach (var productionLotDetail in productionLotDetails)
					{
						var librasRemitidas = productionLotDetail.quantityRecived;
						var guiaRemissiones = productionLotDetail.ProductionLotDetailPurchaseDetail;
						librasRemitidas = Decimal.Round(librasRemitidas, 2);
						foreach (var guiaremission in guiaRemissiones) {
							var numeroguia = guiaremission.RemissionGuideDetail.RemissionGuide.Document.number;
							var idGuiaRemission = guiaremission.RemissionGuideDetail.RemissionGuide.id;
                            var avalue = $"{idLote}|{idProcessType}|{idGuiaRemission}";
                            if (model.FirstOrDefault(fod=> fod.Value == avalue) == null) {
                                model.Add(new SelectListItem
                                {
                                    Text = $"{numeroLote} | {processType} | {numeroguia} | {librasRemitidas} LBS",
                                    Value = $"{idLote}|{idProcessType}|{idGuiaRemission}"
                                    //Value = idLote.ToString() + idProcessType.ToString() + idGuiaRemission.ToString()

                                });
                            }
						}
						
					}	
				}

				if(solicitaMaquina == "SI")
                {
                    var NonProductiveHPI = db.ProductionLot
                   .Where(r => (r.ProductionLotState.code != ("09")) &&//05: ANULADA
							   (r.ProductionLotState.code == ("06") || r.ProductionLotState.code == ("10")) &&
                               (r.ProductionProcess.requestliquidationmachine == true) &&
                               (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.dateTimeEmision)) &&
                               (r.MachineProdOpening.id_Turn == nonProductiveHour.id_turn) &&
                               (r.id_MachineForProd == nonProductiveHour.id_machineForProd)).ToList();

					foreach (var modelAux in NonProductiveHPI)
					{
                        var SecTransaccional = modelAux.number;
                        var numeroLote = modelAux.internalNumber;
						var idLote = modelAux.id;
                        //var idProcessType = modelAux.idProccesType;
                        //var processType = (db.ProcessType.FirstOrDefault(fod => fod.id == modelAux.idProccesType).code == "ENT" ? "ENTERO" : "COLA");
                        var librasLiquidadas = modelAux.totalQuantityLiquidation;
                        librasLiquidadas = Decimal.Round(librasLiquidadas, 2);

                        var productionLotDetails = modelAux.ProductionLotDetail;

						var avalue = $"{idLote}";
						if (model.FirstOrDefault(fod => fod.Value == avalue) == null)
						{
							model.Add(new SelectListItem
							{
								Text = $"{SecTransaccional} | {numeroLote} | {librasLiquidadas} LBS",
								Value = $"{idLote}"
							});
						}
					}
                }
            }

			return GridViewExtension.GetComboBoxCallbackResult(p =>
			{
				//settings.Name = "id_person";
				p.ClientInstanceName = "id_motiveLotProcessTypeGeneral";
				p.Width = Unit.Percentage(100);

				p.ValueField = "Value";
				p.TextField = "Text";
				p.ValueType = typeof(string);
				//p.DropDownStyle = DropDownStyle.DropDownList;
				//p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				//p.EnableSynchronization = DefaultBoolean.False;
				//p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

				p.CallbackRouteValues = new
				{
					Controller = "NonProductiveHour",
					Action = "ComboBoxMotivoLoteEdit",
				};
				p.CallbackPageSize = 15;
				p.ClientSideEvents.BeginCallback = "MotivoLoteComboBox_BeginCallback";
				p.ClientSideEvents.EndCallback = "MotivoLoteComboBox_EndCallback";
				p.ClientSideEvents.Validation = "MotivoLoteComboBox_Validation";
				p.ClientSideEvents.SelectedIndexChanged = "MotivoLoteComboBox_SelectedIndexChanged";

				p.ValidationSettings.RequiredField.IsRequired = true;
				p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
				p.ValidationSettings.CausesValidation = true;
				//p.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
				p.ValidationSettings.ValidateOnLeave = true;
				p.ValidationSettings.SetFocusOnError = true;
				p.ValidationSettings.ErrorText = "Valor Incorrecto";

				p.ValidationSettings.EnableCustomValidation = true;
				p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

				p.BindList(model);

			});

		}

		#endregion

		[HttpPost]
		public JsonResult Save(string jsonNonProductiveHour)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var result = new ApiResult();

					try
					{
						var nonProductiveHourDTO = GetNonProductiveHourDTO();

						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == nonProductiveHourDTO.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para editar y guardar el control de horas de trabajo por máquina.");
								}
							}
						}

						#endregion


						//foreach (var item in nonProductiveHourDTO.NonProductiveHourDetails)
						//{
						//    if (item.cod_state != "04")//04: CERRADA
						//    {
						//        throw new Exception("No se puede guardar la liquidación de Turno, con detalle de liquidación de Carro por Carro con estado diferente a Cerrada");
						//    }
						//}

						JToken token = JsonConvert.DeserializeObject<JToken>(jsonNonProductiveHour);

						//var id_machineProdOpening = nonProductiveHourDTO.id_machineProdOpening;
						//var id_machineProdOpening = token.Value<int>("id_machineProdOpening");
						//                  var machineProdOpening = db.MachineProdOpening.FirstOrDefault(r =>
						//	r.id == id_machineProdOpening);
						//if (machineProdOpening == null)
						//	throw new Exception("Apertura de Turno no encontrada");


						var newObject = false;
						var id = token.Value<int>("id");

						var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoNonProductiveHour));
						var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

						var nonProductiveHour = db.NonProductiveHour.FirstOrDefault(d => d.id == id);
						if (nonProductiveHour == null)
						{
							newObject = true;

							var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
								? ActiveUser.EmissionPoint.First().id
								: 0;
							if (id_emissionPoint == 0)
								throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

							nonProductiveHour = new NonProductiveHour
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
							nonProductiveHour.id_turn = nonProductiveHourDTO.id_turn;
							nonProductiveHour.id_machineForProd = nonProductiveHourDTO.id_machineForProd;
							nonProductiveHour.emissionDate = nonProductiveHourDTO.dateTimeEmision;
							nonProductiveHour.Document.emissionDate = nonProductiveHourDTO.dateTimeEmision;//token.Value<DateTime>("dateTimeEmision");
							nonProductiveHour.id_userCreate = nonProductiveHour.Document.id_userCreate;
							nonProductiveHour.id_userUpdate = nonProductiveHour.Document.id_userCreate;

							documentType.currentNumber++;
							db.DocumentType.Attach(documentType);
							db.Entry(documentType).State = EntityState.Modified;



						}

						//var nonProductiveHourAux = db.NonProductiveHour.AsEnumerable().FirstOrDefault(r => r.id != nonProductiveHour.id &&
						//					   r.idResultProdLotNonProductiveHour == id_reception && r.idNonProductiveHourType == token.Value<int>("idTypeNonProductiveHour") &&
						//					   (r.Document.DocumentState.code == "01" || r.Document.DocumentState.code == "03"));
						//if (nonProductiveHourAux != null)
						//	throw new Exception("Existe la Recepción del Lote: " + nonProductiveHourAux.ResultProdLotNonProductiveHour.numberLot +
						//						" con el mismo tipo de NonProductiveHour: " + nonProductiveHourAux.tbsysCatalogueDetail.name +
						//						" en el NonProductiveHour con número: " + nonProductiveHourAux.Document.number +
						//						" con estado: " + nonProductiveHourAux.Document.DocumentState.name + ".");

						nonProductiveHour.Document.id_documentState = documentState.id;
						nonProductiveHour.Document.id_userUpdate = ActiveUser.id;
						nonProductiveHour.id_userUpdate = nonProductiveHour.Document.id_userUpdate;
						nonProductiveHour.Document.dateUpdate = DateTime.Now;
						nonProductiveHour.Document.reference = token.Value<string>("reference");
						nonProductiveHour.Document.description = token.Value<string>("description");
						nonProductiveHour.hoursStop = nonProductiveHourDTO.hoursStop;
						nonProductiveHour.hoursProduction = nonProductiveHourDTO.hoursProduction;
						nonProductiveHour.totalHours = nonProductiveHourDTO.totalHours;

						//nonProductiveHour.idWeigher = token.Value<int>("idWeigher");
						//nonProductiveHour.idAnalist = token.Value<int>("idAnalist");
						//nonProductiveHour.idNonProductiveHourType = token.Value<int>("idTypeNonProductiveHour");



						//nonProductiveHour.PoundsGarbage = token.Value<decimal>("poundsTrash");
						//nonProductiveHour.gavetaNumber = token.Value<int>("drawersNumber");
						//nonProductiveHour.TotalPoundsWeigthGross = token.Value<decimal>("totalPoundsGrossWeight");
						//nonProductiveHour.porcTara = token.Value<decimal>("percentTara");
						//nonProductiveHour.TotalPoundsWeigthNet = token.Value<decimal>("totalPoundsNetWeight");
						//nonProductiveHour.idItemSize = token.Value<int?>("idSize");
						//nonProductiveHour.idResultProdLotNonProductiveHour = id_reception;

						//Details
						if (nonProductiveHourDTO.NonProductiveHourDetails.Count() <= 0)
						{
							throw new Exception("No se puede guardar el control de horas de trabajo por máquina. Sin detalle.");
						}
						var lastDetails = db.NonProductiveHourDetail.Where(d => d.id_nonProductiveHour == nonProductiveHour.id);
						foreach (var detail in lastDetails)
						{
							db.NonProductiveHourDetail.Remove(detail);
							db.NonProductiveHourDetail.Attach(detail);
							db.Entry(detail).State = EntityState.Deleted;
						}

						//var newDetails = token.Value<JArray>("nonProductiveHourDetails");
						foreach (var detail in nonProductiveHourDTO.NonProductiveHourDetails)
						{
							nonProductiveHour.NonProductiveHourDetail.Add(new NonProductiveHourDetail
							{
								id_nonProductiveHour = nonProductiveHour.id,
								stop = detail.stop,
								id_motiveLot = (int)detail.id_motiveLot,
								id_processType = detail.id_processType,
								startDate = detail.startDate,
								startTime = detail.startTime,
								endDate = detail.endDate,
								endTime = detail.endTime,
								totalHours = detail.totalHours,
								numPerson = detail.numPerson,
								observation = detail.observation,
                                id_RemisionGuide = detail.id_RemisionGuide
							});
						}

						if (newObject)
						{
							db.NonProductiveHour.Add(nonProductiveHour);
							db.Entry(nonProductiveHour).State = EntityState.Added;
						}
						else
						{
							db.NonProductiveHour.Attach(nonProductiveHour);
							db.Entry(nonProductiveHour).State = EntityState.Modified;
						}

						db.SaveChanges();

						trans.Commit();

						result.Data = nonProductiveHour.id.ToString();

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
						result.Data = ApproveNonProductiveHour(id).name;
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

		private DocumentState ApproveNonProductiveHour(int id_nonProductiveHour)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var nonProductiveHour = db.NonProductiveHour.FirstOrDefault(p => p.id == id_nonProductiveHour);
					if (nonProductiveHour != null)
					{
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == nonProductiveHour.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para aprobar el control de horas de trabajo por máquina.");
								}
							}
						}

						#endregion

						var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
						if (aprovedState == null)
							return null;

						nonProductiveHour.Document.id_documentState = aprovedState.id;
						nonProductiveHour.Document.authorizationDate = DateTime.Now;

						db.NonProductiveHour.Attach(nonProductiveHour);
						db.Entry(nonProductiveHour).State = EntityState.Modified;
						db.SaveChanges();

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return nonProductiveHour.Document.DocumentState;
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
						result.Data = ReverseNonProductiveHour(id).name;
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

		private DocumentState ReverseNonProductiveHour(int id_nonProductiveHour)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var nonProductiveHour = db.NonProductiveHour.FirstOrDefault(p => p.id == id_nonProductiveHour);
					if (nonProductiveHour != null)
					{
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == nonProductiveHour.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para reversar el control de horas de trabajo por máquina.");
								}
							}
						}

						#endregion

						var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
						if (reverseState == null)
							return

						nonProductiveHour.Document.DocumentState;
						nonProductiveHour.Document.id_documentState = reverseState.id;
						nonProductiveHour.Document.authorizationDate = DateTime.Now;

						db.NonProductiveHour.Attach(nonProductiveHour);
						db.Entry(nonProductiveHour).State = EntityState.Modified;
						db.SaveChanges();

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return nonProductiveHour.Document.DocumentState;
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
						result.Data = AnnulNonProductiveHour(id).name;
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

		private DocumentState AnnulNonProductiveHour(int id_nonProductiveHour)
		{
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					var nonProductiveHour = db.NonProductiveHour.FirstOrDefault(p => p.id == id_nonProductiveHour);
					if (nonProductiveHour != null)
					{
						#region Validación Permiso

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{
								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == nonProductiveHour.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para anular el control de horas de trabajo por máquina.");
								}
							}
						}

						#endregion

						var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
						if (annulState == null)
							return

						nonProductiveHour.Document.DocumentState;
						nonProductiveHour.Document.id_documentState = annulState.id;
						nonProductiveHour.Document.authorizationDate = DateTime.Now;

						db.NonProductiveHour.Attach(nonProductiveHour);
						db.Entry(nonProductiveHour).State = EntityState.Modified;
						db.SaveChanges();

						trans.Commit();
					}
					else
					{
						throw new Exception("No se encontro el objeto seleccionado");
					}

					return nonProductiveHour.Document.DocumentState;
				}
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			var index = GetNonProductiveHourResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

			var result = new
			{
				maximunPages = GetNonProductiveHourResultConsultDTO().Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			var element = GetNonProductiveHourResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
			var nonProductiveHour = db.NonProductiveHour.FirstOrDefault(d => d.id == element.id);
			if (nonProductiveHour == null)
				return PartialView("Edit", new NonProductiveHourDTO());

			BuildViewDataEdit();
			var model = ConvertToDto(nonProductiveHour);
			SetNonProductiveHourDTO(model);
			//var codeTypeNonProductiveHour = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeNonProductiveHour)?.code;
			//BuildComboBoxSizeNonProductiveHour(codeTypeNonProductiveHour);
			BuilViewBag(false);
			//BuildComboBoxTypeNonProductiveHour(model.id, model.codeTypeProcess);

			return PartialView("Edit", model);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult GetTotales()
		{
			var nonProductiveHour = GetNonProductiveHourDTO();

			var result = new
			{
				message = "OK",
				nonProductiveHour.hoursStop,
				nonProductiveHour.hoursProduction,
				nonProductiveHour.totalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ItsRange(int? id, DateTime? initDate, DateTime? endDate)
		{
			var nonProductiveHour = GetNonProductiveHourDTO();
			var result = new
			{
				itsRange = 0,
				Error = ""

			};

			var nonProductiveHourDetailAux = nonProductiveHour.NonProductiveHourDetails.FirstOrDefault(fod => fod.id != id &&
																				((new DateTime(fod.startDate.Year, fod.startDate.Month, fod.startDate.Day, fod.startTime.Hours, fod.startTime.Minutes, 0) < initDate &&
																				 new DateTime(fod.endDate.Year, fod.endDate.Month, fod.endDate.Day, fod.endTime.Hours, fod.endTime.Minutes, 0) > initDate) ||
																				 (new DateTime(fod.startDate.Year, fod.startDate.Month, fod.startDate.Day, fod.startTime.Hours, fod.startTime.Minutes, 0) < endDate &&
																				 new DateTime(fod.endDate.Year, fod.endDate.Month, fod.endDate.Day, fod.endTime.Hours, fod.endTime.Minutes, 0) > endDate) ||
																				 (new DateTime(fod.startDate.Year, fod.startDate.Month, fod.startDate.Day, fod.startTime.Hours, fod.startTime.Minutes, 0) > initDate &&
																				 new DateTime(fod.endDate.Year, fod.endDate.Month, fod.endDate.Day, fod.endTime.Hours, fod.endTime.Minutes, 0) > initDate &&
																				 new DateTime(fod.startDate.Year, fod.startDate.Month, fod.startDate.Day, fod.startTime.Hours, fod.startTime.Minutes, 0) < endDate &&
																				 new DateTime(fod.endDate.Year, fod.endDate.Month, fod.endDate.Day, fod.endTime.Hours, fod.endTime.Minutes, 0) < endDate))
																			   );
			if (nonProductiveHourDetailAux != null)
			{
				result = new
				{
					itsRange = 1,
					Error = "- El Rango de Fecha y Hora esta incluido en un detalle ya registrado."

				};

			}

			return Json(result, JsonRequestBehavior.AllowGet);

		}

		[HttpPost, ValidateInput(false)]
		public JsonResult GetObservation(string id_motiveLotProcessTypeGeneral)
		{
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            var nonProductiveHour = GetNonProductiveHourDTO();
			var proveedor = "";
			var guiaRemission = "";
            int? motiveLot = null;
			int? idprocessType = null;
			int? idguiaRemission = null;

            var result = new
			{
				observation = ""
			};

            var ids = id_motiveLotProcessTypeGeneral.Split('|');
            if (ids.Length > 1)//(item.id_motiveLotProcessTypeGeneral.Length > 1)
            {
                motiveLot = Convert.ToInt32(ids[0]);
                idprocessType = Convert.ToInt32(ids[1]);
                idguiaRemission = Convert.ToInt32(ids[2]);
            }
            else
            {
                motiveLot = Convert.ToInt32(ids[0]);

            }
           
            var id_lot = motiveLot;
            var productionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == id_lot);
            var id_processType = idprocessType;

            var requestliquidationmachine = productionLot != null
                        ? productionLot.ProductionProcess?.requestliquidationmachine ?? false
                        : false;

            if (solicitaMaquina == "NO" || (solicitaMaquina == "SI" && !requestliquidationmachine))
            {
                //var id_processType = id_motiveLotProcessType % 10;
                var processType = db.ProcessType.FirstOrDefault(fod => fod.id == id_processType);

                var liquidationCartOnCart = db.LiquidationCartOnCart
                       .Where(r => (r.Document.DocumentState.code != ("05")) &&//05: ANULADA
                                   (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.dateTimeEmision)) &&
                                   (r.MachineProdOpening.id_Turn == nonProductiveHour.id_turn) &&
                                   (r.id_MachineForProd == nonProductiveHour.id_machineForProd))
                       .GroupBy(g => new
                       {
                           g.Document.number,
                           g.ProductionLot.internalNumber,
                           g.id_ProductionLot,
                           g.idProccesType
                       }).FirstOrDefault(fod => fod.Key.id_ProductionLot == id_lot && fod.Key.idProccesType == id_processType);

                decimal librasRemitidas = db.RemissionGuideDetail.FirstOrDefault(e => e.id_remisionGuide == idguiaRemission)
                    .ProductionLotDetailPurchaseDetail.FirstOrDefault(e => e.ProductionLotDetail?.id_productionLot == motiveLot)
                    ?.ProductionLotDetail?.quantityRecived ?? 0.00m;


                if (productionLot != null && processType != null)
                {
                    proveedor = db.Provider.FirstOrDefault(fod => fod.id == productionLot.id_provider).Person.fullname_businessName;
                    guiaRemission = db.Document.FirstOrDefault(fod => fod.id == idguiaRemission && fod.DocumentType.code == "08")?.number ?? "";


                    result = new
                    {
                        observation = "Liq No: " + liquidationCartOnCart.Key.number + " Proveedor: " + proveedor + " Proceso: " + (processType.code == "ENT" ? "ENTERO" : "COLA") +
                        " Guía: " + guiaRemission + " Lbs: " + librasRemitidas.ToString("N2")
                    };
                }
            }
			else
			{
                var _productionLot = db.ProductionLot
                   .Where(r => (r.ProductionLotState.code != ("09")) &&//05: ANULADA
                               (r.ProductionLotState.code == ("06") || r.ProductionLotState.code == ("10")) &&
                               (r.ProductionProcess.requestliquidationmachine == true) &&
                               (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(nonProductiveHour.dateTimeEmision)) &&
                               (r.MachineProdOpening.id_Turn == nonProductiveHour.id_turn) &&
                               (r.id_MachineForProd == nonProductiveHour.id_machineForProd))
                   .GroupBy(g => new
                   {
                       g.number,
                       g.internalNumber,
                       g.id,
					   g.totalQuantityLiquidation
                   }).FirstOrDefault(fod => fod.Key.id == id_lot);

				decimal librasLiquidadas = _productionLot.Key.totalQuantityLiquidation;

                if (productionLot != null)
                {

                    result = new
                    {
                        observation = "Proceso Interno No: " + _productionLot.Key.number + " Lote: " + _productionLot.Key.internalNumber + 
                        " Lbs: " + librasLiquidadas.ToString("N2")
                    };
                }


            }


			return Json(result, JsonRequestBehavior.AllowGet);
		}


		public JsonResult NonProductiveHourReport(int id, string codeReport)
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

		//[HttpPost, ValidateInput(false)]
		//public JsonResult UpdateRow(int drawerNumberValue, int grossWeightValue, int poundsTrashValue)
		//{
		//	var nonProductiveHourDetail = GetNonProductiveHourDTO().NonProductiveHourDetails?.FirstOrDefault(fod => fod.drawerNumber == drawerNumberValue);
		//	if (nonProductiveHourDetail != null)
		//	{
		//		nonProductiveHourDetail.grossWeight = grossWeightValue;
		//		nonProductiveHourDetail.poundsTrash = poundsTrashValue;
		//	}

		//	var result = new
		//	{
		//		message = "OK",
		//	};

		//	return Json(result, JsonRequestBehavior.AllowGet);
		//}

		//[HttpPost, ValidateInput(false)]
		//public JsonResult GetTypeNonProductiveHour(int? idTypeNonProductiveHour)
		//{
		//	var codeTypeNonProductiveHour = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == idTypeNonProductiveHour)?.code;

		//	var result = new
		//	{
		//		message = "OK",
		//		enabledSize = (codeTypeNonProductiveHour == "ENT"),
		//		codeTypeNonProductiveHour
		//	};

		//	return Json(result, JsonRequestBehavior.AllowGet);
		//}

		//private void RecalcularTotalesLiquidacion(DBContext db, int idProductionLot)
		//{
		//	// Recuperamos la cabecera del lote de producción
		//	var productionLot = db.ProductionLot
		//		.FirstOrDefault(p => p.id == idProductionLot);

		//	if (productionLot == null)
		//	{
		//		return;
		//	}

		//	// Recuperamos los registros de nonProductiveHour aprobados
		//	var nonProductiveHours = db.NonProductiveHour
		//		.Where(r => r.idResultProdLotNonProductiveHour == idProductionLot && r.Document.DocumentState.code == "03")
		//		.Include(r => r.tbsysCatalogueDetail)
		//		.ToList();

		//	if (nonProductiveHours.Count == 0)
		//	{
		//		// Sin nonProductiveHours aprobados, todo es cero...
		//		productionLot.wholeLeftover = 0m;
		//		productionLot.wholeGarbagePounds = 0m;

		//		productionLot.tailLeftOver = 0m;
		//		productionLot.poundsGarbageTail = 0m;
		//	}
		//	else
		//	{
		//		// Determinamos el tipo de proceso actual
		//		var processCode = db.ResultProdLotNonProductiveHour
		//			.FirstOrDefault(r => r.idProductionLot == idProductionLot)?
		//			.codeProcessType;

		//		// Realizamos el cálculo según el tipo de proceso
		//		if (processCode == "ENT")
		//		{
		//			// Tipo de proceso es entero...
		//			var totalEntero = nonProductiveHours
		//				.Where(r => r.tbsysCatalogueDetail.code == "REC")
		//				.GroupBy(r => 1)
		//				.Select(g => new
		//				{
		//					Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
		//					Basura = g.Sum(r => r.PoundsGarbage),
		//				})
		//				.FirstOrDefault();

		//			var totalCola = nonProductiveHours
		//				.Where(r => r.tbsysCatalogueDetail.code == "ENT" || r.tbsysCatalogueDetail.code == "COL")
		//				.GroupBy(r => 1)
		//				.Select(g => new
		//				{
		//					Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
		//					Basura = g.Sum(r => r.PoundsGarbage),
		//				})
		//				.FirstOrDefault();

		//			productionLot.wholeLeftover = totalEntero?.Sobrante ?? 0m;
		//			productionLot.wholeGarbagePounds = totalEntero?.Basura ?? 0m;

		//			productionLot.tailLeftOver = totalCola?.Sobrante ?? 0m;
		//			productionLot.poundsGarbageTail = totalCola?.Basura ?? 0m;
		//		}
		//		else
		//		{
		//			// Tipo de proceso NO es ENTERO
		//			var totalCola = nonProductiveHours
		//				.Where(r => r.tbsysCatalogueDetail.code == "COL")
		//				.GroupBy(r => 1)
		//				.Select(g => new
		//				{
		//					Sobrante = g.Sum(r => r.TotalPoundsWeigthNet),
		//					Basura = g.Sum(r => r.PoundsGarbage),
		//				})
		//				.FirstOrDefault();

		//			productionLot.wholeLeftover = 0m;
		//			productionLot.wholeGarbagePounds = 0m;

		//			productionLot.tailLeftOver = totalCola?.Sobrante ?? 0m;
		//			productionLot.poundsGarbageTail = totalCola?.Basura ?? 0m;
		//		}
		//	}

		//	db.ProductionLot.Attach(productionLot);
		//	db.Entry(productionLot).State = EntityState.Modified;
		//	db.SaveChanges();
		//}
	}
}