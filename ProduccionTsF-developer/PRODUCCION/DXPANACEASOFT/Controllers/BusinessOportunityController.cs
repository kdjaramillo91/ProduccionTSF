using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Common;
using System.Collections;
using System.Web.Helpers;
using Newtonsoft.Json;
using DXPANACEASOFT.Reports.BusinessOportunity;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.DataProviders;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;

namespace DXPANACEASOFT.Controllers
{
    public class BusinessOportunityController : DefaultController
    {
        static private List<QueryPhase> listQueryPhase = null;

        [HttpPost, ValidateInput(false)]
        public ActionResult Index()
        {
            FileUploadHelper.CleanUpUploadedFilesDirectory();
            return PartialView();
        }

        #region BUSINESS OPORTUNITY VIEW OPPORTUNITIES

        [HttpPost, ValidateInput(false)]
        public ActionResult IndexViewOpportunities()
        {
            return PartialView();
        }

        public ActionResult ViewOpportunitiesDetailResultsPartial(int? id_company)
        {
            var model = listQueryPhase ?? new List<QueryPhase>();

            return PartialView("_ViewOpportunitiesDetailResultQueryPhasePartial", model);
        }

        public ActionResult CharterColumn()
        {
            
            new Chart(width: 600, height: 400, theme: ChartTheme.Green)
            .AddTitle("")
            .AddSeries("Default", 
            chartType: "Funnel", 
            xValue: listQueryPhase, 
            xField: "id_phase",
            yValues: listQueryPhase,
            yFields: "percent")
            //.AddSeries("Default", chartType: "PepeLine o Column o Funnel", xvalue: xValue, yvalue: yValue)
            .Write("bmp");

            return null;
        }

        private class tempQueryPhase
        {
            public int id_businessOportunity { get; set; }
            public int id_documentType { get; set; }
            public int id_documentState { get; set; }
            public int? id_businessOportunityState { get; set; }
            public string address { get; set; }
            public int? id_person { get; set; }
            public int id_executivePerson { get; set; }
            public int id_phase { get; set; }
            public decimal amount { get; set; }
            public decimal advance { get; set; }
            public DateTime startDate { get; set; }
            public DateTime? endDate { get; set; }

        }

        private bool IsAmountLogicalOperatorOtherAmount(decimal amount, int? id_logicalOperator, decimal otherAmount)
        {
            if (id_logicalOperator == 1) return (amount == otherAmount);
            if (id_logicalOperator == null || id_logicalOperator == 2) return (amount > otherAmount);
            if (id_logicalOperator == 3) return (amount < otherAmount);
            if (id_logicalOperator == 4) return (amount != otherAmount);
            if (id_logicalOperator == 5) return (amount >= otherAmount);
            if (id_logicalOperator == 6) return (amount <= otherAmount);

            return false;
        }

        private string getLogicalOperator(int? id_logicalOperator)
        {
            if (id_logicalOperator == 1) return "=";
            if (id_logicalOperator == null || id_logicalOperator == 2) return ">";
            if (id_logicalOperator == 3) return "<";
            if (id_logicalOperator == 4) return "!=";
            if (id_logicalOperator == 5) return ">=";
            if (id_logicalOperator == 6) return "<=";

            return "=";
        }

        [HttpPost]
        public ActionResult BusinessOportunityViewOpportunitiesResults(int? id_documentType, string address,
                                                      int? id_logicalOperator, decimal? amount, 
                                                      int? id_logicalOperatorPC, decimal? percentClosure,
                                                      DateTime? startStartDate, DateTime? endStartDate,
                                                      DateTime? startEndDate, DateTime? endEndDate,
                                                      int[] persons, int[] executivePersons, int[] documentTypePhases, int[] itemSizes,
                                                      int? id_documentState, int? id_businessOportunityState)
        {

            var queryPhases = (db.BusinessOportunityPhase.Where(bop => bop.BusinessOportunity.BusinessOportunityPhase.Max(bobop => bobop.advance) == bop.advance))
                                                                      //.OrderBy(bop2 => bop2.id_phase)
                                                                      .OrderBy(bop2 => bop2.BusinessOportunityDocumentTypePhase.Phase.id)
                                                                      .Select(bop3 => new tempQueryPhase {
                                                                          id_businessOportunity = bop3.id_businessOportunity,
                                                                          id_documentType = bop3.BusinessOportunity.Document.id_documentType,
                                                                          address = bop3.BusinessOportunity.Person.address,
                                                                          id_person = bop3.BusinessOportunity.id_person,
                                                                          id_executivePerson = bop3.BusinessOportunity.id_executivePerson,
                                                                          //id_phase = bop3.id_phase,
                                                                          id_phase = bop3.BusinessOportunityDocumentTypePhase.Phase.id,
                                                                          amount = bop3.BusinessOportunity.BusinessOportunityPlaninng.amount,
                                                                          advance = bop3.advance,
                                                                          startDate = bop3.BusinessOportunity.startDate,
                                                                          endDate = bop3.BusinessOportunity.endDate,
                                                                          id_documentState = bop3.BusinessOportunity.Document.id_documentState,
                                                                          id_businessOportunityState = bop3.BusinessOportunity.BusinessOportunityResults != null ? bop3.BusinessOportunity.BusinessOportunityResults.id_businessOportunityState : null
                                                                      }).ToList();

            if (id_documentType != null)
            {
                queryPhases = queryPhases.Where(o => o.id_documentType == id_documentType).ToList();
            }

            if (id_documentState != null)
            {
                queryPhases = queryPhases.Where(o => o.id_documentState == id_documentState).ToList();
            }

            if (id_businessOportunityState != null)
            {
                queryPhases = queryPhases.Where(o => o.id_businessOportunityState == id_businessOportunityState).ToList();
            }

            if (!string.IsNullOrEmpty(address))
            {
                queryPhases = queryPhases.Where(o => o.address.Contains(address)).ToList();
            }

            if (startStartDate != null && endStartDate != null)
            {
                queryPhases = queryPhases.Where(o => DateTime.Compare(startStartDate.Value.Date, o.startDate.Date) <= 0 && DateTime.Compare(o.startDate.Date, endStartDate.Value.Date) <= 0).ToList();
            }

            if (startEndDate != null && endEndDate != null)
            {
                queryPhases = queryPhases.Where(o => DateTime.Compare(startEndDate.Value.Date, o.endDate?.Date ?? startEndDate.Value.Date) <= 0 && DateTime.Compare(o.endDate?.Date ?? endEndDate.Value.Date, endEndDate.Value.Date) <= 0).ToList();
            }


            if (persons != null && persons.Length > 0)
            {
                var tempQueryPhases = new List<tempQueryPhase>();
                foreach (var qp in queryPhases)
                {
                    var id_personAux = qp.id_person ?? 0;
                    if (persons.Contains(id_personAux))
                    {
                        tempQueryPhases.Add(qp);
                    }
                }

                queryPhases = tempQueryPhases;
            }

            if (executivePersons != null && executivePersons.Length > 0)
            {
                var tempQueryPhases = new List<tempQueryPhase>();
                foreach (var qp in queryPhases)
                {
                    //var details = order.PurchaseOrderDetail.Where(d => items.Contains(d.id_item));
                    //if (details.Any())
                    if (executivePersons.Contains(qp.id_executivePerson))
                    {
                        tempQueryPhases.Add(qp);
                    }
                }

                queryPhases = tempQueryPhases;
            }

            if (documentTypePhases != null && documentTypePhases.Length > 0)
            {
                var tempQueryPhases = new List<tempQueryPhase>();
                foreach (var qp in queryPhases)
                {
                    var id_documentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(d => d.id_documentType == qp.id_documentType && 
                                                                                                          d.id_phase == qp.id_phase)?.id;
                    //var details = order.PurchaseOrderDetail.Where(d => items.Contains(d.id_item));
                    //if (details.Any())
                    if (documentTypePhases.Contains(id_documentTypePhase ?? 0))
                    {
                        tempQueryPhases.Add(qp);
                    }
                }

                queryPhases = tempQueryPhases;
            }

            var tempQueryPhasesAmount = new List<tempQueryPhase>();
            foreach (var qp in queryPhases)
            {
                //var details = order.PurchaseOrderDetail.Where(d => items.Contains(d.id_item));
                //if (details.Any())
                if (IsAmountLogicalOperatorOtherAmount(qp.amount, id_logicalOperator, amount ?? 0) &&
                    IsAmountLogicalOperatorOtherAmount(qp.advance, id_logicalOperatorPC, percentClosure ?? 0))
                {
                    tempQueryPhasesAmount.Add(qp);
                }
            }

            queryPhases = tempQueryPhasesAmount;

            if (itemSizes != null && itemSizes.Length > 0)
            {
                var tempQueryPhases = new List<tempQueryPhase>();
                foreach (var qp in queryPhases)
                {
                    BusinessOportunity businessOportunity = db.BusinessOportunity.FirstOrDefault(bo=> bo.id == qp.id_businessOportunity);
                    //var details = order.PurchaseOrderDetail.Where(d => items.Contains(d.id_item));
                    //if (details.Any())
                    foreach (var dop in businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail)
                    {
                        if (itemSizes.Contains(dop.Item.ItemGeneral.id_size ?? 0))
                        {
                            tempQueryPhases.Add(qp);
                            break;
                        }
                    }
                }

                queryPhases = tempQueryPhases;
            }

            listQueryPhase = new List<QueryPhase>();
            QueryPhase queryPhaseAux = null;
            decimal totalWeightedAmount = 0;
            //BusinessOportunity businessOportunityAux = null;
            List<int> listInt = null;

            foreach (var phase in queryPhases)
            {
                //businessOportunityAux = db.BusinessOportunity.FirstOrDefault(bo => bo.id == phase.id_businessOportunity);
                if (queryPhaseAux == null)
                {
                    queryPhaseAux = new QueryPhase
                    {
                        id_phase = phase.id_phase,
                        phase = db.Phase.FirstOrDefault(p => p.id == phase.id_phase),
                        quantity = 1,
                        amountExpected = phase.amount,
                        weightedAmount = phase.amount,
                        percent = 0                        
                    };
                    listInt = new List<int>();
                    listInt.Add(phase.id_businessOportunity);
                }
                else
                {
                    if (queryPhaseAux.id_phase != phase.id_phase)
                    {
                        queryPhaseAux.listId_BusinessOportunity = JsonConvert.SerializeObject(listInt);
                        listQueryPhase.Add(queryPhaseAux);
                        totalWeightedAmount += (queryPhaseAux.weightedAmount);
                        queryPhaseAux = new QueryPhase
                        {
                            id_phase = phase.id_phase,
                            phase = db.Phase.FirstOrDefault(p => p.id == phase.id_phase),
                            quantity = 1,
                            amountExpected = phase.amount,
                            weightedAmount = phase.amount,
                            percent = 0
                        };
                        listInt = new List<int>();
                        listInt.Add(phase.id_businessOportunity);
                    }
                    else
                    {
                        queryPhaseAux.quantity++;
                        queryPhaseAux.amountExpected += phase.amount;
                        queryPhaseAux.weightedAmount = queryPhaseAux.amountExpected / queryPhaseAux.quantity;
                        listInt.Add(phase.id_businessOportunity);
                        
                    }

                }

             }
            if (queryPhaseAux != null) {
                queryPhaseAux.listId_BusinessOportunity = JsonConvert.SerializeObject(listInt);
                listQueryPhase.Add(queryPhaseAux);
                totalWeightedAmount += (queryPhaseAux.weightedAmount);
            } 

            if (totalWeightedAmount != 0)
            {
                foreach (var l in listQueryPhase)
                {
                    l.percent = Math.Round((l.weightedAmount / totalWeightedAmount) * 100);
                }

            }

            listQueryPhase = listQueryPhase.OrderByDescending(lqp => lqp.percent).ToList();

            ResultQueryPhase resultQueryPhase = new ResultQueryPhase();

            
            //filterDocumentType
            if (id_documentType != null)
            {
                resultQueryPhase.filterDocumentType = db.DocumentType.FirstOrDefault(dt => dt.id == id_documentType).name + ".";
            }
            else
            {
                resultQueryPhase.filterDocumentType = "Todos.";
            }

            //filterAddress
            if (!string.IsNullOrEmpty(address))
            {
                resultQueryPhase.filterAddress = address + ".";
            }
            else
            {
                resultQueryPhase.filterAddress = "Todos.";
            }

            var filterAux = "";
            //filterPersons
            foreach (var p in persons )
            {
                if(filterAux == "") {
                    filterAux = db.Person.FirstOrDefault(pe => pe.id == p).fullname_businessName;
                }
                else {
                    filterAux += ", " + db.Person.FirstOrDefault(pe => pe.id == p).fullname_businessName;
                }
            }
            if (filterAux != "")
            {
                resultQueryPhase.filterPersons = filterAux + ".";
            }
            else
            {
                resultQueryPhase.filterPersons = "Todos.";
            }

            filterAux = "";
            //filterExecutivePersons
            foreach (var ep in executivePersons)
            {
                if (filterAux == "")
                {
                    filterAux = db.Employee.FirstOrDefault(emp => emp.id == ep).Person.fullname_businessName;
                }
                else
                {
                    filterAux += ", " + db.Employee.FirstOrDefault(emp => emp.id == ep).Person.fullname_businessName;
                }
            }
            if (filterAux != "")
            {
                resultQueryPhase.filterExecutivePersons = filterAux + ".";
            }
            else
            {
                resultQueryPhase.filterExecutivePersons = "Todos.";
            }
            //filterRangeStartDate
            if (startStartDate != null && endStartDate != null)
            {
                resultQueryPhase.filterRangeStartDate = startStartDate.Value.ToString("dd/MM/yyyy") + " - " +
                                                        endStartDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                resultQueryPhase.filterRangeStartDate = "Todos.";
            }

            //filterRangeEndDate
            if (startEndDate != null && endEndDate != null)
            {
                resultQueryPhase.filterRangeEndDate = startEndDate.Value.ToString("dd/MM/yyyy") + " - " +
                                                        endEndDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                resultQueryPhase.filterRangeEndDate = "Todos.";
            }
            filterAux = "";
            //filterPhases
            foreach (var ph in documentTypePhases)
            {
                var businessOportunityDocumentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(ph1 => ph1.id == ph);
                if (filterAux == "")
                {
                    filterAux = businessOportunityDocumentTypePhase?.Phase.name + "(" + businessOportunityDocumentTypePhase?.DocumentType.name + ")";
                }
                else
                {
                    filterAux += ", " + businessOportunityDocumentTypePhase?.Phase.name + "(" + businessOportunityDocumentTypePhase?.DocumentType.name + ")";
                }
            }
            if (filterAux != "")
            {
                resultQueryPhase.filterPhases = filterAux + ".";
            }
            else
            {
                resultQueryPhase.filterPhases = "Todos.";
            }

            //filterAmounts
            resultQueryPhase.filterAmounts = " " + getLogicalOperator(id_logicalOperator) + " " + (amount ?? 0).ToString("0.00");

            //filterPercents
            resultQueryPhase.filterPercents = " " + getLogicalOperator(id_logicalOperatorPC) + " " + (percentClosure ?? 0).ToString("0.00");

            filterAux = "";
            //filterItemSizes
            foreach (var p in itemSizes)
            {
                if (filterAux == "")
                {
                    filterAux = db.ItemSize.FirstOrDefault(i => i.id == p).name;
                }
                else
                {
                    filterAux += ", " + db.ItemSize.FirstOrDefault(i => i.id == p).name;
                }
            }
            if (filterAux != "")
            {
                resultQueryPhase.filterItemSizes = filterAux + ".";
            }
            else
            {
                resultQueryPhase.filterItemSizes = "Todos.";
            }

            //filterDocumentState
            if (id_documentState != null)
            {
                resultQueryPhase.filterDocumentState = db.DocumentState.FirstOrDefault(dt => dt.id == id_documentState).name + ".";
            }
            else
            {
                resultQueryPhase.filterDocumentState = "Todos.";
            }

            //filterBusinessOportunityState 
            if (id_businessOportunityState != null)
            {
                resultQueryPhase.filterBusinessOportunityState = db.BusinessOportunityState.FirstOrDefault(dt => dt.id == id_businessOportunityState).name + ".";
            }
            else
            {
                resultQueryPhase.filterBusinessOportunityState = "Todos.";
            }

            resultQueryPhase.listQueryPhase = listQueryPhase;
            resultQueryPhase.company = ActiveCompany;//db.Company.FirstOrDefault(c=> c.id == this.ActiveCompanyId);
            TempData["resultQueryPhase"] = resultQueryPhase;
            TempData.Keep("resultQueryPhase");

            return PartialView("_BusinessOportunityViewOpportunitiesResultsPartial", resultQueryPhase);//model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityViewOpportunitiesResultsReport()
        {
            ResultViewOpportunitiesBusinessOportunity report = new ResultViewOpportunitiesBusinessOportunity();
            ResultQueryPhase tempResultQueryPhase = (TempData["resultQueryPhase"] as ResultQueryPhase);
            report.DataSource = tempResultQueryPhase;
            //report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_BusinessOportunityViewOpportunitiesResultsReport", report);
        }

        #endregion

        #region BUSINESS OPORTUNITY FILTERS RESULTS

        [HttpPost]
        public ActionResult BusinessOportunityResults(Document document,
                                                      BusinessOportunity businessOportunity,
                                                      string nameOportunity,
                                                      DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                      DateTime? startStartDate, DateTime? endStartDate,
                                                      DateTime? startEndDate, DateTime? endEndDate,
                                                      int? id_businessOportunityState)
        {
            FileUploadHelper.CleanUpUploadedFilesDirectory();
            List<BusinessOportunity> model = db.BusinessOportunity.AsEnumerable().ToList();

            #region DOCUMENT FILTERS

            if (document.id_documentType != 0)
            {
                model = model.Where(o => o.Document.id_documentType == document.id_documentType).ToList();
            }

            if (document.id_documentState != 0)
            {
                model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
            }

            if (startEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
            }

            if (endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            #endregion

            #region BUSINESS OPORTUNITY FILTERS

            if (startStartDate != null)
            {
                model = model.Where(o => DateTime.Compare(startStartDate.Value.Date, o.startDate.Date) <= 0).ToList();
            }

            if (endStartDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.startDate.Date, endStartDate.Value.Date) <= 0).ToList();
            }

            if (startEndDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEndDate.Value.Date, (o.endDate != null ? o.endDate.Value.Date : startEndDate.Value.Date)) <= 0).ToList();
            }

            if (endEndDate != null)
            {
                model = model.Where(o => DateTime.Compare((o.endDate != null ? o.endDate.Value.Date : endEndDate.Value.Date), endEndDate.Value.Date) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(nameOportunity))
            {
                model = model.Where(o => o.name.Contains(nameOportunity)).ToList();
            }

            if (businessOportunity.id_businessPartner != 0 && businessOportunity.id_businessPartner != null)
            {
                model = model.Where(o => o.id_businessPartner == businessOportunity.id_businessPartner).ToList();
            }

            if (!string.IsNullOrEmpty(businessOportunity.contactPerson))
            {
                model = model.Where(o => o.contactPerson.Contains(businessOportunity.contactPerson)).ToList();
            }

            if (businessOportunity.id_executivePerson != 0)
            {
                model = model.Where(o => o.id_executivePerson == businessOportunity.id_executivePerson).ToList();
            }

            if (id_businessOportunityState != 0 && id_businessOportunityState != null)
            {
                model = model.Where(o => o.BusinessOportunityResults?.id_businessOportunityState == id_businessOportunityState).ToList();
            }

            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_BusinessOportunityResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunitiesPartial()
        {
            var model = (TempData["model"] as List<BusinessOportunity>);
            model = model ?? new List<BusinessOportunity>();

            TempData.Keep("model");

            //var model = db.BusinessOportunity;
            return PartialView("_BusinessOportunitiesPartial", model.OrderByDescending(o => o.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditBusinessOportunity(int id, string type = null)
        {
            BusinessOportunity businessOportunity = db.BusinessOportunity.FirstOrDefault(o => o.id == id);

            if (businessOportunity == null)
            {
                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code.Equals("01"));
                BusinessOportunityState businessOportunityState = db.BusinessOportunityState.FirstOrDefault(s => s.code.Equals("01"));

                Employee employee = ActiveUser.Employee;
                //string code = (!string.IsNullOrEmpty(type) && type.Equals("SALE")) ? "15" : "16";
                //DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals(code));

                businessOportunity = new BusinessOportunity
                {
                    Document = new Document
                    {
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        //id_documentType = documentType?.id ?? 0,
                        //DocumentType = documentType,
                        
                        emissionDate = DateTime.Now
                    },
                    id_executivePerson = employee?.id ?? 0,
                    Employee = employee,
                    //startDate = DateTime.Now,
                    total = 0,
                    BusinessOportunityResults = new BusinessOportunityResults
                    {
                        BusinessOportunityState = businessOportunityState,
                        id_businessOportunityState = businessOportunityState.id,
                        totalAmount = 0
                    },
                    BusinessOportunityNote = new List<BusinessOportunityNote>(),
                    BusinessOportunityPlaninng = new BusinessOportunityPlaninng
                    {
                        BusinessOportunityPlanningDetail = new List<BusinessOportunityPlanningDetail>()
                    },
                    BusinessOportunityPhase = new List<BusinessOportunityPhase>(),
                    BusinessOportunityPartner = new List<BusinessOportunityPartner>(),
                    BusinessOportunityCompetition = new List<BusinessOportunityCompetition>(),
                    BusinessOportunityDocument = new List<BusinessOportunityDocument>()
                    //{
                    //    BusinessOportunityPhaseAttachment = new List<BusinessOportunityPhaseAttachment>(),
                    //    BusinessOportunityPhaseActivity = new List<BusinessOportunityPhaseActivity>()
                    //}
                };
            }
            
            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return PartialView("_FormEditBusinessOportunity", businessOportunity);
        }

        #endregion

        #region BUSINESS OPORTUNITY HEADERS

        [HttpPost, ValidateInput(false)]
        public ActionResult BussinesOportunityPartialAddNew(bool approve, decimal closingPercentage, string locationName, int id_documentTypeCurrent,
                                                            BusinessOportunity businessOportunity, BusinessOportunityResults businessOportunityResults, 
                                                            BusinessOportunityPlaninng businessOportunityPlaninng, Document document,
                                                            DateTime emissionDateCurrent, string descriptionCurrent)
        {
            var model = db.BusinessOportunity;

            BusinessOportunity tempBusinessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            tempBusinessOportunity = tempBusinessOportunity ?? new BusinessOportunity();

            tempBusinessOportunity.Document.id_documentType = id_documentTypeCurrent;
            tempBusinessOportunity.Document.emissionDate = emissionDateCurrent;
            tempBusinessOportunity.Document.description = descriptionCurrent;

            tempBusinessOportunity.id_businessPartner = businessOportunity.id_businessPartner;
            tempBusinessOportunity.name = businessOportunity.name;
            tempBusinessOportunity.id_executivePerson = businessOportunity.id_executivePerson;
            tempBusinessOportunity.contactPerson = businessOportunity.contactPerson;
            tempBusinessOportunity.startDate = businessOportunity.startDate;
            tempBusinessOportunity.total = businessOportunity.total;
            tempBusinessOportunity.location = locationName;
            tempBusinessOportunity.advance = (closingPercentage * 100);

            tempBusinessOportunity.BusinessOportunityResults.id_businessOportunityState = businessOportunityResults.id_businessOportunityState;
            tempBusinessOportunity.BusinessOportunityResults.totalAmount = businessOportunityResults.totalAmount;

            tempBusinessOportunity.BusinessOportunityPlaninng.estimatedEndDate = businessOportunityPlaninng.estimatedEndDate;
            tempBusinessOportunity.BusinessOportunityPlaninng.amount = businessOportunityResults.totalAmount.Value;
            tempBusinessOportunity.BusinessOportunityPlaninng.estimatedProfit = businessOportunityPlaninng.estimatedProfit;
            tempBusinessOportunity.BusinessOportunityPlaninng.grossProfit = businessOportunityPlaninng.grossProfit;
            tempBusinessOportunity.BusinessOportunityPlaninng.id_priority = businessOportunityPlaninng.id_priority;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Document

                    document.id_userCreate = ActiveUser.id;
                    document.dateCreate = DateTime.Now;
                    document.id_userUpdate = ActiveUser.id;
                    document.dateUpdate = DateTime.Now;

                    document.emissionDate = emissionDateCurrent;

                    document.sequential = GetDocumentSequential(id_documentTypeCurrent);//document.id_documentType);
                    document.number = GetDocumentNumber(id_documentTypeCurrent);//document.id_documentType);

                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == id_documentTypeCurrent);//document.id_documentType);
                    document.DocumentType = documentType;

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == document.id_documentState);
                    document.DocumentState = documentState;

                    document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                    document.id_emissionPoint = ActiveEmissionPoint.id;

                    document.description = descriptionCurrent;

                    //string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

                    //document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                    //                                                document.DocumentType.code,
                    //                                                document.EmissionPoint.BranchOffice.Division.Company.ruc,
                    //                                                "1",
                    //                                                document.EmissionPoint.BranchOffice.code.ToString("D3") + document.EmissionPoint.code.ToString("D3"),
                    //                                                document.sequential.ToString("D9"),
                    //                                                document.sequential.ToString("D8"),
                    //                                                "1");

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }

                    #endregion

                    #region BusinessOportunity

                    businessOportunity.Document = document;

                    businessOportunity.Person1 = db.Person.FirstOrDefault(e => e.id == businessOportunity.id_businessPartner);
                    tempBusinessOportunity.Person1 = businessOportunity.Person1;

                    Person executivePerson = db.Person.FirstOrDefault(e => e.id == businessOportunity.id_executivePerson);
                    businessOportunity.executivePerson = executivePerson.fullname_businessName;
                    tempBusinessOportunity.executivePerson = businessOportunity.executivePerson;

                    businessOportunity.advance = (closingPercentage*100);
                    

                    businessOportunity.id_company = this.ActiveCompanyId;

                    Person businessPartner = db.Person.FirstOrDefault(e => e.id == businessOportunity.id_businessPartner);
                    businessOportunity.businessPartner = businessPartner.fullname_businessName;
                    tempBusinessOportunity.businessPartner = businessOportunity.businessPartner;

                    businessOportunity.location = locationName;

                    #endregion

                    #region BusinessOportunityResults

                    businessOportunity.BusinessOportunityResults = new BusinessOportunityResults
                    {
                        id_businessOportunityState = businessOportunityResults.id_businessOportunityState,
                        BusinessOportunityState = db.BusinessOportunityState.FirstOrDefault(fod => fod.id == businessOportunityResults.id_businessOportunityState),
                        totalAmount = businessOportunityResults.totalAmount
                    };

                    businessOportunity.BusinessOportunityResults.state = businessOportunity.BusinessOportunityResults.BusinessOportunityState.name;
                    tempBusinessOportunity.BusinessOportunityResults.BusinessOportunityState = businessOportunity.BusinessOportunityResults.BusinessOportunityState;
                    tempBusinessOportunity.BusinessOportunityResults.state = businessOportunity.BusinessOportunityResults.state;

                    #endregion

                    #region BusinessOportunityNote

                    if (tempBusinessOportunity?.BusinessOportunityNote != null)
                    {
                        businessOportunity.BusinessOportunityNote = new List<BusinessOportunityNote>();
                        var itemBusinessOportunityNote = tempBusinessOportunity.BusinessOportunityNote.ToList();

                        foreach (var detail in itemBusinessOportunityNote)
                        {
                            var tempItemBusinessOportunityNote = new BusinessOportunityNote
                            {
                                referenceNote = detail.referenceNote,
                                descriptionNote = detail.descriptionNote,
                            };

                            businessOportunity.BusinessOportunityNote.Add(tempItemBusinessOportunityNote);
                        }
                    }

                    #endregion

                    #region BusinessOportunityPlaninng

                    businessOportunity.BusinessOportunityPlaninng = new BusinessOportunityPlaninng
                    {
                        estimatedEndDate = businessOportunityPlaninng.estimatedEndDate,
                        estimatedProfit = businessOportunityPlaninng.estimatedProfit,
                        grossProfit = businessOportunityPlaninng.grossProfit,
                        amount = businessOportunityResults.totalAmount.Value,//businessOportunityPlaninng.amount,
                        id_priority = businessOportunityPlaninng.id_priority,
                        BusinessOportunityPriority = db.BusinessOportunityPriority.FirstOrDefault(fod=> fod.id == businessOportunityPlaninng.id_priority),
                        BusinessOportunity = businessOportunity
                    };
                    tempBusinessOportunity.BusinessOportunityPlaninng.BusinessOportunityPriority = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPriority;
                    #endregion

                    #region BusinessOportunityPlanningDetail

                    if (tempBusinessOportunity?.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail != null)
                    {
                        businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = new List<BusinessOportunityPlanningDetail>();
                        var itemBusinessOportunityPlanningDetail = tempBusinessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.ToList();

                        foreach (var detail in itemBusinessOportunityPlanningDetail)
                        {
                            var tempItemBusinessOportunityPlanningDetail = new BusinessOportunityPlanningDetail
                            {
                                id_item = detail.id_item,
                                Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item),
                                quantity = detail.quantity,
                                price = detail.price,
                                id_person = detail.id_person,
                                Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_person),
                                id_priceList = detail.id_priceList,
                                PriceList = db.PriceList.FirstOrDefault(fod => fod.id == detail.id_priceList),
                                id_document = detail.id_document,
                                Document = db.Document.FirstOrDefault(fod => fod.id == detail.id_document),
                                referencePlanning = detail.referencePlanning,
                                id_businessOportunityPlanning = businessOportunity.BusinessOportunityPlaninng.id,
                                BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng
                            };

                            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Add(tempItemBusinessOportunityPlanningDetail);
                        }
                    }

                    #endregion

                    #region BusinessOportunityPhase

                    if (tempBusinessOportunity?.BusinessOportunityPhase != null)
                    {
                        businessOportunity.BusinessOportunityPhase = new List<BusinessOportunityPhase>();
                        var itemBusinessOportunityPhase = tempBusinessOportunity.BusinessOportunityPhase.ToList();

                        foreach (var detail in itemBusinessOportunityPhase)
                        {
                            var tempItemBusinessOportunityPhase = new BusinessOportunityPhase
                            {
                                id_businessOportunityDocumentTypePhase = detail.id_businessOportunityDocumentTypePhase,
                                BusinessOportunityDocumentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == detail.id_businessOportunityDocumentTypePhase),
                                startDatePhase = detail.startDatePhase,
                                endDatePhase = detail.endDatePhase,
                                id_employee = detail.id_employee,
                                Employee = db.Employee.FirstOrDefault(fod => fod.id == detail.id_employee),
                                phaseName = detail.phaseName,
                                advance = detail.advance,
                                weightedAmount = detail.weightedAmount,
                                referencePhase = detail.referencePhase,
                                descriptionPhase = detail.descriptionPhase,
                                BusinessOportunityPhaseAttachment = new List<BusinessOportunityPhaseAttachment>(),
                                BusinessOportunityPhaseActivity = new List<BusinessOportunityPhaseActivity>(),
                            };

                            //BusinessOportunityPhaseAttachment

                            if (detail.BusinessOportunityPhaseAttachment != null)
                            {
                                var itemBusinessOportunityPhaseAttachment = detail.BusinessOportunityPhaseAttachment.ToList();
                                foreach (var detailBusinessOportunityPhaseAttachment in itemBusinessOportunityPhaseAttachment)
                                {
                                    var tempItemBusinessOportunityPhaseAttachment = new BusinessOportunityPhaseAttachment
                                    {
                                        guidPhase = detailBusinessOportunityPhaseAttachment.guidPhase,
                                        urlPhase = detailBusinessOportunityPhaseAttachment.urlPhase,
                                        attachmentPhase = detailBusinessOportunityPhaseAttachment.attachmentPhase,
                                        referencePhaseAttachment = detailBusinessOportunityPhaseAttachment.referencePhaseAttachment,
                                        descriptionPhaseAttachment = detailBusinessOportunityPhaseAttachment.descriptionPhaseAttachment,
                                    };

                                    //db.BusinessOportunityPhaseAttachment.Add(tempItemBusinessOportunityPhaseAttachment);
                                    tempItemBusinessOportunityPhase.BusinessOportunityPhaseAttachment.Add(tempItemBusinessOportunityPhaseAttachment);
                                }
                            }

                            //BusinessOportunityPhaseActivity

                            if (detail.BusinessOportunityPhaseActivity != null)
                            {
                                var itemBusinessOportunityPhaseActivity = detail.BusinessOportunityPhaseActivity.ToList();
                                foreach (var detailBusinessOportunityPhaseActivity in itemBusinessOportunityPhaseActivity)
                                {
                                    var tempItemBusinessOportunityPhaseActivity = new BusinessOportunityPhaseActivity
                                    {
                                        id_businessOportunityActivity = detailBusinessOportunityPhaseActivity.id_businessOportunityActivity,
                                        BusinessOportunityActivity = db.BusinessOportunityActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_businessOportunityActivity),
                                        id_state = detailBusinessOportunityPhaseActivity.id_state,
                                        BusinessOportunityActivityState = db.BusinessOportunityActivityState.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_state),
                                        referencePhaseActivity = detailBusinessOportunityPhaseActivity.referencePhaseActivity,
                                        descriptionPhaseActivity = detailBusinessOportunityPhaseActivity.descriptionPhaseActivity,
                                    };

                                    //db.BusinessOportunityPhaseActivity.Add(tempItemBusinessOportunityPhaseActivity);
                                    tempItemBusinessOportunityPhase.BusinessOportunityPhaseActivity.Add(tempItemBusinessOportunityPhaseActivity);
                                }
                            }

                            businessOportunity.BusinessOportunityPhase.Add(tempItemBusinessOportunityPhase);
                        }
                    }

                    #endregion

                    if (businessOportunity.BusinessOportunityPhase.Count == 0)
                    {
                        throw new Exception("No se puede guardar la oportunidad sin Etapas.");
                    }

                    #region BusinessOportunityPartner

                    if (tempBusinessOportunity?.BusinessOportunityPartner != null)
                    {
                        businessOportunity.BusinessOportunityPartner = new List<BusinessOportunityPartner>();
                        var itemBusinessOportunityPartner = tempBusinessOportunity.BusinessOportunityPartner.ToList();

                        foreach (var detail in itemBusinessOportunityPartner)
                        {
                            var tempItemBusinessOportunityPartner = new BusinessOportunityPartner
                            {
                                id_partner = detail.id_partner,
                                Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_partner),
                                referencePartner = detail.referencePartner,
                                descriptionPartner = detail.descriptionPartner,
                                manual = detail.manual,
                            };

                            businessOportunity.BusinessOportunityPartner.Add(tempItemBusinessOportunityPartner);
                        }
                    }

                    #endregion

                    #region BusinessOportunityCompetition

                    if (tempBusinessOportunity?.BusinessOportunityCompetition != null)
                    {
                        businessOportunity.BusinessOportunityCompetition = new List<BusinessOportunityCompetition>();
                        var itemBusinessOportunityCompetition = tempBusinessOportunity.BusinessOportunityCompetition.ToList();

                        foreach (var detail in itemBusinessOportunityCompetition)
                        {
                            var tempItemBusinessOportunityCompetition = new BusinessOportunityCompetition
                            {
                                id_competitor = detail.id_competitor,
                                Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_competitor),
                                referenceCompetition = detail.referenceCompetition,
                                descriptionCompetition = detail.descriptionCompetition
                            };

                            businessOportunity.BusinessOportunityCompetition.Add(tempItemBusinessOportunityCompetition);
                        }
                    }

                    #endregion

                    #region BusinessOportunityDocument

                    if (tempBusinessOportunity?.BusinessOportunityDocument != null)
                    {
                        businessOportunity.BusinessOportunityDocument = new List<BusinessOportunityDocument>();
                        var itemBusinessOportunityDocument = tempBusinessOportunity.BusinessOportunityDocument.ToList();

                        foreach (var detail in itemBusinessOportunityDocument)
                        {
                            var tempItemBusinessOportunityDocument = new BusinessOportunityDocument
                            {
                                guid = detail.guid,
                                url = detail.url,
                                attachment = detail.attachment,
                                referenceDocument = detail.referenceDocument,
                                descriptionDocument = detail.descriptionDocument
                            };

                            businessOportunity.BusinessOportunityDocument.Add(tempItemBusinessOportunityDocument);
                        }
                    }

                    #endregion

                    db.BusinessOportunity.Add(businessOportunity);
                    db.SaveChanges();

                    if (approve)
                    {
                        if (businessOportunity.Document.DocumentType.code == "15")//Oportunidad de Venta
                        {
                            ServiceSalesRequest.UpdateSalesRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, false);
                        }
                        if (businessOportunity.Document.DocumentType.code == "16")//Oportunidad de Compra
                        {
                            ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, false);
                        }

                        businessOportunity.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        db.BusinessOportunity.Attach(businessOportunity);
                        db.Entry(businessOportunity).State = EntityState.Modified;

                    }

                    UpdateAttachment(businessOportunity);
                    UpdateAttachmentPhase(businessOportunity);

                    db.SaveChanges();
                    trans.Commit();

                    TempData["businessOportunity"] = businessOportunity;
                    TempData.Keep("businessOportunity");

                    ViewData["EditMessage"] = SuccessMessage("Oportunidad: " + businessOportunity.name + ", con número: "+ businessOportunity.Document.number + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData["businessOportunity"] = tempBusinessOportunity;
                    TempData.Keep("businessOportunity");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    SetViewData();

                    return PartialView("_BusinessOportunityMainFormPartial", tempBusinessOportunity);


                }

                SetViewData();

                return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);

            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BussinesOportunityPartialUpdate(bool approve, decimal closingPercentage, string locationName, int id_documentTypeCurrent,
                                                            BusinessOportunity businessOportunity, BusinessOportunityResults businessOportunityResults,
                                                            BusinessOportunityPlaninng businessOportunityPlaninng, Document document,
                                                            DateTime emissionDateCurrent, string descriptionCurrent)
        {
            var model = db.BusinessOportunity;

            BusinessOportunity modelItem = null;

            BusinessOportunity tempBusinessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            tempBusinessOportunity.Document.emissionDate = emissionDateCurrent;
            tempBusinessOportunity.Document.description = descriptionCurrent;

            tempBusinessOportunity.id_businessPartner = businessOportunity.id_businessPartner;
            tempBusinessOportunity.name = businessOportunity.name;
            tempBusinessOportunity.id_executivePerson = businessOportunity.id_executivePerson;
            tempBusinessOportunity.contactPerson = businessOportunity.contactPerson;
            tempBusinessOportunity.startDate = businessOportunity.startDate;
            tempBusinessOportunity.endDate = businessOportunity.endDate;
            tempBusinessOportunity.total = businessOportunity.total;
            tempBusinessOportunity.location = locationName;
            tempBusinessOportunity.advance = (closingPercentage * 100);

            tempBusinessOportunity.BusinessOportunityResults.id_businessOportunityState = businessOportunityResults.id_businessOportunityState;
            tempBusinessOportunity.BusinessOportunityResults.totalAmount = businessOportunityResults.totalAmount;

            tempBusinessOportunity.BusinessOportunityPlaninng.estimatedEndDate = businessOportunityPlaninng.estimatedEndDate;
            tempBusinessOportunity.BusinessOportunityPlaninng.amount = businessOportunityResults.totalAmount.Value;
            tempBusinessOportunity.BusinessOportunityPlaninng.estimatedProfit = businessOportunityPlaninng.estimatedProfit;
            tempBusinessOportunity.BusinessOportunityPlaninng.grossProfit = businessOportunityPlaninng.grossProfit;
            tempBusinessOportunity.BusinessOportunityPlaninng.id_priority = businessOportunityPlaninng.id_priority;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    modelItem = model.FirstOrDefault(it => it.id == businessOportunity.id);
                    if (modelItem != null)
                    {

                        #region DOCUMENT

                        modelItem.Document.description = descriptionCurrent;
                        //modelItem.Document.emissionDate = document.emissionDate;
                        modelItem.Document.emissionDate = emissionDateCurrent;

                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region BUSINESS OPORTUNITY

                        modelItem.name = businessOportunity.name;

                        modelItem.id_businessPartner = businessOportunity.id_businessPartner;
                        modelItem.Person1 = db.Person.FirstOrDefault(e => e.id == businessOportunity.id_businessPartner);
                        tempBusinessOportunity.Person1 = modelItem.Person1;

                        modelItem.contactPerson = businessOportunity.contactPerson;

                        modelItem.id_executivePerson = businessOportunity.id_executivePerson;
                        modelItem.Employee = db.Employee.FirstOrDefault(e => e.id == businessOportunity.id_executivePerson);
                        modelItem.executivePerson = modelItem.Employee.Person.fullname_businessName;
                        tempBusinessOportunity.Employee = modelItem.Employee;
                        tempBusinessOportunity.executivePerson = modelItem.executivePerson;

                        modelItem.startDate = businessOportunity.startDate;
                        modelItem.endDate = businessOportunity.endDate;

                        modelItem.total = businessOportunity.total;

                        modelItem.location = locationName;

                        modelItem.advance = (closingPercentage * 100);

                        #endregion

                        #region BusinessOportunityResults

                        modelItem.BusinessOportunityResults.id_businessOportunityState = businessOportunityResults.id_businessOportunityState;
                        modelItem.BusinessOportunityResults.BusinessOportunityState = db.BusinessOportunityState.FirstOrDefault(fod => fod.id == businessOportunityResults.id_businessOportunityState);
                        modelItem.BusinessOportunityResults.state = modelItem.BusinessOportunityResults.BusinessOportunityState.name;
                        modelItem.BusinessOportunityResults.totalAmount = businessOportunityResults.totalAmount;

                        tempBusinessOportunity.BusinessOportunityResults.BusinessOportunityState = modelItem.BusinessOportunityResults.BusinessOportunityState;
                        tempBusinessOportunity.BusinessOportunityResults.state = modelItem.BusinessOportunityResults.state;

                        #endregion

                        #region BusinessOportunityNote

                        if (tempBusinessOportunity.BusinessOportunityNote != null)
                        {
                            for (int i = modelItem.BusinessOportunityNote.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityNote.ElementAt(i);

                                modelItem.BusinessOportunityNote.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            var itemBusinessOportunityNote = tempBusinessOportunity.BusinessOportunityNote.ToList();
                            foreach (var detail in itemBusinessOportunityNote)
                            {
                                var tempItemBusinessOportunityNote = new BusinessOportunityNote
                                {
                                    referenceNote = detail.referenceNote,
                                    descriptionNote = detail.descriptionNote,
                                };

                                modelItem.BusinessOportunityNote.Add(tempItemBusinessOportunityNote);
                            }
                        }

                        #endregion

                        #region BusinessOportunityPlaninng

                        modelItem.BusinessOportunityPlaninng.estimatedEndDate = businessOportunityPlaninng.estimatedEndDate;
                        modelItem.BusinessOportunityPlaninng.estimatedProfit = businessOportunityPlaninng.estimatedProfit;
                        modelItem.BusinessOportunityPlaninng.grossProfit = businessOportunityPlaninng.grossProfit;
                        modelItem.BusinessOportunityPlaninng.amount = businessOportunityResults.totalAmount.Value;//businessOportunityPlaninng.amount,businessOportunityPlaninng.amount;
                        modelItem.BusinessOportunityPlaninng.id_priority = businessOportunityPlaninng.id_priority;
                        modelItem.BusinessOportunityPlaninng.BusinessOportunityPriority = db.BusinessOportunityPriority.FirstOrDefault(fod => fod.id == businessOportunityPlaninng.id_priority);

                        tempBusinessOportunity.BusinessOportunityPlaninng.BusinessOportunityPriority = modelItem.BusinessOportunityPlaninng.BusinessOportunityPriority;

                        #endregion

                        #region BusinessOportunityPlanningDetail

                        if (tempBusinessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail != null)
                        {
                            var itemBusinessOportunityPlanningDetail = tempBusinessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.ToList();
                            for (int i = modelItem.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.ElementAt(i);
                                if(itemBusinessOportunityPlanningDetail.FirstOrDefault(fod=> fod.id == detail.id) == null)
                                {
                                    var salesRequestDetailBusinessOportunityAux = detail.SalesRequestDetailBusinessOportunity?.FirstOrDefault();
                                    if(salesRequestDetailBusinessOportunityAux != null)
                                    {
                                        salesRequestDetailBusinessOportunityAux.id_businessOportunityPlanningDetail = null;
                                        salesRequestDetailBusinessOportunityAux.BusinessOportunityPlanningDetail = null;
                                        db.SalesRequestDetailBusinessOportunity.Attach(salesRequestDetailBusinessOportunityAux);
                                        db.Entry(salesRequestDetailBusinessOportunityAux).State = EntityState.Modified;
                                    }
                                    var purchaseRequestDetailBusinessOportunityAux = detail.PurchaseRequestDetailBusinessOportunity?.FirstOrDefault();
                                    if (purchaseRequestDetailBusinessOportunityAux != null)
                                    {
                                        purchaseRequestDetailBusinessOportunityAux.id_businessOportunityPlanningDetail = null;
                                        purchaseRequestDetailBusinessOportunityAux.BusinessOportunityPlanningDetail = null;
                                        db.PurchaseRequestDetailBusinessOportunity.Attach(purchaseRequestDetailBusinessOportunityAux);
                                        db.Entry(purchaseRequestDetailBusinessOportunityAux).State = EntityState.Modified;
                                    }
                                    modelItem.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }
                                else
                                {
                                    if (modelItem.Document.DocumentType.code == "15")//Oportunidad de Venta
                                    {
                                        var purchaseRequestDetailBusinessOportunityAux = detail.PurchaseRequestDetailBusinessOportunity?.FirstOrDefault();
                                        if (purchaseRequestDetailBusinessOportunityAux != null)
                                        {
                                            purchaseRequestDetailBusinessOportunityAux.id_businessOportunityPlanningDetail = null;
                                            purchaseRequestDetailBusinessOportunityAux.BusinessOportunityPlanningDetail = null;
                                            db.PurchaseRequestDetailBusinessOportunity.Attach(purchaseRequestDetailBusinessOportunityAux);
                                            db.Entry(purchaseRequestDetailBusinessOportunityAux).State = EntityState.Modified;
                                        }
                                    }
                                    if (modelItem.Document.DocumentType.code == "16")//Oportunidad de Compra
                                    {
                                        var salesRequestDetailBusinessOportunityAux = detail.SalesRequestDetailBusinessOportunity?.FirstOrDefault();
                                        if (salesRequestDetailBusinessOportunityAux != null)
                                        {
                                            salesRequestDetailBusinessOportunityAux.id_businessOportunityPlanningDetail = null;
                                            salesRequestDetailBusinessOportunityAux.BusinessOportunityPlanningDetail = null;
                                            db.SalesRequestDetailBusinessOportunity.Attach(salesRequestDetailBusinessOportunityAux);
                                            db.Entry(salesRequestDetailBusinessOportunityAux).State = EntityState.Modified;
                                        }
                                    }
                                }
                                
                                
                            }

                            foreach (var detail in itemBusinessOportunityPlanningDetail)
                            {
                                var tempItemBusinessOportunityPlanningDetail = modelItem.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.FirstOrDefault(fod=> fod.id == detail.id);
                                if(tempItemBusinessOportunityPlanningDetail == null)
                                {
                                    tempItemBusinessOportunityPlanningDetail = new BusinessOportunityPlanningDetail
                                    {
                                        id_item = detail.id_item,
                                        Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item),
                                        quantity = detail.quantity,
                                        price = detail.price,
                                        id_person = detail.id_person,
                                        Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_person),
                                        id_priceList = detail.id_priceList,
                                        PriceList = db.PriceList.FirstOrDefault(fod => fod.id == detail.id_priceList),
                                        id_document = detail.id_document,
                                        Document = db.Document.FirstOrDefault(fod => fod.id == detail.id_document),
                                        referencePlanning = detail.referencePlanning
                                    };

                                    modelItem.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Add(tempItemBusinessOportunityPlanningDetail);
                                }
                                else
                                {
                                    tempItemBusinessOportunityPlanningDetail.id_item = detail.id_item;
                                    tempItemBusinessOportunityPlanningDetail.Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                                    tempItemBusinessOportunityPlanningDetail.quantity = detail.quantity;
                                    tempItemBusinessOportunityPlanningDetail.price = detail.price;
                                    tempItemBusinessOportunityPlanningDetail.id_person = detail.id_person;
                                    tempItemBusinessOportunityPlanningDetail.Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_person);

                                    tempItemBusinessOportunityPlanningDetail.id_priceList = detail.id_priceList;
                                    tempItemBusinessOportunityPlanningDetail.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == detail.id_priceList);
                                    //tempItemBusinessOportunityPlanningDetail.id_document = detail.id_document;
                                    //tempItemBusinessOportunityPlanningDetail.Document = db.Document.FirstOrDefault(fod => fod.id == detail.id_document);
                                    tempItemBusinessOportunityPlanningDetail.referencePlanning = detail.referencePlanning;

                                    db.BusinessOportunityPlanningDetail.Attach(tempItemBusinessOportunityPlanningDetail);
                                    db.Entry(tempItemBusinessOportunityPlanningDetail).State = EntityState.Modified;
                                }
                                
                            }
                        }

                        #endregion

                        #region BusinessOportunityPhase

                        if (tempBusinessOportunity.BusinessOportunityPhase != null)
                        {
                            var itemBusinessOportunityPhase = tempBusinessOportunity.BusinessOportunityPhase.ToList();
                            for (int i = modelItem.BusinessOportunityPhase.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityPhase.ElementAt(i);

                                var itemBusinessOportunityPhaseAux = itemBusinessOportunityPhase.FirstOrDefault(fod=> fod.id == detail.id);
                                
                                for (int j = detail.BusinessOportunityPhaseAttachment.Count - 1; j >= 0; j--)
                                {
                                    var detailBusinessOportunityPhaseAttachment = detail.BusinessOportunityPhaseAttachment.ElementAt(j);
                                    if (itemBusinessOportunityPhaseAux == null || itemBusinessOportunityPhaseAux.BusinessOportunityPhaseAttachment.FirstOrDefault(fod=> fod.id == detailBusinessOportunityPhaseAttachment.id) == null)
                                    {
                                        DeleteAttachmentPhase(detailBusinessOportunityPhaseAttachment);
                                        detail.BusinessOportunityPhaseAttachment.Remove(detailBusinessOportunityPhaseAttachment);
                                        db.Entry(detailBusinessOportunityPhaseAttachment).State = EntityState.Deleted;
                                    }
                                    
                                    
                                }

                                for (int j = detail.BusinessOportunityPhaseActivity.Count - 1; j >= 0; j--)
                                {
                                    var detailBusinessOportunityPhaseActivity = detail.BusinessOportunityPhaseActivity.ElementAt(j);
                                    if (itemBusinessOportunityPhaseAux == null || itemBusinessOportunityPhaseAux.BusinessOportunityPhaseActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id) == null)
                                    {
                                        detail.BusinessOportunityPhaseActivity.Remove(detailBusinessOportunityPhaseActivity);
                                        db.Entry(detailBusinessOportunityPhaseActivity).State = EntityState.Deleted;
                                    }
                                }
                                if (itemBusinessOportunityPhaseAux == null)
                                {
                                    modelItem.BusinessOportunityPhase.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }
                                
                            }

                            
                            foreach (var detail in itemBusinessOportunityPhase)
                            {
                                var tempItemBusinessOportunityPhase = modelItem.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == detail.id);
                                if (tempItemBusinessOportunityPhase == null)
                                {
                                    tempItemBusinessOportunityPhase = new BusinessOportunityPhase
                                    {
                                        id_businessOportunityDocumentTypePhase = detail.id_businessOportunityDocumentTypePhase,
                                        BusinessOportunityDocumentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == detail.id_businessOportunityDocumentTypePhase),
                                        startDatePhase = detail.startDatePhase,
                                        endDatePhase = detail.endDatePhase,
                                        id_employee = detail.id_employee,
                                        Employee = db.Employee.FirstOrDefault(fod => fod.id == detail.id_employee),
                                        phaseName = detail.phaseName,
                                        advance = detail.advance,
                                        weightedAmount = detail.weightedAmount,
                                        referencePhase = detail.referencePhase,
                                        descriptionPhase = detail.descriptionPhase,
                                        BusinessOportunityPhaseAttachment = new List<BusinessOportunityPhaseAttachment>(),
                                        BusinessOportunityPhaseActivity = new List<BusinessOportunityPhaseActivity>(),
                                    };

                                    //BusinessOportunityPhaseAttachment

                                    if (detail.BusinessOportunityPhaseAttachment != null)
                                    {
                                        var itemBusinessOportunityPhaseAttachment = detail.BusinessOportunityPhaseAttachment.ToList();
                                        foreach (var detailBusinessOportunityPhaseAttachment in itemBusinessOportunityPhaseAttachment)
                                        {
                                            var tempItemBusinessOportunityPhaseAttachment = new BusinessOportunityPhaseAttachment
                                            {
                                                guidPhase = detailBusinessOportunityPhaseAttachment.guidPhase,
                                                urlPhase = detailBusinessOportunityPhaseAttachment.urlPhase,
                                                attachmentPhase = detailBusinessOportunityPhaseAttachment.attachmentPhase,
                                                referencePhaseAttachment = detailBusinessOportunityPhaseAttachment.referencePhaseAttachment,
                                                descriptionPhaseAttachment = detailBusinessOportunityPhaseAttachment.descriptionPhaseAttachment,
                                            };

                                            tempItemBusinessOportunityPhase.BusinessOportunityPhaseAttachment.Add(tempItemBusinessOportunityPhaseAttachment);
                                        }
                                    }

                                    //BusinessOportunityPhaseActivity

                                    if (detail.BusinessOportunityPhaseActivity != null)
                                    {
                                        var itemBusinessOportunityPhaseActivity = detail.BusinessOportunityPhaseActivity.ToList();
                                        foreach (var detailBusinessOportunityPhaseActivity in itemBusinessOportunityPhaseActivity)
                                        {
                                            var tempItemBusinessOportunityPhaseActivity = new BusinessOportunityPhaseActivity
                                            {
                                                id_businessOportunityActivity = detailBusinessOportunityPhaseActivity.id_businessOportunityActivity,
                                                BusinessOportunityActivity = db.BusinessOportunityActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_businessOportunityActivity),
                                                id_state = detailBusinessOportunityPhaseActivity.id_state,
                                                BusinessOportunityActivityState = db.BusinessOportunityActivityState.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_state),
                                                referencePhaseActivity = detailBusinessOportunityPhaseActivity.referencePhaseActivity,
                                                descriptionPhaseActivity = detailBusinessOportunityPhaseActivity.descriptionPhaseActivity,
                                            };

                                            tempItemBusinessOportunityPhase.BusinessOportunityPhaseActivity.Add(tempItemBusinessOportunityPhaseActivity);
                                        }
                                    }

                                    modelItem.BusinessOportunityPhase.Add(tempItemBusinessOportunityPhase);

                                }
                                else
                                {
                                    tempItemBusinessOportunityPhase.id_businessOportunityDocumentTypePhase = detail.id_businessOportunityDocumentTypePhase;
                                    tempItemBusinessOportunityPhase.BusinessOportunityDocumentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == detail.id_businessOportunityDocumentTypePhase);
                                    tempItemBusinessOportunityPhase.startDatePhase = detail.startDatePhase;
                                    tempItemBusinessOportunityPhase.endDatePhase = detail.endDatePhase;
                                    tempItemBusinessOportunityPhase.id_employee = detail.id_employee;
                                    tempItemBusinessOportunityPhase.Employee = db.Employee.FirstOrDefault(fod => fod.id == detail.id_employee);
                                    tempItemBusinessOportunityPhase.phaseName = detail.phaseName;
                                    tempItemBusinessOportunityPhase.advance = detail.advance;
                                    tempItemBusinessOportunityPhase.weightedAmount = detail.weightedAmount;
                                    tempItemBusinessOportunityPhase.referencePhase = detail.referencePhase;
                                    tempItemBusinessOportunityPhase.descriptionPhase = detail.descriptionPhase;
                                    //BusinessOportunityPhaseAttachment = new List<BusinessOportunityPhaseAttachment>(),
                                    //BusinessOportunityPhaseActivity = new List<BusinessOportunityPhaseActivity>(),
                                    

                                    //BusinessOportunityPhaseAttachment

                                    if (detail.BusinessOportunityPhaseAttachment != null)
                                    {
                                        var itemBusinessOportunityPhaseAttachment = detail.BusinessOportunityPhaseAttachment.ToList();
                                        foreach (var detailBusinessOportunityPhaseAttachment in itemBusinessOportunityPhaseAttachment)
                                        {
                                            var tempItemBusinessOportunityPhaseAttachment = tempItemBusinessOportunityPhase.BusinessOportunityPhaseAttachment.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseAttachment.id);
                                            if (tempItemBusinessOportunityPhaseAttachment == null)
                                            {
                                                tempItemBusinessOportunityPhaseAttachment = new BusinessOportunityPhaseAttachment
                                                {
                                                    guidPhase = detailBusinessOportunityPhaseAttachment.guidPhase,
                                                    urlPhase = detailBusinessOportunityPhaseAttachment.urlPhase,
                                                    attachmentPhase = detailBusinessOportunityPhaseAttachment.attachmentPhase,
                                                    referencePhaseAttachment = detailBusinessOportunityPhaseAttachment.referencePhaseAttachment,
                                                    descriptionPhaseAttachment = detailBusinessOportunityPhaseAttachment.descriptionPhaseAttachment,
                                                };

                                                tempItemBusinessOportunityPhase.BusinessOportunityPhaseAttachment.Add(tempItemBusinessOportunityPhaseAttachment);
                                            }else
                                            {
                                                if (tempItemBusinessOportunityPhaseAttachment.urlPhase != detailBusinessOportunityPhaseAttachment.urlPhase)
                                                {
                                                    DeleteAttachmentPhase(tempItemBusinessOportunityPhaseAttachment);
                                                    tempItemBusinessOportunityPhaseAttachment.guidPhase = detailBusinessOportunityPhaseAttachment.guidPhase;
                                                    tempItemBusinessOportunityPhaseAttachment.urlPhase = detailBusinessOportunityPhaseAttachment.urlPhase;
                                                    tempItemBusinessOportunityPhaseAttachment.attachmentPhase = detailBusinessOportunityPhaseAttachment.attachmentPhase;
                                                }
                                                tempItemBusinessOportunityPhaseAttachment.referencePhaseAttachment = detailBusinessOportunityPhaseAttachment.referencePhaseAttachment;
                                                tempItemBusinessOportunityPhaseAttachment.descriptionPhaseAttachment = detailBusinessOportunityPhaseAttachment.descriptionPhaseAttachment;
                                                db.Entry(tempItemBusinessOportunityPhaseAttachment).State = EntityState.Modified;
                                            }
                                                
                                        }
                                    }

                                    //BusinessOportunityPhaseActivity
                                    if (detail.BusinessOportunityPhaseActivity != null)
                                    {
                                        var itemBusinessOportunityPhaseActivity = detail.BusinessOportunityPhaseActivity.ToList();
                                        foreach (var detailBusinessOportunityPhaseActivity in itemBusinessOportunityPhaseActivity)
                                        {
                                            var tempItemBusinessOportunityPhaseActivity = tempItemBusinessOportunityPhase.BusinessOportunityPhaseActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id);
                                            if (tempItemBusinessOportunityPhaseActivity == null)
                                            {
                                                tempItemBusinessOportunityPhaseActivity = new BusinessOportunityPhaseActivity
                                                {
                                                    id_businessOportunityActivity = detailBusinessOportunityPhaseActivity.id_businessOportunityActivity,
                                                    BusinessOportunityActivity = db.BusinessOportunityActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_businessOportunityActivity),
                                                    id_state = detailBusinessOportunityPhaseActivity.id_state,
                                                    BusinessOportunityActivityState = db.BusinessOportunityActivityState.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_state),
                                                    referencePhaseActivity = detailBusinessOportunityPhaseActivity.referencePhaseActivity,
                                                    descriptionPhaseActivity = detailBusinessOportunityPhaseActivity.descriptionPhaseActivity,
                                                };

                                                tempItemBusinessOportunityPhase.BusinessOportunityPhaseActivity.Add(tempItemBusinessOportunityPhaseActivity);
                                            }
                                            else
                                            {
                                                tempItemBusinessOportunityPhaseActivity.id_businessOportunityActivity = detailBusinessOportunityPhaseActivity.id_businessOportunityActivity;
                                                tempItemBusinessOportunityPhaseActivity.BusinessOportunityActivity = db.BusinessOportunityActivity.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_businessOportunityActivity);
                                                tempItemBusinessOportunityPhaseActivity.id_state = detailBusinessOportunityPhaseActivity.id_state;
                                                tempItemBusinessOportunityPhaseActivity.BusinessOportunityActivityState = db.BusinessOportunityActivityState.FirstOrDefault(fod => fod.id == detailBusinessOportunityPhaseActivity.id_state);
                                                tempItemBusinessOportunityPhaseActivity.referencePhaseActivity = detailBusinessOportunityPhaseActivity.referencePhaseActivity;
                                                tempItemBusinessOportunityPhaseActivity.descriptionPhaseActivity = detailBusinessOportunityPhaseActivity.descriptionPhaseActivity;
                                                db.Entry(tempItemBusinessOportunityPhaseActivity).State = EntityState.Modified;
                                            }

                                        }
                                    }

                                }
                            }
                        }

                        #endregion

                        if (modelItem.BusinessOportunityPhase.Count == 0)
                        {
                            throw new Exception("No se puede guardar la oportunidad sin Etapas.");
                        }

                        if (modelItem.BusinessOportunityResults.BusinessOportunityState.code != "01")//ABIERTA
                        {
                            modelItem.endDate = tempBusinessOportunity.BusinessOportunityPhase.OrderByDescending(obd => obd.id).First().endDatePhase;
                            tempBusinessOportunity.endDate = modelItem.endDate;
                        }

                        #region BusinessOportunityPartner

                        if (tempBusinessOportunity.BusinessOportunityPartner != null)
                        {
                            for (int i = modelItem.BusinessOportunityPartner.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityPartner.ElementAt(i);

                                modelItem.BusinessOportunityPartner.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            var itemBusinessOportunityPartner = tempBusinessOportunity.BusinessOportunityPartner.ToList();

                            foreach (var detail in itemBusinessOportunityPartner)
                            {
                                var tempItemBusinessOportunityPartner = new BusinessOportunityPartner
                                {
                                    id_partner = detail.id_partner,
                                    Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_partner),
                                    referencePartner = detail.referencePartner,
                                    descriptionPartner = detail.descriptionPartner,
                                    manual = detail.manual,
                                };

                                modelItem.BusinessOportunityPartner.Add(tempItemBusinessOportunityPartner);
                            }
                        }

                        #endregion

                        #region BusinessOportunityCompetition

                        if (tempBusinessOportunity.BusinessOportunityCompetition != null)
                        {
                            for (int i = modelItem.BusinessOportunityCompetition.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityCompetition.ElementAt(i);

                                modelItem.BusinessOportunityCompetition.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            var itemBusinessOportunityCompetition = tempBusinessOportunity.BusinessOportunityCompetition.ToList();

                            foreach (var detail in itemBusinessOportunityCompetition)
                            {
                                var tempItemBusinessOportunityCompetition = new BusinessOportunityCompetition
                                {
                                    id_competitor = detail.id_competitor,
                                    Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_competitor),
                                    referenceCompetition = detail.referenceCompetition,
                                    descriptionCompetition = detail.descriptionCompetition
                                };

                                modelItem.BusinessOportunityCompetition.Add(tempItemBusinessOportunityCompetition);
                            }
                        }

                        #endregion

                        #region BusinessOportunityDocument

                        if (tempBusinessOportunity.BusinessOportunityDocument != null)
                        {
                            var itemBusinessOportunityDocument = tempBusinessOportunity.BusinessOportunityDocument.ToList();

                            for (int i = modelItem.BusinessOportunityDocument.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.BusinessOportunityDocument.ElementAt(i);

                                if(itemBusinessOportunityDocument.FirstOrDefault(fod=> fod.id == detail.id) == null)
                                {
                                    DeleteAttachment(detail);
                                    modelItem.BusinessOportunityDocument.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }
                                
                            }

                            foreach (var detail in itemBusinessOportunityDocument)
                            {
                                var tempItemBusinessOportunityDocument = modelItem.BusinessOportunityDocument.FirstOrDefault(fod=> fod.id == detail.id);
                                if(tempItemBusinessOportunityDocument == null)
                                {
                                    tempItemBusinessOportunityDocument = new BusinessOportunityDocument
                                    {
                                        guid = detail.guid,
                                        url = detail.url,
                                        attachment = detail.attachment,
                                        referenceDocument = detail.referenceDocument,
                                        descriptionDocument = detail.descriptionDocument
                                    };
                                    modelItem.BusinessOportunityDocument.Add(tempItemBusinessOportunityDocument);
                                }
                                else
                                {
                                    if(tempItemBusinessOportunityDocument.url != detail.url)
                                    {
                                        DeleteAttachment(tempItemBusinessOportunityDocument);
                                        tempItemBusinessOportunityDocument.guid = detail.guid;
                                        tempItemBusinessOportunityDocument.url = detail.url;
                                        tempItemBusinessOportunityDocument.attachment = detail.attachment;
                                    }
                                    tempItemBusinessOportunityDocument.referenceDocument = detail.referenceDocument;
                                    tempItemBusinessOportunityDocument.descriptionDocument = detail.descriptionDocument;
                                    db.Entry(tempItemBusinessOportunityDocument).State = EntityState.Modified;
                                }
                                
                            }
                        }

                        #endregion

                        db.BusinessOportunity.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();

                        if (approve)
                        {
                            if (modelItem.Document.DocumentType.code == "15")//Oportunidad de Venta
                            {
                                ServiceSalesRequest.UpdateSalesRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem.BusinessOportunityPlaninng, db, false);
                            }
                            if (modelItem.Document.DocumentType.code == "16")//Oportunidad de Compra
                            {
                                ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem.BusinessOportunityPlaninng, db, false);
                            }
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        UpdateAttachment(modelItem);
                        UpdateAttachmentPhase(modelItem);

                        db.BusinessOportunity.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["businessOportunity"] = modelItem;
                        TempData.Keep("businessOportunity");

                        ViewData["EditMessage"] = SuccessMessage("Oportunidad: " + modelItem.name + ", con número: "+ modelItem.Document.number + " guardada exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData["businessOportunity"] = tempBusinessOportunity;
                    TempData.Keep("businessOportunity");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    SetViewData();

                    return PartialView("_BusinessOportunityMainFormPartial", tempBusinessOportunity);

                }
            }

            SetViewData();

            return PartialView("_BusinessOportunityMainFormPartial", modelItem);
        }
        #endregion

        #region BUSINESS OPORTUNITY NOTES

        [ValidateInput(false)]
        public ActionResult BusinessOportunityViewNotesPartial(int? id_businessOportunity)
        {
            ViewData["id_businessOportunity"] = id_businessOportunity;
            var businessOportunity = db.BusinessOportunity.FirstOrDefault(p => p.id == id_businessOportunity);
            var model = businessOportunity?.BusinessOportunityNote.ToList() ?? new List<BusinessOportunityNote>();

            SetViewData();

            return PartialView("_BusinessOportunityNotesPartial", model.OrderByDescending(od=> od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityNotesPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            var model = businessOportunity.BusinessOportunityNote;
            TempData.Keep("businessOportunity");

            SetViewData();

            return PartialView("_BusinessOportunityNotesEditPartial", model.OrderByDescending(od=> od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityNotesPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityNote item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            
            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunity.BusinessOportunityNote.Count() > 0 ? businessOportunity.BusinessOportunityNote.Max(pld => pld.id) + 1 : 1;
                    businessOportunity.BusinessOportunityNote.Add(item);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityNote;

            SetViewData();

            return PartialView("_BusinessOportunityNotesEditPartial", model.OrderByDescending(od => od.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityNotesPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityNote item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityNote.FirstOrDefault(i => i.id == item.id);
                    //var modelItem = model.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.referenceNote = item.referenceNote;
                        modelItem.descriptionNote = item.descriptionNote;

                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityNote;

            SetViewData();

            return PartialView("_BusinessOportunityNotesEditPartial", model.OrderByDescending(od => od.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityNotesPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            //var model = db.BusinessOportunityNote;
            //if (id >= 0)
            //{
            try
            {
                var item = businessOportunity.BusinessOportunityNote.FirstOrDefault(it => it.id == id);
                //var item = model.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunity.BusinessOportunityNote.Remove(item);

                TempData["businessOportunity"] = businessOportunity;
                //if (item != null)
                //    model.Remove(item);
                //db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityNote;

            SetViewData();

            return PartialView("_BusinessOportunityNotesEditPartial", model.OrderByDescending(od => od.id).ToList());
            //return PartialView("_BusinessOportunityNotesPartial", model.ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY PLANNING DETAILS

        [ValidateInput(false)]
        public ActionResult BusinessOportunityViewPlanningDetailsPartial(int? id_businessOportunity)
        {
            ViewData["id_businessOportunity"] = id_businessOportunity;
            var businessOportunity = db.BusinessOportunity.FirstOrDefault(p => p.id == id_businessOportunity);
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();
            var model = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail;
            SetViewData();

            return PartialView("_BusinessOportunityPlanningDetailsPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPlanningDetailsPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            var model = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail;
            TempData.Keep("businessOportunity");

            SetViewData();

            return PartialView("_BusinessOportunityPlanningDetailsEditPartial", model.OrderByDescending(od => od.id).ToList());

            //var model = new object[0];
            //return PartialView("_BusinessOportunityPlanningDetailsPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPlanningDetailsPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityPlanningDetail item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Count() > 0 ? businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Max(pld => pld.id) + 1 : 1;
                    businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Add(item);
                    UpdateBusinessOportunityPartner(item.id_person, false);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    SetViewData();
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            SetViewData();

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail;

            return PartialView("_BusinessOportunityPlanningDetailsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPlanningDetailsPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityPlanningDetail item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.id_item = item.id_item;
                        modelItem.Item = db.Item.FirstOrDefault(fod=> fod.id == item.id_item);

                        modelItem.quantity = item.quantity;
                        modelItem.price = item.price;

                        modelItem.id_person = item.id_person;
                        modelItem.Person = db.Person.FirstOrDefault(fod => fod.id == item.id_person);

                        //modelItem.id_document = item.id_document;
                        //modelItem.Document = db.Document.FirstOrDefault(fod => fod.id == item.id_document);

                        modelItem.referencePlanning = item.referencePlanning;

                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    SetViewData();

                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            SetViewData();

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail;

            return PartialView("_BusinessOportunityPlanningDetailsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPlanningDetailsPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            try
            {
                var item = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Remove(item);

                UpdateBusinessOportunityPartner(item.id_person, true);

                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                SetViewData();

            }

            SetViewData();


            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail;

            return PartialView("_BusinessOportunityPlanningDetailsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY PHASES

        [ValidateInput(false)]
        public ActionResult BusinessOportunityViewPhasesPartial(int? id_businessOportunity)
        {
            ViewData["id_businessOportunity"] = id_businessOportunity;
            var businessOportunity = db.BusinessOportunity.FirstOrDefault(p => p.id == id_businessOportunity);
            var model = businessOportunity?.BusinessOportunityPhase.ToList() ?? new List<BusinessOportunityPhase>();

            SetViewData();

            return PartialView("_BusinessOportunityViewPhasesPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPhasesPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();

            var model = businessOportunity.BusinessOportunityPhase;
            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            return PartialView("_BusinessOportunityPhasesPartial", model.OrderByDescending(od => od.id).ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhasesPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityPhase item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunity.BusinessOportunityPhase.Count() > 0 ? businessOportunity.BusinessOportunityPhase.Max(pld => pld.id) + 1 : 1;
                    var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod=> fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                    item.phaseName = phaseName;

                    item.BusinessOportunityPhaseAttachment = (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
                    item.BusinessOportunityPhaseAttachment = item.BusinessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();
                    foreach (var detail in item.BusinessOportunityPhaseAttachment)
                    {
                        detail.id_businessOportunityPhase = item.id;
                    }

                    item.BusinessOportunityPhaseActivity = (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
                    item.BusinessOportunityPhaseActivity = item.BusinessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();
                    foreach (var detail in item.BusinessOportunityPhaseActivity)
                    {
                        detail.id_businessOportunityPhase = item.id;
                    }

                    businessOportunity.BusinessOportunityPhase.Add(item);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            var model = businessOportunity.BusinessOportunityPhase;
            SetViewData();

            return PartialView("_BusinessOportunityPhasesPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhasesPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityPhase item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();

            //var model = db.BusinessOportunityNote;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityPhase.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        var phaseName = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(fod => fod.id == item.id_businessOportunityDocumentTypePhase)?.Phase.name ?? "";
                        modelItem.phaseName = phaseName;
                        modelItem.startDatePhase = item.startDatePhase;
                        modelItem.endDatePhase = item.endDatePhase;
                        modelItem.id_employee = item.id_employee;
                        modelItem.id_businessOportunityDocumentTypePhase = item.id_businessOportunityDocumentTypePhase;
                        modelItem.weightedAmount = item.weightedAmount;
                        modelItem.advance = item.advance;
                        modelItem.referencePhase = item.referencePhase;
                        modelItem.descriptionPhase = item.descriptionPhase;

                        modelItem.BusinessOportunityPhaseAttachment = (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
                        modelItem.BusinessOportunityPhaseAttachment = modelItem.BusinessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();
                        foreach (var detail in modelItem.BusinessOportunityPhaseAttachment)
                        {
                            detail.id_businessOportunityPhase = item.id;
                        }


                        modelItem.BusinessOportunityPhaseActivity = (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
                        modelItem.BusinessOportunityPhaseActivity = modelItem.BusinessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();
                        foreach (var detail in modelItem.BusinessOportunityPhaseActivity)
                        {
                            detail.id_businessOportunityPhase = item.id;
                        }

                        //item.phaseName = phaseName;
                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            var model = businessOportunity.BusinessOportunityPhase;
            SetViewData();

            return PartialView("_BusinessOportunityPhasesPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhasesPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();

            try
            {
                var item = businessOportunity.BusinessOportunityPhase.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunity.BusinessOportunityPhase.Remove(item);

                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            var model = businessOportunity.BusinessOportunityPhase;
            SetViewData();

            return PartialView("_BusinessOportunityPhasesPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY PHASE ATTACHMENT

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseViewAttachmentsPartial(int? id_businessOportunityPhaseView)
        {
            ViewData["id_businessOportunityPhaseView"] = id_businessOportunityPhaseView;
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhaseView)?.BusinessOportunityPhaseAttachment.ToList();
            var model = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            SetViewData();
            TempData.Keep("businessOportunity");

            return PartialView("_BusinessOportunityPhaseAttachmentPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseAttachmentsPartial(int id_businessOportunityPhase)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod=> fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            var model = businessOportunityPhaseAttachment;
            TempData["businessOportunityPhaseAttachment"] = businessOportunityPhaseAttachment;
            //TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            return PartialView("_BusinessOportunityPhaseAttachmentEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseAttachmentsPartialAddNew(int id_businessOportunityPhase, DXPANACEASOFT.Models.BusinessOportunityPhaseAttachment item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.attachmentPhase))
                    {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.guidPhase) || string.IsNullOrEmpty(item.urlPhase))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }
                        else
                        {
                            var businessOportunityPhaseAttachmentDetailAux = businessOportunityPhaseAttachment.
                                                    FirstOrDefault(fod => fod.attachmentPhase == item.attachmentPhase);
                            if (businessOportunityPhaseAttachmentDetailAux != null)
                            {
                                throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachmentPhase + ", en el detalle de los Documentos Adjunto.");
                            }

                        }
                    }

                    item.id = businessOportunityPhaseAttachment.Count() > 0 ? businessOportunityPhaseAttachment.Max(pld => pld.id) + 1 : 1;
                    businessOportunityPhaseAttachment.Add(item);
                    TempData["businessOportunityPhaseAttachment"] = businessOportunityPhaseAttachment;
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseAttachment;

            return PartialView("_BusinessOportunityPhaseAttachmentEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseAttachmentsPartialUpdate(int id_businessOportunityPhase, DXPANACEASOFT.Models.BusinessOportunityPhaseAttachment item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunityPhaseAttachment.FirstOrDefault(i => i.id == item.id);
                    if (string.IsNullOrEmpty(item.attachmentPhase))
                    {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.guidPhase) || string.IsNullOrEmpty(item.urlPhase))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }
                        else
                        {
                            if(modelItem.attachmentPhase != item.attachmentPhase)
                            {
                                var businessOportunityPhaseAttachmentDetailAux = businessOportunityPhaseAttachment.
                                                    FirstOrDefault(fod => fod.attachmentPhase == item.attachmentPhase);
                                if (businessOportunityPhaseAttachmentDetailAux != null)
                                {
                                    throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachmentPhase + ", en el detalle de los Documentos Adjunto.");
                                }
                            }
                        }
                    }
                    if (modelItem != null)
                    {
                        modelItem.guidPhase = item.guidPhase;
                        modelItem.urlPhase = item.urlPhase;
                        modelItem.attachmentPhase = item.attachmentPhase;
                        modelItem.referencePhaseAttachment = item.referencePhaseAttachment;
                        modelItem.descriptionPhaseAttachment = item.descriptionPhaseAttachment;

                        this.UpdateModel(modelItem);
                    }
                    TempData["businessOportunityPhaseAttachment"] = businessOportunityPhaseAttachment;
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseAttachment;

            return PartialView("_BusinessOportunityPhaseAttachmentEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseAttachmentsPartialDelete(int id_businessOportunityPhase, System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            try
            {
                var item = businessOportunityPhaseAttachment.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunityPhaseAttachment.Remove(item);

                TempData["businessOportunityPhaseAttachment"] = businessOportunityPhaseAttachment;
                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            //TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseAttachment;

            return PartialView("_BusinessOportunityPhaseAttachmentEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY PHASE ACTIVITY

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseViewActivitiesPartial(int? id_businessOportunityPhaseView)
        {
            ViewData["id_businessOportunityPhaseView"] = id_businessOportunityPhaseView;
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhaseView)?.BusinessOportunityPhaseActivity.ToList();
            var model = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();

            SetViewData();
            TempData.Keep("businessOportunity");

            return PartialView("_BusinessOportunityPhaseActivityPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseActivitiesPartial(int id_businessOportunityPhase)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseActivity.ToList();
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();

            var model = businessOportunityPhaseActivity;
            TempData["businessOportunityPhaseActivity"] = businessOportunityPhaseActivity;
            //TempData.Keep("businessOportunityPhaseActivity");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            return PartialView("_BusinessOportunityPhaseActivityEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseActivitiesPartialAddNew(int id_businessOportunityPhase, DXPANACEASOFT.Models.BusinessOportunityPhaseActivity item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseActivity.ToList();
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunityPhaseActivity.Count() > 0 ? businessOportunityPhaseActivity.Max(pld => pld.id) + 1 : 1;
                    businessOportunityPhaseActivity.Add(item);
                    TempData["businessOportunityPhaseActivity"] = businessOportunityPhaseActivity;
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseActivity;

            return PartialView("_BusinessOportunityPhaseActivityEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseActivitiesPartialUpdate(int id_businessOportunityPhase, DXPANACEASOFT.Models.BusinessOportunityPhaseActivity item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseActivity.ToList();
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunityPhaseActivity.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.id_businessOportunityActivity = item.id_businessOportunityActivity;
                        modelItem.id_state = item.id_state;
                        modelItem.referencePhaseActivity = item.referencePhaseActivity;
                        modelItem.descriptionPhaseActivity = item.descriptionPhaseActivity;
                        this.UpdateModel(modelItem);
                    }
                    TempData["businessOportunityPhaseActivity"] = businessOportunityPhaseActivity;
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseActivity;

            return PartialView("_BusinessOportunityPhaseActivityEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityPhaseActivitiesPartialDelete(int id_businessOportunityPhase, System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseActivity.ToList();
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? (TempData["businessOportunityPhaseActivity"] as List<BusinessOportunityPhaseActivity>);
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();

            try
            {
                var item = businessOportunityPhaseActivity.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunityPhaseActivity.Remove(item);

                TempData["businessOportunityPhaseActivity"] = businessOportunityPhaseActivity;
                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            //TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            var model = businessOportunityPhaseActivity;

            return PartialView("_BusinessOportunityPhaseActivityEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY PARTNERS

        [ValidateInput(false)]
        public ActionResult BusinessViewPartnersPartial(int? id_businessOportunity)
        {
            ViewData["id_businessOportunity"] = id_businessOportunity;
            var businessOportunity = db.BusinessOportunity.FirstOrDefault(p => p.id == id_businessOportunity);
            var model = businessOportunity?.BusinessOportunityPartner.ToList() ?? new List<BusinessOportunityPartner>();

            SetViewData();

            return PartialView("_BusinessOportunityPartnersPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessPartnersPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            var model = businessOportunity.BusinessOportunityPartner;
            TempData.Keep("businessOportunity");
            SetViewData();

            return PartialView("_BusinessOportunityPartnersEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessPartnersPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityPartner item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunity.BusinessOportunityPartner.Count() > 0 ? businessOportunity.BusinessOportunityPartner.Max(pld => pld.id) + 1 : 1;
                    item.manual = true;
                    businessOportunity.BusinessOportunityPartner.Add(item);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPartner;
            SetViewData();

            return PartialView("_BusinessOportunityPartnersEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessPartnersPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityPartner item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityPartner.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.referencePartner = item.referencePartner;
                        modelItem.descriptionPartner = item.descriptionPartner;

                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPartner;
            SetViewData();

            return PartialView("_BusinessOportunityPartnersEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessPartnersPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            try
            {
                var item = businessOportunity.BusinessOportunityPartner.FirstOrDefault(it => it.id == id);
                if (item != null)
                {
                    if (item.manual)
                    {
                        businessOportunity.BusinessOportunityPartner.Remove(item);
                    }else
                    {
                        throw new Exception("No se puede eliminar un socio de negocio generado automáticamente por el detalle de la planificación.");
                    }
                }
                    

                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                TempData["businessOportunity"] = businessOportunity;

            }

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityPartner;
            SetViewData();

            return PartialView("_BusinessOportunityPartnersEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY COMPETITIONS

        [ValidateInput(false)]
        public ActionResult BusinessOportunityViewCompetitionsPartial(int? id_businessOportunity)
        {
            ViewData["id_businessOportunity"] = id_businessOportunity;
            var businessOportunity = db.BusinessOportunity.FirstOrDefault(p => p.id == id_businessOportunity);
            var model = businessOportunity?.BusinessOportunityCompetition.ToList() ?? new List<BusinessOportunityCompetition>();

            SetViewData();

            return PartialView("_BusinessOportunityCompetitionsPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BusinessOportunityCompetitionsPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            var model = businessOportunity.BusinessOportunityCompetition;
            TempData.Keep("businessOportunity");
            SetViewData();

            return PartialView("_BusinessOportunityCompetitionsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityCompetitionsPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityCompetition item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    item.id = businessOportunity.BusinessOportunityCompetition.Count() > 0 ? businessOportunity.BusinessOportunityCompetition.Max(pld => pld.id) + 1 : 1;
                    businessOportunity.BusinessOportunityCompetition.Add(item);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityCompetition;
            SetViewData();

            return PartialView("_BusinessOportunityCompetitionsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityCompetitionsPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityCompetition item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityCompetition.FirstOrDefault(i => i.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.referenceCompetition = item.referenceCompetition;
                        modelItem.descriptionCompetition = item.descriptionCompetition;

                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityCompetition;
            SetViewData();

            return PartialView("_BusinessOportunityCompetitionsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityCompetitionsPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            try
            {
                var item = businessOportunity.BusinessOportunityCompetition.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunity.BusinessOportunityCompetition.Remove(item);

                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityCompetition;
            SetViewData();

            return PartialView("_BusinessOportunityCompetitionsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region BUSINESS OPORTUNITY ATTACHED DOCUMENTS

        [ValidateInput(false)]
        public ActionResult BusinessOportunityAttachedDocumentsPartial()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            var model = businessOportunity.BusinessOportunityDocument;
            TempData.Keep("businessOportunity");
            SetViewData();

            return PartialView("_BusinessOportunityAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.BusinessOportunityDocument item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.attachment)) {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    } else {
                        if(string.IsNullOrEmpty(item.guid) || string.IsNullOrEmpty(item.url))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }else
                        {
                            var businessOportunityDocumentDetailAux = businessOportunity.
                                                    BusinessOportunityDocument.
                                                    FirstOrDefault(fod => fod.attachment == item.attachment);
                            if (businessOportunityDocumentDetailAux != null)
                            {
                                throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachment + ", en el detalle de los Documentos Adjunto.");
                            }
                           
                        }
                    }
                    item.id = businessOportunity.BusinessOportunityDocument.Count() > 0 ? businessOportunity.BusinessOportunityDocument.Max(pld => pld.id) + 1 : 1;
                    businessOportunity.BusinessOportunityDocument.Add(item);
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityDocument;
            SetViewData();

            return PartialView("_BusinessOportunityAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.BusinessOportunityDocument item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = businessOportunity.BusinessOportunityDocument.FirstOrDefault(i => i.id == item.id);
                    if (string.IsNullOrEmpty(item.attachment))
                    {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.guid) || string.IsNullOrEmpty(item.url))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }
                        else
                        {
                            if(modelItem.attachment != item.attachment)
                            {
                                var businessOportunityDocumentDetailAux = businessOportunity.
                                                      BusinessOportunityDocument.
                                                      FirstOrDefault(fod => fod.attachment == item.attachment);
                                if (businessOportunityDocumentDetailAux != null)
                                {
                                    throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachment + ", en el detalle de los Documentos Adjunto.");
                                }
                            }
                        }
                    }
                    if (modelItem != null)
                    {
                        modelItem.guid = item.guid;
                        modelItem.url = item.url;
                        modelItem.attachment = item.attachment;
                        modelItem.referenceDocument = item.referenceDocument;
                        modelItem.descriptionDocument = item.descriptionDocument;

                        this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                    TempData["businessOportunity"] = businessOportunity;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityDocument;
            SetViewData();

            return PartialView("_BusinessOportunityAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessOportunityAttachedDocumentsPartialDelete(System.Int32 id)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            try
            {
                var item = businessOportunity.BusinessOportunityDocument.FirstOrDefault(it => it.id == id);
                if (item != null)
                    businessOportunity.BusinessOportunityDocument.Remove(item);

                TempData["businessOportunity"] = businessOportunity;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("businessOportunity");
            var model = businessOportunity.BusinessOportunityDocument;
            SetViewData();

            return PartialView("_BusinessOportunityAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            BusinessOportunity businessOportunity = db.BusinessOportunity.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (businessOportunity.BusinessOportunityResults.BusinessOportunityState.code != "01")//ABIERTA
                    {
                        TempData.Keep("businessOportunity");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la oportunidad debido a tener estado de oportunidad: " + businessOportunity.BusinessOportunityResults.BusinessOportunityState.name);
                        return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);
                    }

                    if (businessOportunity != null && documentState != null)
                    {

                        if (businessOportunity.Document.DocumentState.code != "01")// Pendiente
                        {
                            if (businessOportunity.Document.DocumentType.code == "15")//Oportunidad de Venta
                            {
                                ServiceSalesRequest.UpdateSalesRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, true);
                            }
                            if (businessOportunity.Document.DocumentType.code == "16")//Oportunidad de Compra
                            {
                                ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, true);
                            }
                        }

                        businessOportunity.Document.id_documentState = documentState.id;
                        businessOportunity.Document.DocumentState = documentState;

                        db.BusinessOportunity.Attach(businessOportunity);
                        db.Entry(businessOportunity).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["businessOportunity"] = businessOportunity;
                        TempData.Keep("businessOportunity");

                        ViewData["EditMessage"] = SuccessMessage("Oportunidad: " + businessOportunity.name + ", con número: " + businessOportunity.Document.number + " anulada exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("businessOportunity");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);

        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            BusinessOportunity businessOportunity = db.BusinessOportunity.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    if (businessOportunity.BusinessOportunityResults.BusinessOportunityState.code != "01")//ABIERTA
                    {
                        TempData.Keep("businessOportunity");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la oportunidad debido a tener estado de oportunidad: " + businessOportunity.BusinessOportunityResults.BusinessOportunityState.name);
                        return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);
                    }

                    DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (businessOportunity != null && documentStatePendiente != null)
                    {
                        if (businessOportunity.Document.DocumentState.code != "01")// Pendiente
                        {
                            if (businessOportunity.Document.DocumentType.code == "15")//Oportunidad de Venta
                            {
                                ServiceSalesRequest.UpdateSalesRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, true);
                            }
                            if (businessOportunity.Document.DocumentType.code == "16")//Oportunidad de Compra
                            {
                                ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunity.BusinessOportunityPlaninng, db, true);
                            }
                        }

                        businessOportunity.Document.id_documentState = documentStatePendiente.id;
                        businessOportunity.Document.DocumentState = documentStatePendiente;

                        db.BusinessOportunity.Attach(businessOportunity);
                        db.Entry(businessOportunity).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["businessOportunity"] = businessOportunity;
                        TempData.Keep("businessOportunity");

                        ViewData["EditMessage"] = SuccessMessage("Oportunidad: " + businessOportunity.name + ", con número: " + businessOportunity.Document.number + " reversada exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("businessOportunity");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_BusinessOportunityMainFormPartial", businessOportunity);
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

            BusinessOportunity businessOportunity = db.BusinessOportunity.FirstOrDefault(r => r.id == id);
            //int state = businessOportunity.Document.DocumentState.id;
            string state = businessOportunity.Document.DocumentState.code;
            string stateBusinessOportunity = businessOportunity.BusinessOportunityResults?.BusinessOportunityState?.code ?? "01";//Estado Abierto de la Oportunidad

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
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = stateBusinessOportunity == "01",
                    btnRevert = stateBusinessOportunity == "01"
                };
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
                    btnCancel = stateBusinessOportunity == "01",
                    btnRevert = stateBusinessOportunity == "01"
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region BUSINESS OPORTUNITY ATTACHMENT

        private void UpdateAttachment(BusinessOportunity businessOportunity)
        {
            List<BusinessOportunityDocument> businessOportunityDocument = businessOportunity.BusinessOportunityDocument.ToList() ?? new List<BusinessOportunityDocument>();
            foreach (var item in businessOportunityDocument)
            {
                if (item.url == FileUploadHelper.UploadDirectoryDefaultTemp)
                {
                    try
                    {
                        // Carga el contenido guardado en el temp
                        string nameAttachment;
                        string typeContentAttachment;
                        string guidAux = item.guid;
                        string urlAux = item.url;
                        var contentAttachment = FileUploadHelper.ReadFileUpload(
                            ref guidAux, out nameAttachment, out typeContentAttachment);

                        // Guardamos en el directorio final el fichero que este aun en su ruta temporal
                        item.guid = FileUploadHelper.FileUploadProcessAttachment("/BusinessOportunity/" + businessOportunity.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
                        item.url = urlAux;

                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
                    }

                }
            }
        }

        private void UpdateAttachmentPhase(BusinessOportunity businessOportunity)
        {
            List<BusinessOportunityPhase> businessOportunityPhase = businessOportunity.BusinessOportunityPhase.ToList() ?? new List<BusinessOportunityPhase>();
            foreach (var item in businessOportunityPhase)
            {
                if (item.BusinessOportunityPhaseAttachment != null && item.BusinessOportunityPhaseAttachment.Count > 0)
                {
                    foreach (var detail in item.BusinessOportunityPhaseAttachment.ToList())
                    {
                        if (detail.urlPhase == FileUploadHelper.UploadDirectoryDefaultTemp)
                        {
                            try
                            {
                                // Carga el contenido guardado en el temp
                                string nameAttachment;
                                string typeContentAttachment;
                                string guidAux = detail.guidPhase;
                                string urlAux = detail.urlPhase;
                                var contentAttachment = FileUploadHelper.ReadFileUpload(
                                    ref guidAux, out nameAttachment, out typeContentAttachment);

                                // Guardamos en el directorio final el fichero que este aun en su ruta temporal
                                detail.guidPhase = FileUploadHelper.FileUploadProcessAttachment("/BusinessOportunity/" + businessOportunity.id.ToString() + 
                                                                                                "/BusinessOportunityPhase/" + item.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
                                detail.urlPhase = urlAux;

                            }
                            catch (Exception exception)
                            {
                                throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
                            }

                        }
                    }
                }
                
            }
        }

        private void DeleteAttachment(BusinessOportunityDocument businessOportunityDocument)
        {
            if (businessOportunityDocument.url != FileUploadHelper.UploadDirectoryDefaultTemp)
            {
                try
                {
                    // Carga el contenido guardado en el temp
                    FileUploadHelper.CleanUpUploadedFiles(businessOportunityDocument.url, businessOportunityDocument.guid);

                }
                catch (Exception exception)
                {
                    throw new Exception("Error al borrar el adjunto. Error: " + exception.Message);
                }
            }
        }

        private void DeleteAttachmentPhase(BusinessOportunityPhaseAttachment businessOportunityPhaseAttachment)
        {
            if (businessOportunityPhaseAttachment.urlPhase != FileUploadHelper.UploadDirectoryDefaultTemp)
            {
                try
                {
                    // Carga el contenido guardado en el temp
                    FileUploadHelper.CleanUpUploadedFiles(businessOportunityPhaseAttachment.urlPhase, businessOportunityPhaseAttachment.guidPhase);

                }
                catch (Exception exception)
                {
                    throw new Exception("Error al borrar el adjunto. Error: " + exception.Message);
                }
            }
        }

        [HttpGet]
        [ActionName("download-attachment")]
        public ActionResult DownloadAttachment(int id)
        {
            TempData.Keep("businessOportunity");

            try
            {
                BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
                List<BusinessOportunityDocument> businessOportunityDocument = businessOportunity.BusinessOportunityDocument.ToList() ?? new List<BusinessOportunityDocument>();
                var businessOportunityDocumentAux = businessOportunityDocument.FirstOrDefault(fod=> fod.id == id);
                if(businessOportunityDocumentAux != null)
                {
                    // Carga el contenido guardado en el temp
                    string nameAttachment;
                    string typeContentAttachment;
                    string guidAux = businessOportunityDocumentAux.guid;
                    string urlAux = businessOportunityDocumentAux.url;
                    var contentAttachment = FileUploadHelper.ReadFileUpload(
                        ref guidAux, ref urlAux, out nameAttachment, out typeContentAttachment);

                    return this.File(contentAttachment, typeContentAttachment, nameAttachment);
                }
                else
                {
                    return this.File(new byte[] { }, "", "");
                }

            }
            catch (Exception exception)
            {
                throw new Exception("Error al bajar el adjunto. Error: " + exception.Message);
            }
        }

        [HttpGet]
        [ActionName("download-attachmentPhase")]
        public ActionResult DownloadAttachmentPhase(int id_businessOportunityPhase, int id)
        {
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            try
            {
                BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
                List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
                businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
                businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();


                var businessOportunityPhaseAttachmentAux = businessOportunityPhaseAttachment.FirstOrDefault(fod => fod.id == id);
                if (businessOportunityPhaseAttachmentAux != null)
                {
                    // Carga el contenido guardado en el temp
                    string nameAttachment;
                    string typeContentAttachment;
                    string guidAux = businessOportunityPhaseAttachmentAux.guidPhase;
                    string urlAux = businessOportunityPhaseAttachmentAux.urlPhase;
                    var contentAttachment = FileUploadHelper.ReadFileUpload(
                        ref guidAux, ref urlAux, out nameAttachment, out typeContentAttachment);

                    return this.File(contentAttachment, typeContentAttachment, nameAttachment);
                }
                else
                {
                    return this.File(new byte[] { }, "", "");
                }

            }
            catch (Exception exception)
            {
                throw new Exception("Error al bajar el adjunto. Error: " + exception.Message);
            }
        }


        #region UPLOAD FILE

        [HttpPost]
        [ActionName("upload-attachment")]
        public ActionResult UploadControlUpload()
        {
            TempData.Keep("businessOportunity");
            UploadControlExtension.GetUploadedFiles(
                "attachmentUploadControl", UploadControlSettings.UploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost]
        [ActionName("upload-attachmentPhase")]
        public ActionResult UploadControlUploadPhase()
        {
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            UploadControlExtension.GetUploadedFiles(
                "attachmentPhaseUploadControl", UploadControlSettings.UploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        public class UploadControlSettings
        {
            public readonly static UploadControlValidationSettings UploadValidationSettings;

            static UploadControlSettings()
            {
                UploadValidationSettings = new UploadControlValidationSettings()
                {
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };
            }

            public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
            {
                if (e.UploadedFile.IsValid)
                {
                    e.CallbackData = FileUploadHelper.FileUploadProcess(e);
                }
            }
        }

        #endregion

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_businessOportunity)
        {
            TempData.Keep("bussinesOportunity");

            int index = db.BusinessOportunity.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_businessOportunity);

            var result = new
            {
                maximunPages = db.BusinessOportunity.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            BusinessOportunity bussinesOportunity = db.BusinessOportunity.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (bussinesOportunity != null)
            {
                TempData["bussinesOportunity"] = bussinesOportunity;
                TempData.Keep("bussinesOportunity");
                return PartialView("_BusinessOportunityMainFormPartial", bussinesOportunity);
            }

            TempData.Keep("bussinesOportunity");
            SetViewData();

            return PartialView("_BusinessOportunityMainFormPartial", new BusinessOportunity());
        }

        #endregion

        #region AXILIAR FUNCTIONS
        
        private void UpdateBusinessOportunityPartner(int id_partner, bool delete)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            List<BusinessOportunityPartner> businessOportunityPartner = businessOportunity.BusinessOportunityPartner?.ToList();
            if (businessOportunityPartner != null)
            {
                var businessOportunityPartnerDetailAux = businessOportunityPartner.FirstOrDefault(fod => fod.id_partner == id_partner);
                if (businessOportunityPartnerDetailAux != null)
                {
                    
                    businessOportunityPartnerDetailAux.manual = delete;
                   
                }
                else
                {
                    if (!delete)
                    {
                        businessOportunityPartner.Add(new BusinessOportunityPartner
                        {
                            id = businessOportunityPartner.Count() > 0 ? businessOportunityPartner.Max(pld => pld.id) + 1 : 1,
                            id_businessOportunity = businessOportunity.id,
                            BusinessOportunity = businessOportunity,
                            id_partner = id_partner,
                            Person = db.Person.FirstOrDefault(fod => fod.id == id_partner),
                            referencePartner = "",
                            descriptionPartner = "",
                            manual = false
                        });
                    }
                }
            }
            else
            {
                businessOportunityPartner = new List<BusinessOportunityPartner>();
                if (!delete)
                {
                    businessOportunityPartner.Add(new BusinessOportunityPartner
                    {
                        id = businessOportunityPartner.Count() > 0 ? businessOportunityPartner.Max(pld => pld.id) + 1 : 1,
                        id_businessOportunity = businessOportunity.id,
                        BusinessOportunity = businessOportunity,
                        id_partner = id_partner,
                        Person = db.Person.FirstOrDefault(fod => fod.id == id_partner),
                        referencePartner = "",
                        descriptionPartner = "",
                        manual = false
                    });
                }
            }

            businessOportunity.BusinessOportunityPartner = businessOportunityPartner;
            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
        }

        private void SetViewData()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            ViewData["codeBusinessOportunityDocumentType"] = businessOportunity?.Document?.DocumentType?.code ?? "";
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PlaningDetails(int? id_itemCurrent, int? id_personCurrent, int? id_documentCurrent, int? id_documentType)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPlaninng = businessOportunity.BusinessOportunityPlaninng ?? new BusinessOportunityPlaninng();
            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            var documentTypeAux = db.DocumentType.FirstOrDefault(fod=> fod.id == id_documentType);
            var codeDocumentTypeAux = documentTypeAux?.code ?? "";

            //15: Oportunidad de Venta y 16: Oportunidad de Compra
            
            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && ((codeDocumentTypeAux == "15" && w.isSold) || (codeDocumentTypeAux == "16" && w.isPurchased))) || w.id == id_itemCurrent).ToList();
            //var tempItems = new List<Item>();
            //foreach (var i in items)
            //{
            //    if (!(businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Any(a => a.id_item == i.id)) || i.id == id_itemCurrent)
            //    {
            //        tempItems.Add(i);
            //    }

            //}
            //items = tempItems;

            var persons = db.Person.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && ((codeDocumentTypeAux == "15" && w.Customer != null) || (codeDocumentTypeAux == "16" && w.Provider != null))) || w.id == id_personCurrent).ToList();

            //var documents = db.Document.Where(w => (w.EmissionPoint.BranchOffice.Division.id_company == this.ActiveCompanyId && ((codeDocumentTypeAux == "15" && w.SalesQuotation != null && w.DocumentState.code == "06" && w.SalesQuotation.SalesQuotationDetail.FirstOrDefault(fod=> fod.isActive && fod.id_item == id_itemCurrent) != null) || 
            //                                                                                                                 (codeDocumentTypeAux == "16" && w.PurchaseOrder != null && w.DocumentState.code == "06" && w.PurchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == id_itemCurrent) != null))) || w.id == id_documentCurrent).ToList();
            var result = new
            {
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    MetricUnitCode = (codeDocumentTypeAux) == "15" ? s.ItemSaleInformation?.MetricUnit.code ?? "": ((codeDocumentTypeAux) == "16" ? s.ItemPurchaseInformation?.MetricUnit.code ?? "" : ("")),
                    name = s.name
                }).ToList(),
                persons = persons.Select(s => new
                {
                    id = s.id,
                    fullname_businessName = s.fullname_businessName
                }).ToList(),
                //documents = documents.Select(s => new
                //{
                //    id = s.id,
                //    number = s.number
                //}).ToList(),
                Message = "Ok"
            };

            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemPlaningDetailsData(int? id_item, int? id_documentType, int? id_priceList)
        {

            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var documentTypeAux = db.DocumentType.FirstOrDefault(fod => fod.id == id_documentType);
            var codeDocumentTypeAux = documentTypeAux?.code ?? "";

            var priceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
            //salesOrder.PriceList = priceList;
            //var listSalesOrderDetail = salesOrder.SalesOrderDetail.ToList();
            decimal priceAux = 0;
            if (priceList != null && id_item != null)
            {
                var detailtPriceList = priceList?.PriceListDetail.FirstOrDefault(fod => fod.id_item == id_item);
                var metricUnitPriceList = detailtPriceList?.MetricUnit;

                if (metricUnitPriceList != null)
                {
                    if((codeDocumentTypeAux) == "15")
                    {
                        var metricUnitSale = item.ItemSaleInformation.MetricUnit;

                        var metricUnitAux = metricUnitPriceList;
                        decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

                        if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
                        {
                            metricUnitAux = item.Presentation?.MetricUnit;
                            var factorConversionAux = GetFactorConversion(metricUnitSale, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la planificación de la Oportunidad de Venta Configúrelo, e intente de nuevo", db);

                            priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (item.Presentation?.minimum ?? 1);

                        }

                        var metricUnitDetail = item.Presentation?.MetricUnit;

                        var factorConversion = GetFactorConversion(metricUnitDetail, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle de la planificación de la Oportunidad de Venta Configúrelo, e intente de nuevo", db);

                        priceAux = (priceMetricUnitPresentation * factorConversion) * (item.Presentation?.minimum ?? 1);
                    }

                    if ((codeDocumentTypeAux) == "16")
                    {
                        var metricUnitPurchase = item.ItemPurchaseInformation.MetricUnit;

                        var metricUnitAux = metricUnitPriceList;
                        decimal priceMetricUnitPresentation = detailtPriceList.purchasePrice;

                        if (metricUnitPriceList.id_metricType == metricUnitPurchase.id_metricType)
                        {
                            //metricUnitAux = salesOrderDetail.Item.Presentation?.MetricUnit;
                            var factorConversionAux = GetFactorConversion(metricUnitPurchase, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitPurchase.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la planificación de la Oportunidad de Venta Configúrelo, e intente de nuevo", db);

                            priceAux = (priceMetricUnitPresentation * factorConversionAux);

                        }else
                        {
                            throw new Exception("No existe el Factor de Conversión entre:  " + metricUnitPurchase.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la planificación de la Oportunidad de Venta  debido a pertenecer a tipo de unidad de medida diferente, Verifique el caso , e intente de nuevo");
                        }

                    }
                }
                else
                {
                    priceAux = 0;
                }

            }
            else
            {
                priceAux = 0;
            }
            //15: Oportunidad de Venta y 16: Oportunidad de Compra
            //var documents = db.Document.Where(w => (w.EmissionPoint.BranchOffice.Division.id_company == this.ActiveCompanyId && ((codeDocumentTypeAux == "15" && w.SalesQuotation != null && w.DocumentState.code == "06" && w.SalesQuotation.SalesQuotationDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == id_item) != null) ||
            //                                                                                                                 (codeDocumentTypeAux == "16" && w.PurchaseOrder != null && w.DocumentState.code == "06" && w.PurchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == id_item) != null)))).ToList();

            var result = new
            {
                metricUnit = (codeDocumentTypeAux) == "15" ? item.ItemSaleInformation?.MetricUnit.code ?? "" : ((codeDocumentTypeAux) == "16" ? item.ItemPurchaseInformation?.MetricUnit.code ?? "" : ("")),
                price = (codeDocumentTypeAux) == "15" ? ((priceList != null && id_item != null) ? priceAux : (item.ItemSaleInformation?.salePrice ?? 0)) :
                        ((codeDocumentTypeAux) == "16" ? ((priceList != null && id_item != null) ? priceAux : (item.ItemPurchaseInformation?.purchasePrice ?? 0)) : (0)),
                size = item.ItemGeneral?.ItemSize?.name ?? "",
                itemTypeCategory = item.ItemTypeCategory?.name ?? "",
                //documents = documents.Select(s => new
                //{
                //    id = s.id,
                //    number = s.number
                //}).ToList()
                //ItemDetailData = new
                //{
                //    masterCode = item.masterCode,
                //    metricUnit = item.ItemInventory?.MetricUnit?.code,
                //}
            };

            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            //TempData.Keep("businessOportunityPhaseAttachment");
            //TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedPlaningDetail(int? id_itemNew, int? id_personNew, int? id_documentNew)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var businessOportunityPlanningDetailAux = businessOportunity.
                                                      BusinessOportunityPlaninng.
                                                      BusinessOportunityPlanningDetail.
                                                      FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                     fod.id_person == id_personNew &&
                                                                     fod.id_document == id_documentNew);
            if (businessOportunityPlanningDetailAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var personAux = db.Person.FirstOrDefault(fod => fod.id == id_personNew);
                var documentAux = db.Document.FirstOrDefault(fod => fod.id == id_documentNew);
                var strDocument = documentAux == null ? ", sin Cotización/Orden asignada" : ", con la Cotización/Orden número: " + documentAux.number;
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  con el socio de negocio: " + personAux.fullname_businessName +
                            strDocument + ", en los detalles de la planificación."

                };

            }


            //TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PhaseDetails(int? id_businessOportunityDocumentTypePhaseCurrent, int? idCurrent, int? id_documentType)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == idCurrent)?.BusinessOportunityPhaseAttachment.ToList();
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();
            TempData["businessOportunityPhaseAttachment"] = businessOportunityPhaseAttachment;

            List<BusinessOportunityPhaseActivity> businessOportunityPhaseActivity = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == idCurrent)?.BusinessOportunityPhaseActivity.ToList();
            businessOportunityPhaseActivity = businessOportunityPhaseActivity ?? new List<BusinessOportunityPhaseActivity>();
            TempData["businessOportunityPhaseActivity"] = businessOportunityPhaseActivity;

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();
            //businessOportunity.BusinessOportunityPhase.BusinessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail ?? new List<BusinessOportunityPlanningDetail>();

            var documentTypeAux = db.DocumentType.FirstOrDefault(fod => fod.id == id_documentType);
            var codeDocumentTypeAux = documentTypeAux?.code ?? "";

            var idMax = businessOportunity.BusinessOportunityPhase.Count() > 0 ? businessOportunity.BusinessOportunityPhase.Max(m=> m.id) : 0;
            var enabled = idCurrent == null || idCurrent == 0 || idCurrent == idMax;
            //15: Oportunidad de Venta y 16: Oportunidad de Compra

            var advanceMax = businessOportunity.BusinessOportunityPhase.Count() > 0 ? businessOportunity.BusinessOportunityPhase.Max(m => m.advance) : 0;

            var businessOportunityDocumentTypePhases = db.BusinessOportunityDocumentTypePhase.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_documentType == id_documentType && w.advance >= advanceMax) || w.id == id_businessOportunityDocumentTypePhaseCurrent).ToList();
            var tempBusinessOportunityDocumentTypePhases = new List<BusinessOportunityDocumentTypePhase>();
            foreach (var i in businessOportunityDocumentTypePhases)
            {
                if (!(businessOportunity.BusinessOportunityPhase.Any(a => a.id_businessOportunityDocumentTypePhase == i.id)) || i.id == id_businessOportunityDocumentTypePhaseCurrent)
                {
                    //if(!tempPhases.Contains(i.Phase))
                    tempBusinessOportunityDocumentTypePhases.Add(i);
                }

            }
            businessOportunityDocumentTypePhases = tempBusinessOportunityDocumentTypePhases;
            var potentialAmount = businessOportunity.BusinessOportunityPlaninng?.amount ?? 0;
            var result = new
            {
                enabled = enabled,
                businessOportunityDocumentTypePhases = businessOportunityDocumentTypePhases.Select(s => new { id = s.id, name = s.Phase.name, advance = s.advance }).OrderBy(ob => ob.advance).ToList(),
                potentialAmount = potentialAmount,
                Message = "Ok"
            };

            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PhaseDetailsData(int? id_businessOportunityDocumentTypePhase, string strPotentialAmount)
        {

            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            BusinessOportunityDocumentTypePhase businessOportunityDocumentTypePhase = db.BusinessOportunityDocumentTypePhase.FirstOrDefault(i => i.id == id_businessOportunityDocumentTypePhase);

            if (businessOportunityDocumentTypePhase == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            decimal potentialAmount = Convert.ToDecimal(strPotentialAmount);
            //var potentialAmount = businessOportunity.BusinessOportunityPlaninng?.amount ?? 0;
            var amount = (potentialAmount * businessOportunityDocumentTypePhase.advance) / 100;
            var result = new
            {
                amount = amount,
                advance = businessOportunityDocumentTypePhase.advance
            };

            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult DocumentTypeDetailsData(int? id_documentType)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);


            businessOportunity = businessOportunity ?? new BusinessOportunity();

            List <BusinessOportunityPlanningDetail>  businessOportunityPlanningDetail = businessOportunity.BusinessOportunityPlaninng?.BusinessOportunityPlanningDetail?.ToList() ?? new List<BusinessOportunityPlanningDetail>();

            for (int i = businessOportunityPlanningDetail.Count - 1; i >= 0; i--)
            {
                var detail = businessOportunityPlanningDetail.ElementAt(i);
                UpdateBusinessOportunityPartner(detail.id_person, true);
                businessOportunityPlanningDetail.Remove(detail);
            }

            businessOportunity.Document = businessOportunity.Document ?? new Document();

            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == id_documentType);

            businessOportunity.Document.DocumentType = documentType;


            var documentStates = documentType?.DocumentState.Where(s => s.isActive)?.ToList() ?? new List<DocumentState>();

            //var businessPartners = db.Person.Where(t => (t.Provider != null || t.Customer != null) && t.isActive && t.id_company == this.ActiveCompanyId).ToList();


            if (documentType == null)
            {
                documentStates =  db.DocumentState.Where(s => s.id_company == this.ActiveCompanyId && s.isActive)?.ToList();
            }else
            {
                //if (documentType.code.Equals("15"))//15:Oportunidad de Venta
                //{
                //    businessPartners = businessPartners.Where(t => t.Customer != null).ToList();
                //}
                //if (documentType.code.Equals("16"))//16:Oportunidad de Compra
                //{
                //    businessPartners = businessPartners.Where(t => t.Provider != null).ToList();
                //}
            }

            var result = new
            {
                documentStates = documentStates.Select(s => new
                {
                    id = s.id,
                    name = s.name
                }).ToList(),

            };

            businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail = businessOportunityPlanningDetail;
            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            SetViewData();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult BusinessPartnerDetailsData(int? id_businessPartner, int? id_documentType, string contactPerson, string locationName)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == id_documentType);
            var businessPartner = db.Person.FirstOrDefault(t => t.id == id_businessPartner);

            var businessOportunityPartnerAux = businessOportunity.BusinessOportunityPartner.FirstOrDefault(fod=> fod.id_partner == id_businessPartner);

            if(businessOportunityPartnerAux != null)
            {
                businessOportunity.BusinessOportunityPartner.Remove(businessOportunityPartnerAux);
            }

            foreach (var bbb in businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail)
            {
                if(bbb.id_person != id_businessPartner) UpdateBusinessOportunityPartner(bbb.id_person, false);
            }

            decimal totalAux = 0;
            string contactPersonAux = contactPerson;
            string locationNameAux = locationName;

            if (documentType == null)
            {
                if (businessPartner != null)
                {
                    contactPersonAux = businessPartner.Provider?.ProviderGeneralData.contactName ?? businessPartner.fullname_businessName;
                    locationNameAux = businessPartner.address;
                }
            }
            else
            {
                if (documentType.code.Equals("15"))//15:Oportunidad de Venta
                {
                    if (businessPartner != null)
                    {
                        totalAux = businessPartner.Customer.SalesQuotation.Where(w => w.Document.DocumentState.code.Equals("06")).Sum(s => (s.SalesQuotationTotal?.total ?? 0));
                        contactPersonAux = businessPartner.fullname_businessName;
                        locationNameAux = businessPartner.address;
                    }
                }
                if (documentType.code.Equals("16"))//16:Oportunidad de Compra
                {
                    if (businessPartner != null)
                    {
                        totalAux = businessPartner.Provider.PurchaseOrder.Where(w => w.Document.DocumentState.code.Equals("06")).Sum(s => (s.PurchaseOrderTotal?.total ?? 0));
                        contactPersonAux = businessPartner.Provider.ProviderGeneralData?.contactName ?? businessPartner.fullname_businessName;
                        locationNameAux = businessPartner.address;
                    }
                }
            }

            var result = new
            {
                total = totalAux,
                contactPerson = contactPersonAux,
                locationName = locationNameAux,
            };

            TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBusinessOportunityAmount()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            decimal amountAux = businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail.Sum(s=> (s.quantity * s.price));

            TempData.Keep("businessOportunity");

            return Json(new
            {
                amount = amountAux
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsStartDatePhaseValidWithLastEndDatePhaseDetail(string startDatePhase, int? idCurrent)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPhase = businessOportunity.BusinessOportunityPhase ?? new List<BusinessOportunityPhase>();


            var result = new
            {
                itsValided = 1,
                Error = ""

            };

            DateTime _startDatePhase = JsonConvert.DeserializeObject<DateTime>(startDatePhase);

            var businessOportunityPhaseCurrent =  businessOportunity.
                                                  BusinessOportunityPhase.
                                                  FirstOrDefault(fod => fod.id == idCurrent);
            var advanceMax = businessOportunity.BusinessOportunityPhase.Count() > 0 ? businessOportunity.BusinessOportunityPhase.Max(m => m.advance) : 0;

            var advanceCurrent = businessOportunityPhaseCurrent?.advance ?? advanceMax;

            var businessOportunityPhaseAux = businessOportunity.
                                             BusinessOportunityPhase.
                                             AsEnumerable().
                                             FirstOrDefault(fod => fod.id != idCurrent && advanceCurrent >= fod.advance && _startDatePhase.CompareTo(fod.endDatePhase) < 0);

            if (businessOportunityPhaseAux != null)
            {
                result = new
                {
                    itsValided = 0,
                    Error = "Debe ser mayor o igual a todas las Fechas Fin anteriores a esta fase actual."
                };

            }


            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PartnerDetails(int? id_partner, int? id_documentType)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityPartner = businessOportunity.BusinessOportunityPartner ?? new List<BusinessOportunityPartner>();

            var documentTypeAux = db.DocumentType.FirstOrDefault(fod => fod.id == id_documentType);
            var codeDocumentTypeAux = documentTypeAux?.code ?? "";

            //15: Oportunidad de Venta y 16: Oportunidad de Compra

            var persons = db.Person.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && ((codeDocumentTypeAux == "15" && w.Customer != null) || (codeDocumentTypeAux == "16" && w.Provider != null))) || w.id == id_partner).ToList();
            var tempPersons = new List<Person>();
            foreach (var p in persons)
            {
                if (!(businessOportunity.BusinessOportunityPartner.Any(a => a.id_partner == p.id)) || p.id == id_partner)
                {
                    tempPersons.Add(p);
                }

            }
            persons = tempPersons;
            var result = new
            {
                partners = persons.Select(s => new { id = s.id, fullname_businessName = s.fullname_businessName}).ToList(),
                Message = "Ok"
            };

            TempData.Keep("businessOportunity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CompetitorDetails(int? id_competitor)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            businessOportunity.BusinessOportunityCompetition = businessOportunity.BusinessOportunityCompetition ?? new List<BusinessOportunityCompetition>();

            //15: Oportunidad de Venta y 16: Oportunidad de Compra

            var persons = db.Person.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.Rol.Any(a=> a.name == "Competidor")) || w.id == id_competitor).ToList();
            var tempPersons = new List<Person>();
            foreach (var p in persons)
            {
                if (!(businessOportunity.BusinessOportunityCompetition.Any(a => a.id_competitor == p.id)) || p.id == id_competitor)
                {
                    tempPersons.Add(p);
                }

            }
            persons = tempPersons;
            var result = new
            {
                competitors = persons.Select(s => new { id = s.id, fullname_businessName = s.fullname_businessName }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("businessOportunity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBusinessOportunityClosingPercentage()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();

            var advanceMax = businessOportunity.BusinessOportunityPhase.Count() > 0 ? businessOportunity.BusinessOportunityPhase.Max(m => m.advance) : 0;

            //var endDateCurrent = businessOportunity.BusinessOportunityPhase.OrderByDescending(obd => obd.id).First()?.endDatePhase;
            //foreach (var bbb in businessOportunity.BusinessOportunityPlaninng.BusinessOportunityPlanningDetail)
            //{
            //    amountAux += (bbb.quantity * bbb.price);
            //    UpdateBusinessOportunityPartner(bbb.id_person);
            //}


            TempData.Keep("businessOportunity");

            return Json(new
            {
                closingPercentage = (advanceMax/100),
                //endDateCurrent = endDateCurrent
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBusinessPartner()
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");

            return PartialView("Component/_ComboBoxBusinessPartner", businessOportunity);
        }

        [HttpPost]
        public ActionResult GetPlanningDetailPerson(int? id_person)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");
            //ViewData["id_person"] = id_person;

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_person";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "BusinessOportunity", Action = "GetPlanningDetailPerson"};
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "BusinessPlanningDetailPerson_BeginCallback";

                p.ValueField = "id";
                p.TextField = "fullname_businessName";
                p.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(DataProviderPerson.PersonByCompanyDocumentTypeOportunityAndCurrent(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_person));//.Bind(id_person);
                
            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost]
        public ActionResult GetPlanningDetailItem(int? id_item)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");
            //ViewData["id_item"] = id_item;
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_item";
                p.ClientInstanceName = "id_item";
                //p.DataSource = null;// DataProviderItem.AllItemsByCompany((int?)ViewData["id_company"]);
                p.ValueField = "id";
                //p.TextField = "name";
                p.TextFormatString = "{1}";
                p.ValueType = typeof(int);
                //settings.Properties.CallbackPageSize = 5;
                p.Width = Unit.Percentage(100);
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.EnableSynchronization = DefaultBoolean.False;
                p.Columns.Add("masterCode", "Código", 70);//, Unit.Percentage(50));
                p.Columns.Add("name", "Nombre del Producto", 200);//, Unit.Percentage(70));
                                                                                    //p.Columns.Add("barCode", "EAN", Unit.Percentage(50));
                p.Columns.Add("MetricUnitCode", "UM", 50);//, Unit.Percentage(20));
                                                                            //settings.Properties.ClientSideEvents.Init = "ItemCombo_OnInit";
                                                                            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
                p.ClientSideEvents.SelectedIndexChanged = "DetailsItemCombo_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnItemValidation";


                //settings.Properties.ClientInstanceName = "id_person";
                //settings.Width = Unit.Percentage(100);

                //settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
                //settings.Properties.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;

                //settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "BusinessOportunity", Action = "GetPlanningDetailItem"/*, TextField = "CityName"*/ };
                //settings.Properties.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "BusinessPlanningDetailItem_BeginCallback";
                p.ClientSideEvents.EndCallback = "BusinessPlanningDetailItem_EndCallback";

                //settings.Properties.ValueField = "id";
                //settings.Properties.TextField = "fullname_businessName";
                //settings.Properties.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                p.BindList(DataProviderItem.ItemByCompanyDocumentTypeOportunityAndCurrent(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_item));//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailItem");
        }

        [HttpPost]
        public ActionResult GetPartner(int? id_partner)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");
            //ViewData["id_person"] = id_person;

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_partner";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "BusinessOportunity", Action = "GetPartner" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "BusinessOportunityPartner_BeginCallback";
                p.ClientSideEvents.EndCallback = "BusinessOportunityPartner_EndCallback";

                p.ValueField = "id";
                p.TextField = "fullname_businessName";
                p.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(DataProviderPerson.PersonByCompanyDocumentTypeOportunityAndCurrentDistinctInBusinessOportunityPartner(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_partner, businessOportunity.BusinessOportunityPartner.ToList()));//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost]
        public ActionResult GetCompetitor(int? id_competitor)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_competitor";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "BusinessOportunity", Action = "GetCompetitor" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "BusinessOportunityCompetitor_BeginCallback";
                p.ClientSideEvents.EndCallback = "BusinessOportunityCompetitor_EndCallback";

                p.ValueField = "id";
                p.TextField = "fullname_businessName";
                p.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedAttachmentDetail(string attachmentNameNew)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);

            businessOportunity = businessOportunity ?? new BusinessOportunity();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var businessOportunityDocumentDetailAux = businessOportunity.
                                                      BusinessOportunityDocument.
                                                      FirstOrDefault(fod => fod.attachment == attachmentNameNew);
            if (businessOportunityDocumentDetailAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Documento Adjunto: " + attachmentNameNew + ", en el detalle de los Documentos Adjunto."

                };

            }


            //TempData["businessOportunity"] = businessOportunity;
            TempData.Keep("businessOportunity");
            SetViewData();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedAttachmentPhaseDetail(string attachmentPhaseNameNew)
        {
            //BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            //List<BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = businessOportunity.BusinessOportunityPhase.FirstOrDefault(fod => fod.id == id_businessOportunityPhase)?.BusinessOportunityPhaseAttachment.ToList();
            List <BusinessOportunityPhaseAttachment> businessOportunityPhaseAttachment = (TempData["businessOportunityPhaseAttachment"] as List<BusinessOportunityPhaseAttachment>);
            businessOportunityPhaseAttachment = businessOportunityPhaseAttachment ?? new List<BusinessOportunityPhaseAttachment>();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var businessOportunityPhaseAttachmentDetailAux = businessOportunityPhaseAttachment.
                                                      FirstOrDefault(fod => fod.attachmentPhase == attachmentPhaseNameNew);
            if (businessOportunityPhaseAttachmentDetailAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Documento Adjunto: " + attachmentPhaseNameNew + ", en el detalle de los Documentos Adjunto de la Fase."

                };

            }


            TempData.Keep("businessOportunity");
            TempData.Keep("businessOportunityPhaseAttachment");
            TempData.Keep("businessOportunityPhaseActivity");
            SetViewData();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetPriceList(int? id_person, int? id_priceList)
        {
            BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("businessOportunity");
            //ViewData["id_person"] = id_person;

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_priceList";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "BusinessOportunity", Action = "GetPriceList" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "BusinessPlanningDetailPriceListBeginCallback";
                p.ClientSideEvents.EndCallback = "BusinessPlanningDetailPriceList_EndCallback";

                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnPriceListValidation";
                if(businessOportunity.Document?.DocumentType?.code == "15")
                {
                    p.BindList(DataProviderPriceList.SaleApprovedPriceLists((int?)ViewData["id_company"], id_priceList, id_person));//.Bind(id_person);
                }else
                {
                    if (businessOportunity.Document?.DocumentType?.code == "16")
                    {
                        p.BindList(DataProviderPriceList.PurchaseApprovedPriceLists((int?)ViewData["id_company"], id_priceList, id_person));//.Bind(id_person);
                    }
                }
                //p.TextField = textField;


            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        #endregion
    }
}