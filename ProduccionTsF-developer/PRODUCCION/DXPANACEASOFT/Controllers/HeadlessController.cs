using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
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
using System.Web.UI.WebControls;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System.Data;

namespace DXPANACEASOFT.Controllers
{
    public class HeadlessController : DefaultController
    {
        private const string m_TipoDocumentoHeadless = "144";

        private HeadlessDTO GetHeadlessDTO()
        {
            if (!(Session["HeadlessDTO"] is HeadlessDTO headless))
                headless = new HeadlessDTO();
            return headless;
        }

        private List<HeadlessResultConsultDTO> GetHeadlessResultConsultDTO()
        {
            if (!(Session["HeadlessResultConsultDTO"] is List<HeadlessResultConsultDTO> headlessResultConsult))
                headlessResultConsult = new List<HeadlessResultConsultDTO>();
            return headlessResultConsult;
        }

        private void SetHeadlessDTO(HeadlessDTO headlessDTO)
        {
            Session["HeadlessDTO"] = headlessDTO;
        }

        private void SetHeadlessResultConsultDTO(List<HeadlessResultConsultDTO> headlessResultConsult)
        {
            Session["HeadlessResultConsultDTO"] = headlessResultConsult;
        }

        // GET: Headless
        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(HeadlessConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetHeadlessResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewHeadless()
        {
            return PartialView("_GridViewIndex", GetHeadlessResultConsultDTO());
        }

        private List<HeadlessResultConsultDTO> GetListsConsultDto(HeadlessConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as HeadlessConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }


                var consultResult = db.Headless.Where(HeadlessQueryExtensions.GetRequestByFilter(consulta));

                var query = consultResult.Select(t => new HeadlessResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    emissionDate = t.Document.emissionDate,
                    turn = t.Turn.name,
                    secTransaction = t.ProductionLot.number,
                    numberLot = t.ProductionLot.internalNumber,
                    process = t.ProductionLot.Person1.processPlant,
                    provider = t.ProductionLot.Provider.Person.fullname_businessName,
                    productionUnitProvider = t.ProductionLot.ProductionUnitProvider.name,
                    productionUnitProviderPool = t.ProductionLot.ProductionUnitProviderPool.name,
                    state = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03")

                }).ToList();

                Session["consulta"] = consulta;

                return query;
            }
        }

        private static List<HeadlessPendingNewDTO> GetHeadlessPendingNewDto()
        {
            using (var db = new DBContext())
            {
                //Ejecución de la consulta de pendientes
                //var query = db.ProductionLot
                //    .Where(r => (//r.ProductionProcess.code == "REC" &&
                //                 r.Headless.FirstOrDefault(fod => fod.Document.DocumentState.code != ("05")) == null) &&//05: ANULADA y
                //                 //r.ProductionLotState.code != ("05") && //05: PENDIENTE DE CIERRE
                //                 //r.ProductionLotState.code != ("06") &&//06: CERRADO
                //                 //r.ProductionLotState.code != ("07") &&//07: PENDIENTE DE APROBACIÓN
                //                 //r.ProductionLotState.code != ("08") &&//08: APROBADO
                //                 //r.ProductionLotState.code != ("09")).ToList();//&&//09: ANULADO
                //                 r.ProductionLotState.code == ("01")).ToList();//&&//09: Pendiente

                //var tempModel = new List<ProductionLot>();
                //foreach (var item in query)
                //{
                //    if (item.ProductionLotDetail.FirstOrDefault(fod => db.SettingDetail.FirstOrDefault(fod2 => fod2.Setting.code == "LPPD" && fod2.value == fod.Item.masterCode) != null) != null)
                //    {
                //        tempModel.Add(item);
                //    }
                //}
                //query = tempModel;
                //var q = query.Select(r => new HeadlessPendingNewDTO
                //{
                //    id_productionLot = r.id,
                //    numberLot = r.internalNumber,
                //    secTransaction = r.number,
                //    //receptionDateStr = r.receptionDate.ToString("dd-MM-yyyy"),
                //    receptionDate = r.receptionDate,
                //    poundsRemitted = r.totalQuantityRecived,
                //    process = r.Person1.processPlant,
                //    provider = r.Provider.Person.fullname_businessName,
                //    productionUnitProvider = r.ProductionUnitProvider.name,
                //    productionUnitProviderPool = r.ProductionUnitProviderPool.name,
                //    state = r.ProductionLotState.name
                //}).ToList();
                //var parametrosBusquedaInventoryMoveAux = new SqlParameter();
                //parametrosBusquedaInventoryMoveAux.ParameterName = "@ParametrosBusquedaInventoryMove";
                //parametrosBusquedaInventoryMoveAux.Direction = ParameterDirection.Input;
                //parametrosBusquedaInventoryMoveAux.SqlDbType = SqlDbType.NVarChar;
                //var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaInventoryMove);
                //parametrosBusquedaInventoryMoveAux.Value = jsonAux;
                db.Database.CommandTimeout = 1200;
                List<HeadlessPendingNewDTO> modelAux = db.Database.SqlQuery<HeadlessPendingNewDTO>("exec desc_Consultar_HeadlessPendingNewDTO_StoredProcedure").ToList();
                return modelAux;

                //return q;

            }
        }

        [HttpPost]
        public ActionResult PendingNew()
        {
            return View(GetHeadlessPendingNewDto());
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNew()
        {
            return PartialView("_GridViewPendingNew", GetHeadlessPendingNewDto());
        }

        private HeadlessDTO Create(int id_productionLot)
        {
            using (var db = new DBContext())
            {
                var productionLot =
                    db.ProductionLot.FirstOrDefault(r => r.id == id_productionLot);

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoHeadless));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;

                var headlessDTO = new HeadlessDTO
                {
                    id = 0,
                    id_productionLot = id_productionLot,
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    description = "",
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    provider = productionLot.Provider1.Person.fullname_businessName,
                    productionUnitProvider = productionLot.ProductionUnitProvider.name,
                    productionUnitProviderPool = productionLot.ProductionUnitProviderPool.name,
                    numberLot = productionLot.internalNumber,
                    secTransaction = productionLot.number,
                    process = productionLot.Person1.processPlant,
                    receptionDateStr = productionLot.receptionDate.ToString("dd-MM-yyyy"),
                    receptionTimeStr = productionLot.receptionDate.ToString("HH:mm"),
                    receptionDate = productionLot.receptionDate,
                    poundsRemitted = productionLot.totalQuantityRecived,
                    id_turn = null,
                    timeInitTurn = null,
                    timeEndTurn = null,
                    id_programmer = ActiveUser.id_employee,
                    programmer = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    id_supervisor = ActiveUser.id_employee,
                    supervisor = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    dateStartTime = hoy,
                    endDateTime = hoy,
                    noOfPeople = null,
                    grammage = null,
                    id_color = null,
                    manualPerformance = null,
                    noOfDrawers = null,
                    lbsWholeSurplus = null,
                    lbsDirect = null,
                    isWholeShrimp = productionLot.ProductionLotDetail.FirstOrDefault(fod=> db.SettingDetail.FirstOrDefault(fod2=> fod2.Setting.code == "LPPD" && 
                                                                                                                                  fod2.value == fod.Item.masterCode &&
                                                                                                                                  fod2.valueAux == "ENTERO") != null) != null
                };

                return headlessDTO;
            }
        }
        private decimal Truncate2Decimals(decimal value)
        {
            return Decimal.Truncate(value * 100m) / 100m;
        }

        private HeadlessDTO ConvertToDto(Headless headless)
        {
            var headlessDto = new HeadlessDTO
            {
                id = headless.id,
                id_productionLot = headless.id_productionLot,
                id_documentType = headless.Document.DocumentType.id,
                documentType = headless.Document.DocumentType.name,
                number = headless.Document.number,
                description = headless.Document.description,
                idSate = headless.Document.id_documentState,
                state = headless.Document.DocumentState.name,
                dateTimeEmision = headless.Document.emissionDate,
                dateTimeEmisionStr = headless.Document.emissionDate.ToString("dd-MM-yyyy"),
                provider = headless.ProductionLot.Provider1.Person.fullname_businessName,
                productionUnitProvider = headless.ProductionLot.ProductionUnitProvider.name,
                productionUnitProviderPool = headless.ProductionLot.ProductionUnitProviderPool.name,
                numberLot = headless.ProductionLot.internalNumber,
                secTransaction = headless.ProductionLot.number,
                process = headless.ProductionLot.Person1.processPlant,
                receptionDateStr = headless.ProductionLot.receptionDate.ToString("dd-MM-yyyy"),
                receptionTimeStr = headless.ProductionLot.receptionDate.ToString("HH:mm"),
                receptionDate = headless.ProductionLot.receptionDate,
                poundsRemitted = headless.ProductionLot.totalQuantityRecived,
                id_turn = headless.id_turn,
                timeInitTurn = headless.Turn.timeInit.Hours + ":" + headless.Turn.timeInit.Minutes,
                timeEndTurn = headless.Turn.timeEnd.Hours + ":" + headless.Turn.timeEnd.Minutes,
                id_programmer = headless.id_programmer,
                programmer = db.Person.FirstOrDefault(fod => fod.id == headless.id_programmer).fullname_businessName,
                id_supervisor = headless.id_supervisor,
                supervisor = db.Person.FirstOrDefault(fod => fod.id == headless.id_supervisor).fullname_businessName,
                dateStartTime = headless.dateStartTime,
                endDateTime = headless.endDateTime,
                noOfPeople = headless.noOfPeople,
                grammage = headless.grammage,
                id_color = headless.id_color,
                manualPerformance = headless.manualPerformance,
                noOfDrawers = headless.noOfDrawers,
                lbsWholeSurplus = headless.lbsWholeSurplus,
                lbsDirect = headless.lbsDirect,
                isWholeShrimp = headless.ProductionLot.ProductionLotDetail.FirstOrDefault(fod => db.SettingDetail.FirstOrDefault(fod2 => fod2.Setting.code == "LPPD" &&
                                                                                                                               fod2.value == fod.Item.masterCode &&
                                                                                                                               fod2.valueAux == "ENTERO") != null) != null
            };

            return headlessDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxTurnIndex();
        }

        private void BuildViewDataEdit()
        {
            BuildComboBoxTurn();
            BuildComboBoxProgrammer();
            BuildComboBoxSupervisor();
            BuildComboBoxColor();
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, int id_productionLot = 0, bool enabled = true)
        {

            var model = new HeadlessDTO();
            Headless headless = db.Headless.FirstOrDefault(d => d.id == id);
            if (headless == null)
            {
                model = Create(id_productionLot);
                SetHeadlessDTO(model);

                BuildViewDataEdit();
                BuilViewBag(enabled);
                return PartialView(model);
            }

            model = ConvertToDto(headless);
            SetHeadlessDTO(model);
            BuildViewDataEdit();

            BuilViewBag(enabled);

            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            var headlessDTO = GetHeadlessDTO();
            ViewBag.enabled = enabled;
            ViewBag.canNew = headlessDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == headlessDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == headlessDTO.idSate)
                                     ?.code.Equals("01") ?? false) && headlessDTO.id != 0;
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == headlessDTO.idSate)
                                     ?.code.Equals("03") ?? false) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == headlessDTO.idSate)
                                      ?.code.Equals("01") ?? false) && headlessDTO.id != 0;

            //ViewBag.dateTimeEmision = headlessDTO.dateTimeEmision;
        }

        #region Combobox
        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoHeadless))
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

        private void BuildComboBoxTurn()
        {
            var headlessDTO = GetHeadlessDTO();
            ViewData["Turn"] = db.Turn.Where(e => e.isActive || e.id == headlessDTO.id_turn)
               .Select(s => new SelectListItem
               {
                   Text = s.name,
                   Value = s.id.ToString(),
               }).ToList();

        }

        public ActionResult ComboBoxTurn()
        {
            BuildComboBoxTurn();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxTurn");
        }

        private void BuildComboBoxProgrammer()
        {
            var headlessDTO = GetHeadlessDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = headlessDTO.programmer,
                Value = headlessDTO.id_programmer.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Person.Where(g => g.isActive && g.Rol.FirstOrDefault(fod=> fod.name == "PDescabezado") != null && g.id != headlessDTO.id_programmer)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);
            
            ViewData["Programmer"] = aSelectListItems;
        }

        public ActionResult ComboBoxProgrammer()
        {
            BuildComboBoxProgrammer();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxProgrammer");
        }

        private void BuildComboBoxSupervisor()
        {
            var headlessDTO = GetHeadlessDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = headlessDTO.supervisor,
                Value = headlessDTO.id_supervisor.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Person.Where(g => g.isActive && g.Rol.FirstOrDefault(fod => fod.name == "SDescabezado") != null && g.id != headlessDTO.id_supervisor)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Supervisor"] = aSelectListItems;
        }

        public ActionResult ComboBoxSupervisor()
        {
            BuildComboBoxSupervisor();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxSupervisor");
        }

        private void BuildComboBoxColor()
        {
            var headlessDTO = GetHeadlessDTO();
            ViewData["Color"] = db.ItemColor.Where(e => e.isActive || e.id == headlessDTO.id_color)
               .Select(s => new SelectListItem
               {
                   Text = s.name,
                   Value = s.id.ToString(),
               }).ToList();

        }

        public ActionResult ComboBoxColor()
        {
            BuildComboBoxColor();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxColor");
        }

        #endregion

        [HttpPost]
        public JsonResult Save(string jsonHeadless)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var headlessDTO = GetHeadlessDTO();

                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonHeadless);

                        var newObject = false;
                        var id = token.Value<int>("id");

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoHeadless));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var headless = db.Headless.FirstOrDefault(d => d.id == id);
                        if (headless == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            headless = new Headless
                            {
                                Document = new Document
                                {
                                    number = GetDocumentNumber(documentType?.id ?? 0),
                                    sequential = GetDocumentSequential(documentType?.id ?? 0),
                                    emissionDate = token.Value<DateTime>("dateTimeEmision"),
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
                        headless.Document.id_documentState = documentState.id;
                        headless.Document.id_userUpdate = ActiveUser.id;
                        headless.Document.dateUpdate = DateTime.Now;
                        headless.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
                        headless.Document.description = token.Value<string>("description");
                        //Detalle
                        headless.id_productionLot = headlessDTO.id_productionLot;//token.Value<int>("id_productionLot");
                        headless.id_turn = token.Value<int>("id_turn");
                        headless.id_programmer = token.Value<int>("id_programmer");
                        headless.id_supervisor = token.Value<int>("id_supervisor");
                        headless.dateStartTime = token.Value<DateTime>("dateStartTime");
                        headless.endDateTime = token.Value<DateTime>("endDateTime");
                        headless.noOfPeople = token.Value<int>("noOfPeople");
                        headless.grammage = token.Value<decimal>("grammage");
                        headless.id_color = token.Value<int>("id_color");
                        headless.manualPerformance = token.Value<decimal>("manualPerformance");
                        headless.noOfDrawers = token.Value<int>("noOfDrawers");
                        headless.lbsWholeSurplus = token.Value<decimal?>("lbsWholeSurplus");
                        headless.lbsDirect = token.Value<decimal?>("lbsDirect");

                        if (newObject)
                        {
                            db.Headless.Add(headless);
                            db.Entry(headless).State = EntityState.Added;
                        }
                        else
                        {
                            db.Headless.Attach(headless);
                            db.Entry(headless).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = headless.id.ToString();

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
                        result.Data = ApproveHeadless(id).name;
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

        private DocumentState ApproveHeadless(int id_headless)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var headless = db.Headless.FirstOrDefault(p => p.id == id_headless);
                    if (headless != null)
                    {
                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return null;

                        headless.Document.id_documentState = aprovedState.id;

                        db.Headless.Attach(headless);
                        db.Entry(headless).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return headless.Document.DocumentState;
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
                        result.Data = ReverseHeadless(id).name;
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

        private DocumentState ReverseHeadless(int id_headless)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var headless = db.Headless.FirstOrDefault(p => p.id == id_headless);
                    if (headless != null)
                    {
                        if (headless.ProductionLot.ProductionLotState.code != ("01") && //01: PENDIENTE DE RECEPCION
                            headless.ProductionLot.ProductionLotState.code != ("02") && //02: RECEPCIONADO
                            headless.ProductionLot.ProductionLotState.code != ("03") && //03: PENDIENTE DE PROCESAMIENTO
                            headless.ProductionLot.ProductionLotState.code != ("04")) //04: EN PROCESAMIENTO
                        {
                            throw new Exception("No puede reversar el Descabezado por tener el Lote: " + 
                                                headless.ProductionLot.internalNumberConcatenated + ". En estado: " +
                                                headless.ProductionLot.ProductionLotState.name + ".");
                        }
                       
                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        if (reverseState == null)
                            return null;

                        headless.Document.id_documentState = reverseState.id;

                        db.Headless.Attach(headless);
                        db.Entry(headless).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return headless.Document.DocumentState;
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
                        result.Data = AnnulHeadless(id).name;
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

        private DocumentState AnnulHeadless(int id_headless)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var headless = db.Headless.FirstOrDefault(p => p.id == id_headless);
                    if (headless != null)
                    {
                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return null;

                        headless.Document.id_documentState = annulState.id;
                        headless.Document.DocumentState = annulState;

                        db.Headless.Attach(headless);
                        db.Entry(headless).State = EntityState.Modified;

                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return headless.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetHeadlessResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetHeadlessResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetHeadlessResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var headless = db.Headless.FirstOrDefault(d => d.id == element.id);
            if (headless == null)
                return PartialView("Edit", new HeadlessDTO());


            var model = ConvertToDto(headless);
            SetHeadlessDTO(model);
            BuildViewDataEdit();
            //var codeTypeHeadless = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeHeadless)?.code;
            //BuildComboBoxSizeHeadless(codeTypeHeadless);
            BuilViewBag(false);
            //BuildComboBoxTypeHeadless(model.id, model.codeTypeProcess);

            return PartialView("Edit", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TurnChanged(int? id_turn)
        {
            var headless = GetHeadlessDTO();
            //var proveedor = "";
            var result = new
            {
                message = "",
                timeInitTurn = "",
                timeEndTurn = ""
            };
            var aTurn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            headless.timeInitTurn = aTurn != null ? (aTurn.timeInit.Hours + ":" + aTurn.timeInit.Minutes) : null;
            headless.timeEndTurn = aTurn != null ? (aTurn.timeEnd.Hours + ":" + aTurn.timeEnd.Minutes) : null;
            result = new
            {
                message = "",
                headless.timeInitTurn,
                headless.timeEndTurn
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Reporteria
        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReport(int id_headless, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_headless
            };

            paramLst.Add(_param);

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

		[HttpPost, ValidateInput(false)]
		public JsonResult PrintReportGeneral(DateTime? DateEditInit, DateTime? DateEditEnd, string codeReport)
		{
			List<ParamCR> paramLst = new List<ParamCR>();

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ParamCR _param = new ParamCR();
			string str_starReceptionDate = "";
			if (DateEditInit != null) {
				str_starReceptionDate = DateEditInit.Value.Date.ToString("yyyy/MM/dd");
			}

			_param = new ParamCR();
			_param.Nombre = "@fechaInicio";
			_param.Valor = str_starReceptionDate;
			paramLst.Add(_param);

			string str_endReceptionDate = "";
			if (DateEditEnd != null) {
				str_endReceptionDate = DateEditEnd.Value.Date.ToString("yyyy/MM/dd");
			}

			_param = new ParamCR();
			_param.Nombre = "@fechafin";
			_param.Valor = str_endReceptionDate;
			paramLst.Add(_param);

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = codeReport;
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

	}
}