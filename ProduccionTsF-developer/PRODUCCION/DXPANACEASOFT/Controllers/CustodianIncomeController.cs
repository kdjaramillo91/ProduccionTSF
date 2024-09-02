//using DevExpress.XtraPrinting.Native;
//using DevExpress.XtraRichEdit.Model;
using Dapper;
using DevExpress.CodeParser;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
using DevExpress.XtraSpreadsheet.Import.Xls;
//using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
//using DocumentFormat.OpenXml.Wordprocessing;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.RemGuide;
using DXPANACEASOFT.Operations;
using DXPANACEASOFT.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class CustodianIncomeController : DefaultController
    {
        const string TIPO_DOCUMENTO_CUSTODIANINCOME_CODIGO = "167";
        const string ESTADO_PENDIENTE_CODIGO = "01";
        const string ESTADO_APROBADA_CODIGO = "03";
        const string ESTADO_ANULADO_CODIGO = "05";
        const string ESTADO_AUTORIZADO_CODIGO = "06";
        const string ESTADO_PRE_AUTORIZADO_CODIGO = "09";
        const string SELECT_CUSTODIANINCOME =   "SELECT	Id, id_RemissionGuide, id_PersonCompanyCustodian1, id_PersonCompanyCustodian2, " +
                                                "id_FishingSite1, id_FishingSite2, " +
                                                "id_FishingCustodian1, id_FishingCustodian2, " +
                                                "fishingCustodianField1, fishingCustodianField2, " +
                                                "fishingCustodianValue1, fishingCustodianValue2, " +
                                                "remissionGuideProviderName, remissionGuideProductionUnitProviderName, " +
                                                "remissionGuidePoolReference, remissionGuidePoundTotal, "+
                                                "remissionGuideFishingZoneName, remissionGuideFishingSiteName, "+
                                                "remissionGuideRoute, remissionGuideShippingTypeName, " +
                                                "remissionGuidesDriverName, remissionGuidesProcessPlant, " +
                                                "remissionGuidesProviderTransportName, remissionGuidesCarRegistration, " +
                                                "remissionGuidesTransportBillingName, remissionGuidesTransportValuePrice " +
                                                //"remissionGuideValidateData "+
                                                "FROM CustodianIncome ";

        const string SELECT_CUSTODIANINCOME_GUIAS = "SELECT	ci.id_RemissionGuide " +
                                                    //"remissionGuideValidateData "+
                                                    "FROM CustodianIncome ci INNER JOIN DOCUMENT d on d.id = ci.id " +
                                                    "WHERE d.id_documentState != @id_documentState ";



        const string INSERT_CUSTODIANINCOME = "INSERT INTO CustodianIncome (id,id_RemissionGuide, id_PersonCompanyCustodian1,id_PersonCompanyCustodian2, " +
                                              "id_FishingSite1, id_FishingSite2, " +
                                              "id_FishingCustodian1, id_FishingCustodian2, " +
                                              "fishingCustodianField1, fishingCustodianField2, " +
                                              "fishingCustodianValue1, fishingCustodianValue2,  " +
                                              "remissionGuideProviderName, remissionGuideProductionUnitProviderName, " +
                                              "remissionGuidePoolReference, remissionGuidePoundTotal, " +
                                              "remissionGuideFishingZoneName, remissionGuideFishingSiteName, " +
                                              "remissionGuideRoute, remissionGuideShippingTypeName, " +
                                              "remissionGuidesDriverName, remissionGuidesProcessPlant, " +
                                              "remissionGuidesProviderTransportName, remissionGuidesCarRegistration, " +
                                              "remissionGuidesTransportBillingName, remissionGuidesTransportValuePrice " +
                                              //"remissionGuideValidateData " +
                                              ") VALUES " +
                                              "(@id, @id_RemissionGuide, @id_PersonCompanyCustodian1,@id_PersonCompanyCustodian2, " +
                                              "@id_FishingSite1, @id_FishingSite2," +
                                              "@id_FishingCustodian1, @id_FishingCustodian2, " +
                                              "@fishingCustodianField1, @fishingCustodianField2, " +
                                              "@fishingCustodianValue1, @fishingCustodianValue2,  " +
                                              "@remissionGuideProviderName, @remissionGuideProductionUnitProviderName, " +
                                              "@remissionGuidePoolReference, @remissionGuidePoundTotal, " +
                                              "@remissionGuideFishingZoneName, @remissionGuideFishingSiteName, " +
                                              "@remissionGuideRoute, @remissionGuideShippingTypeName, " +
                                              "@remissionGuidesDriverName, @remissionGuidesProcessPlant, " +
                                              "@remissionGuidesProviderTransportName, @remissionGuidesCarRegistration, " +
                                              "@remissionGuidesTransportBillingName, @remissionGuidesTransportValuePrice " +
                                              //"@remissionGuideValidateData " +
                                              "); SELECT SCOPE_IDENTITY()";

        const string UDPATE_CUSTODIANINCOME = "UPDATE CustodianIncome set id_PersonCompanyCustodian1 = @id_PersonCompanyCustodian1, " +
                                                "id_PersonCompanyCustodian2 = @id_PersonCompanyCustodian2, " +
                                                "id_FishingSite1= @id_FishingSite1, " +
                                                "id_FishingSite2= @id_FishingSite2, " +
                                                "id_FishingCustodian1 = @id_FishingCustodian1, " +
                                                "id_FishingCustodian2 = @id_FishingCustodian2, " +
                                                "fishingCustodianField1= @fishingCustodianField1, " +
                                                "fishingCustodianField2= @fishingCustodianField2, " +
                                                "fishingCustodianValue1= @fishingCustodianValue1, " +
                                                "fishingCustodianValue2= @fishingCustodianValue2, " +
                                                "remissionGuideProviderName= @remissionGuideProviderName, remissionGuideProductionUnitProviderName=@remissionGuideProductionUnitProviderName, " +
                                                "remissionGuidePoolReference = @remissionGuidePoolReference, remissionGuidePoundTotal=@remissionGuidePoundTotal, " +
                                                "remissionGuideFishingZoneName=@remissionGuideFishingZoneName, remissionGuideFishingSiteName=@remissionGuideFishingSiteName, " +
                                                "remissionGuideRoute=@remissionGuideRoute, remissionGuideshippingTypeName=@remissionGuideShippingTypeName, " +
                                                "remissionGuidesDriverName=@remissionGuidesDriverName, remissionGuidesProcessPlant=@remissionGuidesProcessPlant, " +
                                                "remissionGuidesProviderTransportName=@remissionGuidesProviderTransportName, remissionGuidesCarRegistration=@remissionGuidesCarRegistration, " +
                                                "remissionGuidesTransportBillingName=@remissionGuidesTransportBillingName, remissionGuidesTransportValuePrice=@remissionGuidesTransportValuePrice " +
                                                //"remissionGuideValidateData = @remissionGuideValidateData " +
                                                "WHERE ID = @id;";


        const string INSERT_DOCUMENT = "INSERT INTO DOCUMENT (number,sequential,emissionDate,description, " +
                                        "id_emissionPoint, id_documentType, id_documentState, id_userCreate, dateCreate, " +
                                        "id_userUpdate, dateUpdate) VALUES " +
                                        "(@number, @sequential, @emissionDate, @description, " +
                                        "@id_emissionPoint, @id_documentType, @id_documentState, @id_userCreate, @dateCreate, " +
                                        "@id_userUpdate, @dateUpdate); SELECT SCOPE_IDENTITY()";

        const string UPDATE_DOCUMENT = "UPDATE DOCUMENT SET description =@description, " +
                                        "id_userUpdate = @id_userUpdate, dateUpdate =@dateUpdate, " +
                                        "id_documentState = @id_documentState " +
                                        "WHERE id = @id ";

        const string UPDATE_DOCUMENTTYPE_CURRENTNUNBER = "UPDATE DOCUMENTTYPE " +
                                                        "SET currentNumber = @currentNumber, " +
                                                        "dateUpdate= @dateUpdate, " +
                                                        "id_userUpdate= @id_userUpdate " +
                                                        "WHERE id= @id";

        const string SELECT_FISHINGCUSTODIAN = "SELECT id, code,patrol, semiComplete, truckDriver, changeHG, " +
                                               "cabinHR, id_FishingSite, isActive, id_company " +
                                               "FROM fishingcustodian ";



        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustodianIncomePartial()
        {
            var model = (TempData["resultModel"] as CustodianIncomeDto[]);
            model = model ??  Array.Empty<CustodianIncomeDto>();

            TempData.Keep("resultModel");

            return PartialView("_CustodianIncomePartial", model.OrderByDescending(r => r.Id).ToList());
        }

        [HttpPost]
        public ActionResult CustodianIncomeResults(RemissionGuide remissionGuide,
                                                  Document document,
                                                  string carRegistration,
                                                  int? id_ProviderRemisionGuide,
                                                  int? id_driver,
                                                  DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                  DateTime? startDespachureDate, DateTime? endDespachureDate,
                                                  DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                                  DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding)
        {

            string predicate = "";
            int custodianIncomeDocumentStateId = document.id_documentState;
            string custodianIncomeDocumentReference = document.reference;

            #region Search Document Custodian Income
            int[] documentCustodianIncomeIds = null;
            if (custodianIncomeDocumentStateId > 0
                || !string.IsNullOrEmpty(custodianIncomeDocumentReference)
                || startEmissionDate.HasValue
                || endEmissionDate.HasValue
                )
            {
                DocumentType documentoType = db.DocumentType.FirstOrDefault(r => r.code == TIPO_DOCUMENTO_CUSTODIANINCOME_CODIGO);

                var documentCustodianIncome = db.Document.Where(r => r.id_documentType == documentoType.id).AsQueryable();
                if (custodianIncomeDocumentStateId > 0)
                {
                    documentCustodianIncome = documentCustodianIncome.Where(r => r.id_documentState == custodianIncomeDocumentStateId);
                }
                if (!string.IsNullOrEmpty(custodianIncomeDocumentReference))
                {
                    documentCustodianIncome = documentCustodianIncome.Where(r => r.reference.Contains(custodianIncomeDocumentReference));
                }


                if (startEmissionDate.HasValue)
                {
                    documentCustodianIncome = documentCustodianIncome.Where(r => DbFunctions.TruncateTime(r.emissionDate) >= startEmissionDate.Value);
                }
                if (endEmissionDate.HasValue)
                {
                    documentCustodianIncome = documentCustodianIncome.Where(r => DbFunctions.TruncateTime(r.emissionDate) <= endEmissionDate.Value);
                }

                documentCustodianIncomeIds = documentCustodianIncome.Select(r => r.id).ToArray();

                if ((documentCustodianIncomeIds?.Length ?? 0) > 0)
                {
                    predicate += $"{DapperConnection.EvalPred(predicate)} id IN @documentCustodianIncomeIds ";
                }
                else
                {
                    predicate += $"{DapperConnection.EvalPred(predicate)} id IN @documentCustodianIncomeIds ";
                    documentCustodianIncomeIds = new int[] { 0 };
                }
            }

            #endregion

            #region Search Guia de remision
            bool validarGuiasRemision = false;
            int[] guideRemissionIds = null;
            if (!string.IsNullOrEmpty(document.number)
                 || startAuthorizationDate != null
                 || endAuthorizationDate != null
                 || !string.IsNullOrEmpty(document.accessKey)
                 || !string.IsNullOrEmpty(document.authorizationNumber)
                 || startDespachureDate != null
                 || endDespachureDate != null
                 || startexitDateProductionBuilding != null
                 || endexitDateProductionBuilding != null
                 || startentranceDateProductionBuilding != null
                 || endentranceDateProductionBuilding != null
                 || !string.IsNullOrEmpty(carRegistration)
                 || id_ProviderRemisionGuide.HasValue
                 || id_driver.HasValue)
            {
                document.id_documentState = 0;
                document.reference = null;


                string[] codigosStates = new string[] { ESTADO_AUTORIZADO_CODIGO, ESTADO_PRE_AUTORIZADO_CODIGO };
                var documentStates = db.DocumentState.Where(r => codigosStates.Contains(r.code)).Select(r => r.id).ToArray();
                int stateId1 = documentStates[0];
                int stateId2 = documentStates[1];

                document.id_documentState = stateId1;
                List<RGResultsQuery> lstRemissionGuides = ServiceLogistics.GetAllRemissionGuide(remissionGuide,
                                                                    document,
                                                                    carRegistration,
                                                                    null, null,
                                                                    startAuthorizationDate, endAuthorizationDate,
                                                                    startDespachureDate, endDespachureDate,
                                                                    startexitDateProductionBuilding, endexitDateProductionBuilding,
                                                                    startentranceDateProductionBuilding, endentranceDateProductionBuilding,
                                                                    null, null, stateId2,
                                                                    id_ProviderRemisionGuide,
                                                                    id_driver
                                                                    );

                guideRemissionIds = lstRemissionGuides?.Select(r => r.id)?.ToArray();
                //if ((guideRemissionIds?.Length ?? 0) > 0)
                //{
                //    predicate += $"{DapperConnection.EvalPred(predicate)} id_RemissionGuide IN @guideRemissionIds ";
                //}
                //else
                //{
                //    predicate += $"{DapperConnection.EvalPred(predicate)} id_RemissionGuide IN @guideRemissionIds ";
                //    guideRemissionIds = new int[] { 0 };
                //}
                validarGuiasRemision = true;
            }
            #endregion

            var result = DapperConnection.Execute<CustodianIncome>($"{SELECT_CUSTODIANINCOME} {predicate}", new
            {
                //guideRemissionIds = guideRemissionIds,
                documentCustodianIncomeIds = documentCustodianIncomeIds,

            });
            if (validarGuiasRemision)
            {
                if ((guideRemissionIds?.Length ?? 0) > 0)
                {
                    result = result.Where(r => guideRemissionIds.Contains(r.id_RemissionGuide)).ToArray();
                }
                else
                {
                    result = Array.Empty<CustodianIncome>();
                }
            }
            
            

            var resultIncomeCustodian = result.Select(r => fillCustodianIncomeFromData( r, haveTempView: false)).ToArray();
            TempData["resultModel"] = resultIncomeCustodian;
            TempData.Keep("resultModel");

            return PartialView("_CustodianIncomeResultsPartial", resultIncomeCustodian.OrderByDescending(r => r.Id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsPartial()
        {
            List<Models.RemGuide.RGResultsQuery> model = (TempData["modelPod"] as List<Models.RemGuide.RGResultsQuery>);

            TempData.Keep("modelPod");
            return PartialView("_RemissionGuideDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsResults()
        {
            string[] codigosStates = new string[] { ESTADO_AUTORIZADO_CODIGO, ESTADO_PRE_AUTORIZADO_CODIGO };
            var documentStates = db.DocumentState.Where(r => codigosStates.Contains(r.code)).Select(r => r.id).ToArray();
            int stateId1 = documentStates[0];
            int stateId2 = documentStates[1];
            var preResultRemissionGuide = ServiceLogistics.GetAllRemissionGuide(new RemissionGuide(), new Document
            {
                id_documentState = stateId1,
            }, null, null, null, null, null, null,
            null, null, null, null, null, null, null, stateId2, filterCustodianIncome: true);
            var model = preResultRemissionGuide?.Where(r => r.entranceTimePlanctDoc.HasValue).ToList() ?? new List<Models.RemGuide.RGResultsQuery>();

            var documentStateAnulado = db.DocumentState.FirstOrDefault(r => r.code == ESTADO_ANULADO_CODIGO);
            var custodianIncomeList = DapperConnection.Execute<CustodianIncome>(SELECT_CUSTODIANINCOME_GUIAS, new 
            {
                id_documentState = documentStateAnulado.id
            });
            int[] custodianRemissionGuideIds = custodianIncomeList.Select(r => r.id_RemissionGuide).ToArray();

            model = model.Where(r => !custodianRemissionGuideIds.Contains(r.id)).ToList();

            TempData["modelPod"] = model;
            TempData.Keep("modelPod");
            return PartialView("_RemissionGuideDetailsResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCustodianIncome(int id, int? remissionid)
        {
            CustodianIncomeDto model = new CustodianIncomeDto();

            CustodianIncome custodianIncome = DapperConnection.Execute<CustodianIncome>($"{SELECT_CUSTODIANINCOME} WHERE id = {id} ")?.FirstOrDefault();
            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("167"));

            if (custodianIncome == null)
            {
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                if (!remissionid.HasValue)
                {
                    model = new CustodianIncomeDto
                    {
                        Document = new Document
                        {
                            id = 0,
                            id_documentType = documentType?.id ?? 0,
                            DocumentType = documentType,
                            id_documentState = documentState?.id ?? 0,
                            DocumentState = documentState,
                            emissionDate = DateTime.Now
                        },
                        id_RemissionGuide = 0

                    };
                }
                else
                {
                    model = fillCustodianIncomeFromRemissionGuide(remissionid.Value);
                }

            }
            else
            {
                model = fillCustodianIncomeFromData(custodianIncome);
                setFishingCustodian(model);
                //string codeValue1 = null;
                //int? id_FishingSite1 = null;
                //decimal? value1 = null;
                //string codeValue2 = null;
                //int? id_FishingSite2 = null;
                //decimal? value2 = null;
                //if (model.Document.DocumentState.code == ESTADO_APROBADA_CODIGO)
                //{
                //    codeValue1 = model.fishingCustodianField1;
                //    id_FishingSite1 = model.id_FishingSite1;
                //    value1 = model.fishingCustodian1Value;
                //    codeValue2 = model.fishingCustodianField2;
                //    id_FishingSite2 = model.id_FishingSite2;
                //    value2 = model.fishingCustodian2Value;
                //}
                //
                //fillFishingCustodianLValues(    model.id_FishingCustodian1, 
                //                                "fishingCustodianField1", 
                //                                "id_FishingCustodian1",
                //                                codeValue: codeValue1, 
                //                                id_FishingSite: id_FishingSite1, 
                //                                Value: value1);
                //if (model.id_FishingCustodian2.HasValue)
                //{
                //    fillFishingCustodianLValues(    model.id_FishingCustodian2.Value, 
                //                                    "fishingCustodianField2", 
                //                                    "id_FishingCustodian2",
                //                                    codeValue: codeValue2,
                //                                    id_FishingSite: id_FishingSite2,
                //                                    Value: value2);
                //}

            }


            return PartialView("_FormEditCustodianIncome", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Save(bool approve, CustodianIncome custodianIncome, Document document, DateTime emissionDate)
        {
            CustodianIncome toSave = new CustodianIncome();
            CustodianIncomeDto toViewDto = new CustodianIncomeDto();
            Document modelDocument = new Document();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string actioname = "";
            try
            {
                var userId = this.ActiveUserId;

                if ((custodianIncome.id_PersonCompanyCustodian2.HasValue
                    && custodianIncome.id_FishingSite2.HasValue
                    && !string.IsNullOrEmpty(custodianIncome.fishingCustodianField2))
                    &&
                    (custodianIncome.id_PersonCompanyCustodian1 == 0
                    && custodianIncome.id_FishingSite2 == 0
                    && string.IsNullOrEmpty(custodianIncome.fishingCustodianField1))
                    )
                {
                    custodianIncome.id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian2.Value;
                    custodianIncome.id_FishingSite1 = custodianIncome.id_FishingSite2.Value;
                    custodianIncome.fishingCustodianField1 = custodianIncome.fishingCustodianField2;

                    custodianIncome.id_PersonCompanyCustodian2 = null;
                    custodianIncome.id_FishingSite2 = null;
                    custodianIncome.fishingCustodianField2 = null;

                }

                var fishingCustodianValueId1 = DapperConnection.Execute<FishinngCustodianFieldValue>($"SELECT TOP 1 id_FishingSite,{custodianIncome.fishingCustodianField1} FieldValue FROM FishingCustodian WHERE Id = {custodianIncome.id_FishingCustodian1}")?.FirstOrDefault();
                if (custodianIncome.id_FishingCustodian2.HasValue)
                {
                    var fishingCustodianValueId2 = DapperConnection.Execute<FishinngCustodianFieldValue>($"SELECT TOP 1 id_FishingSite,{custodianIncome.fishingCustodianField2} FieldValue FROM FishingCustodian WHERE Id = {custodianIncome.id_FishingCustodian2}")?.FirstOrDefault();
                    custodianIncome.id_FishingSite2 = fishingCustodianValueId2.id_FishingSite;
                    custodianIncome.fishingCustodianValue2 = fishingCustodianValueId2.FieldValue;
                }

                custodianIncome.id_FishingSite1 = fishingCustodianValueId1.id_FishingSite;
                custodianIncome.fishingCustodianValue1 = fishingCustodianValueId1.FieldValue;

                DocumentState _documentState = null;
                if (approve)
                {
                    actioname = "aprobado";
                    // Validar estado de la guia de remision

                    string[] codigosStates = new string[] { ESTADO_AUTORIZADO_CODIGO, ESTADO_PRE_AUTORIZADO_CODIGO };
                    int[] documentStates = db.DocumentState.Where(r => codigosStates.Contains(r.code)).Select(r => r.id).ToArray();

                    Document documentoRemissionGuide = db.Document.FirstOrDefault(r => r.id == custodianIncome.id_RemissionGuide);
                    if (!documentStates.Contains(documentoRemissionGuide.id_documentState))
                    {
                        throw new Exception($"La gu&iacute;a de remisi&oacute;n {documentoRemissionGuide.number}, no esta en estado AUTORIZADO &oacute; PRE-AUTORIZADO");
                    }

                    _documentState = db.DocumentState
                                                .FirstOrDefault(r => r.code == "03");
                }
                else
                {
                    actioname = "guardado";
                    _documentState = db.DocumentState
                                                .FirstOrDefault(r => r.code == "01");
                }

                if (custodianIncome.Id == 0)
                {
                    #region -- Document --
                    var _documentType = db.DocumentType
                                                .FirstOrDefault(r => r.code == "167");

                    var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                                 ? ActiveUser.EmissionPoint.First().id
                                                 : 0;

                    int sequential = GetDocumentSequential(_documentType?.id ?? 0);
                    _documentType.currentNumber = _documentType.currentNumber + 1;
                    modelDocument = new Document
                    {
                        number = GetDocumentNumber(_documentType?.id ?? 0),
                        sequential = sequential,
                        id_emissionPoint = id_emissionPoint,
                        id_documentState = _documentState.id,
                        DocumentState = _documentState,
                        id_documentType = _documentType.id,
                        DocumentType = _documentType,
                        dateCreate = DateTime.Now,
                        id_userCreate = userId,
                        dateUpdate = DateTime.Now,
                        id_userUpdate = userId,
                        emissionDate = emissionDate,
                        description = document.description
                    };
                    #endregion
                    toSave = custodianIncome;
                    toSave.Document = modelDocument;
                }
                else
                {
                    toSave = DapperConnection.Execute<CustodianIncome>($"{SELECT_CUSTODIANINCOME} WHERE ID={custodianIncome.Id}")?.FirstOrDefault();
                    toSave.Document = new Document
                    {
                        id = custodianIncome.Id,
                        description = document.description,
                        id_documentState = _documentState.id,
                        dateUpdate = DateTime.Now,
                        id_userUpdate = userId,
                        DocumentState = _documentState

                    };

                    toSave.id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian1;
                    toSave.id_FishingSite1 = custodianIncome.id_FishingSite1;
                    toSave.fishingCustodianField1 = custodianIncome.fishingCustodianField1;
                    toSave.id_FishingCustodian1 = custodianIncome.id_FishingCustodian1;
                    toSave.fishingCustodianValue1 = custodianIncome.fishingCustodianValue1;

                    toSave.id_PersonCompanyCustodian2 = custodianIncome.id_PersonCompanyCustodian2;
                    toSave.id_FishingSite2 = custodianIncome.id_FishingSite2;
                    toSave.fishingCustodianField2 = custodianIncome.fishingCustodianField2;
                    toSave.id_FishingCustodian2 = custodianIncome.id_FishingCustodian2;
                    toSave.fishingCustodianValue2 = custodianIncome.fishingCustodianValue2;

                    if (approve)
                    {
                        // actualizar los datos de la guia de remision
                        var custodianRemissionGuideInfo =   fillCustodianIncomeFromRemissionGuide(custodianIncome.id_RemissionGuide, haveTempView: false);
                        toSave.remissionGuideProviderName = custodianRemissionGuideInfo.remissionGuideProviderName;
                        toSave.remissionGuideProductionUnitProviderName = custodianRemissionGuideInfo.remissionGuideProductionUnitProviderName;
                        toSave.remissionGuidePoolReference = custodianRemissionGuideInfo.remissionGuidePoolReference;
                        toSave.remissionGuidePoundTotal = custodianRemissionGuideInfo.remissionGuidePoundTotal;
                        toSave.remissionGuideFishingZoneName = custodianRemissionGuideInfo.remissionGuideFishingZoneName;
                        toSave.remissionGuideFishingSiteName = custodianRemissionGuideInfo.remissionGuideFishingSiteName;
                        toSave.remissionGuideRoute = custodianRemissionGuideInfo.remissionGuideRoute;
                        toSave.remissionGuideShippingTypeName = custodianRemissionGuideInfo.remissionGuideShippingTypeName;
                        toSave.remissionGuidesDriverName = custodianRemissionGuideInfo.remissionGuidesDriverName;
                        toSave.remissionGuidesProcessPlant = custodianRemissionGuideInfo.remissionGuidesProcessPlant;
                        toSave.remissionGuidesProviderTransportName = custodianRemissionGuideInfo.remissionGuidesProviderTransportName;
                        toSave.remissionGuidesCarRegistration = custodianRemissionGuideInfo.remissionGuidesCarRegistration;
                        toSave.remissionGuidesTransportBillingName = custodianRemissionGuideInfo.remissionGuidesTransportBillingName;
                        toSave.remissionGuidesTransportValuePrice = custodianRemissionGuideInfo.remissionGuidesTransportValuePrice;
                    }
                }


                transacction(toSave);
                ViewData["EditMessage"] = SuccessMessage($"Ingreso de Custodio {actioname} exitosamente");

                toViewDto = fillCustodianIncome(toSave);
                setFishingCustodian(toViewDto);

            }
            catch (Exception e)
            {
                toViewDto = (TempData["custodianincome"] as CustodianIncomeDto);
                TempData.Keep("custodianincome");
                MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "CUSTODIAINCOME_SAVE", "PROD");
                ViewData["EditMessage"] = ErrorMessage(e.Message);
            }

            return PartialView("_CustodianIncomeMainFormPartial", toViewDto);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Revert(int id)
        {
            CustodianIncomeDto toViewDto = new CustodianIncomeDto();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            try
            {


                Document document = db.Document.Include("DocumentState").FirstOrDefault(r => r.id == id);
                if (document == null)
                {
                    throw new Exception("El documento indicado no existe");
                }
                if (document.DocumentState.code != ESTADO_APROBADA_CODIGO)
                {
                    throw new Exception("El documento indicado no se puede reversar");
                }

                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == ESTADO_PENDIENTE_CODIGO);
                document.DocumentState = documentState;
                document.id_documentState = documentState.id;
                document.id_userUpdate = this.ActiveUserId;
                document.dateUpdate = DateTime.Now;

                transacction(new CustodianIncome
                {
                    Document = document
                }, onlyUpdateDocument: true);

                CustodianIncome custodianIncome = DapperConnection.Execute<CustodianIncome>($"{SELECT_CUSTODIANINCOME} WHERE id = {id} ")?.FirstOrDefault();
                toViewDto = fillCustodianIncome(custodianIncome);
                ViewData["EditMessage"] = SuccessMessage($"Ingreso de Custodio reversado exitosamente");
            }
            catch (Exception e)
            {
                toViewDto = (TempData["custodianincome"] as CustodianIncomeDto);
                TempData.Keep("custodianincome");
                MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "CUSTODIAINCOME_REVERT", "PROD");
                ViewData["EditMessage"] = ErrorMessage(e.Message);
            }

            return PartialView("_CustodianIncomeMainFormPartial", toViewDto);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Cancel(int id)
        {
            CustodianIncomeDto toViewDto = new CustodianIncomeDto();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            try
            {
                Document document = db.Document.Include("DocumentState").FirstOrDefault(r => r.id == id);
                if (document == null)
                {
                    throw new Exception("El documento indicado no existe");
                }
                if (document.DocumentState.code != ESTADO_PENDIENTE_CODIGO)
                {
                    throw new Exception("El documento indicado no se puede anular");
                }

                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == ESTADO_ANULADO_CODIGO);
                document.DocumentState = documentState;
                document.id_documentState = documentState.id;
                document.id_userUpdate = this.ActiveUserId;
                document.dateUpdate = DateTime.Now;

                transacction(new CustodianIncome
                {
                    Document = document
                }, onlyUpdateDocument: true);
                CustodianIncome custodianIncome = DapperConnection.Execute<CustodianIncome>($"{SELECT_CUSTODIANINCOME} WHERE id = {id} ")?.FirstOrDefault();
                toViewDto = fillCustodianIncome(custodianIncome);
                ViewData["EditMessage"] = SuccessMessage($"Ingreso de Custodio anulado exitosamente");
            }
            catch (Exception e)
            {
                toViewDto = (TempData["custodianincome"] as CustodianIncomeDto);
                TempData.Keep("custodianincome");
                MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "CUSTODIAINCOME_CANCEL", "PROD");
                ViewData["EditMessage"] = ErrorMessage(e.Message);
            }
            return PartialView("_CustodianIncomeMainFormPartial", toViewDto);
        }

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            TempData.Keep("custodianincome");
            var actions = new
            {
                btnApprove = false,
                btnCancel = false,
                btnRevert = false
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }
            Document documentCustodianIncome = db.Document.Include("DocumentState").FirstOrDefault(r => r.id == id);
            string state = documentCustodianIncome.DocumentState.code;

            if (state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnCancel = true,
                    btnRevert = false
                };
            }
            else if (state == "03") // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnCancel = false,
                    btnRevert = true
                };
            }
            else if (state == "05") // ANULADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnCancel = false,
                    btnRevert = false
                };
            }


            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Combo Cascada
        private string fielNameFromCodeValue(string codeValue)
        {
            string fieldName = "";
            switch (codeValue)
            {
                case "patrol":
                    fieldName = "Patrulla";
                    break;
                case "semiComplete":
                    fieldName = "Semi Completa";
                    break;
                case "truckDriver":
                    fieldName = "Camioneta";
                    break;
                case "changeHG":
                    fieldName = "H/Cambina Gye";
                    break;
                case "cabinHR":
                    fieldName = "H/Cabina Ruta ";
                    break;

            }
            return fieldName;
        }
        private void fillFishingCustodianLValues(int id_FishingCustodian, string controlName, string controlDependName, string codeValue= null, int? id_FishingSite = null, decimal? Value = null )
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                TempData[$"FishingCustodianValues_{id_FishingCustodian}_{controlName}"] = new FishingCustodianValueDto[]
                {
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = (id_FishingSite??0),
                        ControlName = controlName,
                        ControlDependName = controlDependName,
                        CodeValue = codeValue,
                        NameValue = $"{fielNameFromCodeValue(codeValue)} | {(Value??0).ToCurrencyFormat()}"
                    }
                };
                TempData.Keep($"FishingCustodianValues_{id_FishingCustodian}_{controlName}");
            }
            else
            {
                var fishingCustodian = DapperConnection.Execute<FishingCustodian>($"{SELECT_FISHINGCUSTODIAN} WHERE id = {id_FishingCustodian}")?.FirstOrDefault();
                if (fishingCustodian != null)
                {
                    TempData[$"FishingCustodianValues_{id_FishingCustodian}_{controlName}"] = new FishingCustodianValueDto[]
                    {
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = fishingCustodian.id_FishingSite,
                        ControlName = controlName,
                        ControlDependName = controlDependName,
                        CodeValue = "patrol",
                        NameValue = $"Patrulla | {fishingCustodian.patrol.ToCurrencyFormat()}"
                    },
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = fishingCustodian.id_FishingSite,
                        ControlDependName = controlDependName,
                        ControlName = controlName,
                        CodeValue = "semiComplete",
                        NameValue = $"Semi Completa | {fishingCustodian.semiComplete.ToCurrencyFormat()}"
                    },
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = fishingCustodian.id_FishingSite,
                        ControlDependName = controlDependName,
                        ControlName = controlName,
                        CodeValue = "truckDriver",
                        NameValue = $"Camioneta | {fishingCustodian.truckDriver.ToCurrencyFormat()}"
                    },
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = fishingCustodian.id_FishingSite,
                        ControlName = controlName,
                        ControlDependName = controlDependName,
                        CodeValue = "changeHG",
                        NameValue = $"H/Cambina Gye | {fishingCustodian.changeHG.ToCurrencyFormat()}"
                    },
                    new FishingCustodianValueDto
                    {
                        FishingCustodianId = id_FishingCustodian,
                        FishingSiteId = fishingCustodian.id_FishingSite,
                        ControlName = controlName,
                        ControlDependName = controlDependName,
                        CodeValue = "cabinHR",
                        NameValue = $"H/Cabina Ruta | {fishingCustodian.cabinHR.ToCurrencyFormat()}"
                    }
                    };

                    TempData.Keep($"FishingCustodianValues_{id_FishingCustodian}_{controlName}");
                }
            }
            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingCustodianValues(int? id_FishingCustodian, string controlName, string controlDependName)
        {
            fillFishingCustodianLValues((id_FishingCustodian ?? 0), controlName, controlDependName);
            return PartialView("LogisticShared/comboboxcascading/_cmbFishingCustodiaValue", new FishingCustodianValueDto
            {
                FishingCustodianId = (id_FishingCustodian ?? 0),
                CodeValue = null,
                ControlName = controlName,
                ControlDependName = controlDependName
            });
        }
        #endregion

        #region Fill Custodian Model
        private CustodianIncomeDto fillCustodianIncomeFromRemissionGuide(int id_RemissionGuide, bool haveTempView = true) 
        {
            Document document = db.Document
                                    .Include("DocumentType")
                                    .Include("DocumentState")
                                    .FirstOrDefault(r => r.id == id_RemissionGuide);
            DocumentState documentState = new DocumentState
            {
                name = "Pendiente",
                code = "01",
            };
            DateTime dateEmission = DateTime.Now;
            string descripcion = null;
             
            document.DocumentState = documentState;
            document.emissionDate = dateEmission;
            document.description = descripcion;

            CustodianIncomeDto model = new CustodianIncomeDto
            {

                Document = document,
                id_RemissionGuide = id_RemissionGuide,
                fishingCustodianField1 = null,
                fishingCustodianField2 = null,
                id_FishingSite1 = 0,
                id_FishingSite2 = null,
                id_PersonCompanyCustodian1 = 0,
                id_PersonCompanyCustodian2 = null,
                id_FishingCustodian1 = 0,
                id_FishingCustodian2 = null
            };
            
             

            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id_RemissionGuide);
            if (remissionGuide.id_providerRemisionGuide.HasValue)
            {
                model.remissionGuideProviderName = db.Person.FirstOrDefault(r => r.id == remissionGuide.id_providerRemisionGuide.Value)?.fullname_businessName;
            }
            if (remissionGuide.id_productionUnitProvider.HasValue)
            {
                model.remissionGuideProductionUnitProviderName = db.ProductionUnitProvider.FirstOrDefault(r => r.id == remissionGuide.id_productionUnitProvider.Value)?.name;
            }

            if (remissionGuide.id_personProcessPlant.HasValue)
            {
                model.remissionGuidesProcessPlant = db.Person.FirstOrDefault(r => r.id == remissionGuide.id_personProcessPlant.Value)?.processPlant;
            }

            model.remissionGuideRoute = remissionGuide.route;

            if (remissionGuide.id_shippingType.HasValue)
            {
                model.remissionGuideShippingTypeName = db.PurchaseOrderShippingType.FirstOrDefault(r => r.id == remissionGuide.id_shippingType.Value)?.name;
            }

            var prePiscinas = db.RemissionGuideDetail
                                .Where(r => r.id_remisionGuide == id_RemissionGuide)?
                                .Select(r => r.productionUnitProviderPoolreference)?
                                .ToArray();
            model.remissionGuidePoolReference = prePiscinas?.Aggregate((i, j) => i + j);

            model.remissionGuidePoundTotal = db.RemissionGuideDetail
                                        .Where(r => r.id_remisionGuide == id_RemissionGuide)
                                        .Sum(r => r.quantityOrdered);

            var transportation = db.RemissionGuideTransportation
                                            .FirstOrDefault(r => r.id_remionGuide == id_RemissionGuide);
            model.remissionGuidesCarRegistration = transportation.carRegistration;

            model.remissionGuidesTransportValuePrice = (transportation.valuePrice ?? 0);

            int? sitioPescaId = transportation?.id_FishingSiteRG;
            if (sitioPescaId.HasValue)
            {
                var sitioPesca = db.FishingSite.FirstOrDefault(r => r.id == sitioPescaId.Value);
                model.remissionGuideFishingSiteName = sitioPesca?.name;
                if (sitioPesca.id_FishingZone.HasValue)
                {
                    model.remissionGuideFishingZoneName = db.FishingZone.FirstOrDefault(r => r.id == sitioPesca.id_FishingZone.Value)?.name;
                }
            }
            if (transportation.id_DriverVeicleProviderTransport.HasValue)
            {
                int? proveedorTransporteId = db.DriverVeicleProviderTransport.FirstOrDefault(r => r.id == transportation.id_DriverVeicleProviderTransport.Value)?.idVeicleProviderTransport;
                if (proveedorTransporteId.HasValue)
                {
                    int? proveerPersonId = db.VeicleProviderTransport.FirstOrDefault(r => r.id == proveedorTransporteId.Value)?.id_Provider;
                    if (proveerPersonId.HasValue)
                    {
                        model.remissionGuidesProviderTransportName = db.Person.FirstOrDefault(r => r.id == proveerPersonId.Value)?.fullname_businessName;
                    }

                }
            }

            if (transportation.id_VehicleProviderTranportistBilling.HasValue)
            {
                int? proveedorTransporteId = db.VehicleProviderTransportBilling.FirstOrDefault(r => r.id == transportation.id_VehicleProviderTranportistBilling.Value)?.id_provider;
                if (proveedorTransporteId.HasValue)
                {
                    Person personBilling = db.Person.FirstOrDefault(r => r.id == proveedorTransporteId.Value);
                    if (personBilling != null)
                    {
                        model.remissionGuidesTransportBillingName = $"{personBilling.identification_number} - {personBilling.fullname_businessName}";
                    }

                }
            }

            model.remissionGuidesDriverName = transportation?.driverName;
            if (haveTempView)
            {
                TempData["custodianincome"] = model;
                TempData.Keep("custodianincome");
            }
            

            return model;
        }

        private CustodianIncomeDto fillCustodianIncomeFromData( CustodianIncome custodianIncome, bool haveTempView = true)
        {
            int remissionid = custodianIncome.id_RemissionGuide;
            var  documentoRemissionGuide = db.Document
                                    .Include("DocumentType")
                                    .FirstOrDefault(r => r.id == custodianIncome.id_RemissionGuide);
            Document document = new Document
            {
                accessKey = documentoRemissionGuide.accessKey,
                authorizationDate = documentoRemissionGuide.authorizationDate,
                authorizationNumber = documentoRemissionGuide.authorizationNumber,
                number = documentoRemissionGuide.number,
                sequential = documentoRemissionGuide.sequential,
                reference = documentoRemissionGuide.reference,
                id_emissionPoint = documentoRemissionGuide.id_emissionPoint,
                id_documentType = documentoRemissionGuide.id_documentType,
                DocumentType = new DocumentType
                {
                    id = documentoRemissionGuide.DocumentState.id,
                    code = documentoRemissionGuide.DocumentState.code,
                    name = documentoRemissionGuide.DocumentState.name,
                    description = documentoRemissionGuide.DocumentState.description
                }
            };
            DocumentState documentState = new DocumentState
            {
                name = "Pendiente",
                code = "01",
            };
            DateTime dateEmission = DateTime.Now;
            string descripcion = null;
            if (custodianIncome.Id != 0)
            {
                Document documentCustodianIncome = db.Document
                                                        .Include("DocumentState")
                                                        .FirstOrDefault(r => r.id == custodianIncome.Id);
                documentState = new DocumentState
                {
                    id = documentCustodianIncome.DocumentState.id,
                    code = documentCustodianIncome.DocumentState.code,
                    name = documentCustodianIncome.DocumentState.name,
                    description = documentCustodianIncome.DocumentState.description
                };
                dateEmission = documentCustodianIncome.emissionDate;
                descripcion = documentCustodianIncome.description;
            }
            document.DocumentState = documentState;
            document.emissionDate = dateEmission;
            document.description = descripcion;

            CustodianIncomeDto model = new CustodianIncomeDto
            {
                Id = custodianIncome.Id,
                Document = document,
                id_RemissionGuide = remissionid,
                fishingCustodianField1 = custodianIncome.fishingCustodianField1,
                fishingCustodianField2 = custodianIncome.fishingCustodianField2,
                id_FishingSite1 = custodianIncome.id_FishingSite1,
                id_FishingSite2 = custodianIncome.id_FishingSite2,
                id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian1,
                id_PersonCompanyCustodian2 = custodianIncome.id_PersonCompanyCustodian2,
                id_FishingCustodian1 = custodianIncome.id_FishingCustodian1,
                id_FishingCustodian2 = custodianIncome.id_FishingCustodian2,
                fishingCustodian1Value = custodianIncome.fishingCustodianValue1,
                fishingCustodian2Value = custodianIncome.fishingCustodianValue2,
                remissionGuidesTransportBillingName = custodianIncome.remissionGuidesTransportBillingName,
                remissionGuidesProviderTransportName = custodianIncome.remissionGuidesProviderTransportName,
                remissionGuideRoute = custodianIncome.remissionGuideRoute,
                remissionGuideProductionUnitProviderName = custodianIncome.remissionGuideProductionUnitProviderName,
                remissionGuidesDriverName = custodianIncome.remissionGuidesDriverName,
                remissionGuidesProcessPlant = custodianIncome.remissionGuidesProcessPlant,
                remissionGuideProviderName = custodianIncome.remissionGuideProviderName,
                remissionGuideFishingSiteName = custodianIncome.remissionGuideFishingSiteName,
                remissionGuideShippingTypeName = custodianIncome.remissionGuideShippingTypeName,
                remissionGuideFishingZoneName = custodianIncome.remissionGuideFishingZoneName,
                remissionGuidePoolReference = custodianIncome.remissionGuidePoolReference,
                remissionGuidePoundTotal = custodianIncome.remissionGuidePoundTotal,
                remissionGuidesTransportValuePrice = custodianIncome.remissionGuidesTransportValuePrice,
                remissionGuidesCarRegistration = custodianIncome.remissionGuidesCarRegistration
                 
            };

            if (custodianIncome.id_PersonCompanyCustodian1 != 0)
            {
                Person person1 = db.Person.FirstOrDefault(r => r.id == custodianIncome.id_PersonCompanyCustodian1);
                model.PersonCompanyCustodian1 = person1;
            }
            if (custodianIncome.id_PersonCompanyCustodian2.HasValue)
            {
                Person person2 = db.Person.FirstOrDefault(r => r.id == custodianIncome.id_PersonCompanyCustodian2);
                model.PersonCompanyCustodian2 = person2;
            }
            model.fishingCustodianField1Descrip = !string.IsNullOrEmpty(custodianIncome.fishingCustodianField1) ? FishinngCustodianFieldConvert.GetDescrip(custodianIncome.fishingCustodianField1) : "";
            model.fishingCustodianField2Descrip = !string.IsNullOrEmpty(custodianIncome.fishingCustodianField2) ? FishinngCustodianFieldConvert.GetDescrip(custodianIncome.fishingCustodianField2) : "";

            if (custodianIncome.id_FishingSite1 != 0)
            {
                FishingSite fishingSite = db.FishingSite.FirstOrDefault(r => r.id == custodianIncome.id_FishingSite1);
                model.FishingSite1 = fishingSite;
            }
            if (custodianIncome.id_FishingSite2 != 0)
            {
                FishingSite fishingSite = db.FishingSite.FirstOrDefault(r => r.id == custodianIncome.id_FishingSite2);
                model.FishingSite2 = fishingSite;
            }

            if (haveTempView)
            {
                TempData["custodianincome"] = model;
                TempData.Keep("custodianincome");
            }

            return model;
        }
        private void setFishingCustodian(CustodianIncomeDto custodianIncome)
        {
            string codeValue1 = null;
            int? id_FishingSite1 = null;
            decimal? value1 = null;
            string codeValue2 = null;
            int? id_FishingSite2 = null;
            decimal? value2 = null;
            if (custodianIncome.Document.DocumentState.code == ESTADO_APROBADA_CODIGO)
            {
                codeValue1 = custodianIncome.fishingCustodianField1;
                id_FishingSite1 = custodianIncome.id_FishingSite1;
                value1 = custodianIncome.fishingCustodian1Value;
                codeValue2 = custodianIncome.fishingCustodianField2;
                id_FishingSite2 = custodianIncome.id_FishingSite2;
                value2 = custodianIncome.fishingCustodian2Value;
            }

            fillFishingCustodianLValues(    custodianIncome.id_FishingCustodian1,
                                            "fishingCustodianField1",
                                            "id_FishingCustodian1",
                                            codeValue: codeValue1,
                                            id_FishingSite: id_FishingSite1,
                                            Value: value1);
            if (custodianIncome.id_FishingCustodian2.HasValue)
            {
                fillFishingCustodianLValues(    custodianIncome.id_FishingCustodian2.Value,
                                                "fishingCustodianField2",
                                                "id_FishingCustodian2",
                                                codeValue: codeValue2,
                                                id_FishingSite: id_FishingSite2,
                                                Value: value2);
            }
        }
        #endregion
        private CustodianIncomeDto fillCustodianIncome(CustodianIncome custodianIncome)
        {
            int remissionid = custodianIncome.id_RemissionGuide;
            Document document = db.Document
                                    .Include("DocumentType")
                                    .Include("DocumentState")
                                    .FirstOrDefault(r => r.id == custodianIncome.id_RemissionGuide);
            DocumentState documentState = new DocumentState
            {
               name = "Pendiente",
               code = "01",
            };
            DateTime dateEmission = DateTime.Now;
            string descripcion = null;
            if (custodianIncome.Id != 0)
            {
                Document documentCustodianIncome = db.Document
                                                        .Include("DocumentState")
                                                        .FirstOrDefault(r => r.id == custodianIncome.Id);
                documentState = documentCustodianIncome.DocumentState;
                dateEmission = documentCustodianIncome.emissionDate;
                descripcion = documentCustodianIncome.description;
            }
            document.DocumentState = documentState;
            document.emissionDate = dateEmission;
            document.description = descripcion;

            CustodianIncomeDto model = new CustodianIncomeDto
            {
                Id = custodianIncome.Id,
                Document = document,
                id_RemissionGuide = remissionid,
                fishingCustodianField1 = custodianIncome.fishingCustodianField1,
                fishingCustodianField2 = custodianIncome.fishingCustodianField2,
                id_FishingSite1 = custodianIncome.id_FishingSite1,
                id_FishingSite2 = custodianIncome.id_FishingSite2,
                id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian1,
                id_PersonCompanyCustodian2 = custodianIncome.id_PersonCompanyCustodian2,
                id_FishingCustodian1 = custodianIncome.id_FishingCustodian1,
                id_FishingCustodian2 = custodianIncome.id_FishingCustodian2,
                fishingCustodian1Value = custodianIncome.fishingCustodianValue1,
                fishingCustodian2Value = custodianIncome.fishingCustodianValue2
            };

            if (custodianIncome.id_PersonCompanyCustodian1 !=0)
            {
                Person person1 = db.Person.FirstOrDefault(r => r.id == custodianIncome.id_PersonCompanyCustodian1);
                model.PersonCompanyCustodian1 = person1;
            }
            if (custodianIncome.id_PersonCompanyCustodian2.HasValue)
            {
                Person person2 = db.Person.FirstOrDefault(r => r.id == custodianIncome.id_PersonCompanyCustodian2);
                model.PersonCompanyCustodian2 = person2;
            }
            model.fishingCustodianField1Descrip = !string.IsNullOrEmpty(custodianIncome.fishingCustodianField1) ? FishinngCustodianFieldConvert.GetDescrip(custodianIncome.fishingCustodianField1) : "";
            model.fishingCustodianField2Descrip = !string.IsNullOrEmpty(custodianIncome.fishingCustodianField2) ? FishinngCustodianFieldConvert.GetDescrip(custodianIncome.fishingCustodianField2) : "";

            if (custodianIncome.id_FishingSite1 != 0)
            {
                FishingSite fishingSite = db.FishingSite.FirstOrDefault(r => r.id == custodianIncome.id_FishingSite1);
                model.FishingSite1 = fishingSite;
            }
            if (custodianIncome.id_FishingSite2 != 0)
            {
                FishingSite fishingSite = db.FishingSite.FirstOrDefault(r => r.id == custodianIncome.id_FishingSite2);
                model.FishingSite2 = fishingSite;
            }


            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == remissionid);
            if (remissionGuide.id_providerRemisionGuide.HasValue)
            {
                model.remissionGuideProviderName = db.Person.FirstOrDefault(r => r.id == remissionGuide.id_providerRemisionGuide.Value)?.fullname_businessName;
            }
            if (remissionGuide.id_productionUnitProvider.HasValue)
            {
                model.remissionGuideProductionUnitProviderName = db.ProductionUnitProvider.FirstOrDefault(r => r.id == remissionGuide.id_productionUnitProvider.Value)?.name;
            }

            if (remissionGuide.id_personProcessPlant.HasValue)
            {
                model.remissionGuidesProcessPlant = db.Person.FirstOrDefault(r => r.id == remissionGuide.id_personProcessPlant.Value)?.processPlant;
            }

            model.remissionGuideRoute = remissionGuide.route;

            if (remissionGuide.id_shippingType.HasValue)
            {
                model.remissionGuideShippingTypeName = db.PurchaseOrderShippingType.FirstOrDefault(r => r.id == remissionGuide.id_shippingType.Value)?.name;
            }

            var prePiscinas = db.RemissionGuideDetail
                                .Where(r => r.id_remisionGuide == remissionid)?
                                .Select(r => r.productionUnitProviderPoolreference)?
                                .ToArray();
            model.remissionGuidePoolReference = prePiscinas?.Aggregate((i, j) => i + j);

            model.remissionGuidePoundTotal = db.RemissionGuideDetail
                                                    .Where(r => r.id_remisionGuide == remissionid)
                                                    .Sum(r => r.quantityOrdered);

            var transportation = db.RemissionGuideTransportation
                                            .FirstOrDefault(r => r.id_remionGuide == remissionid);
            model.remissionGuidesCarRegistration = transportation.carRegistration;

            model.remissionGuidesTransportValuePrice = (transportation.valuePrice ?? 0);

            int? sitioPescaId = transportation?.id_FishingSiteRG;
            if (sitioPescaId.HasValue)
            {
                var sitioPesca = db.FishingSite.FirstOrDefault(r => r.id == sitioPescaId.Value);
                model.remissionGuideFishingSiteName = sitioPesca?.name;
                if (sitioPesca.id_FishingZone.HasValue)
                {
                    model.remissionGuideFishingZoneName = db.FishingZone.FirstOrDefault(r => r.id == sitioPesca.id_FishingZone.Value)?.name;
                }
            }
            if (transportation.id_DriverVeicleProviderTransport.HasValue)
            {
                int? proveedorTransporteId = db.DriverVeicleProviderTransport.FirstOrDefault(r => r.id == transportation.id_DriverVeicleProviderTransport.Value)?.idVeicleProviderTransport;
                if (proveedorTransporteId.HasValue)
                {
                    int? proveerPersonId = db.VeicleProviderTransport.FirstOrDefault(r => r.id == proveedorTransporteId.Value)?.id_Provider;
                    if (proveerPersonId.HasValue)
                    {
                        model.remissionGuidesProviderTransportName = db.Person.FirstOrDefault(r => r.id == proveerPersonId.Value)?.fullname_businessName;
                    }

                }
            }

            if (transportation.id_VehicleProviderTranportistBilling.HasValue)
            {
                int? proveedorTransporteId = db.VehicleProviderTransportBilling.FirstOrDefault(r => r.id == transportation.id_VehicleProviderTranportistBilling.Value)?.id_provider;
                if (proveedorTransporteId.HasValue)
                {
                    Person personBilling = db.Person.FirstOrDefault(r => r.id == proveedorTransporteId.Value);
                    if (personBilling != null)
                    {
                        model.remissionGuidesTransportBillingName = $"{personBilling.identification_number} - {personBilling.fullname_businessName}";
                    }

                }
            }

            model.remissionGuidesDriverName = transportation?.driverName;

            TempData["custodianincome"] = model;
            TempData.Keep("custodianincome");

            return model;
        }
        private void transacction(CustodianIncome custodianIncome, bool onlyUpdateDocument = false)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            {
                try
                {
                    db.Open();
                    using (var tr = db.BeginTransaction())
                    {
                        try
                        {
                            if (onlyUpdateDocument)
                            {
                                DapperConnection.Transaction<Document>(custodianIncome.Document, actionDB: updateDocument, connection: db, transaction: tr);
                            }
                            else
                            {
                                if (custodianIncome.Id == 0)
                                {
                                    int documentId = DapperConnection.Transaction<Document>(custodianIncome.Document, functionDB: insertDocument, connection: db, transaction: tr);
                                    custodianIncome.Id = documentId;
                                    DapperConnection.Transaction<CustodianIncome>(custodianIncome, actionDB: insertCustodianIncome, connection: db, transaction: tr);
                                    DapperConnection.Transaction<DocumentType>(custodianIncome.Document.DocumentType, actionDB: updateDocumentType, connection: db, transaction: tr);
                                }
                                else
                                {
                                    DapperConnection.Transaction<Document>(custodianIncome.Document, actionDB: updateDocument, connection: db, transaction: tr);
                                    DapperConnection.Transaction<CustodianIncome>(custodianIncome, actionDB: updateCustodianIncome, connection: db, transaction: tr);
                                }
                            }
                            
                            
                            tr.Commit();
                        }
                        catch (Exception e)
                        {
                            tr.Rollback();
                            throw;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }
        private void insertCustodianIncome(CustodianIncome custodianIncome, SqlConnection connection, SqlTransaction transaction)
        {
            connection.Execute(INSERT_CUSTODIANINCOME
                , new 
                {   id = custodianIncome.Id,
                    id_RemissionGuide = custodianIncome.id_RemissionGuide,
                    id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian1,
                    id_PersonCompanyCustodian2 = custodianIncome.id_PersonCompanyCustodian2,
                    id_FishingSite1 = custodianIncome.id_FishingSite1,
                    id_FishingSite2 = custodianIncome.id_FishingSite2,
                    id_FishingCustodian1 = custodianIncome.id_FishingCustodian1,
                    id_FishingCustodian2 = custodianIncome.id_FishingCustodian2,
                    fishingCustodianField1 =    custodianIncome.fishingCustodianField1,
                    fishingCustodianField2 = custodianIncome.fishingCustodianField2,
                    fishingCustodianValue1 = custodianIncome.fishingCustodianValue1,
                    fishingCustodianValue2 = custodianIncome.fishingCustodianValue2,
                    remissionGuideProviderName = custodianIncome.remissionGuideProviderName,
                    remissionGuideProductionUnitProviderName = custodianIncome.remissionGuideProductionUnitProviderName,
                    remissionGuidePoolReference = custodianIncome.remissionGuidePoolReference,
                    remissionGuidePoundTotal = custodianIncome.remissionGuidePoundTotal,
                    remissionGuideFishingZoneName = custodianIncome.remissionGuideFishingZoneName,
                    remissionGuideFishingSiteName = custodianIncome.remissionGuideFishingSiteName,
                    remissionGuideRoute = custodianIncome.remissionGuideRoute,
                    remissionGuideshippingTypeName = custodianIncome.remissionGuideShippingTypeName,
                    remissionGuidesDriverName = custodianIncome.remissionGuidesDriverName,
                    remissionGuidesProcessPlant = custodianIncome.remissionGuidesProcessPlant,
                    remissionGuidesProviderTransportName = custodianIncome.remissionGuidesProviderTransportName,
                    remissionGuidesCarRegistration = custodianIncome.remissionGuidesCarRegistration,
                    remissionGuidesTransportBillingName = custodianIncome.remissionGuidesTransportBillingName,
                    remissionGuidesTransportValuePrice = custodianIncome.remissionGuidesTransportValuePrice

                }, transaction);
            
        }
        private void updateCustodianIncome(CustodianIncome custodianIncome, SqlConnection connection, SqlTransaction transaction)
        {
            connection.Execute(UDPATE_CUSTODIANINCOME,new 
            {
                id_PersonCompanyCustodian1 = custodianIncome.id_PersonCompanyCustodian1,
                id_PersonCompanyCustodian2 = custodianIncome.id_PersonCompanyCustodian2,
                id_FishingSite1 = custodianIncome.id_FishingSite1,
                id_FishingSite2  = custodianIncome.id_FishingSite2,
                id_FishingCustodian1 = custodianIncome.id_FishingCustodian1,
                id_FishingCustodian2 = custodianIncome.id_FishingCustodian2,
                fishingCustodianField1 = custodianIncome.fishingCustodianField1,
                fishingCustodianField2 = custodianIncome.fishingCustodianField2,
                fishingCustodianValue1 = custodianIncome.fishingCustodianValue1,
                fishingCustodianValue2 = custodianIncome.fishingCustodianValue2,
                remissionGuideProviderName = custodianIncome.remissionGuideProviderName,
                remissionGuideProductionUnitProviderName = custodianIncome.remissionGuideProductionUnitProviderName,
                remissionGuidePoolReference = custodianIncome.remissionGuidePoolReference,
                remissionGuidePoundTotal = custodianIncome.remissionGuidePoundTotal,
                remissionGuideFishingZoneName = custodianIncome.remissionGuideFishingZoneName,
                remissionGuideFishingSiteName = custodianIncome.remissionGuideFishingSiteName,
                remissionGuideRoute = custodianIncome.remissionGuideRoute,
                remissionGuideshippingTypeName = custodianIncome.remissionGuideShippingTypeName,
                remissionGuidesDriverName = custodianIncome.remissionGuidesDriverName,
                remissionGuidesProcessPlant = custodianIncome.remissionGuidesProcessPlant,
                remissionGuidesProviderTransportName = custodianIncome.remissionGuidesProviderTransportName,
                remissionGuidesCarRegistration = custodianIncome.remissionGuidesCarRegistration,
                remissionGuidesTransportBillingName = custodianIncome.remissionGuidesTransportBillingName,
                remissionGuidesTransportValuePrice = custodianIncome.remissionGuidesTransportValuePrice,
                id = custodianIncome.Id

            }, transaction);
        }
        private int insertDocument(Document document, SqlConnection connection, SqlTransaction transaction)
        {
            var documentId = connection.ExecuteScalar<int>(INSERT_DOCUMENT,
            new
                {
                    number = document.number,
                    sequential = document.sequential,
                    emissionDate = document.emissionDate,
                    description = document.description,
                    id_emissionPoint = document.id_emissionPoint,
                    id_documentType = document.id_documentType,
                    id_documentState = document.id_documentState,
                    id_userCreate = document.id_userCreate,
                    dateCreate = document.dateCreate,
                    id_userUpdate = document.id_userUpdate,
                    dateUpdate = document.dateUpdate,
            }, transaction);
            return documentId;
        }
        private void updateDocument(Document document, SqlConnection connection, SqlTransaction transaction)
        {
            connection.Execute(UPDATE_DOCUMENT, new
            {
                description = document.description,
                id_userUpdate = document.id_userUpdate,
                dateUpdate = document.dateUpdate,
                id_documentState = document.id_documentState,
                id = document.id
            }, transaction);
        }
        private void updateDocumentType(DocumentType documentType,SqlConnection connection, SqlTransaction transaction)
        {
            int documentTypeId = documentType.id;
            int currentNumberSec = documentType.currentNumber;
            int userUpdate = documentType.id_userUpdate;
            connection.Execute(UPDATE_DOCUMENTTYPE_CURRENTNUNBER, new
            {
                currentNumber = currentNumberSec,
                dateUpdate = DateTime.Now,
                id_userUpdate = userUpdate,
                id = documentTypeId
            }, transaction);
        }
    
    }
}