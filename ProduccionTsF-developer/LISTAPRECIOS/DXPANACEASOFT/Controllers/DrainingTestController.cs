using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.UI;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DXPANACEASOFT.Controllers
{
    public class DrainingTestController : DefaultController
    {
        private DrainingTestDTO GetDrainingTestDTO()
        {
            if (!(Session["DrainingTestDTO"] is DrainingTestDTO drainingTest))
                drainingTest = new DrainingTestDTO();
            return drainingTest;
        }

        private List<DrainingTestResultConsultDTO> GetDrainingTestResultConsultDTO()
        {
            if (!(Session["DrainingTestResultConsultDTO"] is List<DrainingTestResultConsultDTO> drainingTestResultConsult))
                drainingTestResultConsult = new List<DrainingTestResultConsultDTO>();
            return drainingTestResultConsult;
        }

        private void SetDrainingTestDTO(DrainingTestDTO drainingTestDTO)
        {
            Session["DrainingTestDTO"] = drainingTestDTO;
        }

        private void SetDrainingTestResultConsultDTO(List<DrainingTestResultConsultDTO> drainingTestResultConsult)
        {
            Session["DrainingTestResultConsultDTO"] = drainingTestResultConsult;
        }

        // GET: Escurrido
        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(DrainingTestConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetDrainingTestResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewDrainingTest()
        {
            return PartialView("_GridViewIndex", GetDrainingTestResultConsultDTO());
        }

        private static List<DrainingTestResultConsultDTO> GetListsConsultDto(DrainingTestConsultDTO consult)
        {
            using (var db = new DBContext())
            {
                var consultResult = db.DrainingTest.Where(DrainingTestQueryExtensions.GetRequestByFilter(consult));
                var query = consultResult.Select(t => new DrainingTestResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    secTransaction = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                        ? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberLotSequential : "0",
                    dateTimeTesting = t.dateTimeTesting,
                    emissionDate = t.Document.emissionDate,
                    numberLot = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id)!= null
                                ? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberLot : "0",
                    numberRemissionGuide = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                                ? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberRemissionGuide : "0",
                    provider = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                        ? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).nameProvider : "0",
                    poundsRemitted = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                        ? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).poundsRemitted : 0,
                    poundsProjected = t.poundsProjected,
                    poundsDrained = t.poundsDrained,
                    analist = t.Employee.Person.fullname_businessName,
                    state = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03"),
                    canAnnul = !t.Document.DocumentState.code.Equals("05"),
                }).OrderByDescending(o => o.id).ToList();

                return query;
            }
        }

        private static List<DrainingTestPendingNewDTO> GetDrainingTestPendingNewDto()
        {
            using (var db = new DBContext())
            {
                var query = db.ResultProdLotReceptionDetail.AsEnumerable().Where(r => r.idDrainingTest == null && 
                                                                                      r.ProductionLotDetail.ProductionLot.ProductionLotState.code.Equals("01"))
                    .Select(re => new DrainingTestPendingNewDTO
                    {
                        idProductionLotReceptionDetail = re.idProductionLotReceptionDetail,
                        number = re.numberLotSequential,
                        numberLot = re.numberLot,
                        numberRemissionGuide = re.numberRemissionGuide,
                        dateReception = re.dateTimeArrived != null ? re.dateTimeArrived.Value.ToString("dd-MM-yyyy") : "",
                        provider = re.nameProvider,
                        shrimper = re.nameProviderShrimp,
                        item = re.nameItem,
                        poundsRemitted = re.poundsRemitted,
                        countKavetas = re.drawersNumber,
                        metricUnit = re.MetricUnit.name,
                    }).ToList();

                return query;
            }
        }

        [HttpPost]
        public ActionResult PendingNew()
        {
            return View(GetDrainingTestPendingNewDto());
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNew()
        {
            return PartialView("_GridViewPendingNew", GetDrainingTestPendingNewDto());
        }

        private DrainingTestDTO Create(int idProductionLotReceptionDetail)
        {
            using (var db = new DBContext())
            {
                var receptionDetail =
                    db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idProductionLotReceptionDetail == idProductionLotReceptionDetail);
                if (receptionDetail == null)
                    return new DrainingTestDTO();

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("93"));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var draininTestDto = new DrainingTestDTO
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
                    dateTimeTesting = DateTime.Now,
                    temp = 0,
                    drawersNumberSampling = receptionDetail.drawersNumber,
                    poundsDrained = 0,
                    poundsAverage = 0,
                    poundsProjected = 0,
                    DrainingTestDetails = new List<DrainingTestDetail>(),
                    ReceptionDetail = receptionDetail
                };

                for (var i = 0; i < draininTestDto.drawersNumberSampling; i++)
                {
                    draininTestDto.DrainingTestDetails.Add(new DrainingTestDetail
                    {
                        id = i,
                        order = i + 1,
                        quantity = 0,
                    });
                }

                return draininTestDto;
            }
        }

        private DrainingTestDTO ConvertToDto(DrainingTest drainingTest)
        {
            var receptionDetail = drainingTest.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == drainingTest.id);
            if (receptionDetail == null)
                return null;

            var drainingTestDto = new DrainingTestDTO
            {
                id = drainingTest.id,
                description = drainingTest.Document.description,
                id_documentType = drainingTest.Document.id_documentType,
                number = drainingTest.Document.number,
                state = drainingTest.Document.DocumentState.name,
                documentType = drainingTest.Document.DocumentType.name,
                dateTimeEmision = drainingTest.Document.emissionDate,
                dateTimeTesting = drainingTest.dateTimeTesting,
                idSate = drainingTest.Document.id_documentState,
                reference = drainingTest.Document.reference,
                poundsDrained = drainingTest.poundsDrained,
                poundsProjected = drainingTest.poundsProjected,
                drawersNumberSampling = drainingTest.drawersNumberSampling,
                analist = drainingTest.Employee.Person.fullname_businessName,
                idAnalist = drainingTest.idAnalist,
                poundsAverage = drainingTest.poundsAverage,
                temp = drainingTest.temp,
                ReceptionDetail = receptionDetail,
                DrainingTestDetails = drainingTest.DrainingTestDetail.ToList()
            };

            return drainingTestDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
        }

        private void BuildViewDataEdit()
        {
            BuildComboBoxAnalist();
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, int idProductionLotReceptionDetail = 0, bool enabled = true)
        {
            BuildViewDataEdit();

            var model = new DrainingTestDTO();
            var drainingTest = db.DrainingTest.FirstOrDefault(d => d.id == id);
            if (drainingTest == null)
            {
                model = Create(idProductionLotReceptionDetail);
                SetDrainingTestDTO(model);
                BuilViewBag(enabled);
                return PartialView(model);
            }

            model = ConvertToDto(drainingTest);
            SetDrainingTestDTO(model);
            BuilViewBag(enabled);
            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            ViewBag.enabled = enabled;
            ViewBag.canEdit = !enabled && 
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == GetDrainingTestDTO().idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == GetDrainingTestDTO().idSate)
                                     ?.code.Equals("01") ?? false;
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == GetDrainingTestDTO().idSate)
                                     ?.code.Equals("03")??false) && !enabled;
            ViewBag.canAnnul = !(db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == GetDrainingTestDTO().idSate)
                                      ?.code.Equals("05") ?? false);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(int drawersNumberSampling)
        {
            var drainingTestDetails = new List<DrainingTestDetail>();
            for (var i = 0; i < drawersNumberSampling; i++)
            {
                drainingTestDetails.Add(new DrainingTestDetail
                {
                    id = i,
                    order = i + 1,
                    quantity = 0,
                });
            }

            return PartialView("_GridViewDetails", drainingTestDetails);
        }

        #region Combobox
        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState.Where(e => e.isActive)
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
        #endregion

        [HttpPost]
        public JsonResult Save(string jsonDrainingTest)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonDrainingTest);
                        
                        var id_receptionDetail = token.Value<int>("id_receptionDetail");
                        var receptionDetail = db.ResultProdLotReceptionDetail.FirstOrDefault(r =>
                            r.idProductionLotReceptionDetail == id_receptionDetail);
                        if (receptionDetail == null)
                            throw new Exception("Detalle de Recepción Lote no encontrado");

                        var newObject = false;
                        var id = token.Value<int>("id");

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("93"));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var drainingTest = db.DrainingTest.FirstOrDefault(d => d.id == id);
                        if (drainingTest == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            drainingTest = new DrainingTest
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
                                }
                            };

                            documentType.currentNumber++;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        drainingTest.Document.id_documentState = documentState.id;
                        drainingTest.Document.id_userUpdate = ActiveUser.id;
                        drainingTest.Document.dateUpdate = DateTime.Now;
                        drainingTest.Document.reference = token.Value<string>("reference");
                        drainingTest.Document.description = token.Value<string>("description");
                        drainingTest.Document.emissionDate = DateTime.ParseExact(token.Value<string>("emissionDate"), 
                                                                                 "dd/MM/yyyy", 
                                                                                 CultureInfo.InvariantCulture);

                        drainingTest.idAnalist = token.Value<int>("idAnalist");
                        drainingTest.temp = token.Value<int>("temp");
                        drainingTest.drawersNumberSampling = token.Value<int>("drawersNumberSampling");
                        drainingTest.poundsDrained = token.Value<decimal>("poundsDrained");
                        drainingTest.poundsAverage = token.Value<decimal>("poundsAverage");
                        drainingTest.poundsProjected = token.Value<int>("poundsProjected");

                        drainingTest.dateTimeTesting = DateTime.ParseExact(token.Value<string>("dateTimeTesting"),
                                                                           "dd/MM/yyyy HH:mm",
                                                                           CultureInfo.InvariantCulture);
                        //Details
                        var lastDetails = db.DrainingTestDetail.Where(d => d.idDrainingTest == drainingTest.id);
                        foreach (var detail in lastDetails)
                        {
                            db.DrainingTestDetail.Remove(detail);
                            db.DrainingTestDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        var newDetails = token.Value<JArray>("drainingTestDetails");
                        foreach (var detail in newDetails)
                        {
                            drainingTest.DrainingTestDetail.Add(new DrainingTestDetail
                            {
                                order = detail.Value<int>("order"),
                                quantity = detail.Value<decimal>("quantity"),
                                idMetricUnit = receptionDetail?.idMetricUnit??0
                            });
                        }

                        if (newObject)
                        {
                            db.DrainingTest.Add(drainingTest);
                            db.Entry(drainingTest).State = EntityState.Added;
                        }
                        else
                        {
                            db.DrainingTest.Attach(drainingTest);
                            db.Entry(drainingTest).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        receptionDetail.idDrainingTest = drainingTest.id;
                        db.ResultProdLotReceptionDetail.Attach(receptionDetail);
                        db.Entry(receptionDetail).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        result.Data = drainingTest.id.ToString();
                        
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
                        result.Data = ApproveDrainingTest(id).name;
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

        private DocumentState ApproveDrainingTest(int id_draining)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var drainingTest = db.DrainingTest.FirstOrDefault(p => p.id == id_draining);
                    if (drainingTest != null)
                    {
                        if (drainingTest.DrainingTestDetail.All(d => d.quantity == 0))
                        {
                            throw new Exception("No puede aprobar sin al menos un valor en la culumna Peso en Libras");
                        }
                        if (drainingTest.DrainingTestDetail.Any(c => c.quantity == 0))
                        {
                            throw new Exception("No puede aprobar con un Peso en Libras con valor cero");
                        }

                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return null;

                        drainingTest.Document.id_documentState = aprovedState.id;
                        drainingTest.Document.authorizationDate = DateTime.Now;

                        var resultProdLotReceptionDetail =
                            db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == drainingTest.id);

                        if (resultProdLotReceptionDetail != null)
                        {
                            resultProdLotReceptionDetail.ProductionLotDetail.quantitydrained = drainingTest.poundsProjected;
                            db.ResultProdLotReceptionDetail.Attach(resultProdLotReceptionDetail);
                            db.Entry(resultProdLotReceptionDetail).State = EntityState.Modified;
                        }

                        db.DrainingTest.Attach(drainingTest);
                        db.Entry(drainingTest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return drainingTest.Document.DocumentState;
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
                        result.Data = ReverseDrainingTest(id).name;
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

        private DocumentState ReverseDrainingTest(int id_draining)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var drainingTest = db.DrainingTest.FirstOrDefault(p => p.id == id_draining);
                    if (drainingTest != null)
                    {
                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        if (reverseState == null)
                            return

                        drainingTest.Document.DocumentState;
                        drainingTest.Document.id_documentState = reverseState.id;
                        drainingTest.Document.authorizationDate = DateTime.Now;

                        db.DrainingTest.Attach(drainingTest);
                        db.Entry(drainingTest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return drainingTest.Document.DocumentState;
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
                        result.Data = AnnulDrainingTest(id).name;
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

        private DocumentState AnnulDrainingTest(int id_draining)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var drainingTest = db.DrainingTest.FirstOrDefault(p => p.id == id_draining);
                    if (drainingTest != null)
                    {
                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return

                        drainingTest.Document.DocumentState;
                        drainingTest.Document.id_documentState = annulState.id;
                        drainingTest.Document.authorizationDate = DateTime.Now;

                        db.DrainingTest.Attach(drainingTest);
                        db.Entry(drainingTest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return drainingTest.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetDrainingTestResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetDrainingTestResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetDrainingTestResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var drainingTest = db.DrainingTest.FirstOrDefault(d => d.id == element.id);
            if (drainingTest == null)
                return PartialView("Edit", new DrainingTestDTO());

            var model = ConvertToDto(drainingTest);
            SetDrainingTestDTO(model);
            BuilViewBag(false);
            return PartialView("Edit", model);
        }
    }
}