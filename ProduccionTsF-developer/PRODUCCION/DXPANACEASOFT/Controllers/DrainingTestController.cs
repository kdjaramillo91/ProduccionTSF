using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.UI;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DXPANACEASOFT.Models.GenericProcess;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

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
            var xxxx = GetDrainingTestResultConsultDTO();
            return PartialView("_GridViewIndex", GetDrainingTestResultConsultDTO());
        }

        private static List<DrainingTestResultConsultDTO> GetListsConsultDto(DrainingTestConsultDTO consult)
        {
            using (var db = new DBContext())
            {
                var consultResult = db.DrainingTest
                                        .Where(DrainingTestQueryExtensions.GetRequestByFilter(consult))
                                        .ToList();

                int[] idsDrainingTest = consultResult.Select(r => r.id).ToArray();

                var oResultProdLotReceptionDetailList =
                                                db.ReceptionDetailDrainingTest
                                                .Where(r => idsDrainingTest.Contains(r.idDrainingTest))
                                                .Select(r => new
                                                {
                                                    idDrainingTest = r.idDrainingTest,
                                                    secTransaction = r.ResultProdLotReceptionDetail.numberLotSequential,
                                                    numberLot = r.ResultProdLotReceptionDetail.numberLot,
                                                    idRemissionGuide = r.ResultProdLotReceptionDetail.idRemissionGuide,
                                                    numberRemissionGuide = r.ResultProdLotReceptionDetail.numberRemissionGuide,
                                                    providerName = r.ResultProdLotReceptionDetail.nameProvider,
                                                    poundsRemitted = r.ResultProdLotReceptionDetail.poundsRemitted,
                                                })
                                                .ToList();

                /*
				 
				var preDocument2 = from _document in documentList
								   join _lote in lotesCompraValidados on
								   _document.id equals _lote.id
								   select _document;
				 */


                List<DrainingTestResultConsultDTO> query = (from oDrainingTest in consultResult
                                                            join oReceptionDetail in oResultProdLotReceptionDetailList on
                                                            oDrainingTest.id equals oReceptionDetail.idDrainingTest
                                                            select new DrainingTestResultConsultDTO
                                                            {
                                                                id = oDrainingTest.id,
                                                                number = oDrainingTest.Document.number,
                                                                secTransaction = oReceptionDetail.secTransaction,
                                                                dateTimeTesting = oDrainingTest.dateTimeTesting,
                                                                emissionDate = oDrainingTest.Document.emissionDate,
                                                                numberLot = oReceptionDetail.numberLot,
                                                                numberRemissionGuide = oReceptionDetail.numberRemissionGuide,
                                                                personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == oReceptionDetail.idRemissionGuide)?.Person2.processPlant,
                                                                provider = oReceptionDetail.providerName,
                                                                poundsRemitted = oReceptionDetail.poundsRemitted,
                                                                poundsProjected = oDrainingTest.poundsProjected,
                                                                poundsDrained = oDrainingTest.poundsDrained,
                                                                analist = oDrainingTest.Employee.Person.fullname_businessName,
                                                                state = oDrainingTest.Document.DocumentState.name,
                                                                canEdit = oDrainingTest.Document.DocumentState.code.Equals("01"),
                                                                canAproved = oDrainingTest.Document.DocumentState.code.Equals("01"),
                                                                canReverse = oDrainingTest.Document.DocumentState.code.Equals("03"),
                                                                canAnnul = (!oDrainingTest.Document.DocumentState.code.Equals("05") || !oDrainingTest.Document.DocumentState.code.Equals("03")),

                                                            }).OrderBy(o => o.id).ToList();

                //var query2 = query.ToList();


                //                     .OrderBy(o => o.id)
                //.ToList();



                //consultResult.Select(t => new DrainingTestResultConsultDTO
                //{
                //	id = t.id,
                //	number = t.Document.number,
                //	secTransaction = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                //	? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberLotSequential : "0",
                //	dateTimeTesting = t.dateTimeTesting,
                //	emissionDate = t.Document.emissionDate,
                //	numberLot = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                //			? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberLot : "0",
                //	numberRemissionGuide = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                //			? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).numberRemissionGuide : "0",
                //	provider = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                //	? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).nameProvider : "0",
                //	poundsRemitted = db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id) != null
                //	? db.ResultProdLotReceptionDetail.FirstOrDefault(r => r.idDrainingTest == t.id).poundsRemitted : 0,
                //	poundsProjected = t.poundsProjected,
                //	poundsDrained = t.poundsDrained,
                //	analist = t.Employee.Person.fullname_businessName,
                //	state = t.Document.DocumentState.name,

                //	canEdit = t.Document.DocumentState.code.Equals("01"),
                //	canAproved = t.Document.DocumentState.code.Equals("01"),
                //	canReverse = t.Document.DocumentState.code.Equals("03"),
                //	canAnnul = !t.Document.DocumentState.code.Equals("05"),
                //}).OrderByDescending(o => o.id).ToList();

                return query;
            }
        }

        private  List<DrainingTestPendingNewDTO> GetDrainingTestPendingNewDto()
        {
            var _receptionDetailDrainingTestIds = db.ReceptionDetailDrainingTest
                                                                    .Where(r => !r.isActive)                                                                    .Select(r=> r.idReceptionDetail)
                                                                    .ToArray();

            var _query = db.ResultProdLotReceptionDetail
                                .Where(r => r.ProductionLotDetail.ProductionLot.ProductionLotState.code.Equals("01") &&
                                            r.ProductionLotDetail.quantitydrained == 0 &&
                                            !_receptionDetailDrainingTestIds.Contains(r.idProductionLotReceptionDetail))
                                .ToList();
           
            var query =         _query.Select(re => new DrainingTestPendingNewDTO
                                   {
                                     idProductionLotReceptionDetail = re.idProductionLotReceptionDetail,
                                     number = re.numberLotSequential,
                                     numberLot = re.numberLot,
                                     numberRemissionGuide = re.numberRemissionGuide,
                                     remissionGuideExternalGuide = db.RemissionGuide.FirstOrDefault(e => e.id == re.idRemissionGuide)?.Guia_Externa,
                                     personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == re.idRemissionGuide)?.Person2.processPlant,
                                     dateReception = re.dateTimeArrived,
                                     provider = re.nameProvider,
                                     shrimper = re.nameProviderShrimp,
                                     item = re.nameItem,
                                     poundsRemitted = re.poundsRemitted,
                                     countKavetas = re.drawersNumber,
                                     metricUnit = re.MetricUnit.name,
                                }).ToList();

                return query;
            //}
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
                    DrainingTestSampligDetails = new List<DrainingTestSamplingDTO>(),
                    DrainingTestDetails = new List<DrainingTestDetail>(),
                    ReceptionDetail = receptionDetail,
                    DrainingTestDetailsInfo = new DataTable(),
                    id_personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.id_personProcessPlant,
                    personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.Person2.processPlant,
                    RemissionGuideExternalGuide = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.Guia_Externa,
                    certificationName = receptionDetail.ProductionLotDetail.ProductionLot.Certification?.name
                };

                //for (var i = 0; i < draininTestDto.drawersNumberSampling; i++)
                //{
                //    draininTestDto.DrainingTestDetails.Add(new DrainingTestDetail
                //    {
                //        id = i,
                //        order = i + 1,
                //        quantity = 0,
                //    });
                //}

                return draininTestDto;
            }
        }

        private DrainingTestDTO ConvertToDto(DrainingTest drainingTest)
        {
            var receptionDetail = drainingTest.ReceptionDetailDrainingTest.FirstOrDefault().ResultProdLotReceptionDetail;
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
                numberSampling = (drainingTest.numberSampling == null) ? 0 : (int)drainingTest.numberSampling,
                ReceptionDetail = receptionDetail,
                id_personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.id_personProcessPlant,
                personProcessPlant = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.Person2.processPlant,
                RemissionGuideExternalGuide = db.RemissionGuide.FirstOrDefault(r => r.id == receptionDetail.idRemissionGuide)?.Guia_Externa,
                certificationName = receptionDetail.ProductionLotDetail.ProductionLot.Certification?.name

                //DrainingTestSampligDetails = drainingTest.DrainingTestSampling.ToList()
                //DrainingTestDetails = drainingTest.DrainingTestDetail.ToList()
            };

            // List<DrainingTestSampling> _drainingTestSamplingList = new List<DrainingTestSampling>();

            List<MetricUnit> _metricUnitList = db.MetricUnit
                                                        .Where(r => r.isActive && r.id_company == this.ActiveCompanyId)
                                                        .ToList();
            int oderDrainingTestSampling = 1;

            List<DrainingTestSamplingDTO> _drainingTestSamplingList = db.DrainingTestSampling
                                                                        .Where(r => r.idrainingTest == drainingTest.id)
                                                                        .Select(r => new DrainingTestSamplingDTO
                                                                        {
                                                                            idrainingTest = r.idrainingTest,
                                                                            id = r.id,
                                                                            capacity = r.capacity,
                                                                            drawersNumber = r.drawersNumber,
                                                                            idMetricUnitCapacity = r.idMetricUnitCapacity,
                                                                            poundsAverage = r.poundsAverage,
                                                                            poundsDrained = r.poundsDrained,
                                                                            poundsProjected = r.poundsProjected
                                                                        })
                                                                        .ToList();

            _drainingTestSamplingList.ForEach(r => { r.order = oderDrainingTestSampling++; });

            int[] idsDrainingTestSampling = _drainingTestSamplingList.Select(r => r.id).ToArray();

            drainingTestDto.DrainingTestDetails = db.DrainingTestDetail
                                                        .Where(r => r.idDrainingTestSampling != null && idsDrainingTestSampling.Contains((int)r.idDrainingTestSampling))
                                                        .ToList();

            var _drainingTestDetailsQuantityList = drainingTestDto
                                                    .DrainingTestDetails
                                                    .Select(r => new
                                                    {
                                                        id = r.idDrainingTestSampling,
                                                        quantity = r.quantity
                                                    })
                                                    .GroupBy(r => r.id).Select(s => new { id = s.Key, arrayQuantity = s.ToArray() });


            var _drainingTestSamplingFinal = (from _drainingTestSampling in _drainingTestSamplingList
                                              join _metricUnit in _metricUnitList on
                                              _drainingTestSampling.idMetricUnitCapacity equals _metricUnit.id
                                              join _detailsQuantity in _drainingTestDetailsQuantityList on
                                              _drainingTestSampling.id equals _detailsQuantity.id
                                              select new DrainingTestSamplingDTO
                                              {
                                                  id = _drainingTestSampling.id,
                                                  order = _drainingTestSampling.order,
                                                  capacity = _drainingTestSampling.capacity,
                                                  idMetricUnitCapacity = _drainingTestSampling.idMetricUnitCapacity,
                                                  codeMetricUnitCapacity = _metricUnit.code,
                                                  nameMetricUnitCapacity = _metricUnit.name,
                                                  idrainingTest = _drainingTestSampling.idrainingTest,
                                                  drawersNumber = _drainingTestSampling.drawersNumber,
                                                  drawersNumberSamplig = 0,
                                                  poundsDrained = _drainingTestSampling.poundsDrained,
                                                  poundsAverage = _drainingTestSampling.poundsAverage,
                                                  poundsProjected = _drainingTestSampling.poundsProjected,
                                                  valuesDrainingTestDetail = _detailsQuantity.arrayQuantity.Select(r => r.quantity).ToArray()
                                              }).ToList();

            drainingTestDto.DrainingTestSampligDetails = _drainingTestSamplingFinal;

            drainingTestDto.DrainingTestDetailsInfo = GetDataTableDrainingTestDetail(_drainingTestSamplingFinal);

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
                                     ?.code.Equals("03") ?? false) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == GetDrainingTestDTO().idSate)
                                      ?.code.Equals("01") ?? false);
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
                        drainingTest.numberSampling = token.Value<int>("numberSampling");
                        drainingTest.drawersNumberSampling = token.Value<int>("drawersNumberSampling");
                        drainingTest.poundsDrained = token.Value<decimal>("poundsDrained");
                        drainingTest.poundsAverage = token.Value<decimal>("poundsAverage");
                        drainingTest.poundsProjected = token.Value<int>("poundsProjected");
                        drainingTest.dateTimeTesting = DateTime.ParseExact(token.Value<string>("dateTimeTesting"),
                                                                           "dd/MM/yyyy HH:mm",
                                                                           CultureInfo.InvariantCulture);



                        // DrainingTestSampling
                        List<DrainingTestSampling> _drainingTestSamplingList = db.DrainingTestSampling
                                                                                        .Where(r => r.idrainingTest == drainingTest.id)
                                                                                        .ToList();

                        int[] idsDrainingTestDetail = _drainingTestSamplingList.Select(r => r.id).ToArray();

                        //Details
                        var lastDetails = db.DrainingTestDetail.Where(d => idsDrainingTestDetail.Contains((int)d.idDrainingTestSampling)).ToList();
                        foreach (var detail in lastDetails)
                        {
                            db.DrainingTestDetail.Remove(detail);
                            db.DrainingTestDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }



                        foreach (DrainingTestSampling _drainingTestSampling in _drainingTestSamplingList)
                        {
                            db.DrainingTestSampling.Remove(_drainingTestSampling);
                            db.DrainingTestSampling.Attach(_drainingTestSampling);
                            db.Entry(_drainingTestSampling).State = EntityState.Deleted;
                        }

                        int i = 1;
                        var jDrainingTestSampling = token.Value<JArray>("DrainingTestSampling");
                        List<DrainingTestSamplingDTO> oDrainingTestSampling = (List<DrainingTestSamplingDTO>)jDrainingTestSampling.ToObject(typeof(List<DrainingTestSamplingDTO>));
                        drainingTest.DrainingTestSampling = oDrainingTestSampling.Select(r => new DrainingTestSampling
                        {
                            capacity = r.capacity,
                            idMetricUnitCapacity = r.idMetricUnitCapacity,
                            drawersNumber = r.drawersNumber,
                            poundsDrained = r.poundsDrained,
                            poundsAverage = r.poundsAverage,
                            poundsProjected = r.poundsProjected,
                            DrainingTestDetail = r.valuesDrainingTestDetail
                                                                                                    .Select(s => new DrainingTestDetail
                                                                                                    {
                                                                                                        order = i++,
                                                                                                        quantity = s,
                                                                                                        idMetricUnit = r.idMetricUnitCapacity
                                                                                                    }).ToList()
                        })
                                                                    .ToList();

                        //var newDetails = token.Value<JArray>("drainingTestDetails");

                        //foreach (var drainingTestSampling in newDrainingTestSampling)
                        //{
                        //    var iDetail = newDetails
                        //                    .Where(r => r.Value<int>("idDrainingTestSampling") == drainingTestSampling.Value<int>("id"))
                        //                    .Select(r=> new DrainingTestDetail
                        //                    {
                        //                         order = r.Value<int>("order"),
                        //                         quantity = r.Value<int>("quantity"),
                        //                         idMetricUnit = receptionDetail?.idMetricUnit ?? 0
                        //                    })
                        //                    .ToList();

                        //    List<DrainingTestDetail> _drainingTestDetailList = new List<DrainingTestDetail>();

                        //    drainingTest.DrainingTestSampling.Add(
                        //         new DrainingTestSampling
                        //         {
                        //              capacity = drainingTestSampling.Value<decimal>("capacity"),
                        //              drawersNumber = drainingTestSampling.Value<int>("drawersNumber"),
                        //              idMetricUnitCapacity = drainingTestSampling.Value<int>("idMetricUnitCapacity"),
                        //              poundsAverage = drainingTestSampling.Value<decimal>("poundsAverage"),
                        //              poundsDrained = drainingTestSampling.Value<decimal>("poundsDrained"),
                        //              poundsProjected = drainingTestSampling.Value<decimal>("poundsProjected"),
                        //              DrainingTestDetail = iDetail
                        //         });

                        //}



                        //foreach (var detail in newDetails)
                        //{
                        //    drainingTest.DrainingTestDetail.Add(new DrainingTestDetail
                        //    {
                        //        order = detail.Value<int>("order"),
                        //        quantity = detail.Value<decimal>("quantity"),
                        //        idMetricUnit = receptionDetail?.idMetricUnit??0
                        //    });
                        //}

                        if (newObject)
                        {
                            ReceptionDetailDrainingTest _ReceptionDetailDrainingTest = new ReceptionDetailDrainingTest
                            {

                                idReceptionDetail = receptionDetail.idProductionLotReceptionDetail,
                                isActive = true
                            };
                            drainingTest.ReceptionDetailDrainingTest.Add(_ReceptionDetailDrainingTest);


                            db.ReceptionDetailDrainingTest.Add(_ReceptionDetailDrainingTest);
                            db.Entry(_ReceptionDetailDrainingTest).State = EntityState.Added;

                            db.DrainingTest.Add(drainingTest);
                            db.Entry(drainingTest).State = EntityState.Added;

                        }
                        else
                        {
                            db.DrainingTest.Attach(drainingTest);
                            db.Entry(drainingTest).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        //receptionDetail.idDrainingTest = drainingTest.id;
                        //db.ResultProdLotReceptionDetail.Attach(receptionDetail);
                        //db.Entry(receptionDetail).State = EntityState.Modified;

                        //db.SaveChanges();
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
                    int[] idsDrainingTestSampling = db.DrainingTestSampling
                                                            .Where(r => r.idrainingTest == id_draining)
                                                            .Select(s => s.id)
                                                            .ToArray();

                    List<DrainingTestDetail> _drainingTestDetailList = db.DrainingTestDetail
                                                                        .Where(r => r.idDrainingTestSampling != null && idsDrainingTestSampling.Contains((int)r.idDrainingTestSampling))
                                                                        .ToList();

                    if (drainingTest != null)
                    {
                        if (_drainingTestDetailList != null)
                        {
                            if (_drainingTestDetailList.All(d => d.quantity == 0))
                            {
                                throw new Exception("No puede aprobar sin al menos un valor en la culumna Peso en Libras");
                            }
                            if (_drainingTestDetailList.Any(c => c.quantity == 0))
                            {
                                throw new Exception("No puede aprobar con un Peso en Libras con valor cero");
                            }

                        }


                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return null;

                        drainingTest.Document.id_documentState = aprovedState.id;
                        drainingTest.Document.authorizationDate = DateTime.Now;



                        var resultProdLotReceptionDetail =
                                                 db.DrainingTest.FirstOrDefault(r => r.id == drainingTest.id)?
                                                                .ReceptionDetailDrainingTest?
                                                                .FirstOrDefault()?
                                                                .ResultProdLotReceptionDetail;



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
                        if (!drainingTest.ValidateCanAnnul(db, out var errorMessage)) throw new Exception("Prueba de Escurrido no puede ser Reversada. " + errorMessage);

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
                        if (!drainingTest.ValidateCanAnnul(db, out var errorMessage)) throw new Exception("Prueba de Escurrido no puede ser Anulada. " + errorMessage);

                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return drainingTest.Document.DocumentState;

                        int? idReceptionDetailDrainingTest = drainingTest
                                                                    .ReceptionDetailDrainingTest
                                                                    .FirstOrDefault(r => r.isActive)?.id;
                        if (idReceptionDetailDrainingTest != null)
                        {

                            ReceptionDetailDrainingTest _receptionDetailDrainingTest =
                                                                                db.ReceptionDetailDrainingTest
                                                                                        .FirstOrDefault(r => r.id == idReceptionDetailDrainingTest);

                            _receptionDetailDrainingTest.isActive = false;

                            db.ReceptionDetailDrainingTest.Attach(_receptionDetailDrainingTest);
                            db.Entry(_receptionDetailDrainingTest).State = EntityState.Modified;

                        }

                        drainingTest.Document.id_documentState = annulState.id;
                        drainingTest.Document.authorizationDate = DateTime.Now;
                        drainingTest.ColateralAnnul(db);


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
            var drainingTestResult = GetDrainingTestResultConsultDTO();
            var index = drainingTestResult.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);
            
            var result = new
            {
                maximunPages = drainingTestResult.Count(),
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

        #region "Cambios 20181022"
        [HttpPost]
        public ActionResult GridViewDrainingTestSampling(int drawersNumber, int drawersTotal, Boolean isChange)
        {

            // no se puede cambiar si esta en proceso de edicion
            List<DrainingTestSamplingDTO> _drainingTestSamplingList = new List<DrainingTestSamplingDTO>();
            MetricUnit _metricUnit = db.MetricUnit.FirstOrDefault(r => r.code == "Lbs");
            int _drawersNumber = (drawersNumber == 1) ? drawersTotal : 0;

            int ids = 1;
            for (int i = 0; i < drawersNumber; i++)
            {
                _drainingTestSamplingList.Add(new DrainingTestSamplingDTO
                {
                    capacity = 0,
                    drawersNumber = _drawersNumber,
                    id = ids,
                    order = ids,
                    poundsAverage = 0.0M,
                    poundsDrained = 0.0M,
                    poundsProjected = 0.0M,
                    idMetricUnitCapacity = (_metricUnit?.id ?? 0),
                    nameMetricUnitCapacity = (_metricUnit?.name ?? ""),
                    codeMetricUnitCapacity = (_metricUnit?.code ?? ""),
                    drawersNumberSamplig = 0,
                    valuesDrainingTestDetail = new decimal[0]
                });
                ids++;
            }


            return PartialView("_GridViewCapacity", _drainingTestSamplingList);
        }

        private DataTable GetDataTableDrainingTestDetail(List<DrainingTestSamplingDTO> drainingTestSamplingList)
        {

            string prefixId = "ID";
            int indexRow = 1;
            int indexColumn = 1;
            int maxRow = 10;
            DataTable dt = new DataTable();

            try
            {


                List<DrainingTestSamplingDTO> jInfoList = drainingTestSamplingList;

                var detailValue = jInfoList.Select(r => r.valuesDrainingTestDetail).ToList();
                int numeroMaximosFilas = detailValue.Max(r => r.Count());


                int countInfo = jInfoList.Max(r => r.drawersNumber);
                countInfo = (numeroMaximosFilas > 0) ? numeroMaximosFilas : (countInfo >= maxRow ? maxRow : countInfo);

                DataColumn colIndex = dt.Columns.Add("Index", typeof(int));
                colIndex.Caption = "#";
                colIndex.ExtendedProperties.Add("Width", 25);
                colIndex.ReadOnly = false;

                foreach (DrainingTestSamplingDTO _info in jInfoList)
                {
                    string nameID = prefixId + "_" + _info.id.ToString();
                    DataColumn _ColB = dt.Columns.Add(nameID, typeof(decimal));
                    _ColB.ReadOnly = false;
                    _ColB.ExtendedProperties.Add("Width", 100);
                    _ColB.ExtendedProperties.Add("idDrainingTestSampling", _info.id);
                    _ColB.Caption = "Capacidad Bines " + _info.capacity.ToString() + " " + _info.codeMetricUnitCapacity;

                }

                for (int i = 0; i < countInfo; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Index"] = indexRow;
                    foreach (DrainingTestSamplingDTO _info in jInfoList)
                    {
                        int maxLenArr = (_info.valuesDrainingTestDetail.Length - 1);
                        int maxDrawersNumber = (_info.drawersNumber - 1);

                        decimal? valueSamplig = null;

                        if (i <= maxDrawersNumber)
                        {
                            //valueSamplig = null;
                            if (i <= maxLenArr)
                            {
                                valueSamplig = _info.valuesDrainingTestDetail[i];
                            }
                            else
                            {
                                valueSamplig = 0;
                            }
                        }

                        if (valueSamplig == null)
                        {
                            dr[indexColumn] = DBNull.Value;
                        }
                        else
                        {
                            dr[indexColumn] = valueSamplig;
                        }


                        //dr[indexColumn] = _info.idDrainingTestSampling;
                        //dr[(indexColumn+1)] = _info.capacity;
                        //dr[(indexColumn+2)] = 0.00;
                        //indexColumn +=3 ;
                        indexColumn++;
                    }


                    dt.Rows.Add(dr);
                    indexRow++;
                    indexColumn = 1;

                }

            }
            catch (Exception e)
            {
                LogWrite(e, null, "GetDataTableDrainingTestDetail");

            }

            return dt;
        }

        [HttpPost]
        public ActionResult GridViewDetailsDrainingTest(string jDrainingTestSamplingInfo)
        {

            //string prefixId = "ID";
            //int indexRow= 1;            
            //int indexColumn = 1;
            DataTable dt = new DataTable();

            try
            {


                List<DrainingTestSamplingDTO> jInfoList = JsonConvert.DeserializeObject<List<DrainingTestSamplingDTO>>(jDrainingTestSamplingInfo);
                dt = GetDataTableDrainingTestDetail(jInfoList);



                //int countInfo = jInfoList.Max(r => r.drawersNumber) ;
                //countInfo = (countInfo >= 5 ? 5 : countInfo);

                //DataColumn colIndex =  dt.Columns.Add("Index", typeof(int));
                //colIndex.ExtendedProperties.Add("Width", 25);
                //colIndex.ReadOnly = false;                

                //foreach (DrainingTestSamplingDTO _info  in jInfoList)
                //{
                //    string nameID = prefixId + "_" + _info.id.ToString();//+"_"+ _info.capacity.ToString();
                //    //string Capacidad = prefixDesc + _info.capacity.ToString();
                //    //string ValueCaption = prefixValue + _info.capacity.ToString();
                //    //DataColumn _ColA = dt.Columns.Add(nameID, typeof(int));
                //    //_ColA.ReadOnly = true;

                //    DataColumn _ColB =  dt.Columns.Add(nameID, typeof(decimal));
                //    _ColB.ReadOnly = false;
                //    _ColB.ExtendedProperties.Add("Width", 100);
                //    _ColB.ExtendedProperties.Add("idDrainingTestSampling", _info.id);
                //    _ColB.Caption = "Capacidad Kavetas " + _info.capacity.ToString() +" "+ _info.codeMetricUnitCapacity; 

                //    //DataColumn _ColC =  dt.Columns.Add(ValueCaption, typeof(decimal));
                //    //_ColC.ReadOnly = false;
                //    //_ColC.ExtendedProperties.Add("Width", 80);
                //    //_ColC.Caption = "Peso Muestra";


                //}


                //for (int i=0; i< countInfo; i++)
                //{

                //    DataRow dr = dt.NewRow();
                //    dr["Index"] = indexRow;
                //    foreach (DrainingTestSamplingDTO _info in jInfoList)
                //    {
                //        dr[indexColumn] = 0.00;
                //        //dr[indexColumn] = _info.idDrainingTestSampling;
                //        //dr[(indexColumn+1)] = _info.capacity;
                //        //dr[(indexColumn+2)] = 0.00;

                //        //indexColumn +=3 ;
                //        indexColumn ++;
                //    }


                //    dt.Rows.Add(dr);
                //    indexRow++;
                //    indexColumn = 1;

                //}


            }
            catch (Exception e)
            {
                LogWrite(e, null, "GridViewDetailsDrainingTest");

            }
            return PartialView("_GridViewDetails", dt);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult CalculateDrainingTestSampling(string jDrainingTestSamplingInfo)
        {
            List<DrainingTestSamplingDTO> jInfoList = JsonConvert.DeserializeObject<List<DrainingTestSamplingDTO>>(jDrainingTestSamplingInfo);

            jInfoList.ForEach(r =>
            {
                int countRows = r.valuesDrainingTestDetail.Count();
                decimal _totalSampling = r.valuesDrainingTestDetail.Sum(s => decimal.Round(s, 1));
                decimal _promedio = (countRows == 0) ? 0 : (_totalSampling / countRows);
                _promedio = TruncateDecimal(_promedio, 1);
                _totalSampling = TruncateDecimal(_totalSampling, 1);
                r.poundsDrained = _totalSampling;
                //Decimal.Round(_totalSampling,1);
                r.poundsAverage = _promedio;
                //Decimal.Round(_promedio, 1);
                r.poundsProjected = TruncateDecimal(_promedio * r.drawersNumber, 0);
                //Decimal.Round(_promedio * r.drawersNumber,1);
            });

            decimal totalPoundsProjected = TruncateDecimal(jInfoList.Sum(r => r.poundsProjected), 0);
            string CalPRESC = db.Setting.FirstOrDefault(fod => fod.code == "CALFES")?.value ?? "";
            if (CalPRESC == "SI")
            {
                var cantidadBines = jInfoList.Sum(r => r.valuesDrainingTestDetail.Count());
                var cantidadDetalles = jInfoList.Max(r => r.id);
                var valueDrawersNumber = (cantidadDetalles == 0) ? 0 : (jInfoList.Sum(r => r.drawersNumberSamplig) / cantidadDetalles);
                totalPoundsProjected = (totalPoundsProjected == 0) ? 0 : ((totalPoundsProjected / cantidadBines) * valueDrawersNumber);

            }

            // Decimal.Round( jInfoList.Sum(r => r.poundsProjected),1);
            jInfoList.ForEach(r => r.poundsTotalProjected = totalPoundsProjected);


            //var result = JsonConvert.SerializeObject(jInfoList);

            return Json(jInfoList, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        [HttpPost]
        public JsonResult PrintDrainingReport(int id_drainingTest)
        {
            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@idDraining";
            _param.Valor = id_drainingTest;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RPEV1";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);


            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
        public JsonResult PrintDrainingReportIndex(DateTime? fechaEmisionInicio, DateTime? fechaEmisionFinal)
        {
            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param;
            _param = new ParamCR();
            _param.Nombre = "@fechaEmisionInicio";
            _param.Valor = fechaEmisionInicio;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@fechaEmisionFinal";
            _param.Valor = fechaEmisionFinal;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RPGV1";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);


            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
    }
}