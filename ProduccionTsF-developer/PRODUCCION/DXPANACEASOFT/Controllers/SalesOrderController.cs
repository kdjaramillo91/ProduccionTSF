using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.FE.Xmls.Common;

using XmlFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Factura;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Extensions.Querying;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using DevExpress.CodeParser.Diagnostics;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class SalesOrderController : DefaultController
    {
        private const string m_TipoDocumentoProductionOrderStock = "139";
        private const string m_TipoDocumentoProductionOrderForeignCustomers = "140";
        private const string m_TipoDocumentoProductionOrderLocalClient = "141";

        private SalesOrderDTO GetSalesOrderDTO()
        {
            if (!(Session["SalesOrderDTO"] is SalesOrderDTO salesOrder))
                salesOrder = new SalesOrderDTO();
            return salesOrder;
        }

        private List<SalesOrderResultConsultDTO> GetSalesOrderResultConsultDTO()
        {
            if (!(Session["SalesOrderResultConsultDTO"] is List<SalesOrderResultConsultDTO> salesOrderResultConsult))
                salesOrderResultConsult = new List<SalesOrderResultConsultDTO>();
            return salesOrderResultConsult;
        }

        private void SetSalesOrderDTO(SalesOrderDTO salesOrderDTO)
        {
            Session["SalesOrderDTO"] = salesOrderDTO;
        }

        private void SetSalesOrderResultConsultDTO(List<SalesOrderResultConsultDTO> salesOrderResultConsult)
        {
            Session["SalesOrderResultConsultDTO"] = salesOrderResultConsult;
        }

        // GET: SalesOrder
        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(SalesOrderConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetSalesOrderResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewSalesOrder()
        {
            return PartialView("_GridViewIndex", GetSalesOrderResultConsultDTO());
        }

        private List<SalesOrderResultConsultDTO> GetListsConsultDto(SalesOrderConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as SalesOrderConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }


                var consultResult = db.SalesOrder.Where(SalesOrderQueryExtensions.GetRequestByFilter(consulta)).ToList();
                if (string.IsNullOrEmpty(consulta.number)) {
                    if (consulta.items != null && consulta.items.Length > 0)
                    {
                        var tempModel = new List<SalesOrder>();
                        foreach (var order in consultResult)
                        {
                            var details = order.SalesOrderDetail.Where(d => consulta.items.Contains(d.id_item));
                            if (details.Any())
                            {
                                tempModel.Add(order);
                            }
                        }

                        consultResult = tempModel;
                    }
                }
                
                var query = consultResult.Select(t => new SalesOrderResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    documentType = t.Document.DocumentType.name,
                    customer = db.Person.FirstOrDefault(fod => fod.id == t.id_customer).fullname_businessName,
                    emissionDate = t.Document.emissionDate,
                    logistics = t.requiredLogistic,
                    state = t.Document.DocumentState.name,
                    numberProforma = "",
                    //numberProforma = t.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                    //            ? (t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Invoice.Document.number)
                    //            : (t.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                    //               ? (t.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.Document.number)
                    //               : ("")),
                    emissionDateProforma = (DateTime?)null,
                    //emissionDateProforma = t.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                    //            ? (t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Invoice.Document.emissionDate)
                    //            : (t.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                    //               ? (t.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.Document.emissionDate)
                    //               : (DateTime?)null),
                    sellerProforma = "",
                    //sellerProforma = t.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                    //               ? ((t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.idVendor != null) ? (db.Person.FirstOrDefault(fod => fod.id == (t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.idVendor)).fullname_businessName) : "")
                    //            //? (t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Person2.fullname_businessName)
                    //            : (t.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                    //               ? ((t.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.id_employeeSeller != null) ? (db.Person.FirstOrDefault(fod => fod.id == (t.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.id_employeeSeller)).fullname_businessName) : "")
                    //               : ("")),

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canClosed = t.Document.DocumentState.code.Equals("03"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03") || t.Document.DocumentState.code.Equals("04")

                }).OrderBy(ob => ob.number).ToList();

                foreach (var item in query)
                {
                    var aSalesOrder = db.SalesOrder.First(fod => fod.id == item.id);
                    item.numberProforma = aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                                ? (aSalesOrder.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Invoice.Document.number)
                                : (aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                                   ? (aSalesOrder.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.Document.number)
                                   : (""));
                    item.emissionDateProforma = aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                                ? (aSalesOrder.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Invoice.Document.emissionDate)
                                : (aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                                   ? (aSalesOrder.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.Document.emissionDate)
                                   : (DateTime?)null);

                    var aSalesOrderDetail = aSalesOrder.SalesOrderDetail.First();
                    var aSalesOrderDetailSalesQuotationExterior = aSalesOrderDetail.SalesOrderDetailSalesQuotationExterior.Count() > 0 ? aSalesOrderDetail.SalesOrderDetailSalesQuotationExterior.First() : null;
                    var aSalesOrderDetailSalesRequest = aSalesOrderDetail.SalesOrderDetailSalesRequest.Count() > 0 ? aSalesOrderDetail.SalesOrderDetailSalesRequest.First() : null;

                    item.sellerProforma = aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                                   ? ((aSalesOrderDetailSalesQuotationExterior?.SalesQuotationExterior.idVendor != null) ? (db.Person.FirstOrDefault(fod => fod.id == (aSalesOrderDetailSalesQuotationExterior.SalesQuotationExterior.idVendor)).fullname_businessName) : "")
                                //? (t.SalesOrderDetail.First().SalesOrderDetailSalesQuotationExterior.First().SalesQuotationExterior.Person2.fullname_businessName)
                                : (aSalesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                                   ? ((aSalesOrderDetailSalesRequest?.SalesRequest.id_employeeSeller != null) ? (db.Person.FirstOrDefault(fod => fod.id == (aSalesOrderDetailSalesRequest.SalesRequest.id_employeeSeller)).fullname_businessName) : "")
                                   : (""));
                }

                Session["consulta"] = consulta;

                return query;
            }
        }

        private static List<SalesOrderPendingNewDTO> GetSalesOrderPendingNewDto(string codeDocumentType)
        {
            using (var db = new DBContext())
            {
                // Ejecución de la consulta de pendientes

                var q = new List<SalesOrderPendingNewDTO>();
                if (codeDocumentType.Equals(m_TipoDocumentoProductionOrderForeignCustomers))
                {
                    q = db.SalesQuotationExterior.AsEnumerable()
                    .Where(r => (r.Invoice.Document.DocumentState.code == ("03") &&//03: APROBADO
                                 (r.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.SalesOrderDetailSalesQuotationExterior == null ||
                                                                                fod.SalesOrderDetailSalesQuotationExterior != null &&
                                                                                fod.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod2 => fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null) != null ||
                                  r.SalesOrderDetailSalesQuotationExterior == null ||
                                 (r.SalesOrderDetailSalesQuotationExterior != null &&
                                  r.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null))))//05: ANULADA
                    .Select(r => new SalesOrderPendingNewDTO
                    {
                        numberRequestProforma = r.Invoice.Document.number,
                        id_salesRequestDetail = null,
                        id_salesQuotationExterior = r.id,
                        customer = db.Person.FirstOrDefault(fod => fod.id == r.id_consignee).fullname_businessName,
                        emissionDateStr = r.Invoice.Document.emissionDate.ToString("dd-MM-yyyy"),
                        emissionDate = r.Invoice.Document.emissionDate,
                        name_item = "",
                        cartons = 0.00M
                    }).ToList();
                }
                else
                {
                    if (codeDocumentType.Equals(m_TipoDocumentoProductionOrderLocalClient))
                    {
                        q = db.SalesRequestDetail.AsEnumerable()
                    .Where(r => (r.SalesRequest.Document.DocumentState.code == ("03") &&//03: APROBADO
                                 (r.SalesOrderDetailSalesRequest == null ||
                                 (r.SalesOrderDetailSalesRequest != null &&
                                  r.SalesOrderDetailSalesRequest.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null))))//05: ANULADA
                    .Select(r => new SalesOrderPendingNewDTO
                    {
                        numberRequestProforma = r.SalesRequest.Document.number,
                        id_salesRequestDetail = r.id,
                        id_salesQuotationExterior = null,
                        customer = db.Person.FirstOrDefault(fod => fod.id == r.SalesRequest.id_customer).fullname_businessName,
                        name_item = r.Item.name,
                        cartons = r.quantityApproved
                    }).ToList();
                    }
                }

                return q;

            }
        }

        [HttpPost]
        public ActionResult PendingNewOrderForeignCustomers()
        {
            //var a = GetSalesOrderPendingNewDto(m_TipoDocumentoProductionOrderForeignCustomers);
            var model = db.SalesQuotationExterior.Where(s =>
                s.Invoice.Document.DocumentTransactionState.code != "02" &&
                (s.Invoice.Document.DocumentState.code == "03"))
                .OrderByDescending(a => a.Invoice.Document.emissionDate).ToList();

            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNewOrderForeignCustomers()
        {
            var model = db.SalesQuotationExterior.Where(s =>
                            s.Invoice.Document.DocumentTransactionState.code != "02" &&
                            (s.Invoice.Document.DocumentState.code == "03"))
                .OrderByDescending(a => a.Invoice.Document.emissionDate).ToList();
            return PartialView("_GridViewPendingNewOrderForeignCustomers", model);
        }

        [HttpPost]
        public ActionResult PendingNewOrderLocalClient()
        {
            return View(GetSalesOrderPendingNewDto(m_TipoDocumentoProductionOrderLocalClient));
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNewOrderLocalClient()
        {
            return PartialView("_GridViewPendingNewOrderLocalClient", GetSalesOrderPendingNewDto(m_TipoDocumentoProductionOrderLocalClient));
        }

        private SalesOrderDTO Create(int[] ids)
        {
            using (var db = new DBContext())
            {
                var ids0 = ids[0];
                var salesRequestDetail =
                    db.SalesRequestDetail.FirstOrDefault(r => r.id == ids0);

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoProductionOrderLocalClient));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var salesOrderDTO = new SalesOrderDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    code_documentType = documentType?.code ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    reference = "",
                    id_employeeApplicant = ActiveUser.id_employee,
                    employeeApplicant = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    description = "",
                    customer = db.Person.FirstOrDefault(fod => fod.id == salesRequestDetail.SalesRequest.id_customer)?.fullname_businessName ?? "",
                    id_customer = salesRequestDetail.SalesRequest.id_customer,
                    proformaOrder = salesRequestDetail.SalesRequest.Document.number,
                    seller = db.Person.FirstOrDefault(fod => fod.id == salesRequestDetail.SalesRequest.id_employeeSeller)?.fullname_businessName ?? "",
                    id_seller = salesRequestDetail.SalesRequest.id_employeeSeller,
                    paymentMethod = db.PaymentMethod.FirstOrDefault(fod => fod.id == salesRequestDetail.SalesRequest.id_PaymentMethod)?.name ?? "",
                    id_paymentMethod = salesRequestDetail.SalesRequest.id_PaymentMethod,
                    dateShipment = hoy,
                    portDestination = db.Port.FirstOrDefault(fod => fod.id == salesRequestDetail.SalesRequest.id_portDestination)?.City.name ?? "",
                    id_portDestination = salesRequestDetail.SalesRequest.id_portDestination,
                    portDischarge = "",
                    id_portDischarge = null,
                    logistics = false,
                    totalCM = "",
                    netoLbs = "",
                    netoKg = "",
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    numeroLote = "",
                    orderReason = "",
                    id_orderReason = null,
                    Provider = "",
                    id_provider = null,
                    additionalInstructions = db.SalesOrderInstructions.FirstOrDefault().descriptionDocument,
                    shippingDocument = db.SalesOrderInstructions.FirstOrDefault().shippingDocument,

                    SalesOrderDetails = new List<SalesOrderDetailDTO>(),
                    SalesOrderMPMaterialDetails = new List<SalesOrderMPMaterialDetailDTO>(),
                    SalesOrderMPMaterialDetailsSummary = new List<SalesOrderMPMaterialDetailSummaryDTO>()
                };

                FillSalesOrderDetails(salesOrderDTO, ids);

                return salesOrderDTO;
            }
        }

        private void FillSalesOrderDetails(SalesOrderDTO salesOrderDTO, int[] ids)
        {
            decimal totalCMAux = 0.00M;
            decimal neto1Aux = 0.00M;
            decimal neto2Aux = 0.00M;
            foreach (var id in ids)
            {
                var salesRequestDetail = db.SalesRequestDetail.FirstOrDefault(r => r.id == id);
                var originQuantityAux = Math.Round(salesRequestDetail.quantityApproved * (salesRequestDetail.Item.Presentation?.maximum ?? 1) * (salesRequestDetail.Item.Presentation?.minimum ?? 1), 2);
                var cartonsAux = Math.Round(salesRequestDetail.quantityApproved, 2);
                totalCMAux += cartonsAux;
                //neto2Aux += originQuantityAux;
                var id_metricUnitPresentation = salesRequestDetail.Item.Presentation?.id_metricUnit;
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                var id_metricUnitLbs = metricUnitLbs?.id;
                var factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitLbs)?.factor ?? 1;
                neto1Aux += Math.Round(originQuantityAux * factorAux, 2);
                var metricUnitKg = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Kg"));
                var id_metricUnitKg = metricUnitKg?.id;
                factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitKg)?.factor ?? 1;
                neto2Aux += Math.Round(originQuantityAux * factorAux, 2);
                salesOrderDTO.SalesOrderDetails.Add(new SalesOrderDetailDTO
                {
                    id = salesOrderDTO.SalesOrderDetails.Count() > 0 ? salesOrderDTO.SalesOrderDetails.Max(pld => pld.id) + 1 : 1,
                    noProgProduction = "",
                    noRequestProforma = salesRequestDetail.SalesRequest.Document.number,
                    id_salesRequestDetail = id,
                    id_invoiceDetail = null,
                    id_item = salesRequestDetail.id_item,
                    name_item = salesRequestDetail.Item.name,
                    description_item = salesRequestDetail.Item.description,
                    cod_item = salesRequestDetail.Item.masterCode,
                    codAux_item = salesRequestDetail.Item.auxCode,
                    cartons = cartonsAux,
                    originQuantity = originQuantityAux,
                    originQuantityStr = originQuantityAux.ToString() + " " + (salesRequestDetail.Item.Presentation?.MetricUnit.code ?? ""),
                });
            }
            var ids0 = ids[0];
            var salesRequestDetail0 = db.SalesRequestDetail.FirstOrDefault(r => r.id == ids0);
            salesOrderDTO.totalCM = totalCMAux.ToString("#");
            salesOrderDTO.netoLbs = neto1Aux.ToString() + " Lbs";
            salesOrderDTO.netoKg = neto2Aux.ToString() + " Kg";
        }

        private SalesOrderDTO Create(int id_proforma)
        {
            using (var db = new DBContext())
            {
                var salesQuotationExterior =
                    db.SalesQuotationExterior.FirstOrDefault(r => r.id == id_proforma);

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoProductionOrderForeignCustomers));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var salesOrderDTO = new SalesOrderDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    code_documentType = documentType?.code ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    reference = "",
                    id_employeeApplicant = ActiveUser.id_employee,
                    employeeApplicant = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    description = "",
                    customer = db.Person.FirstOrDefault(fod => fod.id == salesQuotationExterior.id_consignee)?.fullname_businessName ?? "",
                    id_customer = salesQuotationExterior.id_consignee,
                    proformaOrder = salesQuotationExterior.Invoice.Document.number,
                    seller = db.Person.FirstOrDefault(fod => fod.id == salesQuotationExterior.idVendor)?.fullname_businessName ?? "",
                    id_seller = salesQuotationExterior.idVendor,
                    paymentMethod = db.PaymentMethod.FirstOrDefault(fod => fod.id == salesQuotationExterior.id_PaymentMethod)?.name ?? "",
                    id_paymentMethod = salesQuotationExterior.id_PaymentMethod,
                    dateShipment = salesQuotationExterior.dateShipment.Value,
                    portDestination = db.Port.FirstOrDefault(fod => fod.id == salesQuotationExterior.id_portDestination)?.nombre ?? "",
                    id_portDestination = salesQuotationExterior.id_portDestination,
                    portDischarge = db.Port.FirstOrDefault(fod => fod.id == salesQuotationExterior.id_portDischarge)?.nombre ?? "",
                    id_portDischarge = salesQuotationExterior.id_portDischarge,
                    logistics = false,
                    totalCM = "",
                    netoLbs = "",
                    netoKg = "",
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    numeroLote = "",
                    orderReason = "",
                    id_orderReason = null,
                    Provider = "",
                    id_provider = null,
                    additionalInstructions = db.SalesOrderInstructions.FirstOrDefault().descriptionDocument,
                    shippingDocument = db.SalesOrderInstructions.FirstOrDefault().shippingDocument,

                    Product = salesQuotationExterior.Product,
                    ColourGrade = salesQuotationExterior.ColourGrade,
                    PackingDetails = salesQuotationExterior.PackingDetails,
                    ContainerDetails = salesQuotationExterior.ContainerDetails,

                    SalesOrderDetails = new List<SalesOrderDetailDTO>(),
                    SalesOrderMPMaterialDetails = new List<SalesOrderMPMaterialDetailDTO>(),
                    SalesOrderMPMaterialDetailsSummary = new List<SalesOrderMPMaterialDetailSummaryDTO>()
                };

                FillSalesOrderDetails(salesOrderDTO, salesQuotationExterior);

                return salesOrderDTO;
            }
        }

        private void FillSalesOrderDetails(SalesOrderDTO salesOrderDTO, SalesQuotationExterior salesQuotationExterior)
        {
            decimal totalCMAux = 0.00M;
            decimal neto1Aux = 0.00M;
            decimal neto2Aux = 0.00M;

            var model = salesQuotationExterior.Invoice.InvoiceDetail.Where(w => w.isActive && salesOrderDTO.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == w.id_item) == null &&
                                                                                       (w.SalesOrderDetailSalesQuotationExterior == null ||
                                                                                       (w.SalesOrderDetailSalesQuotationExterior != null &&
                                                                                       w.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null))).ToList();

            //foreach (var itemDetail in salesQuotationExterior.Invoice.InvoiceDetail.Where(d => d.isActive))
            foreach (var itemDetail in model)
                {
                var originQuantityAux = Math.Round(itemDetail.numBoxes.Value * (itemDetail.Item.Presentation?.maximum ?? 1) * (itemDetail.Item.Presentation?.minimum ?? 1), 2);
                totalCMAux += itemDetail.numBoxes.Value;
                //neto2Aux += originQuantityAux;
                var id_metricUnitPresentation = itemDetail.Item.Presentation?.id_metricUnit;
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                var id_metricUnitLbs = metricUnitLbs?.id;
                var factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitLbs)?.factor ?? 1;
                neto1Aux += Math.Round(originQuantityAux * factorAux, 2);

                var metricUnitKg = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Kg"));
                var id_metricUnitKg = metricUnitKg?.id;
                factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitKg)?.factor ?? 1;
                neto2Aux += Math.Round(originQuantityAux * factorAux, 2);
                var aSalesOrderDetailDTO = new SalesOrderDetailDTO
                {
                    id = salesOrderDTO.SalesOrderDetails.Count() > 0 ? salesOrderDTO.SalesOrderDetails.Max(pld => pld.id) + 1 : 1,
                    noProgProduction = "",
                    noRequestProforma = itemDetail.Invoice.Document.number,
                    id_salesRequestDetail = null,
                    id_invoiceDetail = itemDetail.id,
                    id_item = itemDetail.id_item,
                    name_item = itemDetail.Item.name,
                    description_item = itemDetail.Item.description,
                    cod_item = itemDetail.Item.masterCode,
                    codAux_item = itemDetail.Item.auxCode,
                    cartons = itemDetail.numBoxes.Value,
                    quantityProgrammed = itemDetail.numBoxes.Value,
                    quantityApproved = itemDetail.numBoxes.Value,
                    quantityProduced = 0,
                    originQuantity = originQuantityAux,
                    originQuantityStr = originQuantityAux.ToString() + " " + (itemDetail.Item.Presentation?.MetricUnit.code ?? ""),

                    SalesOrderDetailMPMaterialDetails = new List<SalesOrderDetailMPMaterialDetailDTO>()
                    //salesOrder.SalesOrderDetails.Add(item);
                    //UpdateSalesOrderMPMaterialDetails(salesOrder, item, false);

                    //UpdateTotals();
                    //UpdateSummary();
                    //SalesOrderDetailMPMaterialDetails = aSalesOrderDetailMPMaterialDetailDTO
                };
                salesOrderDTO.SalesOrderDetails.Add(aSalesOrderDetailDTO);
                UpdateSalesOrderMPMaterialDetails(salesOrderDTO, aSalesOrderDetailDTO, false);
            }
            var invoiceDetail = salesQuotationExterior.Invoice.InvoiceDetail.FirstOrDefault();
            salesOrderDTO.totalCM = Math.Round(totalCMAux, 2).ToString("#");
            salesOrderDTO.netoLbs = neto1Aux.ToString() + " Lbs";
            salesOrderDTO.netoKg = neto2Aux.ToString() + " Kg";

            //UpdateSummary();
        }

        private SalesOrderDTO Create()
        {
            using (var db = new DBContext())
            {
                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoProductionOrderStock));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var salesOrderDTO = new SalesOrderDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    code_documentType = documentType?.code ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    reference = "",
                    id_employeeApplicant = ActiveUser.id_employee,
                    employeeApplicant = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    description = "",
                    customer = "",
                    id_customer = null,
                    proformaOrder = "",
                    seller = "",
                    id_seller = null,
                    paymentMethod = "",
                    id_paymentMethod = null,
                    dateShipment = hoy,
                    portDestination = "",
                    id_portDestination = null,
                    portDischarge = "",
                    id_portDischarge = null,
                    logistics = false,
                    totalCM = "",
                    netoLbs = "",
                    netoKg = "",
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    numeroLote = "",
                    orderReason = "",
                    id_orderReason = null,
                    Provider = "",
                    id_provider = null,
                    additionalInstructions = db.SalesOrderInstructions.FirstOrDefault().descriptionDocument,
                    shippingDocument = db.SalesOrderInstructions.FirstOrDefault().shippingDocument,

                    SalesOrderDetails = new List<SalesOrderDetailDTO>(),
                    SalesOrderMPMaterialDetails = new List<SalesOrderMPMaterialDetailDTO>(),
                    SalesOrderMPMaterialDetailsSummary = new List<SalesOrderMPMaterialDetailSummaryDTO>()
                };

                return salesOrderDTO;
            }
        }

        private SalesOrderDTO ConvertToDto(SalesOrder salesOrder)
        {
            var hoy = DateTime.Now;
            Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
            int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
            var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

            var aSalesOrderDetail1 = salesOrder.SalesOrderDetail.First();
            var aSalesOrderDetailSalesQuotationExterior = aSalesOrderDetail1.SalesOrderDetailSalesQuotationExterior != null &&
                                                          aSalesOrderDetail1.SalesOrderDetailSalesQuotationExterior.Count() > 0 ? aSalesOrderDetail1.SalesOrderDetailSalesQuotationExterior.First() : null;
            var aSalesQuotationExterior = aSalesOrderDetailSalesQuotationExterior?.SalesQuotationExterior;

            var salesOrderDto = new SalesOrderDTO
            {
                id = salesOrder.id,
                id_documentType = salesOrder.Document.id_documentType,
                documentType = salesOrder.Document.DocumentType.name,
                code_documentType = salesOrder.Document.DocumentType.code,
                number = salesOrder.Document.number,
                idSate = salesOrder.Document.id_documentState,
                state = salesOrder.Document.DocumentState.name,
                dateTimeEmisionStr = salesOrder.Document.emissionDate.ToString("dd-MM-yyyy"),
                dateTimeEmision = salesOrder.Document.emissionDate,
                reference = salesOrder.Document.reference,
                employeeApplicant = db.Employee.FirstOrDefault(fod => fod.id == salesOrder.id_employeeApplicant)?.Person.fullname_businessName,
                id_employeeApplicant = salesOrder.id_employeeApplicant,
                description = salesOrder.Document.description,
                customer = db.Customer.FirstOrDefault(fod => fod.id == salesOrder.id_customer)?.Person.fullname_businessName,
                id_customer = salesOrder.id_customer,
                proformaOrder = salesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderForeignCustomers
                                ? (aSalesQuotationExterior.Invoice.Document.number)
                                : (salesOrder.Document.DocumentType.code == m_TipoDocumentoProductionOrderLocalClient
                                   ? (salesOrder.SalesOrderDetail.First().SalesOrderDetailSalesRequest.First().SalesRequest.Document.number)
                                   : ("")),
                seller = db.Person.FirstOrDefault(fod => fod.id == salesOrder.id_employeeSeller)?.fullname_businessName,
                id_seller = salesOrder.id_employeeSeller,
                paymentMethod = db.PaymentMethod.FirstOrDefault(fod => fod.id == salesOrder.id_PaymentMethod)?.name,
                id_paymentMethod = salesOrder.id_PaymentMethod,
                dateShipment = salesOrder.dateShipment.Value,
                portDestination = db.Port.FirstOrDefault(fod => fod.id == salesOrder.id_portDestination)?.nombre,
                id_portDestination = salesOrder.id_portDestination,
                portDischarge = db.Port.FirstOrDefault(fod => fod.id == salesOrder.id_portDischarge)?.nombre,
                id_portDischarge = salesOrder.id_portDischarge,
                logistics = salesOrder.requiredLogistic,
                totalCM = "",
                netoLbs = "",
                netoKg = "",
                dateHoy = hoy.ToString("dd-MM-yyyy"),
                dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                numeroLote = salesOrder.numeroLote,
                orderReason = db.OrderReason.FirstOrDefault(fod => fod.id == salesOrder.id_orderReason)?.name,
                id_orderReason = salesOrder.id_orderReason,
                Provider = db.Person.FirstOrDefault(fod => fod.id == salesOrder.id_provider)?.fullname_businessName,
                id_provider = salesOrder.id_provider,
                additionalInstructions = salesOrder.additionalInstructions,
                shippingDocument = salesOrder.shippingDocument,

                Product = aSalesQuotationExterior?.Product ?? "",
                ColourGrade = aSalesQuotationExterior?.ColourGrade ?? "",
                PackingDetails = aSalesQuotationExterior?.PackingDetails ?? "",
                ContainerDetails = aSalesQuotationExterior?.ContainerDetails ?? "",
                
                SalesOrderDetails = new List<SalesOrderDetailDTO>(),
                SalesOrderMPMaterialDetails = new List<SalesOrderMPMaterialDetailDTO>(),
                SalesOrderMPMaterialDetailsSummary = new List<SalesOrderMPMaterialDetailSummaryDTO>()
            };

            decimal totalCMAux = 0.00M;
            decimal neto1Aux = 0.00M;
            decimal neto2Aux = 0.00M;
            foreach (var itemDetail in salesOrder.SalesOrderDetail)
            {
                var originQuantityAux = Math.Round(itemDetail.quantityTypeUMSale * (itemDetail.Item.Presentation?.maximum ?? 1) * (itemDetail.Item.Presentation?.minimum ?? 1), 2);
                var cartonsAux = Math.Round(itemDetail.quantityTypeUMSale, 2);
                totalCMAux += cartonsAux;
                //neto2Aux += originQuantityAux;
                var id_metricUnitPresentation = itemDetail.Item.Presentation?.id_metricUnit;
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                var id_metricUnitLbs = metricUnitLbs?.id;
                var factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitLbs)?.factor ?? 1;
                neto1Aux += Math.Round(originQuantityAux * factorAux, 2);

                var metricUnitKg = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Kg"));
                var id_metricUnitKg = metricUnitKg?.id;
                factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitKg)?.factor ?? 1;
                neto2Aux += Math.Round(originQuantityAux * factorAux, 2);

                var aSalesOrderDetailMPMaterialDetailDTO = new List<SalesOrderDetailMPMaterialDetailDTO>();

                foreach (var itemDetailMPMaterialDetail in itemDetail.SalesOrderDetailMPMaterialDetail)
                {
                    aSalesOrderDetailMPMaterialDetailDTO.Add(new SalesOrderDetailMPMaterialDetailDTO
                    {
                        id = itemDetailMPMaterialDetail.id,
                        id_salesOrderDetail = itemDetailMPMaterialDetail.id_salesOrderDetail,
                        id_salesOrderMPMaterialDetail = itemDetailMPMaterialDetail.id_salesOrderMPMaterialDetail,
                        quantity = itemDetailMPMaterialDetail.quantity
                    });
                }

                salesOrderDto.SalesOrderDetails.Add(new SalesOrderDetailDTO
                {
                    id = itemDetail.id,
                    noProgProduction = "",
                    noRequestProforma = salesOrderDto.proformaOrder,
                    id_salesRequestDetail = itemDetail.SalesOrderDetailSalesRequest != null && itemDetail.SalesOrderDetailSalesRequest.Count > 0 ? itemDetail.SalesOrderDetailSalesRequest.First().id_salesRequestDetail : null,
                    id_invoiceDetail = itemDetail.SalesOrderDetailSalesQuotationExterior != null && itemDetail.SalesOrderDetailSalesQuotationExterior.Count > 0 ? itemDetail.SalesOrderDetailSalesQuotationExterior.First().id_invoiceDetail : null,
                    id_item = itemDetail.id_item,
                    name_item = itemDetail.Item.name,
                    description_item = itemDetail.Item.description,
                    cod_item = itemDetail.Item.masterCode,
                    codAux_item = itemDetail.Item.auxCode,
                    cartons = cartonsAux,
                    quantityProgrammed = itemDetail.quantityRequested,
                    quantityApproved = itemDetail.quantityApproved,
                    quantityProduced = itemDetail.quantityDelivered,
                    originQuantity = originQuantityAux,
                    originQuantityStr = originQuantityAux.ToString() + " " + (itemDetail.Item.Presentation?.MetricUnit.code ?? ""),
                    SalesOrderDetailMPMaterialDetails = aSalesOrderDetailMPMaterialDetailDTO

                });
            }

            var aSalesOrderDetail = salesOrder.SalesOrderDetail.FirstOrDefault();
            salesOrderDto.totalCM = totalCMAux.ToString("#");
            salesOrderDto.netoLbs = neto1Aux.ToString() + " Lbs";
            salesOrderDto.netoKg = neto2Aux.ToString() + " Kg";

            foreach (var itemMPMaterialDetail in salesOrder.SalesOrderMPMaterialDetail)
            {
                var aCodProduct = itemMPMaterialDetail.SalesOrderDetailMPMaterialDetail.ToList()[0].SalesOrderDetail.Item.masterCode;
                var aId_product = itemMPMaterialDetail.SalesOrderDetailMPMaterialDetail.ToList()[0].SalesOrderDetail.id_item;
                //foreach (var itemDetailMPMaterialDetail in itemMPMaterialDetail.SalesOrderDetailMPMaterialDetail)
                //{
                //    if (string.IsNullOrEmpty(aCodProducts))
                //        aCodProducts = itemDetailMPMaterialDetail.SalesOrderDetail.Item.masterCode;
                //    else
                //        aCodProducts += " | " + itemDetailMPMaterialDetail.SalesOrderDetail.Item.masterCode;

                //    if (string.IsNullOrEmpty(aNameProducts))
                //        aNameProducts = itemDetailMPMaterialDetail.SalesOrderDetail.Item.name;
                //    else
                //        aNameProducts += " | " + itemDetailMPMaterialDetail.SalesOrderDetail.Item.name;
                //}

                salesOrderDto.SalesOrderMPMaterialDetails.Add(new SalesOrderMPMaterialDetailDTO
                {
                    id = itemMPMaterialDetail.id,
                    codProduct = aCodProduct,
                    id_product = aId_product,
                    //codProducts = aCodProducts,
                    //nameProducts = aNameProducts,
                    id_inventoryLine = itemMPMaterialDetail.Item.id_inventoryLine,
                    id_itemType = itemMPMaterialDetail.Item.id_itemType,
                    id_itemTypeCategory = itemMPMaterialDetail.Item.id_itemTypeCategory,
                    cod_item = itemMPMaterialDetail.Item.masterCode,
                    name_item = itemMPMaterialDetail.Item.name,
                    id_item = itemMPMaterialDetail.id_item,
                    quantityRequiredForFormulation = itemMPMaterialDetail.quantityRequiredForFormulation,
                    quantity = itemMPMaterialDetail.quantity,
                    id_metricUnit = itemMPMaterialDetail.id_metricUnit,
                    manual = itemMPMaterialDetail.manual
                });

                var aSalesOrderMPMaterialDetailsSummary = salesOrderDto.SalesOrderMPMaterialDetailsSummary.FirstOrDefault(fod => fod.id_item == itemMPMaterialDetail.id_item && 
                                                                                                                                 fod.id_metricUnit == itemMPMaterialDetail.id_metricUnit);
                if (aSalesOrderMPMaterialDetailsSummary == null)
                {
                    salesOrderDto.SalesOrderMPMaterialDetailsSummary.Add(new SalesOrderMPMaterialDetailSummaryDTO
                    {
                        id = salesOrderDto.SalesOrderMPMaterialDetailsSummary.Count() > 0 ? salesOrderDto.SalesOrderMPMaterialDetailsSummary.Max(ppd => ppd.id) + 1 : 1,
                        id_inventoryLine = itemMPMaterialDetail.Item.id_inventoryLine,
                        cod_item = itemMPMaterialDetail.Item.masterCode,
                        name_item = itemMPMaterialDetail.Item.name,
                        id_item = itemMPMaterialDetail.id_item,
                        quantityRequiredForFormulation = itemMPMaterialDetail.quantityRequiredForFormulation,
                        quantity = itemMPMaterialDetail.quantity,
                        id_metricUnit = itemMPMaterialDetail.id_metricUnit,
                    });
                }
                else {
                    aSalesOrderMPMaterialDetailsSummary.quantityRequiredForFormulation += itemMPMaterialDetail.quantityRequiredForFormulation;
                    aSalesOrderMPMaterialDetailsSummary.quantity += itemMPMaterialDetail.quantity;
                }
            }

            return salesOrderDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxDocumentTypeIndex();
            BuildComboBoxCustomerIndex();
            BuildComboBoxSellerIndex();
            BuildTokenBoxItemsIndex();
            BuildComboBoxLogisticsIndex();
        }

        private void BuildViewDataEdit()
        {
            BuildComboBoxEmployeeApplicant();
            BuildComboBoxCustomer();
            BuildComboBoxProvider();
            BuildComboBoxReason();
            BuildComboBoxPortDestination();
            BuildComboBoxPortDischarge();
            var salesOrder = GetSalesOrderDTO();
            if (salesOrder.code_documentType == m_TipoDocumentoProductionOrderStock)
            {
                BuildComboBoxSizeBegin();
                BuildComboBoxSizeEnd();
                BuildComboBoxInventoryLine();
                BuildComboBoxItemType();
                BuildComboBoxItemTypeCategory();
                BuildComboBoxItemGroup();
                BuildComboBoxItemSubGroup();
                BuildComboBoxItemSize();
                BuildComboBoxItemTrademark();
                BuildComboBoxItemTrademarkModel();
                BuildComboBoxItemColor();
            }
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, int[] ids = null, int? id_proforma = null, bool enabled = true)
        {

            var model = new SalesOrderDTO();
            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(d => d.id == id);
            if (salesOrder == null)
            {
                if (ids != null)
                {
                    model = Create(ids);
                }
                else
                {
                    if (id_proforma != null)
                    {
                        model = Create(id_proforma.Value);
                    }
                    else
                    {
                        model = Create();  
                    }
                }
                SetSalesOrderDTO(model);
                UpdateSummary();
                BuilViewBag(enabled);
                //return PartialView(model);
            }
            else
            {
                model = ConvertToDto(salesOrder);
                SetSalesOrderDTO(model);
                BuilViewBag(enabled);
            }

            BuildViewDataEdit();
            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            var salesOrderDTO = GetSalesOrderDTO();
            ViewBag.enabled = enabled;
            ViewBag.canNew = salesOrderDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canClosed = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                     ?.code.Equals("03") ?? false) && salesOrderDTO.id != 0;
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                     ?.code.Equals("01") ?? false) && salesOrderDTO.id != 0;
            ViewBag.canReverse = ((db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                     ?.code.Equals("03") ?? false) ||
                                  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                     ?.code.Equals("04") ?? false)) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == salesOrderDTO.idSate)
                                      ?.code.Equals("01") ?? false) && salesOrderDTO.id != 0;

            ViewBag.dateTimeEmision = salesOrderDTO.dateTimeEmision;
            ViewBag.code_documentType = salesOrderDTO.code_documentType;
        }

        #region GridViewDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();

            ViewBag.enabled = enabled;
            ViewBag.code_documentType = salesOrder.code_documentType;
            //ViewBag.dateTimeEmision = salesOrder.dateTimeEmision;

            return PartialView("_GridViewDetails", salesOrder.SalesOrderDetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsAddNew(SalesOrderDetailDTO item, bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();
            //ModelState.IsValid = true;
            if (ModelState.IsValid)
            {
                item.id = salesOrder.SalesOrderDetails.Count() > 0 ? salesOrder.SalesOrderDetails.Max(ppd => ppd.id) + 1 : 1;
                item.noProgProduction = "";
                item.noRequestProforma = item.noRequestProforma;
                item.id_salesRequestDetail = null;
                if (!string.IsNullOrEmpty(salesOrder.proformaOrder))
                {
                    var aId_documentOrigen = db.Document.FirstOrDefault(fod => fod.Invoice != null && fod.Invoice.SalesQuotationExterior != null && fod.number == salesOrder.proformaOrder)?.id;
                    var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                    item.id_invoiceDetail = proforma.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == item.id_item)?.id;
                }
                else
                {
                    item.id_invoiceDetail = null;
                }
                var aItem = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                item.name_item = aItem.name;
                item.description_item = aItem.description;
                item.cod_item = aItem.masterCode;
                item.codAux_item = aItem.auxCode;
                item.cartons = item.quantityApproved;
                item.originQuantity = Math.Round(item.quantityApproved * aItem.Presentation.maximum * aItem.Presentation.minimum, 2);
                item.originQuantityStr = item.originQuantity.ToString() + " " + aItem.Presentation.MetricUnit.code;
                //item.quantityApproved = item.quantityApproved;
                //item.quantityProgrammed = item.quantityProgrammed;
                //item.quantityProduced= item.quantityProduced;
                //item.totalHours = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursHours"]) ? "0" : Request.Params["totalHoursHours"]),
                //                              int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursMinutes"]) ? "0" : Request.Params["totalHoursMinutes"]), 0);
                //item.observation = item.stop ? item.observation : item.motiveLot;
                item.SalesOrderDetailMPMaterialDetails = new List<SalesOrderDetailMPMaterialDetailDTO>();
                salesOrder.SalesOrderDetails.Add(item);
                UpdateSalesOrderMPMaterialDetails(salesOrder, item, false);

                UpdateTotals();
                UpdateSummary();
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
            ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewDetails", salesOrder.SalesOrderDetails);
        }
        
        private void UpdateTotals()
        {
            var salesOrder = GetSalesOrderDTO();
            decimal totalCMAux = 0.00M;
            decimal neto1Aux = 0.00M;
            decimal neto2Aux = 0.00M;
            foreach (var itemDetail in salesOrder.SalesOrderDetails)
            {
                var originQuantityAux = itemDetail.originQuantity;
                totalCMAux += itemDetail.cartons;
                //neto2Aux += originQuantityAux;
                var item = db.Item.FirstOrDefault(fod => fod.id == itemDetail.id_item);
                var id_metricUnitPresentation = item?.Presentation?.id_metricUnit;
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                var id_metricUnitLbs = metricUnitLbs?.id;
                var factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitLbs)?.factor ?? 1;
                neto1Aux += Math.Round(originQuantityAux * factorAux, 2);

                var metricUnitKg = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Kg"));
                var id_metricUnitKg = metricUnitKg?.id;
                factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitKg)?.factor ?? 1;
                neto2Aux += Math.Round(originQuantityAux * factorAux, 2);
            }
            //var aSalesOrderDetail = salesOrder.SalesOrderDetails.FirstOrDefault();
            salesOrder.totalCM = totalCMAux.ToString("#");
            //var aItem = db.Item.FirstOrDefault(fod => fod.id == aSalesOrderDetail.id_item);
            salesOrder.netoLbs = neto1Aux.ToString() + " Lbs";
            salesOrder.netoKg = neto2Aux.ToString() + " Kg";
        }

        private void UpdateSalesOrderMPMaterialDetails(SalesOrderDTO salesOrder, SalesOrderDetailDTO salesOrderDetailDTO, bool delete)
        {

            var aSalesOrderDetailDTOItem = db.Item.FirstOrDefault(fod => fod.id == salesOrderDetailDTO.id_item);

            if (delete)
            {
                //var aSalesOrderMPMaterialDetails = salesOrder.SalesOrderMPMaterialDetails.Where(w => w.id_product == salesOrderDetailDTO.id_item).ToList();
                for (int i = salesOrder.SalesOrderMPMaterialDetails.Count() - 1; i >= 0; i--)
                {
                    var detail = salesOrder.SalesOrderMPMaterialDetails.ElementAt(i);

                    if (detail.id_product == salesOrderDetailDTO.id_item) {
                        //var aSalesOrderDetailMPMaterialDetails = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Where(w => w.id_salesOrderDetail == salesOrderDetailDTO.id &&
                        //                                                                                                                              w.id_salesOrderMPMaterialDetail == detail.id).ToList();
                        for (int j = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Count() - 1; j >= 0; j--)
                        {
                            var detailASalesOrderDetailMPMaterialDetails = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.ElementAt(j);
                            if (detailASalesOrderDetailMPMaterialDetails.id_salesOrderDetail == salesOrderDetailDTO.id && detailASalesOrderDetailMPMaterialDetails.id_salesOrderMPMaterialDetail == detail.id) {
                                salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Remove(detailASalesOrderDetailMPMaterialDetails);
                            }
                        }
                        salesOrder.SalesOrderMPMaterialDetails.Remove(detail);
                    }
                }
            }
            else {
                if (aSalesOrderDetailDTOItem.ItemIngredient.Count() == 0) return;
                var id_metricUnitItemHeadIngredient = aSalesOrderDetailDTOItem.ItemHeadIngredient?.id_metricUnit;
                if (id_metricUnitItemHeadIngredient == null)
                {
                    throw new Exception("La unidad de medida en la cabecera de la formulación del Ítem: " + aSalesOrderDetailDTOItem.name + " no está configurada, debe configurar un valor. Configúrelo, e intente de nuevo");
                }
                foreach (var iimdd in aSalesOrderDetailDTOItem.ItemIngredient)
                {
                    var aASalesOrderDetailDTOItem = aSalesOrderDetailDTOItem.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == iimdd.id_ingredientItem && fod.id_costumerItem == salesOrder.id_customer);
                    var aPerson = db.Person.FirstOrDefault(fod => fod.fullname_businessName == "SIN CLIENTE");
                    if (((iimdd.id_costumerItem == null || iimdd.id_costumerItem == aPerson?.id) && aASalesOrderDetailDTOItem == null) || iimdd.id_costumerItem == salesOrder.id_customer)
                    {
                        var quantityMetricUnitItemHeadIngredient = salesOrderDetailDTO.quantityApproved;
                        var amountItemHeadIngredient = (aSalesOrderDetailDTOItem.ItemHeadIngredient?.amount ?? 0);
                        if (amountItemHeadIngredient == 0)
                        {
                            throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + aSalesOrderDetailDTOItem.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                        }
                        var quantityItemIngredientMDE = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                        if (quantityItemIngredientMDE == 0) continue;

                        var truncateQuantityItemIngredientMDE = decimal.Truncate(quantityItemIngredientMDE);
                        if ((quantityItemIngredientMDE - truncateQuantityItemIngredientMDE) > 0)
                        {
                            quantityItemIngredientMDE = truncateQuantityItemIngredientMDE + 1;
                        };
                        var id_metricUnitFormulation = iimdd.id_metricUnit;


                        //if (delete)
                        //{
                        //    if (aSalesOrderMPMaterialDetails != null)
                        //    {
                        //        var aSalesOrderDetailMPMaterialDetails = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.FirstOrDefault(fod => fod.id_salesOrderMPMaterialDetail == aSalesOrderMPMaterialDetails.id);
                        //        if (aSalesOrderDetailMPMaterialDetails != null)
                        //        {
                        //            //var quantityAux = aSalesOrderDetailMPMaterialDetails.quantity;
                        //            salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Remove(aSalesOrderDetailMPMaterialDetails);
                        //            //if (0 >= (aSalesOrderMPMaterialDetails.quantity - quantityAux))
                        //            //{
                        //            salesOrder.SalesOrderMPMaterialDetails.Remove(aSalesOrderMPMaterialDetails);
                        //            //}
                        //            //else
                        //            //{
                        //                //string[] subsCodProducts = aSalesOrderMPMaterialDetails.codProducts.Split('|');
                        //                //var aCodProducts = "";
                        //                //foreach (var sub in subsCodProducts)
                        //                //{
                        //                //    var aSubTrim = sub.Trim();
                        //                //    if (aSubTrim != aSalesOrderDetailDTOItem.masterCode)
                        //                //    {
                        //                //        if (string.IsNullOrEmpty(aCodProducts))
                        //                //        {
                        //                //            aCodProducts = aSubTrim;
                        //                //        }
                        //                //        else
                        //                //        {
                        //                //            aCodProducts += " | " + aSubTrim;
                        //                //        }
                        //                //    }
                        //                //}
                        //                //aSalesOrderMPMaterialDetails.codProducts = aCodProducts;

                        //                //string[] subsNameProducts = aSalesOrderMPMaterialDetails.nameProducts.Split('|');
                        //                //var aNameProducts = "";
                        //                //foreach (var sub in subsNameProducts)
                        //                //{
                        //                //    var aSubTrim = sub.Trim();
                        //                //    if (aSubTrim != aSalesOrderDetailDTOItem.name)
                        //                //    {
                        //                //        if (string.IsNullOrEmpty(aNameProducts))
                        //                //        {
                        //                //            aNameProducts = aSubTrim;
                        //                //        }
                        //                //        else
                        //                //        {
                        //                //            aNameProducts += " | " + aSubTrim;
                        //                //        }
                        //                //    }
                        //                //}
                        //                //aSalesOrderMPMaterialDetails.nameProducts = aNameProducts;

                        //                //aSalesOrderMPMaterialDetails.quantityRequiredForFormulation -= quantityAux;
                        //                //aSalesOrderMPMaterialDetails.quantity -= quantityAux;
                        //            //}
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        var aSalesOrderMPMaterialDetails = salesOrder.SalesOrderMPMaterialDetails.FirstOrDefault(fod => fod.id_product == salesOrderDetailDTO.id_item &&
                                                                                                                    fod.id_item == iimdd.id_ingredientItem &&
                                                                                                                    fod.id_metricUnit == iimdd.id_metricUnit);
                        if (aSalesOrderMPMaterialDetails != null)
                        {
                            var aSalesOrderDetailMPMaterialDetails = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.FirstOrDefault(fod => fod.id_salesOrderMPMaterialDetail == aSalesOrderMPMaterialDetails.id);
                            if (aSalesOrderDetailMPMaterialDetails != null)
                            {
                                //aSalesOrderMPMaterialDetails.quantityRequiredForFormulation += (quantityItemIngredientMDE - aSalesOrderDetailMPMaterialDetails.quantity);
                                //aSalesOrderMPMaterialDetails.quantity += (quantityItemIngredientMDE - aSalesOrderDetailMPMaterialDetails.quantity);
                                //aSalesOrderDetailMPMaterialDetails.quantity = quantityItemIngredientMDE;
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(aSalesOrderMPMaterialDetails.codProducts))
                                //{
                                //    aSalesOrderMPMaterialDetails.codProducts = aSalesOrderDetailDTOItem.masterCode;
                                //    aSalesOrderMPMaterialDetails.nameProducts = aSalesOrderDetailDTOItem.name;
                                //}
                                //else
                                //{
                                //    aSalesOrderMPMaterialDetails.codProducts += " | " + aSalesOrderDetailDTOItem.masterCode;
                                //    aSalesOrderMPMaterialDetails.nameProducts += " | " + aSalesOrderDetailDTOItem.name;
                                //}
                                aSalesOrderMPMaterialDetails.codProduct = aSalesOrderDetailDTOItem.masterCode;
                                aSalesOrderMPMaterialDetails.id_product = aSalesOrderDetailDTOItem.id;
                                aSalesOrderMPMaterialDetails.quantityRequiredForFormulation = quantityItemIngredientMDE;
                                aSalesOrderMPMaterialDetails.quantity = quantityItemIngredientMDE;
                                salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Add(new SalesOrderDetailMPMaterialDetailDTO
                                {
                                    id = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Count() > 0 ? salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1,
                                    id_salesOrderDetail = salesOrderDetailDTO.id,
                                    id_salesOrderMPMaterialDetail = aSalesOrderMPMaterialDetails.id,
                                    quantity = quantityItemIngredientMDE
                                });
                                //aSalesOrderDetailMPMaterialDetails.quantity = quantityItemIngredientMDE;
                            }
                        }
                        else
                        {
                            aSalesOrderMPMaterialDetails = new SalesOrderMPMaterialDetailDTO
                            {
                                id = salesOrder.SalesOrderMPMaterialDetails.Count() > 0 ? salesOrder.SalesOrderMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1,
                                //codProducts = aSalesOrderDetailDTOItem.masterCode,
                                codProduct = aSalesOrderDetailDTOItem.masterCode,
                                id_product = aSalesOrderDetailDTOItem.id,
                                //nameProducts = aSalesOrderDetailDTOItem.name,
                                id_inventoryLine = iimdd.Item1.id_inventoryLine,
                                id_itemType = iimdd.Item1.id_itemType,
                                id_itemTypeCategory = iimdd.Item1.id_itemTypeCategory,
                                cod_item = iimdd.Item1.masterCode,
                                name_item = iimdd.Item1.name,
                                id_item = iimdd.id_ingredientItem,
                                quantityRequiredForFormulation = quantityItemIngredientMDE,
                                quantity = quantityItemIngredientMDE,
                                id_metricUnit = iimdd.id_metricUnit,
                                manual = false
                            };
                            salesOrder.SalesOrderMPMaterialDetails.Add(aSalesOrderMPMaterialDetails);

                            salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Add(new SalesOrderDetailMPMaterialDetailDTO
                            {
                                id = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Count() > 0 ? salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1,
                                id_salesOrderDetail = salesOrderDetailDTO.id,
                                id_salesOrderMPMaterialDetail = aSalesOrderMPMaterialDetails.id,
                                quantity = quantityItemIngredientMDE
                            });
                        }
                        //}
                    }
                }
            }
            

        }

        private void UpdateSummary()
        {
            var salesOrder = GetSalesOrderDTO();

            for (int j = salesOrder.SalesOrderMPMaterialDetailsSummary.Count - 1; j >= 0; j--)
            {
                var detailSalesOrderMPMaterialDetailsSummary = salesOrder.SalesOrderMPMaterialDetailsSummary.ElementAt(j);
                salesOrder.SalesOrderMPMaterialDetailsSummary.Remove(detailSalesOrderMPMaterialDetailsSummary);
            }

            foreach (var itemMPMaterialDetail in salesOrder.SalesOrderMPMaterialDetails)
            {
                var aSalesOrderMPMaterialDetailsSummary = salesOrder.SalesOrderMPMaterialDetailsSummary.FirstOrDefault(fod => fod.id_item == itemMPMaterialDetail.id_item &&
                                                                                                                               fod.id_metricUnit == itemMPMaterialDetail.id_metricUnit);
                if (aSalesOrderMPMaterialDetailsSummary == null)
                {
                    salesOrder.SalesOrderMPMaterialDetailsSummary.Add(new SalesOrderMPMaterialDetailSummaryDTO
                    {
                        id = salesOrder.SalesOrderMPMaterialDetailsSummary.Count() > 0 ? salesOrder.SalesOrderMPMaterialDetailsSummary.Max(ppd => ppd.id) + 1 : 1,
                        id_inventoryLine = itemMPMaterialDetail.id_inventoryLine,
                        cod_item = itemMPMaterialDetail.cod_item,
                        name_item = itemMPMaterialDetail.name_item,
                        id_item = itemMPMaterialDetail.id_item,
                        quantityRequiredForFormulation = itemMPMaterialDetail.quantityRequiredForFormulation,
                        quantity = itemMPMaterialDetail.quantity,
                        id_metricUnit = itemMPMaterialDetail.id_metricUnit,
                    });
                }
                else
                {
                    aSalesOrderMPMaterialDetailsSummary.quantityRequiredForFormulation += itemMPMaterialDetail.quantityRequiredForFormulation;
                    aSalesOrderMPMaterialDetailsSummary.quantity += itemMPMaterialDetail.quantity;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsUpdate(SalesOrderDetailDTO item, bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = salesOrder.SalesOrderDetails.FirstOrDefault(it => it.id == item.id);

                    if (modelItem.id_item != item.id_item) {
                        UpdateSalesOrderMPMaterialDetails(salesOrder, modelItem, true);
                    }

                    if (modelItem != null)
                    {
                        modelItem.noProgProduction = "";
                        //modelItem.noRequestProforma = "";
                        modelItem.noRequestProforma = item.noRequestProforma;
                        modelItem.id_salesRequestDetail = null;
                        //modelItem.id_invoiceDetail = null;
                        if (!string.IsNullOrEmpty(salesOrder.proformaOrder))
                        {
                            var aId_documentOrigen = db.Document.FirstOrDefault(fod => fod.Invoice != null && fod.Invoice.SalesQuotationExterior != null && fod.number == salesOrder.proformaOrder)?.id;
                            var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                            modelItem.id_invoiceDetail = proforma.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == item.id_item)?.id;
                        }
                        else
                        {
                            modelItem.id_invoiceDetail = null;
                        }
                        modelItem.id_item = item.id_item;
                        modelItem.cartons = item.quantityApproved;
                        modelItem.quantityApproved = item.quantityApproved;
                        modelItem.quantityProgrammed = item.quantityProgrammed;
                        modelItem.quantityProduced = item.quantityProduced;
                        var aItem = db.Item.FirstOrDefault(fod => fod.id == modelItem.id_item);
                        modelItem.name_item = aItem.name;
                        modelItem.description_item = aItem.description;
                        modelItem.cod_item = aItem.masterCode;
                        modelItem.codAux_item = aItem.auxCode;
                        modelItem.originQuantity = Math.Round(modelItem.quantityApproved * aItem.Presentation.maximum * aItem.Presentation.minimum, 2);
                        modelItem.originQuantityStr = modelItem.originQuantity.ToString() + " " + aItem.Presentation.MetricUnit.code;
                        //modelItem.totalHours = new TimeSpan(int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursHours"]) ? "0" : Request.Params["totalHoursHours"]),
                        //                              int.Parse(string.IsNullOrEmpty(Request.Params["totalHoursMinutes"]) ? "0" : Request.Params["totalHoursMinutes"]), 0);

                        //this.UpdateModel(modelItem);
                        UpdateSalesOrderMPMaterialDetails(salesOrder, modelItem, false);
                        UpdateTotals();
                        UpdateSummary();

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
            ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewDetails", salesOrder.SalesOrderDetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsDelete(System.Int32 id)
        {
            var salesOrder = GetSalesOrderDTO();

            try
            {
                var modelItem = salesOrder.SalesOrderDetails.FirstOrDefault(it => it.id == id);
                if (modelItem != null && modelItem.quantityProduced > 0)
                {
                    //model = invoice.InvoiceDetail.Where(x => x.isActive).ToList();
                    throw new Exception("No se puede eliminar el detalle por tener Cant. Producida");
                }
                if (modelItem != null)
                    salesOrder.SalesOrderDetails.Remove(modelItem);

                UpdateSalesOrderMPMaterialDetails(salesOrder, modelItem, true);

                UpdateTotals();
                UpdateSummary();


            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            ViewBag.enabled = bool.Parse(string.IsNullOrEmpty(Request.Params["enabledCurrent"]) ? "true" : Request.Params["enabledCurrent"]);
            ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewDetails", salesOrder.SalesOrderDetails);
        }

        #endregion

        #region GridViewMPMaterialDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewMPMaterialDetails(bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();

            ViewBag.enabled = enabled;
            //ViewBag.code_documentType = salesOrder.code_documentType;
            //ViewBag.dateTimeEmision = salesOrder.dateTimeEmision;

            return PartialView("_GridViewMPMaterialDetails", salesOrder.SalesOrderMPMaterialDetails.OrderBy(o => o.codProduct).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewMPMaterialDetailsAddNew(SalesOrderMPMaterialDetailDTO item, bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();
            //ModelState.IsValid = true;
            if (ModelState.IsValid)
            {
                item.id = salesOrder.SalesOrderMPMaterialDetails.Count() > 0 ? salesOrder.SalesOrderMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1;
                var modelItem = db.Item.FirstOrDefault(it => it.id == item.id_product);
                item.codProduct = modelItem?.masterCode ?? "";
                //item.nameProducts = modelItem?.name ?? "";
                var aItem = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                item.name_item = aItem.name;
                item.cod_item = aItem.masterCode;
                item.quantityRequiredForFormulation = 0.00M;
                item.manual = true;
                salesOrder.SalesOrderMPMaterialDetails.Add(item);

                var salesOrderDetailDTO = salesOrder.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == item.id_product);
                salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Add(new SalesOrderDetailMPMaterialDetailDTO
                {
                    id = salesOrderDetailDTO != null && salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails != null && salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Count() > 0 ? salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1,
                    id_salesOrderDetail = salesOrderDetailDTO.id,
                    id_salesOrderMPMaterialDetail = item.id,
                    quantity = item.quantity
                });

                UpdateSummary();
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
            //ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewMPMaterialDetails", salesOrder.SalesOrderMPMaterialDetails.OrderBy(o => o.codProduct).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewMPMaterialDetailsUpdate(SalesOrderMPMaterialDetailDTO item, bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = salesOrder.SalesOrderMPMaterialDetails.FirstOrDefault(it => it.id == item.id);

                    if (modelItem != null)
                    {
                        modelItem.quantity = item.quantity;

                        var salesOrderDetailDTO = salesOrder.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == modelItem.id_product);

                        var salesOrderDetailMPMaterialDetails = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.FirstOrDefault(it => it.id_salesOrderDetail == salesOrderDetailDTO.id && 
                                                                                                                    it.id_salesOrderMPMaterialDetail == item.id);

                        salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Remove(salesOrderDetailMPMaterialDetails);

                        //salesOrder.SalesOrderMPMaterialDetails.Remove(aSalesOrderMPMaterialDetails);

                        var productAux = db.Item.FirstOrDefault(it => it.id == item.id_product);
                        modelItem.codProduct = productAux?.masterCode ?? "";
                        modelItem.id_product = item.id_product;
                        //modelItem.codProducts = productAux?.masterCode ?? "";
                        //modelItem.nameProducts = productAux?.name ?? "";
                        var aItem = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                        modelItem.id_item = item.id_item;
                        modelItem.name_item = aItem.name;
                        modelItem.cod_item = aItem.masterCode;
                        modelItem.quantityRequiredForFormulation = 0.00M;
                        //item.manual = true;
                        //salesOrder.SalesOrderMPMaterialDetails.Add(item);
                        salesOrderDetailDTO = salesOrder.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == modelItem.id_product);

                        salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Add(new SalesOrderDetailMPMaterialDetailDTO
                        {
                            id = salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Count() > 0 ? salesOrderDetailDTO.SalesOrderDetailMPMaterialDetails.Max(ppd => ppd.id) + 1 : 1,
                            id_salesOrderDetail = salesOrderDetailDTO.id,
                            id_salesOrderMPMaterialDetail = item.id,
                            quantity = item.quantity
                        });

                        UpdateSummary();
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
            //ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewMPMaterialDetails", salesOrder.SalesOrderMPMaterialDetails.OrderBy(o => o.codProduct).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewMPMaterialDetailsDelete(System.Int32 id)
        {
            var salesOrder = GetSalesOrderDTO();

            try
            {
                var modelItem = salesOrder.SalesOrderMPMaterialDetails.FirstOrDefault(it => it.id == id);
                //if (modelItem != null)
                //{
                //    var aSalesOrderDetails = salesOrder.SalesOrderDetails.Where(w => w.SalesOrderDetailMPMaterialDetails.FirstOrDefault(fod => fod.id_salesOrderMPMaterialDetail == id) != null).ToList();
                //    //if (aSalesOrderDetails.Count() > 1)
                //    //{
                //    //    throw new Exception("No se puede eliminar el detalle de MP & Meteriales por proceder de los productos: " + modelItem.nameProducts);
                //    //}
                //    if (aSalesOrderDetails.Count() == 1)
                //    {
                //        throw new Exception("No se puede eliminar el detalle de MP & Meteriales por proceder del producto: " + modelItem.nameProducts);
                //    }
                //}
                if (modelItem != null) {

                    var aSalesOrderDetails = salesOrder.SalesOrderDetails.FirstOrDefault(w => w.id_item == modelItem.id_product);
                    
                    var aSalesOrderDetailMPMaterialDetails = aSalesOrderDetails.SalesOrderDetailMPMaterialDetails.Where(w => w.id_salesOrderDetail == aSalesOrderDetails.id &&
                                                                                                                                w.id_salesOrderMPMaterialDetail == modelItem.id).ToList();
                    for (int j = aSalesOrderDetailMPMaterialDetails.Count() - 1; j >= 0; j--)
                    {
                        var detailASalesOrderDetailMPMaterialDetails = aSalesOrderDetailMPMaterialDetails.ElementAt(j);
                        aSalesOrderDetails.SalesOrderDetailMPMaterialDetails.Remove(detailASalesOrderDetailMPMaterialDetails);
                    }

                    salesOrder.SalesOrderMPMaterialDetails.Remove(modelItem);

                    UpdateSummary();
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            ViewBag.enabled = bool.Parse(string.IsNullOrEmpty(Request.Params["enabledCurrent"]) ? "true" : Request.Params["enabledCurrent"]);
            //ViewBag.code_documentType = salesOrder.code_documentType;

            return PartialView("_GridViewMPMaterialDetails", salesOrder.SalesOrderMPMaterialDetails.OrderBy(o => o.codProduct).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadInventoryLineIngredientItem(int? id_inventoryLineIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxInventoryLineIngredientItemProperties(id_inventoryLineIngredientItem);
            //TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxInventoryLineIngredientItemProperties(int? id_inventoryLineIngredientItem)
        {
            //inventoryLines
            var inventoryLines = db.InventoryLine.Where(t => t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_inventoryLineIngredientItem)).Select(t => new { t.id, t.name }).ToList();

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_inventoryLine";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadInventoryLineIngredientItem" };
            p.ClientSideEvents.BeginCallback = "InventoryLineIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "InventoryLineIngredientItem_EndCallback";
            p.ClientSideEvents.SelectedIndexChanged = "ComboInventoryLineIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnInventoryLineIngredientItemValidation";
            p.ClientSideEvents.Init = "OnInventoryLineIngredientItemInit";

            p.BindList(inventoryLines);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemTypeIngredientItem(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxItemTypeIngredientItemProperties(id_inventoryLineIngredientItem, id_itemTypeIngredientItem);
            //TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxItemTypeIngredientItemProperties(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem)
        {
            //itemTypes
            var itemTypes = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLineIngredientItem && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_itemTypeIngredientItem)).Select(t => new { t.id, t.name }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_itemType";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadItemTypeIngredientItem" };
            p.ClientSideEvents.BeginCallback = "ItemTypeIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "ItemTypeIngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemTypeIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemTypeIngredientItemValidation";

            p.BindList(itemTypes);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemTypeCategoryIngredientItem(int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxItemTypeCategoryIngredientItemProperties(id_itemTypeIngredientItem, id_itemTypeCategoryIngredientItem);
            //TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxItemTypeCategoryIngredientItemProperties(int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem)
        {
            //itemTypeCategories
            var itemTypeCategories = db.ItemTypeCategory.Where(c => c.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemTypeIngredientItem || c.id == id_itemTypeCategoryIngredientItem)).Select(c => new { c.id, c.name }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_itemType";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadItemTypeCategoryIngredientItem" };
            p.ClientSideEvents.BeginCallback = "ItemTypeCategoryIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "ItemTypeCategoryIngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemTypeCategoryIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemTypeCategoryIngredientItemValidation";

            p.BindList(itemTypeCategories);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadIngredientItem(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem, int? id_ingredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxIngredientItemProperties(id_inventoryLineIngredientItem, id_itemTypeIngredientItem, id_itemTypeCategoryIngredientItem, id_ingredientItem);
            //TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxIngredientItemProperties(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem, int? id_ingredientItem)
        {
            //ingredientItems
            var ingredientItems = db.Item.Where(t => t.id_inventoryLine == id_inventoryLineIngredientItem &&
                                                     t.id_itemType == id_itemTypeIngredientItem &&
                                                     t.id_itemTypeCategory == id_itemTypeCategoryIngredientItem &&
                                                     t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_ingredientItem)).Select(t => new { t.id, t.name }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_item";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadIngredientItem" };
            p.ClientSideEvents.BeginCallback = "IngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "IngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemIngredientItemValidation";
            p.ClientSideEvents.Init = "OnItemIngredientItemInit";

            p.BindList(ingredientItems);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadMetricUnitIngredientItem(int? id_ingredientItem, int? id_metricUnitIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxMetricUnitIngredientItemProperties(id_ingredientItem, id_metricUnitIngredientItem);
            //TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxMetricUnitIngredientItemProperties(int? id_ingredientItem, int? id_metricUnitIngredientItem)
        {
            //metricUnits
            var id_metricTypeAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.id_metricType;
            var metricUnits = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_metricUnitIngredientItem)).Select(t => new { t.id, t.code }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_metricUnit";
            p.ValueField = "id";
            p.TextField = "code";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadMetricUnitIngredientItem" };
            p.ClientSideEvents.BeginCallback = "MetricUnitIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "MetricUnitIngredientItem_EndCallback";
            //p.EnableSynchronization = DefaultBoolean.False;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboMetricUnitIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnMetricUnitIngredientItemValidation";

            p.BindList(metricUnits);
            return p;

        }

        public JsonResult GetValueMetricUnitIngredientItem(int? id_ingredientItem)
        {
            var result = new
            {
                id_metricUnitIngredientItem = (int?)null

            };
            var id_metricAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.ItemInventory?.id_metricUnitInventory;
            result = new
            {
                id_metricUnitIngredientItem = id_metricAux

            };

            //TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ItsRepeatedIngredientItem(int? id_ingredientItem, int? id_metricUnitIngredientItem, int? id_product)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var salesOrderDTO = GetSalesOrderDTO();
            //Item item = (TempData["item"] as Item);

            //item = item ?? new Item();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };
            var aProduct = db.Item.FirstOrDefault(fod => fod.id == id_product);
            var codeProduct = aProduct?.masterCode ?? "";
            var itemItemIngredientAux = salesOrderDTO.
                                        SalesOrderMPMaterialDetails.
                                        FirstOrDefault(fod => fod.id_item == id_ingredientItem && fod.id_metricUnit == id_metricUnitIngredientItem && fod.id_product == id_product);
            if (itemItemIngredientAux != null)
            {
                var aItem = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem);
                var aMetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitIngredientItem);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Detalle: " + (aProduct?.name ?? "SIN DETALLE") + " con el Ingrediente: " + (aItem?.name ?? "SIN INGREDIENTE") + " y con la UM: " + (aMetricUnit?.code ?? "SIN UM") + " en el detalle de MP & Matariales."

                };

            }

            //TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailItem(int? id_product)
        {
            var salesOrder = GetSalesOrderDTO();
            //MVCxColumnComboBoxProperties p = CreateComboBoxIngredientItemProperties(id_inventoryLineIngredientItem, id_itemTypeIngredientItem, id_itemTypeCategoryIngredientItem, id_ingredientItem);
            //var products = db.Item.Where(t => salesOrder.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == t.id) == null).Select(t => new { t.id, t.name }).ToList();
            var products = salesOrder.SalesOrderDetails.Where(w => w.id_item != id_product).Select(t => new { id = t.id_item, name = t.name_item }).ToList();
            if (id_product != null) {
                products.Add(new { id = id_product.Value, db.Item.FirstOrDefault(fod => fod.id == id_product).name });
            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_product";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "LoadDetailItem" };
            p.ClientSideEvents.BeginCallback = "DetailItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "DetailItem_EndCallback";
            //p.ShowModelErrors = true;
            //p.ClientSideEvents.SelectedIndexChanged = "ComboItemIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemDetailItemValidation";
            p.ClientSideEvents.Init = "OnItemDetailItemInit";

            p.BindList(products);

            return GridViewExtension.GetComboBoxCallbackResult(p);

        }


        #endregion


        #region GridViewMPMaterialSummaryDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewMPMaterialSummaryDetails(bool? enabled)
        {
            var salesOrder = GetSalesOrderDTO();

            ViewBag.enabled = enabled;
            //ViewBag.code_documentType = salesOrder.code_documentType;
            //ViewBag.dateTimeEmision = salesOrder.dateTimeEmision;

            return PartialView("_GridViewMPMaterialSummaryDetails", salesOrder.SalesOrderMPMaterialDetailsSummary.OrderBy(o => o.id_inventoryLine).ThenBy(tb => tb.cod_item).ToList());
        }

        #endregion

        #region Combobox
        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoProductionOrderStock))
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

        private void BuildComboBoxDocumentTypeIndex()
        {
            ViewData["DocumentTypeIndex"] = db.DocumentType.Where(e => e.isActive && (e.code.Equals("139") || e.code.Equals("140") || e.code.Equals("141")))
               .Select(s => new SelectListItem
               {
                   Text = s.name,
                   Value = s.id.ToString(),
               }).ToList();

        }

        public ActionResult ComboBoxDocumentTypeIndex()
        {
            BuildComboBoxDocumentTypeIndex();
            return PartialView("_ComboBoxDocumentTypeIndex");
        }

        private void BuildComboBoxCustomerIndex()
        {
            List<string> listRol = new List<string>();
            listRol.Add("Cliente Local");
            listRol.Add("Cliente Exterior");
            ViewData["CustomerIndex"] = (DataProviderPerson.RolsByCompanyAndCurrent(ActiveCompany.id, null, listRol) as List<Person>)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxCustomerIndex()
        {
            BuildComboBoxCustomerIndex();
            return PartialView("_ComboBoxCustomerIndex");
        }

        private void BuildComboBoxSellerIndex()
        {
            ViewData["SellerIndex"] = (DataProviderPerson.RolsByCompany(ActiveCompany.id, "Vendedor") as List<Person>)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxSellerIndex()
        {
            BuildComboBoxSellerIndex();
            return PartialView("_ComboBoxSellerIndex");
        }

        private void BuildTokenBoxItemsIndex()
        {
            ViewData["ItemsIndex"] = (DataProviderItem.ItemsByCompanyAndInventoryLine(ActiveCompany.id, "PT") as List<Item>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult TokenBoxItemsIndex()
        {
            BuildTokenBoxItemsIndex();
            return PartialView("_TokenBoxItemsIndex");
        }

        private void BuildComboBoxLogisticsIndex()
        {
            var aSelectListItems = new List<SelectListItem>();
            aSelectListItems.Add(new SelectListItem
            {
                Text = "SI",
                Value = "1"
            });
            aSelectListItems.Add(new SelectListItem
            {
                Text = "NO",
                Value = "0"
            });
            ViewData["LogisticsIndex"] = aSelectListItems;
        }

        public ActionResult ComboBoxLogisticsIndex()
        {
            BuildComboBoxLogisticsIndex();
            return PartialView("_ComboBoxLogisticsIndex");
        }

        private void BuildComboBoxEmployeeApplicant()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.employeeApplicant,
                Value = salesOrderDTO.id_employeeApplicant.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Employee.Where(g => g.Person.isActive && g.id != salesOrderDTO.id_employeeApplicant)
                .Select(s => new SelectListItem
                {
                    Text = s.Person.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["EmployeeApplicant"] = aSelectListItems;
        }

        public ActionResult ComboBoxEmployeeApplicant()
        {
            BuildComboBoxEmployeeApplicant();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxEmployeeApplicant");
        }

        private void BuildComboBoxCustomer()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.customer,
                Value = salesOrderDTO.id_customer.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange((DataProviderPerson.Customers() as List<Person>).Where(g => g.isActive && g.id != salesOrderDTO.id_customer)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Customer"] = aSelectListItems;
        }

        public ActionResult ComboBoxCustomer()
        {
            BuildComboBoxCustomer();
            ViewBag.enabled = true;
            var salesOrderDTO = GetSalesOrderDTO();
            ViewBag.code_documentType = salesOrderDTO.code_documentType;
            return PartialView("_ComboBoxCustomer");
        }

        private void BuildComboBoxProvider()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.Provider,
                Value = salesOrderDTO.id_provider.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange((DataProviderPerson.PersonsExportPlant() as List<Person>).Where(g => g.isActive && g.id != salesOrderDTO.id_provider)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Provider"] = aSelectListItems;
        }

        public ActionResult ComboBoxProvider()
        {
            BuildComboBoxProvider();
            ViewBag.enabled = true;
            var salesOrderDTO = GetSalesOrderDTO();
            ViewBag.code_documentType = salesOrderDTO.code_documentType;
            return PartialView("_ComboBoxProvider");
        }

        private void BuildComboBoxReason()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.orderReason,
                Value = salesOrderDTO.id_orderReason.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange((DataProviderOrderReason.Reasons() as List<OrderReason>).Where(g => g.isActive && g.id != salesOrderDTO.id_orderReason)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Reason"] = aSelectListItems;
        }

        public ActionResult ComboBoxReason()
        {
            BuildComboBoxReason();
            ViewBag.enabled = true;
            var salesOrderDTO = GetSalesOrderDTO();
            ViewBag.code_documentType = salesOrderDTO.code_documentType;
            return PartialView("_ComboBoxReason");
        }

        private void BuildComboBoxPortDestination()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.portDestination,
                Value = salesOrderDTO.id_portDestination.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Port.Where(g => g.isActive && g.id != salesOrderDTO.id_portDestination)
                .Select(s => new SelectListItem
                {
                    Text = s.nombre,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["PortDestination"] = aSelectListItems;
        }

        public ActionResult ComboBoxPortDestination()
        {
            BuildComboBoxPortDestination();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxPortDestination");
        }

        private void BuildComboBoxPortDischarge()
        {
            var salesOrderDTO = GetSalesOrderDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = salesOrderDTO.portDischarge,
                Value = salesOrderDTO.id_portDischarge.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Port.Where(g => g.isActive && g.id != salesOrderDTO.id_portDischarge)
                .Select(s => new SelectListItem
                {
                    Text = s.nombre,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["PortDischarge"] = aSelectListItems;
        }

        public ActionResult ComboBoxPortDischarge()
        {
            BuildComboBoxPortDischarge();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxPortDischarge");
        }

        private void BuildComboBoxSizeBegin()
        {
            ViewData["SizeBegin"] = (DataProviderItemSize.ItemSizes() as List<ItemSize>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxSizeBegin()
        {
            BuildComboBoxSizeBegin();
            return PartialView("_ComboBoxSizeBegin");
        }

        private void BuildComboBoxSizeEnd()
        {
            ViewData["SizeEnd"] = (DataProviderItemSize.ItemSizes() as List<ItemSize>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxSizeEnd()
        {
            BuildComboBoxSizeEnd();
            return PartialView("_ComboBoxSizeEnd");
        }

        private void BuildComboBoxInventoryLine()
        {
            ViewData["InventoryLine"] = (DataProviderInventoryLine.InventoryLinesCartByCart(ActiveCompany.id) as List<InventoryLine>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxInventoryLine()
        {
            BuildComboBoxInventoryLine();
            return PartialView("_ComboBoxInventoryLine");
        }

        private void BuildComboBoxItemType(int? id_inventoryLine = null)
        {
            ViewData["ItemType"] = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemType(int? id_inventoryLine)
        {
            BuildComboBoxItemType(id_inventoryLine);
            return PartialView("_ComboBoxItemType");
        }

        private void BuildComboBoxItemTypeCategory(int? id_itemType = null)
        {
            ViewData["ItemTypeCategory"] = db.ItemTypeItemTypeCategory.Where(t => t.id_itemType == id_itemType && t.ItemTypeCategory.id_company == ActiveCompany.id && t.ItemTypeCategory.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.ItemTypeCategory.name,
                    Value = s.ItemTypeCategory.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemTypeCategory(int? id_itemType)
        {
            BuildComboBoxItemTypeCategory(id_itemType);
            return PartialView("_ComboBoxItemTypeCategory");
        }

        private void BuildComboBoxItemGroup()
        {
            ViewData["ItemGroup"] = (DataProviderItemGroup.ItemGroups(ActiveCompany.id) as List<ItemGroup>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemGroup()
        {
            BuildComboBoxItemGroup();
            return PartialView("_ComboBoxItemGroup");
        }

        private void BuildComboBoxItemSubGroup(int id_itemGroup = 0)
        {
            ViewData["ItemSubGroup"] = db.ItemGroup.Where(t => t.id_parentGroup == id_itemGroup && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemSubGroup(int id_itemGroup = 0)
        {
            BuildComboBoxItemSubGroup(id_itemGroup);
            return PartialView("_ComboBoxItemSubGroup");
        }

        private void BuildComboBoxItemSize()
        {
            ViewData["ItemSize"] = (DataProviderItemSize.ItemSizes() as List<ItemSize>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemSize()
        {
            BuildComboBoxItemSize();
            return PartialView("_ComboBoxItemSize");
        }

        private void BuildComboBoxItemTrademark()
        {
            ViewData["ItemTrademark"] = (DataProviderItemTrademark.ItemTrademarks(ActiveCompany.id) as List<ItemTrademark>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemTrademark()
        {
            BuildComboBoxItemTrademark();
            return PartialView("_ComboBoxItemTrademark");
        }

        private void BuildComboBoxItemTrademarkModel()
        {
            ViewData["ItemTrademarkModel"] = (DataProviderItemTrademarkModel.ItemTrademarkModels() as List<ItemTrademarkModel>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemTrademarkModel()
        {
            BuildComboBoxItemTrademarkModel();
            return PartialView("_ComboBoxItemTrademarkModel");
        }

        private void BuildComboBoxItemColor()
        {
            ViewData["ItemColor"] = (DataProviderItemColor.ItemsColors() as List<ItemColor>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemColor()
        {
            BuildComboBoxItemColor();
            return PartialView("_ComboBoxItemColor");
        }

        [HttpPost]
        public ActionResult ComboBoxItemEdit(int? id_item, string nameItem, int? sizeBegin, int? sizeEnd, int? inventoryLine, int? itemType, int? itemTypeCategory,
                                             int? itemGroup, int? itemSubGroup, int? itemSize, int? itemTrademark, int? itemTrademarkModel, int? itemColor)
        {
            //proformaOrder
            List<SelectListItem> model = new List<SelectListItem>();

            var query = model.Select(s => new
            {
                s.Text,
                s.Value,
                Code = ""
            }).ToList();

            var aSalesOrderDTO = GetSalesOrderDTO();

            if (!string.IsNullOrEmpty(aSalesOrderDTO.proformaOrder))
            {
                //(r.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.SalesOrderDetailSalesQuotationExterior == null ||
                //                                                                fod.SalesOrderDetailSalesQuotationExterior != null &&
                //                                                                fod.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod2 => fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null) != null ||
                //                  r.SalesOrderDetailSalesQuotationExterior == null ||
                //                 (r.SalesOrderDetailSalesQuotationExterior != null &&
                //                  r.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05") == null))))//05: ANULADA

                var aId_documentOrigen = db.Document.FirstOrDefault(fod => fod.Invoice != null && fod.Invoice.SalesQuotationExterior != null && fod.number == aSalesOrderDTO.proformaOrder)?.id;
                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                query = proforma.Invoice.InvoiceDetail.Where(w => w.id_item == id_item || (w.isActive /*&& w.proformaPendingNumBoxes > 0*/ && aSalesOrderDTO.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == w.id_item) == null &&
                                                                                           (w.SalesOrderDetailSalesQuotationExterior == null || 
                                                                                           (w.SalesOrderDetailSalesQuotationExterior != null && 
                                                                                           w.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05" &&
                                                                                                                                                             fod.SalesOrderDetail.SalesOrder.id != aSalesOrderDTO.id) == null))))
                    .Select(s => new 
                    {
                        Text = s.Item.name,
                        Value = s.Item.id.ToString(),
                        Code = s.Item.masterCode
                    }).Distinct().ToList();
            }
            else
            {
                //var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
                //var orderSizeBegin = db.ItemSize
                //		.FirstOrDefault(fod => fod.id == sizeBegin)?
                //		.orderSize;
                //var orderSizeEnd = db.ItemSize
                //		.FirstOrDefault(fod => fod.id == sizeEnd)?
                //		.orderSize;
                //model = db.Item
                //	.Where(w => w.id == id_item || ((w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
                //			&& (w.ItemType != null) && (w.ItemType.ProcessType != null)
                //			&& (w.Presentation != null)
                //			&& (w.Presentation.code.Substring(0, 1) != codeMaster)
                //			&& w.isActive
                //			&& (nameItem == "" || nameItem == null ? true : w.name.Contains(nameItem))
                //			&& (orderSizeBegin == null ? true : w.ItemGeneral.ItemSize.orderSize >= orderSizeBegin)
                //			&& (orderSizeEnd == null ? true : w.ItemGeneral.ItemSize.orderSize <= orderSizeEnd)
                //			&& (inventoryLine == null ? true : w.id_inventoryLine == inventoryLine)
                //			&& (itemType == null ? true : w.id_itemType == itemType)
                //			&& (itemTypeCategory == null ? true : w.id_itemTypeCategory == itemTypeCategory)
                //			&& (itemGroup == null ? true : w.ItemGeneral.id_group == itemGroup)
                //			&& (itemSubGroup == null ? true : w.ItemGeneral.id_subgroup == itemSubGroup)
                //			&& (itemSize == null ? true : w.ItemGeneral.id_size == itemSize)
                //			&& (itemTrademark == null ? true : w.ItemGeneral.id_trademark == itemTrademark)
                //			&& (itemTrademarkModel == null ? true : w.ItemGeneral.id_trademarkModel == itemTrademarkModel)
                //			&& (itemColor == null ? true : w.ItemGeneral.id_color == itemColor)))
                //	.Select(s => new SelectListItem
                //	{
                //		Text = s.name,
                //		Value = s.id.ToString()
                //	}).ToList();
                var queryItems = db.Item
                    .Where(w => (w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
                            && (w.ItemType != null) //&& (w.ItemType.ProcessType != null)
                            && (w.Presentation != null) //&& (w.Presentation.minimum == 1m)
                            //&& (w.Presentation.code.Substring(0, 1) != codeMaster)
                            //&& (w.ItemType.ProcessType.id == idProcessType)
                            && w.isActive && w.id != id_item);
                if (!String.IsNullOrEmpty(nameItem))
                {
                    queryItems = queryItems
                        .Where(w => w.name.Contains(nameItem));
                }
                //if (!String.IsNullOrEmpty(nameCodigoItem))
                //{
                //    queryItems = queryItems
                //        .Where(w => w.masterCode.Contains(nameCodigoItem));
                //}

                if (sizeBegin > 0)
                {
                    var orderSizeBegin = db.ItemSize
                        .FirstOrDefault(fod => fod.id == sizeBegin)?
                        .orderSize;

                    if (orderSizeBegin.HasValue)
                    {
                        queryItems = (from v in queryItems
                                      join c in db.ItemGeneral on v.id equals c.id_item
                                      join d in db.ItemSize on c.id_size equals d.id
                                      where d.orderSize >= orderSizeBegin.Value && d.isActive
                                      select v);
                    }
                }

                if (sizeEnd > 0)
                {
                    var orderSizeEnd = db.ItemSize
                        .FirstOrDefault(fod => fod.id == sizeEnd)?
                        .orderSize;

                    if (orderSizeEnd.HasValue)
                    {
                        queryItems = (from v in queryItems
                                      join c in db.ItemGeneral on v.id equals c.id_item
                                      join d in db.ItemSize on c.id_size equals d.id
                                      where d.orderSize <= orderSizeEnd.Value && d.isActive
                                      select v);
                    }
                }

                if (inventoryLine > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.id_inventoryLine == inventoryLine);
                }

                if (itemType > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.id_itemType == itemType);
                }

                if (itemTypeCategory > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.id_itemTypeCategory == itemTypeCategory);
                }

                if (itemGroup > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_group == itemGroup);
                }

                if (itemSubGroup > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_subgroup == itemSubGroup);
                }

                if (itemSize > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_size == itemSize);
                }

                if (itemTrademark > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_trademark == itemTrademark);
                }

                if (itemTrademarkModel > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_trademarkModel == itemTrademarkModel);
                }

                if (itemColor > 0)
                {
                    queryItems = queryItems
                        .Where(w => w.ItemGeneral.id_color == itemColor);
                }

                if (id_item != null) {
                    var aItem = db.Item.FirstOrDefault(s => s.id == id_item);
                    query.Insert(0, new
                    {
                        Text = aItem.name,
                        Value = aItem.id.ToString(),
                        Code = aItem.masterCode
                    });
                }
                query.AddRange(queryItems
                    .Select(s => new 
                    {
                        Text = s.name,
                        Value = s.id.ToString(),
                        Code = s.masterCode
                    }).ToList());
            }

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_item";
                p.Width = Unit.Percentage(100);

                //p.ValueField = "Value";
                //p.TextField = "Text";
                //p.ValueType = typeof(int);

                //p.TextField = "name";
                //p.ValueField = "id";
                p.ValueType = typeof(int);

                //p.TextField = "name";
                //p.ValueField = "id";
                //p.ValueType = typeof(string);
                p.TextFormatString = "{0}, {1}";
                p.ValueField = "Value";
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.Columns.Add("code", "Cod.", 70);
                p.Columns.Add("Text", "Nombre", 320);

                p.CallbackRouteValues = new
                {
                    Controller = "SalesOrder",
                    Action = "ComboBoxItemEdit",
                };
                p.CallbackPageSize = 15;
                p.ClientSideEvents.Init = "ItemComboBox_Init";
                p.ClientSideEvents.BeginCallback = "ItemComboBox_BeginCallback";
                p.ClientSideEvents.EndCallback = "ItemComboBox_EndCallback";
                p.ClientSideEvents.Validation = "ItemComboBox_Validation";
                p.ClientSideEvents.SelectedIndexChanged = "ItemComboBox_SelectedIndexChanged";

                p.ValidationSettings.RequiredField.IsRequired = true;
                p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
                p.ValidationSettings.CausesValidation = true;
                p.ValidationSettings.ValidateOnLeave = true;
                p.ValidationSettings.SetFocusOnError = true;
                p.ValidationSettings.ErrorText = "Valor Incorrecto";

                p.ValidationSettings.EnableCustomValidation = true;
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.BindList(query);

            });

        }

        #endregion

        [HttpPost]
        public JsonResult Save(string jsonSalesOrder)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var salesOrderDTO = GetSalesOrderDTO();


                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonSalesOrder);

                        //var id_machineProdOpening = salesOrderDTO.id_machineProdOpening;
                        //var id_machineProdOpening = token.Value<int>("id_machineProdOpening");
                        //                  var machineProdOpening = db.MachineProdOpening.FirstOrDefault(r =>
                        //	r.id == id_machineProdOpening);
                        //if (machineProdOpening == null)
                        //	throw new Exception("Apertura de Turno no encontrada");

                        var newObject = false;
                        var id = token.Value<int>("id");

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(salesOrderDTO.code_documentType));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var salesOrder = db.SalesOrder.FirstOrDefault(d => d.id == id);
                        if (salesOrder == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            var _numberSequential = salesOrderDTO.proformaOrder.Split('-');
                            var _number = _numberSequential.Length > 2
                                ? _numberSequential[2] : string.Empty;

                            var modPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("SECOP"));

                            salesOrder = new SalesOrder
                            {
                                Document = new Document
                                {
                                    number = (modPrecio.value == "SI" && documentType.code == "140") ? salesOrderDTO.proformaOrder : GetDocumentNumber(documentType?.id ?? 0),
                                    sequential = (modPrecio.value == "SI" && documentType.code == "140") ? Int32.Parse(_number) : GetDocumentSequential(documentType?.id ?? 0),
                                    emissionDate = DateTime.Now,
                                    authorizationDate = DateTime.Now,
                                    id_emissionPoint = id_emissionPoint,
                                    id_documentType = documentType.id,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                }
                            };
                            //salesOrder.id_turn = salesOrderDTO.id_turn;
                            //salesOrder.id_machineForProd = salesOrderDTO.id_machineForProd;
                            //salesOrder.emissionDate = salesOrderDTO.dateTimeEmision;
                            //salesOrder.id_userCreate = salesOrder.Document.id_userCreate;
                            //salesOrder.id_userUpdate = salesOrder.Document.id_userCreate;

                            documentType.currentNumber++;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;



                        }

                        salesOrder.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
                        salesOrder.Document.reference = token.Value<string>("reference");
                        salesOrder.Document.description = token.Value<string>("description");
                        salesOrder.Document.id_documentState = documentState.id;
                        salesOrder.Document.id_userUpdate = ActiveUser.id;
                        salesOrder.Document.dateUpdate = DateTime.Now;
                        salesOrder.id_customer = token.Value<int>("id_customer");
                        salesOrder.id_employeeSeller = salesOrderDTO.id_seller;
                        salesOrder.requiredLogistic = token.Value<bool>("requiredLogistic");
                        salesOrder.id_PaymentMethod = salesOrderDTO.id_paymentMethod;
                        salesOrder.dateShipment = token.Value<DateTime>("dateShipment");
                        salesOrder.id_portDestination = token.Value<int>("id_portDestination");
                        salesOrder.id_portDischarge = token.Value<int?>("id_portDischarge");
                        salesOrder.id_employeeApplicant = token.Value<int>("id_employeeApplicant");
                        salesOrder.id_orderReason = token.Value<int?>("id_orderReason");
                        salesOrder.numeroLote = token.Value<string>("numeroLote");
                        salesOrder.id_provider = token.Value<int?>("id_provider");
                        salesOrder.additionalInstructions = token.Value<string>("additionalInstructions");
                        salesOrder.shippingDocument = token.Value<string>("shippingDocument");

                        //Details
                        if (salesOrderDTO.SalesOrderDetails.Count() <= 0)
                        {
                            throw new Exception("No se puede guardar una Orden de Producción. Sin detalle.");
                        }
                        var lastDetails = db.SalesOrderDetail.Where(d => d.id_salesOrder == salesOrder.id).ToList();
                        //foreach (var detail in lastDetails)
                        //{
                        //    db.SalesOrderDetail.Remove(detail);
                        //    db.SalesOrderDetail.Attach(detail);
                        //    db.Entry(detail).State = EntityState.Deleted;

                        //    if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderForeignCustomers)
                        //    {
                        //        var aSalesOrderDetailSalesQuotationExterior = db.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(d => d.id_salesOrderDetail == detail.id);
                        //        db.SalesOrderDetailSalesQuotationExterior.Remove(aSalesOrderDetailSalesQuotationExterior);
                        //        db.SalesOrderDetailSalesQuotationExterior.Attach(aSalesOrderDetailSalesQuotationExterior);
                        //        db.Entry(aSalesOrderDetailSalesQuotationExterior).State = EntityState.Deleted;
                        //    }
                        //    if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderLocalClient)
                        //    {
                        //        var aSalesOrderDetailSalesRequest = db.SalesOrderDetailSalesRequest.FirstOrDefault(d => d.id_salesOrderDetail == detail.id);
                        //        db.SalesOrderDetailSalesRequest.Remove(aSalesOrderDetailSalesRequest);
                        //        db.SalesOrderDetailSalesRequest.Attach(aSalesOrderDetailSalesRequest);
                        //        db.Entry(aSalesOrderDetailSalesRequest).State = EntityState.Deleted;
                        //    }


                        //}

                        for (int i = lastDetails.Count() - 1; i >= 0; i--)
                        {
                            var detail = lastDetails.ElementAt(i);

                            for (int j = detail.SalesOrderDetailMPMaterialDetail.Count - 1; j >= 0; j--)
                            {
                                var detailSalesOrderDetailMPMaterialDetail = detail.SalesOrderDetailMPMaterialDetail.ElementAt(j);
                                detail.SalesOrderDetailMPMaterialDetail.Remove(detailSalesOrderDetailMPMaterialDetail);
                                db.Entry(detailSalesOrderDetailMPMaterialDetail).State = EntityState.Deleted;
                            }

                            db.SalesOrderDetail.Remove(detail);
                            db.SalesOrderDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;

                            if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderForeignCustomers)
                            {
                                var aSalesOrderDetailSalesQuotationExterior = db.SalesOrderDetailSalesQuotationExterior.FirstOrDefault(d => d.id_salesOrderDetail == detail.id);
                                db.SalesOrderDetailSalesQuotationExterior.Remove(aSalesOrderDetailSalesQuotationExterior);
                                db.SalesOrderDetailSalesQuotationExterior.Attach(aSalesOrderDetailSalesQuotationExterior);
                                db.Entry(aSalesOrderDetailSalesQuotationExterior).State = EntityState.Deleted;
                            }
                            if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderLocalClient)
                            {
                                var aSalesOrderDetailSalesRequest = db.SalesOrderDetailSalesRequest.FirstOrDefault(d => d.id_salesOrderDetail == detail.id);
                                db.SalesOrderDetailSalesRequest.Remove(aSalesOrderDetailSalesRequest);
                                db.SalesOrderDetailSalesRequest.Attach(aSalesOrderDetailSalesRequest);
                                db.Entry(aSalesOrderDetailSalesRequest).State = EntityState.Deleted;
                            }
                        }

                        var lastDetailsMPMaterial = db.SalesOrderMPMaterialDetail.Where(d => d.id_salesOrder == salesOrder.id).ToList();
                        for (int i = lastDetailsMPMaterial.Count() - 1; i >= 0; i--)
                        {
                            var detail = lastDetailsMPMaterial.ElementAt(i);

                            for (int j = detail.SalesOrderDetailMPMaterialDetail.Count - 1; j >= 0; j--)
                            {
                                var detailSalesOrderDetailMPMaterialDetail = detail.SalesOrderDetailMPMaterialDetail.ElementAt(j);
                                detail.SalesOrderDetailMPMaterialDetail.Remove(detailSalesOrderDetailMPMaterialDetail);
                                db.Entry(detailSalesOrderDetailMPMaterialDetail).State = EntityState.Deleted;
                            }

                            db.SalesOrderMPMaterialDetail.Remove(detail);
                            db.SalesOrderMPMaterialDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }


                        //var newDetails = token.Value<JArray>("salesOrderDetails");
                        foreach (var detail in salesOrderDTO.SalesOrderDetails)
                        {
                            var aSalesOrderDetail = new SalesOrderDetail
                            {
                                id_salesOrder = salesOrder.id,
                                id_item = detail.id_item,
                                quantityTypeUMSale = detail.quantityApproved,//detail.cartons,
                                quantityRequested = detail.quantityProgrammed,
                                quantityOrdered = 0.00M,
                                quantityApproved = detail.quantityApproved,
                                quantityDelivered = detail.quantityProduced,
                                id_metricUnitTypeUMPresentation = db.Item.FirstOrDefault(fod => fod.id == detail.id_item).Presentation.id_metricUnit,
                                price = 0.00M,
                                discount = 0.00M,
                                subtotal = 0.00M,
                                total = detail.originQuantity,
                                isActive = true,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now
                            };
                            if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderForeignCustomers)
                            {
                                aSalesOrderDetail.SalesOrderDetailSalesQuotationExterior = new List<SalesOrderDetailSalesQuotationExterior>();
                                var aInvoiceDetail = db.InvoiceDetail.FirstOrDefault(fod => fod.id == detail.id_invoiceDetail);
                                aSalesOrderDetail.SalesOrderDetailSalesQuotationExterior.Add(new SalesOrderDetailSalesQuotationExterior
                                {
                                    id_invoiceDetail = detail.id_invoiceDetail,
                                    id_salesQuotationExterior = aInvoiceDetail.id_invoice,
                                    id_salesOrderDetail = aSalesOrderDetail.id,
                                    quantity = detail.cartons
                                });
                            }
                            if (salesOrderDTO.code_documentType == m_TipoDocumentoProductionOrderLocalClient)
                            {
                                aSalesOrderDetail.SalesOrderDetailSalesRequest = new List<SalesOrderDetailSalesRequest>();
                                var aSalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(fod => fod.id == detail.id_salesRequestDetail);
                                aSalesOrderDetail.SalesOrderDetailSalesRequest.Add(new SalesOrderDetailSalesRequest
                                {
                                    id_salesRequest = aSalesRequestDetail.id_salesRequest,
                                    id_salesRequestDetail = detail.id_salesRequestDetail,
                                    id_salesOrderDetail = aSalesOrderDetail.id,
                                    quantity = detail.cartons
                                });
                            }

                            foreach (var detailSalesOrderDetailMPMaterialDetails in detail.SalesOrderDetailMPMaterialDetails)
                            {
                                var newDetailSalesOrderMPMaterialDetail = salesOrder.SalesOrderMPMaterialDetail.FirstOrDefault(d => d.id == detailSalesOrderDetailMPMaterialDetails.id_salesOrderMPMaterialDetail);
                                if (newDetailSalesOrderMPMaterialDetail == null)
                                {
                                    var aSalesOrderMPMaterialDetails = salesOrderDTO.SalesOrderMPMaterialDetails.FirstOrDefault(fod => fod.id == detailSalesOrderDetailMPMaterialDetails.id_salesOrderMPMaterialDetail);
                                    newDetailSalesOrderMPMaterialDetail = new SalesOrderMPMaterialDetail
                                    {
                                        id = aSalesOrderMPMaterialDetails.id,
                                        id_salesOrder = salesOrder.id,
                                        id_item = aSalesOrderMPMaterialDetails.id_item,
                                        Item = db.Item.FirstOrDefault(fod => fod.id == aSalesOrderMPMaterialDetails.id_item),
                                        quantity = aSalesOrderMPMaterialDetails.quantity,
                                        quantityRequiredForFormulation = aSalesOrderMPMaterialDetails.quantityRequiredForFormulation,
                                        id_metricUnit = aSalesOrderMPMaterialDetails.id_metricUnit.Value,
                                        isActive = true,
                                        manual = aSalesOrderMPMaterialDetails.manual,
                                        id_userCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = ActiveUser.id
                                    };
                                    newDetailSalesOrderMPMaterialDetail.dateUpdate = newDetailSalesOrderMPMaterialDetail.dateCreate;


                                    salesOrder.SalesOrderMPMaterialDetail.Add(newDetailSalesOrderMPMaterialDetail);
                                }

                                var newSalesOrderDetailMPMaterialDetail = new SalesOrderDetailMPMaterialDetail
                                {
                                    id_salesOrderDetail = aSalesOrderDetail.id,
                                    SalesOrderDetail = aSalesOrderDetail,
                                    id_salesOrderMPMaterialDetail = newDetailSalesOrderMPMaterialDetail.id,
                                    SalesOrderMPMaterialDetail = newDetailSalesOrderMPMaterialDetail,
                                    quantity = detailSalesOrderDetailMPMaterialDetails.quantity
                                };
                                aSalesOrderDetail.SalesOrderDetailMPMaterialDetail.Add(newSalesOrderDetailMPMaterialDetail);
                                newDetailSalesOrderMPMaterialDetail.SalesOrderDetailMPMaterialDetail.Add(newSalesOrderDetailMPMaterialDetail);

                            }
                            salesOrder.SalesOrderDetail.Add(aSalesOrderDetail);
                        }

                       //Agregar los MP & Materiales que sean manuales
                        //var aSalesOrderMPMaterialDetailsManual = salesOrderDTO.SalesOrderMPMaterialDetails.Where(fod => fod.codProducts == "" || fod.codProducts == null);
                        //foreach (var detailASalesOrderMPMaterialDetailsManual in aSalesOrderMPMaterialDetailsManual)
                        //{
                        //    var newDetailSalesOrderMPMaterialDetailManual = new SalesOrderMPMaterialDetail
                        //    {
                        //        id = detailASalesOrderMPMaterialDetailsManual.id,
                        //        id_salesOrder = salesOrder.id,
                        //        id_item = detailASalesOrderMPMaterialDetailsManual.id_item,
                        //        Item = db.Item.FirstOrDefault(fod => fod.id == detailASalesOrderMPMaterialDetailsManual.id_item),
                        //        quantity = detailASalesOrderMPMaterialDetailsManual.quantity,
                        //        quantityRequiredForFormulation = detailASalesOrderMPMaterialDetailsManual.quantityRequiredForFormulation,
                        //        id_metricUnit = detailASalesOrderMPMaterialDetailsManual.id_metricUnit.Value,
                        //        isActive = true,
                        //        manual = detailASalesOrderMPMaterialDetailsManual.manual,
                        //        id_userCreate = ActiveUser.id,
                        //        dateCreate = DateTime.Now,
                        //        id_userUpdate = ActiveUser.id
                        //    };
                        //    newDetailSalesOrderMPMaterialDetailManual.dateUpdate = newDetailSalesOrderMPMaterialDetailManual.dateCreate;


                        //    salesOrder.SalesOrderMPMaterialDetail.Add(newDetailSalesOrderMPMaterialDetailManual);
                        //}

                        if (newObject)
                        {
                            db.SalesOrder.Add(salesOrder);
                            db.Entry(salesOrder).State = EntityState.Added;
                        }
                        else
                        {
                            db.SalesOrder.Attach(salesOrder);
                            db.Entry(salesOrder).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = salesOrder.id.ToString();

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
        public JsonResult Close(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = CloseSalesOrder(id).name;
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

        private DocumentState CloseSalesOrder(int id_salesOrder)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var salesOrder = db.SalesOrder.FirstOrDefault(p => p.id == id_salesOrder);
                    if (salesOrder != null)
                    {

                        //#endregion

                        var closedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("04"));
                        if (closedState == null)
                            return null;

                        salesOrder.Document.id_documentState = closedState.id;
                        //salesOrder.Document.authorizationDate = DateTime.Now;

                        db.SalesOrder.Attach(salesOrder);
                        db.Entry(salesOrder).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return salesOrder.Document.DocumentState;
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
                        result.Data = ApproveSalesOrder(id).name;
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

        private DocumentState ApproveSalesOrder(int id_salesOrder)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var salesOrder = db.SalesOrder.FirstOrDefault(p => p.id == id_salesOrder);
                    if (salesOrder != null)
                    {
                        #region Validación Permiso

                        int id_user = (int)ViewData["id_user"];
                        int id_menu = (int)ViewData["id_menu"];

                        User user = DataProviderUser.UserById(id_user);
                        UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                        if (userMenu != null)
                        {
                            Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Aprobar");
                            if (permission == null)
                            {
                                throw new Exception("No tiene Permiso para Aprobar la Orden de Producción");
                            }
                        }

                        #endregion

                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return null;

                        salesOrder.Document.id_documentState = aprovedState.id;
                        //salesOrder.Document.authorizationDate = DateTime.Now;
                        salesOrder.Document.id_userUpdate = ActiveUser.id;
                        salesOrder.Document.dateUpdate = DateTime.Now;

                        db.SalesOrder.Attach(salesOrder);
                        db.Entry(salesOrder).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return salesOrder.Document.DocumentState;
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
                        result.Data = ReverseSalesOrder(id).name;
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

        private DocumentState ReverseSalesOrder(int id_salesOrder)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var salesOrder = db.SalesOrder.FirstOrDefault(p => p.id == id_salesOrder);
                    if (salesOrder != null)
                    {
                        //#region Validación Permiso

                        //var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        //if (entityObjectPermissions != null)
                        //{
                        //    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                        //    if (entityPermissions != null)
                        //    {
                        //        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == salesOrder.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                        //        if (entityValuePermissions == null)
                        //        {
                        //            throw new Exception("No tiene Permiso para reversar el control de horas de trabajo por máquina.");
                        //        }
                        //    }
                        //}

                        //#endregion
                        #region Validación Permiso por usuario
                        int id_user = (int)ViewData["id_user"];
                        int id_menu = (int)ViewData["id_menu"];

                        User user = DataProviderUser.UserById(id_user);
                        UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                        if (userMenu != null)
                        {
                            Permission permissionReversar = userMenu.Permission.FirstOrDefault(p => p.name == "Reversar");
                            if (permissionReversar == null)
                            {
                                throw new Exception("No tiene Permiso para Reversar la Orden de Producción");
                            }
                        }
                        #endregion
                        DocumentState reverseState = null;
                        if (salesOrder.Document.DocumentState.code == "03")
                        {
                            reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        }
                        else
                        {
                            if (salesOrder.Document.DocumentState.code == "04")
                            {
                                reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            }
                        }
                        if (reverseState == null)
                            return

                        salesOrder.Document.DocumentState = reverseState;
                        salesOrder.Document.id_documentState = reverseState.id;
                        //salesOrder.Document.authorizationDate = DateTime.Now;

                        db.SalesOrder.Attach(salesOrder);
                        db.Entry(salesOrder).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return salesOrder.Document.DocumentState;
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
                        result.Data = AnnulSalesOrder(id).name;
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

        private DocumentState AnnulSalesOrder(int id_salesOrder)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var salesOrder = db.SalesOrder.FirstOrDefault(p => p.id == id_salesOrder);
                    if (salesOrder != null)
                    {
                        //#region Validación Permiso

                        //var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        //if (entityObjectPermissions != null)
                        //{
                        //    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                        //    if (entityPermissions != null)
                        //    {
                        //        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == salesOrder.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                        //        if (entityValuePermissions == null)
                        //        {
                        //            throw new Exception("No tiene Permiso para anular el control de horas de trabajo por máquina.");
                        //        }
                        //    }
                        //}

                        //#endregion

                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return

                        salesOrder.Document.DocumentState;
                        salesOrder.Document.id_documentState = annulState.id;
                        salesOrder.Document.authorizationDate = DateTime.Now;

                        db.SalesOrder.Attach(salesOrder);
                        db.Entry(salesOrder).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return salesOrder.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetSalesOrderResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetSalesOrderResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetSalesOrderResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var salesOrder = db.SalesOrder.FirstOrDefault(d => d.id == element.id);
            if (salesOrder == null)
                return PartialView("Edit", new SalesOrderDTO());

            BuildViewDataEdit();
            var model = ConvertToDto(salesOrder);
            SetSalesOrderDTO(model);
            //var codeTypeSalesOrder = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeSalesOrder)?.code;
            //BuildComboBoxSizeSalesOrder(codeTypeSalesOrder);
            BuilViewBag(false);
            //BuildComboBoxTypeSalesOrder(model.id, model.codeTypeProcess);

            return PartialView("Edit", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTotales()
        {
            var salesOrder = GetSalesOrderDTO();

            var result = new
            {
                message = "OK",
                salesOrder.totalCM,
                salesOrder.netoLbs,
                salesOrder.netoKg
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeat(int? id_itemNew)
        {
            var salesOrder = GetSalesOrderDTO();
            var result = new
            {
                itsRepeat = 0,
                Error = ""

            };

            var salesOrderDetailAux = salesOrder.SalesOrderDetails.FirstOrDefault(fod => fod.id_item == id_itemNew);
            if (salesOrderDetailAux != null)
            {
                result = new
                {
                    itsRepeat = 1,
                    Error = "- Nombre del Producto: " + salesOrderDetailAux.name_item + " ya esta incluido en un detalle ya registrado."
                };

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetItem(int? id_item, decimal? quantityApproved)
        {
            var quantityApprovedAux = quantityApproved == null ? 0.00M : quantityApproved.Value;
            var result = new
            {
                description_item = "",
                cod_item = "",
                noRequestProforma = "",
                quantityProgrammed = 0.00M,
                quantityApproved = quantityApprovedAux,
            };

            var aSalesOrderDTO = GetSalesOrderDTO();

            if (!string.IsNullOrEmpty(aSalesOrderDTO.proformaOrder))
            {
                var aId_documentOrigen = db.Document.FirstOrDefault(fod => fod.Invoice != null && fod.Invoice.SalesQuotationExterior != null && fod.number == aSalesOrderDTO.proformaOrder)?.id;
                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                var aInvoiceDetail = proforma.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == id_item);
                if (aInvoiceDetail != null)
                {
                    var aNumBoxes = (decimal)aInvoiceDetail.numBoxes.Value;
                    result = new
                    {
                        description_item = aInvoiceDetail.Item.description,
                        cod_item = aInvoiceDetail.Item.masterCode,
                        noRequestProforma = proforma.Invoice.Document.number,
                        quantityProgrammed = aNumBoxes,
                        quantityApproved = quantityApprovedAux > aNumBoxes ? quantityApprovedAux : aNumBoxes,
                    };
                }
            }
            else
            {
                var aItem = db.Item.FirstOrDefault(fod => fod.id == id_item);
                if (aItem != null)
                {
                    result = new
                    {
                        description_item = aItem.description,
                        cod_item = aItem.masterCode,
                        noRequestProforma = "",
                        quantityProgrammed = 0.00M,
                        quantityApproved = quantityApprovedAux
                        //codAux_item = aItem.auxCode,
                        //originQuantityStr = Math.Round((cartonsAux * aItem.Presentation.maximum * aItem.Presentation.minimum), 2).ToString() + " " + aItem.Presentation.MetricUnit.code,
                    };
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReport(int id, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id
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


        //        [HttpPost]
        //        public ActionResult Index()
        //        {
        //            return PartialView();
        //        }

        //        #region SALES ORDER FILTERS RESULTS

        //        [HttpPost]
        //        public ActionResult SalesOrderResults(SalesOrder salesOrder,
        //                                                 Document document,
        //                                                 DateTime? startEmissionDate, DateTime? endEmissionDate,
        //                                                 DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
        //                                                 int[] items)
        //        {
        //            List<SalesOrder> model = db.SalesOrder.AsEnumerable().ToList();

        //            #region DOCUMENT FILTERS

        //            if (document.id_documentState != 0)
        //            {
        //                model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
        //            }

        //            if (!string.IsNullOrEmpty(document.number))
        //            {
        //                model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
        //            }

        //            if (!string.IsNullOrEmpty(document.reference))
        //            {
        //                model = model.Where(o => o.Document.reference.Contains(document.reference)).ToList();
        //            }

        //            if (startEmissionDate != null)
        //            {
        //                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
        //            }

        //            if (endEmissionDate != null)
        //            {
        //                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
        //            }
        //            //if (startEmissionDate != null && endEmissionDate != null)
        //            //{
        //            //    model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
        //            //}

        //            if (startAuthorizationDate != null)
        //            {
        //                model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0).ToList();
        //            }

        //            if (endAuthorizationDate != null)
        //            {
        //                model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
        //            }

        //            //if (startAuthorizationDate != null && endAuthorizationDate != null)
        //            //{
        //            //    model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0 && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
        //            //}

        //            if (!string.IsNullOrEmpty(document.accessKey))
        //            {
        //                model = model.Where(o => o.Document.accessKey.Contains(document.accessKey)).ToList();
        //            }

        //            if (!string.IsNullOrEmpty(document.authorizationNumber))
        //            {
        //                model = model.Where(o => o.Document.authorizationNumber.Contains(document.authorizationNumber)).ToList();
        //            }

        //            #endregion

        //            #region SALES ORDER FILTERS

        //            if (salesOrder.id_customer != null && salesOrder.id_customer != 0)
        //            {
        //                model = model.Where(o => o.id_customer == salesOrder.id_customer).ToList();
        //            }

        //            if (salesOrder.id_employeeSeller != null && salesOrder.id_employeeSeller != 0)
        //            {
        //                model = model.Where(o => o.id_employeeSeller == salesOrder.id_employeeSeller).ToList();
        //            }

        //            if (items != null && items.Length > 0)
        //            {
        //                var tempModel = new List<SalesOrder>();
        //                foreach (var order in model)
        //                {
        //                    var details = order.SalesOrderDetail.Where(d => items.Contains(d.id_item));
        //                    if (details.Any())
        //                    {
        //                        tempModel.Add(order);
        //                    }
        //                }

        //                model = tempModel;
        //            }

        //            //if (salesOrder.id_priceList != 0)
        //            //{
        //            //    model = model.Where(o => o.id_priceList == salesOrder.id_priceList).ToList();
        //            //}

        //            //if (salesOrder.id_paymentTerm != 0)
        //            //{
        //            //    model = model.Where(o => o.id_paymentTerm == salesOrder.id_paymentTerm).ToList();
        //            //}

        //            //if (salesOrder.id_paymentMethod != 0)
        //            //{
        //            //    model = model.Where(o => o.id_paymentMethod == salesOrder.id_paymentMethod).ToList();
        //            //}

        //            model = model.Where(o => o.requiredLogistic == salesOrder.requiredLogistic).ToList();

        //            #endregion

        //            TempData["model"] = model;
        //            TempData.Keep("model");

        //            return PartialView("_SalesOrderResultsPartial", model.OrderByDescending(o => o.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesRequestDetailsResults()
        //        {
        //            //List<SalesRequestDetail> model = db.SalesRequestDetail.AsEnumerable().Where(d =>
        //            //            (d.SalesRequest.Document.DocumentState.id == 2 ||
        //            //             d.SalesRequest.Document.DocumentState.id == 3 ||
        //            //             d.SalesRequest.Document.DocumentState.id == 6) &&
        //            //             d.quantityApproved > 0).OrderByDescending(d => d.id_salesRequest).ToList().OrderByDescending(d => d.id_item).ToList();
        //            var model = db.SalesRequestDetail.Where(d => d.SalesRequest.Document.DocumentState.code == "06" && d.quantityOutstandingOrder > 0)//"06" AUTORIZADA
        //                         .OrderByDescending(d => d.id_salesRequest).ToList();
        //            return PartialView("_SalesRequestDetailsResultsPartial", model);
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesRequestDetailsPartial()
        //        {
        //            //List<SalesRequestDetail> model = db.SalesRequestDetail.AsEnumerable().Where(d =>
        //            //            (d.SalesRequest.Document.DocumentState.id == 2 ||
        //            //             d.SalesRequest.Document.DocumentState.id == 3 ||
        //            //             d.SalesRequest.Document.DocumentState.id == 6) &&
        //            //             d.quantityApproved > 0).OrderByDescending(d => d.id_salesRequest).ToList().OrderByDescending(d => d.id_item).ToList();
        //            var model = db.SalesRequestDetail.Where(d => d.SalesRequest.Document.DocumentState.code == "06" && d.quantityOutstandingOrder > 0)//"06" AUTORIZADA
        //                         .OrderByDescending(d => d.id_salesRequest).ToList();
        //            return PartialView("_SalesRequestDetailsPartial", model);
        //        }

        //        #endregion

        //        #region SALES ORDER MASTER DETAILS

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrderResultsDetailViewDetailsPartial()
        //        {
        //            int id_salesOrder = (Request.Params["id_salesOrder"] != null && Request.Params["id_salesOrder"] != "") ? int.Parse(Request.Params["id_salesOrder"]) : -1;
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id_salesOrder);
        //            return PartialView("_SalesOrderResultsDetailViewDetailsPartial", salesOrder.SalesOrderDetail.ToList());
        //        }

        //        #endregion

        //        #region SALES ORDER EDITFORM

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult FormEditSalesOrder(int id, int[] requestDetails)//SalesRequestDetail[] requestDetails)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(o => o.id == id);

        //            if (salesOrder == null)
        //            {
        //                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("11"));//Orden de Producción
        //                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//01: Estado Pendiente
        //                    //DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

        //                //Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id);
        //                Employee employee = ActiveUser.Employee;
        //                Department department = employee?.Department;

        //                salesOrder = new SalesOrder
        //                {
        //                    Document = new Document
        //                    {
        //                        id = 0,
        //                        id_documentType = documentType?.id ?? 0,
        //                        DocumentType = documentType,
        //                        id_documentState = documentState?.id ?? 0,
        //                        DocumentState = documentState,
        //                        emissionDate = DateTime.Now
        //                    },
        //                    //id_personRequesting = employee?.id ?? 0,
        //                    id_employeeSeller = employee?.id ?? 0,
        //                    Employee = employee,
        //                    Customer = null,
        //                    id_customer = null,
        //                    PriceList = null,
        //                    id_priceList = null,
        //                    SalesOrderTotal = new SalesOrderTotal(),
        //                    SalesOrderDetail = new List<SalesOrderDetail>()

        //                };
        //            }

        //            if (requestDetails != null)
        //            {
        //                foreach (var id_salesRequestDetail in requestDetails)
        //                {
        //                    SalesRequestDetail
        //                    requestDetail =
        //                        db.SalesRequestDetail.FirstOrDefault(
        //                            d =>
        //                              d.id == id_salesRequestDetail);
        //                    //d.id_salesRequest == salesRequestDetail.id_salesRequest &&
        //                    //d.id_item == salesRequestDetail.id_item);

        //                    if (requestDetail != null)
        //                    {
        //                        if (salesOrder.id_customer == null)
        //                        {
        //                            salesOrder.id_customer = requestDetail.SalesRequest.id_customer;
        //                            salesOrder.Customer = requestDetail.SalesRequest.Customer;
        //                            salesOrder.id_priceList = requestDetail.SalesRequest.id_priceList;
        //                            salesOrder.PriceList = requestDetail.SalesRequest.PriceList;
        //                        }

        //                        decimal price = requestDetail.price;


        //                        var metricUnitPresentation = requestDetail.Item.Presentation?.MetricUnit;

        //                        var metricUnitAux = requestDetail.MetricUnit;

        //                        var factorConversionAux = GetFactorConversion(metricUnitPresentation, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitPresentation.code + " y " + (metricUnitAux.code) + ".Necesario para la cantidad de Venta del detalle de la Orden de Producción Configúrelo, e intente de nuevo", db);

        //                        var quantitySalesAux = (requestDetail.quantityOutstandingOrder) / ((requestDetail.Item.Presentation?.minimum * factorConversionAux) ?? 1);
        //                        quantitySalesAux = TruncateDecimal(quantitySalesAux);
        //                        var quantityTypeUMPresentation = quantitySalesAux * ((requestDetail.Item.Presentation?.minimum * factorConversionAux) ?? 1);

        //                        decimal iva = SalesOrderDetailIVA(requestDetail.id_item, quantityTypeUMPresentation, price);

        //                        SalesOrderDetail newSalesOrderDetail = new SalesOrderDetail

        //                        {
        //                            id_item = requestDetail.id_item,
        //                            Item = requestDetail.Item,
        //                            quantityTypeUMSale = quantitySalesAux,
        //                            quantityRequested = requestDetail.quantityApproved,
        //                            quantityOrdered = quantityTypeUMPresentation,
        //                            quantityApproved = quantityTypeUMPresentation,
        //                            quantityDelivered = 0,
        //                            id_metricUnitTypeUMPresentation = requestDetail.id_metricUnitTypeUMPresentation,
        //                            MetricUnit = requestDetail.MetricUnit,

        //                            price = price,
        //                            iva = iva,
        //                            subtotal = price * quantityTypeUMPresentation,
        //                            total = ((price * quantityTypeUMPresentation) + iva) - requestDetail.discount,
        //                            discount = requestDetail.discount,

        //                                  isActive = true,
        //                                    id_userCreate = ActiveUser.id,
        //                                    dateCreate = DateTime.Now,
        //                                    id_userUpdate = ActiveUser.id,
        //                                    dateUpdate = DateTime.Now,

        //                                    SalesOrderDetailSalesRequest = new List<SalesOrderDetailSalesRequest>()

        //                                };

        //                                SalesOrderDetailSalesRequest newSalesOrderDetailSalesRequest = new SalesOrderDetailSalesRequest
        //                                {

        //                                    id_salesRequest = requestDetail.id_salesRequest,
        //                                    SalesRequest = requestDetail.SalesRequest,
        //                                    id_salesRequestDetail = requestDetail.id,
        //                                    SalesRequestDetail = requestDetail,
        //                                    quantity = quantityTypeUMPresentation
        //                                };
        //                        newSalesOrderDetail.SalesOrderDetailSalesRequest.Add(newSalesOrderDetailSalesRequest);


        //                                salesOrder.SalesOrderDetail.Add(newSalesOrderDetail);


        //                    }
        //                }
        //            }

        //            salesOrder.SalesOrderTotal = SalesOrderTotals(salesOrder.id, salesOrder.SalesOrderDetail.ToList());

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");

        //            return PartialView("_FormEditSalesOrder", salesOrder);
        //        }

        //        #endregion

        //        #region SALES ORDER HEADER

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrdersPartial()
        //        {
        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();

        //            TempData.Keep("model");

        //            return PartialView("_SalesOrdersPartial", model.OrderByDescending(o => o.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrdersPartialAddNew(bool approve, DXPANACEASOFT.Models.SalesOrder itemSO, Document itemDoc/*, SalesOrderImportationInformation importationInformation*/)
        //        {
        //            var model = db.SalesOrder;

        //            SalesOrder tempSalesOrder = (TempData["salesOrder"] as SalesOrder);

        //            tempSalesOrder = tempSalesOrder ?? new SalesOrder();

        //    tempSalesOrder.id_customer = itemSO.id_customer;
        //    tempSalesOrder.id_employeeSeller = itemSO.id_employeeSeller;
        //    tempSalesOrder.id_priceList = itemSO.id_priceList;
        //    tempSalesOrder.Document = itemDoc;
        //    tempSalesOrder.requiredLogistic = itemSO.requiredLogistic;

        //    using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    #region Document

        //                    itemDoc.id_userCreate = ActiveUser.id;
        //                    itemDoc.dateCreate = DateTime.Now;
        //                    itemDoc.id_userUpdate = ActiveUser.id;
        //                    itemDoc.dateUpdate = DateTime.Now;
        //                    itemDoc.sequential = GetDocumentSequential(itemDoc.id_documentType);
        //                    itemDoc.number = GetDocumentNumber(itemDoc.id_documentType);

        //                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == itemDoc.id_documentType);
        //                    itemDoc.DocumentType = documentType;

        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
        //                    itemDoc.DocumentState = documentState;

        //                    Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id);
        //                    itemSO.Employee = employee;
        //                    tempSalesOrder.Employee = employee;

        //                    itemDoc.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
        //                    itemDoc.id_emissionPoint = ActiveEmissionPoint.id;

        //                    string emissionDate = itemDoc.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

        //                    itemDoc.accessKey = AccessKey.GenerateAccessKey(emissionDate,
        //                                                                    itemDoc.DocumentType.code,
        //                                                                    itemDoc.EmissionPoint.BranchOffice.Division.Company.ruc,
        //                                                                    "1",
        //                                                                    itemDoc.EmissionPoint.BranchOffice.code.ToString() + itemDoc.EmissionPoint.code.ToString("D3"),
        //                                                                    itemDoc.sequential.ToString("D9"),
        //                                                                    itemDoc.sequential.ToString("D8"),
        //                                                                    "1");

        //                    //Actualiza Secuencial
        //                    if (documentType != null)
        //                    {
        //                        documentType.currentNumber = documentType.currentNumber + 1;
        //                        db.DocumentType.Attach(documentType);
        //                        db.Entry(documentType).State = EntityState.Modified;
        //                    }
        //            //itemPR.Employee.id = itemPR.id_personRequesting;


        //            #endregion

        //            #region SalesOrder

        //            itemSO.Document = itemDoc;

        //                    #endregion

        //                    #region SalesOrderDetails

        //                    if (tempSalesOrder?.SalesOrderDetail != null)
        //                    {
        //                itemSO.SalesOrderDetail = new List<SalesOrderDetail>();
        //                        var itemPODetails = tempSalesOrder.SalesOrderDetail.ToList();

        //                        foreach (var detail in itemPODetails)
        //                        {
        //                    var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
        //                    if (approve)
        //                    {
        //                        if (detail.price <= 0)
        //                        {
        //                            throw new Exception("No se puede aprobar la Orden de Producción, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
        //                        }
        //                        if (detail.quantityApproved <= 0)
        //                        {
        //                            throw new Exception("No se puede aprobar la Orden de Producción, Ítem: " + itemAux.name + " debe tener la cantidad aprobada mayor que cero.");
        //                        }
        //                    }

        //                    var tempItemSODetail = new SalesOrderDetail
        //                            {
        //                                id_item = detail.id_item,
        //                                Item = itemAux,

        //                                quantityRequested = detail.quantityRequested,
        //                                quantityOrdered = detail.quantityOrdered,
        //                                quantityApproved = detail.quantityApproved,
        //                                quantityDelivered = detail.quantityDelivered,
        //                        quantityTypeUMSale = detail.quantityTypeUMSale,
        //                        id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
        //                        MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),

        //                        price = detail.price,
        //                        discount = detail.discount,
        //                        iva = detail.iva,

        //                                subtotal = detail.subtotal,
        //                                total = detail.total,

        //                                isActive = detail.isActive,
        //                                id_userCreate = detail.id_userCreate,
        //                                dateCreate = detail.dateCreate,
        //                                id_userUpdate = detail.id_userUpdate,
        //                                dateUpdate = detail.dateUpdate,

        //                                SalesOrderDetailSalesRequest = new List<SalesOrderDetailSalesRequest>(),
        //                        ProductionScheduleProductionOrderDetailSalesOrderDetail = new List<ProductionScheduleProductionOrderDetailSalesOrderDetail>()
        //                    };

        //                            foreach (var requestDetail in detail.SalesOrderDetailSalesRequest)
        //                            {
        //                        tempItemSODetail.SalesOrderDetailSalesRequest.Add(new SalesOrderDetailSalesRequest
        //                                {
        //                            id_salesOrderDetail = detail.id,
        //                            SalesOrderDetail = tempItemSODetail,
        //                            id_salesRequestDetail = requestDetail.id_salesRequestDetail,
        //                            SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
        //                            id_salesRequest = requestDetail.id_salesRequest,
        //                            SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
        //                            quantity = detail.quantityApproved
        //                        });
        //                            }

        //                    foreach (var productionOrderDetail in detail.ProductionScheduleProductionOrderDetailSalesOrderDetail)
        //                    {
        //                        tempItemSODetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Add(new ProductionScheduleProductionOrderDetailSalesOrderDetail
        //                        {
        //                            id_salesOrderDetail = detail.id,
        //                            SalesOrderDetail = tempItemSODetail,
        //                            id_productionScheduleProductionOrderDetail = productionOrderDetail.id_productionScheduleProductionOrderDetail,
        //                            ProductionScheduleProductionOrderDetail = db.ProductionScheduleProductionOrderDetail.FirstOrDefault(i => i.id == productionOrderDetail.id_productionScheduleProductionOrderDetail),
        //                            id_productionSchedule = productionOrderDetail.id_productionSchedule,
        //                            ProductionSchedule = db.ProductionSchedule.FirstOrDefault(i => i.id == productionOrderDetail.id_productionSchedule),
        //                            quantity = detail.quantityApproved
        //                        });
        //                    }

        //                    if (tempItemSODetail.isActive)
        //                        itemSO.SalesOrderDetail.Add(tempItemSODetail);
        //                        }
        //                    }

        //            #endregion

        //            //#region IMPORTATION INFORMATION

        //            //if (itemPO.isImportation)
        //            //{
        //            //    importationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
        //            //    importationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
        //            //    importationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);
        //            //    itemPO.SalesOrderImportationInformation = importationInformation;
        //            //}

        //            //#endregion

        //            #region TOTALS

        //            itemSO.SalesOrderTotal = SalesOrderTotals(itemSO.id, itemSO.SalesOrderDetail.ToList());

        //                    #endregion

        //                    if (itemSO.SalesOrderDetail.Count == 0)
        //            {
        //                throw new Exception("No se puede guardar una orden de producción sin detalles");
        //                //TempData.Keep("salesOrder");
        //                //ViewData["EditMessage"] = ErrorMessage("No se puede guardar una orden de venta sin detalles");
        //                //return PartialView("_SalesOrderMainFormPartial", tempSalesOrder);
        //            }

        //            if (approve)
        //            {
        //                foreach (var detail in itemSO.SalesOrderDetail)
        //                {
        //                    foreach (var salesOrderDetailSalesRequest in detail.SalesOrderDetailSalesRequest)
        //                    {
        //                        ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesOrderDetailSalesRequest.id_salesRequestDetail,
        //                                                                       salesOrderDetailSalesRequest.quantity);
        //                    }
        //                }

        //                itemSO.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
        //            }

        //            //Guarda Orden de Venta
        //            db.SalesOrder.Add(itemSO);
        //                    db.SaveChanges();
        //                    trans.Commit();

        //                    TempData["salesOrder"] = itemSO;
        //                    TempData.Keep("salesOrder");

        //                    ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + itemSO.Document.number + " guardada exitosamente");
        //                }
        //                catch (Exception e)
        //                {
        //                   TempData["salesOrder"] = tempSalesOrder;
        //                    TempData.Keep("salesOrder");
        //                    ViewData["EditMessage"] = ErrorMessage(e.Message);
        //                    trans.Rollback();
        //                return PartialView("_SalesOrderMainFormPartial", tempSalesOrder);

        //            //TempData.Keep("salesOrder");
        //            //ViewData["EditError"] = ErrorMessage(e.Message);
        //            //trans.Rollback();
        //        }

        //        return PartialView("_SalesOrderMainFormPartial", itemSO);

        //            }
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrdersPartialUpdate(bool approve, DXPANACEASOFT.Models.SalesOrder itemSO, Document itemDoc/*, SalesOrderImportationInformation importationInformation*/)
        //        {
        //            var model = db.SalesOrder;

        //            SalesOrder modelItem = null;
        //    SalesOrder tempSalesOrder = (TempData["salesOrder"] as SalesOrder);

        //    using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    modelItem = model.FirstOrDefault(it => it.id == itemSO.id);
        //                    if (modelItem != null)
        //                    {
        //                #region UPDATE TEMPORDER

        //                tempSalesOrder.Document.description = itemDoc.description;
        //                tempSalesOrder.Document.reference = itemDoc.reference;
        //                tempSalesOrder.Document.emissionDate = itemDoc.emissionDate;

        //                tempSalesOrder.id_customer = itemSO.id_customer;
        //                tempSalesOrder.id_priceList = itemSO.id_priceList;
        //                tempSalesOrder.requiredLogistic = itemSO.requiredLogistic;

        //                #endregion              

        //                        #region DOCUMENT

        //                        modelItem.Document.description = itemDoc.description;
        //                modelItem.Document.reference = itemDoc.reference;
        //                modelItem.Document.emissionDate = itemDoc.emissionDate;

        //                modelItem.Document.id_userUpdate = ActiveUser.id;
        //                        modelItem.Document.dateUpdate = DateTime.Now;

        //                #endregion

        //                #region SALES ORDER

        //                modelItem.id_customer = itemSO.id_customer;
        //                modelItem.Customer = db.Customer.FirstOrDefault(fod=> fod.id == itemSO.id_customer);
        //                tempSalesOrder.Customer = modelItem.Customer;

        //                modelItem.id_employeeSeller = itemSO.id_employeeSeller;
        //                modelItem.Employee = db.Employee.FirstOrDefault(fod => fod.id == itemSO.id_employeeSeller);
        //                tempSalesOrder.Employee = modelItem.Employee;

        //                modelItem.id_priceList = itemSO.id_priceList;
        //                modelItem.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == itemSO.id_priceList);
        //                tempSalesOrder.PriceList = modelItem.PriceList;

        //                modelItem.requiredLogistic = itemSO.requiredLogistic;

        //                #endregion

        //                #region SALES ORDER DETAILS

        //                int count = 0;
        //                        if (tempSalesOrder?.SalesOrderDetail != null)
        //                        {
        //                            var details = tempSalesOrder.SalesOrderDetail.ToList();

        //                            foreach (var detail in details)
        //                            {
        //                                SalesOrderDetail orderDetail = modelItem.SalesOrderDetail.FirstOrDefault(d => d.id == detail.id);

        //                        var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
        //                        if (approve)
        //                        {
        //                            if (detail.price <= 0)
        //                            {
        //                                throw new Exception("No se puede aprobar la Orden de Producción, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
        //                            }
        //                            if (detail.quantityApproved <= 0)
        //                            {
        //                                throw new Exception("No se puede aprobar la Orden de Producción, Ítem: " + itemAux.name + " debe tener la cantidad aprobada mayor que cero.");
        //                            }
        //                        }

        //                        if (orderDetail == null)
        //                                {
        //                                    orderDetail = new SalesOrderDetail
        //                                    {
        //                                        id_item = detail.id_item,
        //                                        Item = itemAux,

        //                                        quantityRequested = detail.quantityRequested,
        //                                        quantityOrdered = detail.quantityOrdered,
        //                                        quantityApproved = detail.quantityApproved,
        //                                        quantityDelivered = detail.quantityDelivered,
        //                                        quantityTypeUMSale = detail.quantityTypeUMSale,
        //                                        id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
        //                                        MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),

        //                                        price = detail.price,
        //                                        discount = detail.discount,
        //                                        iva = detail.iva,

        //                                        subtotal = detail.subtotal,
        //                                        total = detail.total,

        //                                        isActive = detail.isActive,
        //                                        id_userCreate = detail.id_userCreate,
        //                                        dateCreate = detail.dateCreate,
        //                                        id_userUpdate = detail.id_userUpdate,
        //                                        dateUpdate = detail.dateUpdate,

        //                                        SalesOrderDetailSalesRequest = new List<SalesOrderDetailSalesRequest>(),
        //                                        ProductionScheduleProductionOrderDetailSalesOrderDetail = new List<ProductionScheduleProductionOrderDetailSalesOrderDetail>()
        //                                    };

        //                            foreach (var requestDetail in detail.SalesOrderDetailSalesRequest)
        //                            {
        //                                orderDetail.SalesOrderDetailSalesRequest.Add(new SalesOrderDetailSalesRequest
        //                                {
        //                                    id_salesOrderDetail = detail.id,
        //                                    SalesOrderDetail = orderDetail,
        //                                    id_salesRequestDetail = requestDetail.id_salesRequestDetail,
        //                                    SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
        //                                    id_salesRequest = requestDetail.id_salesRequest,
        //                                    SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
        //                                    quantity = detail.quantityApproved
        //                                });
        //                            }

        //                            foreach (var productionOrderDetail in detail.ProductionScheduleProductionOrderDetailSalesOrderDetail)
        //                            {
        //                                orderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Add(new ProductionScheduleProductionOrderDetailSalesOrderDetail
        //                                {
        //                                    id_salesOrderDetail = detail.id,
        //                                    SalesOrderDetail = orderDetail,
        //                                    id_productionScheduleProductionOrderDetail = productionOrderDetail.id_productionScheduleProductionOrderDetail,
        //                                    ProductionScheduleProductionOrderDetail = db.ProductionScheduleProductionOrderDetail.FirstOrDefault(i => i.id == productionOrderDetail.id_productionScheduleProductionOrderDetail),
        //                                    id_productionSchedule = productionOrderDetail.id_productionSchedule,
        //                                    ProductionSchedule = db.ProductionSchedule.FirstOrDefault(i => i.id == productionOrderDetail.id_productionSchedule),
        //                                    quantity = detail.quantityApproved
        //                                });
        //                            }

        //                            if (orderDetail.isActive)
        //                                    {
        //                                        modelItem.SalesOrderDetail.Add(orderDetail);
        //                                        count++;
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    orderDetail.id_item = detail.id_item;
        //                                    orderDetail.Item = itemAux;

        //                                    orderDetail.quantityRequested = detail.quantityRequested;
        //                                    orderDetail.quantityOrdered = detail.quantityOrdered;
        //                                    orderDetail.quantityApproved = detail.quantityApproved;
        //                                    orderDetail.quantityDelivered = detail.quantityDelivered;
        //                            orderDetail.quantityTypeUMSale = detail.quantityTypeUMSale;
        //                            orderDetail.id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation;
        //                            orderDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation);

        //                            orderDetail.price = detail.price;
        //                            orderDetail.discount = detail.discount;
        //                            orderDetail.iva = detail.iva;

        //                                    orderDetail.subtotal = detail.subtotal;
        //                                    orderDetail.total = detail.total;

        //                                    orderDetail.isActive = detail.isActive;
        //                                    //orderDetail.id_userCreate = detail.id_userCreate;
        //                                    //orderDetail.dateCreate = detail.dateCreate;
        //                                    orderDetail.id_userUpdate = detail.id_userUpdate;
        //                                    orderDetail.dateUpdate = detail.dateUpdate;


        //                            foreach (var requestDetail in detail.SalesOrderDetailSalesRequest)
        //                            {
        //                                SalesOrderDetailSalesRequest tempSalesOrderDetailSalesRequest = orderDetail.SalesOrderDetailSalesRequest.FirstOrDefault(d => d.id == requestDetail.id);
        //                                tempSalesOrderDetailSalesRequest.quantity = detail.quantityApproved;
        //                                //orderDetail.SalesOrderDetailSalesRequest.Add(new SalesOrderDetailSalesRequest
        //                                //{
        //                                //    id_salesOrderDetail = detail.id,
        //                                //    SalesOrderDetail = orderDetail,
        //                                //    id_salesRequestDetail = requestDetail.id_salesRequestDetail,
        //                                //    SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
        //                                //    id_salesRequest = requestDetail.id_salesRequest,
        //                                //    SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
        //                                //    quantity = detail.quantityApproved
        //                                //});
        //                            }

        //                            foreach (var productionOrderDetail in detail.ProductionScheduleProductionOrderDetailSalesOrderDetail)
        //                            {
        //                                ProductionScheduleProductionOrderDetailSalesOrderDetail tempProductionScheduleProductionOrderDetailSalesOrderDetail = orderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.FirstOrDefault(d => d.id == productionOrderDetail.id);
        //                                tempProductionScheduleProductionOrderDetailSalesOrderDetail.quantity = detail.quantityApproved;
        //                                //orderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Add(new ProductionScheduleProductionOrderDetailSalesOrderDetail
        //                                //{
        //                                //    id_salesOrderDetail = detail.id,
        //                                //    SalesOrderDetail = orderDetail,
        //                                //    id_productionScheduleProductionOrderDetail = productionOrderDetail.id_productionScheduleProductionOrderDetail,
        //                                //    ProductionScheduleProductionOrderDetail = db.ProductionScheduleProductionOrderDetail.FirstOrDefault(i => i.id == productionOrderDetail.id_productionScheduleProductionOrderDetail),
        //                                //    id_productionSchedule = productionOrderDetail.id_productionSchedule,
        //                                //    ProductionSchedule = db.ProductionSchedule.FirstOrDefault(i => i.id == productionOrderDetail.id_productionSchedule),
        //                                //    quantity = detail.quantityApproved
        //                                //});
        //                            }

        //                            if (orderDetail.isActive)
        //                                        count++;
        //                                }
        //                            }

        //                            // UPDATE TOTALS
        //                            modelItem.SalesOrderTotal = SalesOrderTotals(modelItem.id, details);
        //                        }

        //                        #endregion

        //                        //#region IMPORTATION INFORMATION

        //                        //if (itemPO.isImportation)
        //                        //{
        //                        //    if (modelItem.isImportation && modelItem.SalesOrderImportationInformation != null)
        //                        //    {
        //                        //        modelItem.SalesOrderImportationInformation.customsDocumentNumber = importationInformation.customsDocumentNumber;
        //                        //        modelItem.SalesOrderImportationInformation.referendumNumber = importationInformation.referendumNumber;
        //                        //        modelItem.SalesOrderImportationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
        //                        //        modelItem.SalesOrderImportationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
        //                        //        modelItem.SalesOrderImportationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);

        //                        //        db.Entry(modelItem.SalesOrderImportationInformation).State = EntityState.Modified;

        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        importationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
        //                        //        importationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
        //                        //        importationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);
        //                        //        modelItem.SalesOrderImportationInformation = importationInformation;
        //                        //    }
        //                        //}
        //                        //else
        //                        //{
        //                        //    if (modelItem.isImportation)
        //                        //    {
        //                        //        db.SalesOrderImportationInformation.Remove(modelItem.SalesOrderImportationInformation);
        //                        //        db.Entry(modelItem.SalesOrderImportationInformation).State = EntityState.Deleted;
        //                        //    }
        //                        //}

        //                        //#endregion

        //                        if (count == 0)
        //                {
        //                    throw new Exception("No se puede guardar una orden de producción sin detalles");
        //                    //TempData.Keep("salesOrder");
        //                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar una orden de venta sin detalles");
        //                    //return PartialView("_SalesOrderMainFormPartial", tempSalesOrder);
        //                }

        //                if (approve)
        //                {
        //                    foreach (var detail in modelItem.SalesOrderDetail)
        //                    {
        //                        foreach (var salesOrderDetailSalesRequest in detail.SalesOrderDetailSalesRequest)
        //                        {
        //                            ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesOrderDetailSalesRequest.id_salesRequestDetail,
        //                                                                           salesOrderDetailSalesRequest.quantity);
        //                        }
        //                    }

        //                    modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
        //                }

        //                db.SalesOrder.Attach(modelItem);
        //                        db.Entry(modelItem).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();

        //                        TempData["salesOrder"] = modelItem;
        //                        TempData.Keep("salesOrder");

        //                        ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + modelItem.Document.number + " guardada exitosamente");
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //            TempData["salesOrder"] = tempSalesOrder;
        //            TempData.Keep("salesOrder");
        //            ViewData["EditMessage"] = ErrorMessage(e.Message);
        //            trans.Rollback();
        //            return PartialView("_SalesOrderMainFormPartial", tempSalesOrder);

        //            //TempData.Keep("salesOrder");
        //            //ViewData["EditMessage"] = ErrorMessage();
        //            //trans.Rollback();
        //        }
        //    }

        //            return PartialView("_SalesOrderMainFormPartial", modelItem);
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrdersPartialDelete(System.Int32 id)
        //        {
        //            var model = db.SalesOrder;
        //            if (id >= 0)
        //            {
        //                try
        //                {
        //                    var item = model.FirstOrDefault(it => it.id == id);
        //                    if (item != null)
        //                        model.Remove(item);
        //                    db.SaveChanges();
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;
        //                }
        //            }
        //            return PartialView("_SalesOrdersPartial", model.ToList());
        //        }

        //        #endregion

        //        #region SALES ORDER DETAILS

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrderDetailsPartial()
        //        {
        //            SalesOrder salesOrder = (SalesOrder)TempData["salesOrder"];

        //            var model = (salesOrder != null) ? salesOrder.SalesOrderDetail.Where(d => d.isActive).ToList() : new List<SalesOrderDetail>();

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderDetailsPartial", model.OrderByDescending(obd => obd.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrderDetailsPartialAddNew(SalesOrderDetail orderDetail)
        //        {
        //            SalesOrder order = (TempData["salesOrder"] as SalesOrder);
        //            //order = order ?? db.SalesOrder.FirstOrDefault(i => i.id == order.id);
        //            order = order ?? new SalesOrder();

        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //            orderDetail.id = order.SalesOrderDetail.Count() > 0 ? order.SalesOrderDetail.Max(pld => pld.id) + 1 : 1;
        //            orderDetail.id_salesOrder = order.id;
        //            orderDetail.SalesOrder = order;
        //            orderDetail.Item = db.Item.FirstOrDefault(i => i.id == orderDetail.id_item);
        //            orderDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == orderDetail.id_metricUnitTypeUMPresentation);

        //            orderDetail.isActive = true;
        //                    orderDetail.id_userCreate = ActiveUser.id;
        //                    orderDetail.dateCreate = DateTime.Now;
        //                    orderDetail.id_userUpdate = ActiveUser.id;
        //                    orderDetail.dateUpdate = DateTime.Now;

        //                    order.SalesOrderDetail.Add(orderDetail);

        //            order.SalesOrderTotal = SalesOrderTotals(order.id, order.SalesOrderDetail.ToList());
        //            TempData["salesOrder"] = order;
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;

        //                }
        //            }
        //            else
        //                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

        //            TempData.Keep("salesOrder");

        //            var model = order?.SalesOrderDetail.Where(d => d.isActive).ToList() ?? new List<SalesOrderDetail>();
        //            return PartialView("_SalesOrderDetailsPartial", model.OrderByDescending(obd => obd.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrderDetailsPartialUpdate(SalesOrderDetail orderDetail)
        //        {
        //            SalesOrder order = (TempData["salesOrder"] as SalesOrder);

        //          //order = order ?? db.SalesOrder.FirstOrDefault(i => i.id == order.id);
        //            order = order ?? new SalesOrder();

        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //            SalesOrderDetail modelItem = order.SalesOrderDetail.FirstOrDefault(it => it.id == orderDetail.id);
        //            //var modelItem = order.SalesOrderDetail.FirstOrDefault(it => it.id_item == orderDetail.id_item);
        //            if (modelItem != null)
        //                    {
        //                modelItem.id_item = orderDetail.id_item;
        //                modelItem.Item = db.Item.FirstOrDefault(i => i.id == orderDetail.id_item);

        //                modelItem.quantityRequested = orderDetail.quantityRequested;
        //                modelItem.quantityOrdered = orderDetail.quantityOrdered;
        //                modelItem.quantityApproved = orderDetail.quantityApproved;
        //                modelItem.quantityDelivered = orderDetail.quantityDelivered;
        //                modelItem.quantityTypeUMSale = orderDetail.quantityTypeUMSale;
        //                modelItem.id_metricUnitTypeUMPresentation = orderDetail.id_metricUnitTypeUMPresentation;
        //                modelItem.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == orderDetail.id_metricUnitTypeUMPresentation);

        //                modelItem.price = orderDetail.price;
        //                modelItem.discount = orderDetail.discount;
        //                modelItem.iva = orderDetail.iva;
        //                modelItem.subtotal = orderDetail.subtotal;
        //                modelItem.total = orderDetail.total;

        //                modelItem.id_userUpdate = ActiveUser.id;
        //                        modelItem.dateCreate = DateTime.Now;

        //                        this.UpdateModel(modelItem);

        //                order.SalesOrderTotal = SalesOrderTotals(order.id, order.SalesOrderDetail.ToList());
        //                TempData["salesOrder"] = order;
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;
        //                }
        //            }
        //            else
        //                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

        //            TempData.Keep("salesOrder");

        //            var model = order?.SalesOrderDetail.Where(d => d.isActive).ToList() ?? new List<SalesOrderDetail>();

        //            return PartialView("_SalesOrderDetailsPartial", model.OrderByDescending(obd => obd.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult SalesOrderDetailsPartialDelete(System.Int32 id)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //           //salesOrder = salesOrder ?? db.SalesOrder.FirstOrDefault(i => i.id == salesOrder.id);
        //            salesOrder = salesOrder ?? new SalesOrder();

        //            //if (id_item >= 0)
        //            //{
        //                try
        //                {
        //                    var orderDetail = salesOrder.SalesOrderDetail.FirstOrDefault(p => p.id == id);
        //                    if (orderDetail != null)
        //                    {
        //                        orderDetail.isActive = false;
        //                        orderDetail.id_userUpdate = ActiveUser.id;
        //                        orderDetail.dateCreate = DateTime.Now;
        //                    }

        //        salesOrder.SalesOrderTotal = SalesOrderTotals(salesOrder.id, salesOrder.SalesOrderDetail.ToList());
        //        TempData["salesOrder"] = salesOrder;
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;
        //                }
        //           //}

        //            TempData.Keep("salesOrder");

        //            var model = salesOrder?.SalesOrderDetail.Where(d => d.isActive).ToList() ?? new List<SalesOrderDetail>();
        //            return PartialView("_SalesOrderDetailsPartial", model.OrderByDescending(obd => obd.id).ToList());
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public void SalesOrderDetailsDeleteSeleted(int[] ids)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);
        //            salesOrder = salesOrder ?? db.SalesOrder.FirstOrDefault(i => i.id == salesOrder.id);
        //            salesOrder = salesOrder ?? new SalesOrder();

        //            if (ids != null)
        //            {
        //                try
        //                {
        //                    var orderDetail = salesOrder.SalesOrderDetail.Where(i => ids.Contains(i.id_item));

        //                    foreach (var detail in orderDetail)
        //                    {
        //                        detail.isActive = false;
        //                        detail.id_userUpdate = ActiveUser.id;
        //                        detail.dateUpdate = DateTime.Now;
        //                    }

        //                    TempData["salesOrder"] = salesOrder;
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;
        //                }
        //            }

        //            TempData.Keep("salesOrder");

        //        }

        //        #endregion

        //        #region SINGLE CHANGE DOCUMENT STATE

        //        [HttpPost]
        //        public ActionResult Approve(int id)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

        //                    if (salesOrder != null && documentState != null)
        //                    {
        //                        salesOrder.Document.id_documentState = documentState.id;
        //                        salesOrder.Document.DocumentState = documentState;

        //                        foreach (var details in salesOrder.SalesOrderDetail)
        //                        {
        //                            details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

        //                            db.SalesOrderDetail.Attach(details);
        //                            db.Entry(details).State = EntityState.Modified;
        //                        }

        //                        db.SalesOrder.Attach(salesOrder);
        //                        db.Entry(salesOrder).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewData["EditError"] = e.Message;
        //                    trans.Rollback();
        //                }
        //            }

        //            Invoice invoice = GenerateInvoice(id);

        //            if (invoice != null)
        //            {
        //                XmlFactura xmlFactura = DB2XML.Factura2Xml(invoice.id);
        //                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlFactura));
        //                StringWriter textWriter = new StringWriter();
        //                xmlSerializer.Serialize(textWriter, xmlFactura);
        //                string xml = textWriter.ToString();

        //                invoice.Document.ElectronicDocument.xml = xml;

        //                db.Invoice.Attach(invoice);
        //                db.Entry(invoice).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //        }

        //        [HttpPost]
        //        public ActionResult Autorize(int id)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

        //                    if (salesOrder != null && documentState != null)
        //                    {
        //                        salesOrder.Document.id_documentState = documentState.id;
        //                        salesOrder.Document.DocumentState = documentState;

        //                        //foreach (var details in salesOrder.SalesOrderDetail)
        //                        //{
        //                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

        //                        //    db.SalesOrderDetail.Attach(details);
        //                        //    db.Entry(details).State = EntityState.Modified;
        //                        //}

        //                        db.SalesOrder.Attach(salesOrder);
        //                        db.Entry(salesOrder).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();

        //                TempData["salesOrder"] = salesOrder;
        //                TempData.Keep("salesOrder");

        //                ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + salesOrder.Document.number + " autorizada exitosamente");

        //            }
        //        }
        //                catch (Exception e)
        //                {
        //            TempData.Keep("salesOrder");
        //            ViewData["EditError"] = ErrorMessage(e.Message);
        //            trans.Rollback();
        //                }
        //            }

        //            //Invoice invoice = GenerateInvoice(id);

        //            //if(invoice != null)
        //            //{
        //            //    XmlFactura xmlFactura = DB2XML.Factura2Xml(invoice.id);
        //            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlFactura));
        //            //    StringWriter textWriter = new StringWriter();
        //            //    xmlSerializer.Serialize(textWriter, xmlFactura);
        //            //    string xml = textWriter.ToString();

        //            //    invoice.Document.ElectronicDocument.xml = xml;

        //            //    db.Invoice.Attach(invoice);
        //            //    db.Entry(invoice).State = EntityState.Modified;
        //            //    db.SaveChanges();
        //            //}

        //            //TempData["salesOrder"] = salesOrder;
        //            //TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //        }

        //        [HttpPost]
        //        public ActionResult Protect(int id)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04"); //CERRADA

        //            if (salesOrder != null && documentState != null)
        //                    {
        //                        salesOrder.Document.id_documentState = documentState.id;
        //                        salesOrder.Document.DocumentState = documentState;

        //                        db.SalesOrder.Attach(salesOrder);
        //                        db.Entry(salesOrder).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();

        //                TempData["salesOrder"] = salesOrder;
        //                TempData.Keep("salesOrder");

        //                ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + salesOrder.Document.number + " cerrada exitosamente");

        //            }
        //                }
        //                catch (Exception e)
        //                {
        //            TempData.Keep("salesOrder");
        //            ViewData["EditError"] = ErrorMessage(e.Message);
        //            trans.Rollback();
        //                }
        //            }

        //            //TempData["salesOrder"] = salesOrder;
        //            //TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //        }

        //        [HttpPost]
        //        public ActionResult Cancel(int id)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //            var existInProductionLotLiquidation = salesOrder.SalesOrderDetail.FirstOrDefault(fod => fod.ProductionLotLiquidation.FirstOrDefault(fod2 => fod2.ProductionLot.ProductionLotState.code != "01" && //PENDIENTE DE RECEPCION
        //               fod2.ProductionLot.ProductionLotState.code != "02" && //RECEPCIONADO             
        //               fod2.ProductionLot.ProductionLotState.code != "03") != null);//PENDIENTE DE PROCESAMIENTO Diferente de estos estados

        //            if (existInProductionLotLiquidation != null)
        //            {
        //                throw new Exception("No se puede cancelar la orden de Producción, debido a tener detalles que pertenecen a alguna Liquidación en Producción.");
        //            }

        //            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

        //            if (salesOrder != null && documentState != null)
        //                    {
        //                if (salesOrder.Document.DocumentState.code != "01")
        //                {
        //                    foreach (var detail in salesOrder.SalesOrderDetail)
        //                    {
        //                        foreach (var salesOrderDetailSalesRequest in detail.SalesOrderDetailSalesRequest)
        //                        {
        //                            ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesOrderDetailSalesRequest.id_salesRequestDetail,
        //                                                                           -salesOrderDetailSalesRequest.quantity);
        //                        }
        //                    }
        //                }

        //                salesOrder.Document.id_documentState = documentState.id;
        //                        salesOrder.Document.DocumentState = documentState;

        //                        db.SalesOrder.Attach(salesOrder);
        //                        db.Entry(salesOrder).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();

        //                TempData["salesOrder"] = salesOrder;
        //                TempData.Keep("salesOrder");

        //                ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + salesOrder.Document.number + " cancelada exitosamente");

        //            }
        //                }
        //                catch (Exception e)
        //                {
        //            TempData.Keep("salesOrder");
        //            ViewData["EditError"] = ErrorMessage(e.Message);
        //            trans.Rollback();
        //                }
        //            }

        //            //TempData["salesOrder"] = salesOrder;
        //            //TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //        }

        //        [HttpPost]
        //        public ActionResult Revert(int id)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {

        //            var existInProductionLotLiquidation = salesOrder.SalesOrderDetail.FirstOrDefault(fod => fod.ProductionLotLiquidation.FirstOrDefault(fod2 => fod2.ProductionLot.ProductionLotState.code != "01" && //PENDIENTE DE RECEPCION
        //                                                                                                                                                                                           fod2.ProductionLot.ProductionLotState.code != "02" && //RECEPCIONADO
        //                                                                                                                                                                                           fod2.ProductionLot.ProductionLotState.code != "03") != null);//PENDIENTE DE PROCESAMIENTO Diferente de estos estados

        //            if (existInProductionLotLiquidation != null)
        //            {
        //                throw new Exception("No se puede reversar la orden de Producción, debido a tener detalles que pertenecen a alguna Liquidación en Producción.");
        //            }

        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

        //                    if (salesOrder != null && documentState != null)
        //                    {
        //                        foreach (var detail in salesOrder.SalesOrderDetail)
        //                        {
        //                            foreach (var salesOrderDetailSalesRequest in detail.SalesOrderDetailSalesRequest)
        //                            {
        //                                ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesOrderDetailSalesRequest.id_salesRequestDetail,
        //                                                                               -salesOrderDetailSalesRequest.quantity);
        //                            }
        //                        }

        //                        salesOrder.Document.id_documentState = documentState.id;
        //                        salesOrder.Document.DocumentState = documentState;

        //                        //foreach (var details in salesOrder.SalesOrderDetail)
        //                        //{
        //                        //    details.quantityApproved = 0.0M;

        //                        //    db.SalesOrderDetail.Attach(details);
        //                        //    db.Entry(details).State = EntityState.Modified;
        //                        //}

        //                        db.SalesOrder.Attach(salesOrder);
        //                        db.Entry(salesOrder).State = EntityState.Modified;

        //                        db.SaveChanges();
        //                        trans.Commit();
        //                TempData["salesOrder"] = salesOrder;
        //                TempData.Keep("salesOrder");

        //                ViewData["EditMessage"] = SuccessMessage("Orden de Producción: " + salesOrder.Document.number + " reversada exitosamente");

        //            }
        //                }
        //                catch (Exception e)
        //                {
        //            TempData.Keep("salesOrder");
        //            ViewData["EditError"] = ErrorMessage(e.Message);
        //            trans.Rollback();
        //                }
        //            }

        //            //TempData["salesOrder"] = salesOrder;
        //            //TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //        }

        //        #endregion

        //        #region SELECTED DOCUMENT STATE CHANGE

        //        [HttpPost, ValidateInput(false)]
        //        public void ApproveDocuments(int[] ids)
        //        {
        //            if (ids != null)
        //            {
        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        foreach (var id in ids)
        //                        {
        //                            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

        //                            if (salesOrder != null && documentState != null)
        //                            {
        //                                salesOrder.Document.id_documentState = documentState.id;
        //                                salesOrder.Document.DocumentState = documentState;

        //                                foreach (var details in salesOrder.SalesOrderDetail)
        //                                {
        //                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;
        //                                    db.SalesOrderDetail.Attach(details);
        //                                    db.Entry(details).State = EntityState.Modified;
        //                                }

        //                                db.SalesOrder.Attach(salesOrder);
        //                                db.Entry(salesOrder).State = EntityState.Modified;
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = e.Message;
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();
        //            int[] filters = model.Select(i => i.id).ToArray();
        //            model = db.SalesOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

        //            TempData["model"] = model;
        //            TempData.Keep("model");
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public void AutorizeDocuments(int[] ids)
        //        {
        //            if (ids != null)
        //            {
        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        foreach (var id in ids)
        //                        {
        //                            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

        //                            if (salesOrder != null && documentState != null)
        //                            {
        //                                salesOrder.Document.id_documentState = documentState.id;
        //                                salesOrder.Document.DocumentState = documentState;

        //                                foreach (var details in salesOrder.SalesOrderDetail)
        //                                {
        //                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;

        //                                    db.SalesOrderDetail.Attach(details);
        //                                    db.Entry(details).State = EntityState.Modified;
        //                                }

        //                                db.SalesOrder.Attach(salesOrder);
        //                                db.Entry(salesOrder).State = EntityState.Modified;
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = e.Message;
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();
        //            int[] filters = model.Select(i => i.id).ToArray();
        //            model = db.SalesOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

        //            TempData["model"] = model;
        //            TempData.Keep("model");
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public void ProtectDocuments(int[] ids)
        //        {
        //            if (ids != null)
        //            {
        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        foreach (var id in ids)
        //                        {
        //                            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

        //                            if (salesOrder != null && documentState != null)
        //                            {
        //                                salesOrder.Document.id_documentState = documentState.id;
        //                                salesOrder.Document.DocumentState = documentState;

        //                                db.SalesOrder.Attach(salesOrder);
        //                                db.Entry(salesOrder).State = EntityState.Modified;
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = e.Message;
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();
        //            int[] filters = model.Select(i => i.id).ToArray();
        //            model = db.SalesOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

        //            TempData["model"] = model;
        //            TempData.Keep("model");
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public void CancelDocuments(int[] ids)
        //        {
        //            if (ids != null)
        //            {
        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        foreach (var id in ids)
        //                        {
        //                            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

        //                            if (salesOrder != null && documentState != null)
        //                            {
        //                                salesOrder.Document.id_documentState = documentState.id;
        //                                salesOrder.Document.DocumentState = documentState;

        //                                db.SalesOrder.Attach(salesOrder);
        //                                db.Entry(salesOrder).State = EntityState.Modified;
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = e.Message;
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();
        //            int[] filters = model.Select(i => i.id).ToArray();
        //            model = db.SalesOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

        //            TempData["model"] = model;
        //            TempData.Keep("model");
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public void RevertDocuments(int[] ids)
        //        {
        //            if (ids != null)
        //            {
        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        foreach (var id in ids)
        //                        {
        //                            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);

        //                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

        //                            if (salesOrder != null && documentState != null)
        //                            {
        //                                salesOrder.Document.id_documentState = documentState.id;
        //                                salesOrder.Document.DocumentState = documentState;

        //                                foreach (var details in salesOrder.SalesOrderDetail)
        //                                {
        //                                    details.quantityApproved = 0.0M;

        //                                    db.SalesOrderDetail.Attach(details);
        //                                    db.Entry(details).State = EntityState.Modified;
        //                                }

        //                                db.SalesOrder.Attach(salesOrder);
        //                                db.Entry(salesOrder).State = EntityState.Modified;
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = e.Message;
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            var model = (TempData["model"] as List<SalesOrder>);
        //            model = model ?? new List<SalesOrder>();
        //            int[] filters = model.Select(i => i.id).ToArray();
        //            model = db.SalesOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

        //            TempData["model"] = model;
        //            TempData.Keep("model");
        //        }

        //        #endregion

        //        #region SALES ORDER REPORTS

        //        [HttpPost]
        //        public ActionResult SalesOrdersReport(int[] ids)
        //        {
        //            PurchaseOrdersReport report = new PurchaseOrdersReport();
        //            PurchaseOrderListReport orderListReport = new PurchaseOrderListReport();
        //            PurchaseOrderHeadListReport orderHeadListReport = new PurchaseOrderHeadListReport();
        //            var partial = "";

        //            if (ids == null)
        //            {
        //                partial = "_SalesOrderHeadListReport";
        //                return PartialView(partial, orderHeadListReport);
        //            }

        //            if (ids.Length > 0)
        //            {

        //                if (ids.Length == 1)
        //                {
        //                    report.Parameters["numberSalesOrder"].Value = ids[0];
        //                    report.Parameters["numberSalesOrder"].Visible = false;
        //                    partial = "_SalesOrderReport";
        //                    return PartialView(partial, report);
        //                }
        //                else
        //                {
        //                    orderListReport.Parameters["numberSalesOrders"].MultiValue = true;
        //                    orderListReport.Parameters["numberSalesOrders"].Value = ids;
        //                    orderListReport.Parameters["numberSalesOrders"].Visible = false;
        //                    partial = "_SalesOrderListReport";
        //                    return PartialView(partial, orderListReport);
        //                }

        //            }
        //            return PartialView(partial, report);
        //            //return PartialView("_SalesOrderReport");
        //        }

        //        #endregion

        //        #region ACTIONS

        //        [HttpPost, ValidateInput(false)]
        //        public JsonResult Actions(int id)
        //        {
        //            var actions = new
        //            {
        //                btnApprove = true,
        //                btnAutorize = false,
        //                btnProtect = false,
        //                btnCancel = false,
        //                btnRevert = false,
        //            };

        //            if (id == 0)
        //            {
        //                return Json(actions, JsonRequestBehavior.AllowGet);
        //            }

        //            SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(r => r.id == id);
        //    string state = salesOrder.Document.DocumentState.code;

        //            if (state == "01") // PENDIENTE
        //            {
        //                actions = new
        //                {
        //                    btnApprove = true,
        //                    btnAutorize = false,
        //                    btnProtect = false,
        //                    btnCancel = true,
        //                    btnRevert = false,
        //                };
        //            }
        //            else if (state == "03")//|| state == 3) // APROBADA
        //            {
        //                actions = new
        //                {
        //                    btnApprove = false,
        //                    btnAutorize = true,
        //                    btnProtect = false,
        //                    btnCancel = true,
        //                    btnRevert = true,
        //                };
        //            }
        //            else if (state == "04" || state == "05") // CERRADA O ANULADA
        //            {
        //                actions = new
        //                {
        //                    btnApprove = false,
        //                    btnAutorize = false,
        //                    btnProtect = false,
        //                    btnCancel = false,
        //                    btnRevert = false,
        //                };
        //            }
        //            else if (state == "06") // AUTORIZADA
        //            {
        //                actions = new
        //                {
        //                    btnApprove = false,
        //                    btnAutorize = false,
        //                    btnProtect = true,
        //                    btnCancel = true,
        //                    btnRevert = true,
        //                };
        //            }

        //            return Json(actions, JsonRequestBehavior.AllowGet);
        //        }

        //        #endregion

        //        #region PAGINATION

        //        [HttpPost, ValidateInput(false)]
        //        public JsonResult InitializePagination(int id_salesOrder)
        //        {
        //            TempData.Keep("salesOrder");

        //            int index = db.SalesOrder.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_salesOrder);

        //            var result = new
        //            {
        //                maximunPages = db.SalesOrder.Count(),
        //                currentPage = index + 1
        //            };

        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public ActionResult Pagination(int page)
        //        {
        //            SalesOrder salesOrder = db.SalesOrder.OrderByDescending(p => p.id).Take(page).ToList().Last();

        //            if (salesOrder != null)
        //            {
        //                TempData["salesOrder"] = salesOrder;
        //                TempData.Keep("salesOrder");
        //                return PartialView("_SalesOrderMainFormPartial", salesOrder);
        //            }

        //            TempData.Keep("salesOrder");

        //            return PartialView("_SalesOrderMainFormPartial", new SalesOrder());
        //        }

        //        #endregion

        //        #region INVOICE

        //        public Invoice GenerateInvoice(int id_saleOrder)
        //        {
        //            using (DbContextTransaction trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    SalesOrder salesOrder = db.SalesOrder.FirstOrDefault(o => o.id == id_saleOrder);

        //                    if (salesOrder == null)
        //                        return null;

        //                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("07"));
        //                    DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);
        //                    ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.code.Equals("01"));

        //                    int sequential = GetDocumentSequential(documentType?.id ?? 0);

        //                    Invoice invoice = new Invoice
        //                    {
        //                        Document = new Document
        //                        {
        //                            id_documentType = documentType?.id ?? 0,
        //                            DocumentType = documentType,
        //                            id_documentState = documentState?.id ?? 0,
        //                            DocumentState = documentState,
        //                            emissionDate = DateTime.Now,

        //                            id_emissionPoint = ActiveEmissionPoint.id,

        //                            id_userCreate = ActiveUser.id,
        //                            dateCreate = DateTime.Now,
        //                            id_userUpdate = ActiveUser.id,
        //                            dateUpdate = DateTime.Now,
        //                            sequential = sequential,
        //                            number = GetDocumentNumber(documentType?.id ?? 0),

        //                            accessKey = AccessKey.GenerateAccessKey(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", ""),
        //                                                                    documentType?.code ?? "",
        //                                                                    ActiveCompany.ruc,
        //                                                                    ActiveCompany.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString(),
        //                                                                    ActiveSucursal.code.ToString() + ActiveEmissionPoint.code.ToString("D3"),
        //                                                                    sequential.ToString("D9"),
        //                                                                    sequential.ToString("D8"),
        //                                                                    ActiveCompany.CompanyElectronicFacturation.EmissionType.codeSRI.ToString()),

        //                            ElectronicDocument = new ElectronicDocument
        //                            {
        //                                id_electronicDocumentState = electronicState?.id ?? 0,
        //                                ElectronicDocumentState = electronicState
        //                            }
        //                        },
        //                        id_saleOrder = salesOrder.id,
        //                        id_buyer = salesOrder.id_customer.Value,
        //                        id_remissionGuide = null,
        //                        subtotalIVA = salesOrder.SalesOrderTotal.subtotalIVA12Percent,
        //                        subTotalIVA0 = salesOrder.SalesOrderTotal.subtotalIVA0Percent,
        //                        subTotalNoObjectIVA = salesOrder.SalesOrderTotal.subtotalIVANoObjectIVA,
        //                        subTotalExentIVA = salesOrder.SalesOrderTotal.subtotalExentedIVA,
        //                        subTotal = salesOrder.SalesOrderTotal.subtotal,
        //                        totalDiscount = salesOrder.SalesOrderTotal.discount,
        //                        valueICE = salesOrder.SalesOrderTotal.valueICE,
        //                        valueIRBPNR = salesOrder.SalesOrderTotal.valueIRBPNR,
        //                        IVA = salesOrder.SalesOrderTotal.totalIVA12,
        //                        tip = 0.0M,
        //                        totalValue = salesOrder.SalesOrderTotal.total,
        //                        InvoiceDetail = new List<InvoiceDetail>(),
        //                        InvoiceTotalTaxes = new List<InvoiceTotalTaxes>()
        //                    };

        //                    foreach (var detail in salesOrder.SalesOrderDetail)
        //                    {
        //                        InvoiceDetail invoiceDetail = new InvoiceDetail
        //                        {
        //                            id_item = detail.id_item,
        //                            //Item = detail.Item,
        //                            description = detail.Item.name,
        //                            amount = detail.quantityApproved,
        //                            unitPrice = detail.price,
        //                            discount = detail.discount,
        //                            totalPriceWithoutTax = detail.subtotal,
        //                            iva = detail.iva,
        //                            iva0 = 0.0M,
        //                            ivaNoObject = 0.0M,
        //                            ivaExented = 0.0M,
        //                            valueICE = 0.0M,
        //                            valueIRBPNR = 0.0M,
        //                            total = detail.total,
        //                            InvoiceDetailsTaxes = new List<InvoiceDetailsTaxes>()
        //                        };

        //                        foreach (var taxation in detail.Item.ItemTaxation)
        //                        {
        //                            invoiceDetail.InvoiceDetailsTaxes.Add(new InvoiceDetailsTaxes
        //                            {
        //                                id_tax = taxation.id_taxType,
        //                                id_rate = taxation.id_rate,
        //                                taxableBase = detail.subtotal,
        //                                value = detail.subtotal * taxation.Rate.percentage
        //                            });
        //                        }

        //                        invoice.InvoiceDetail.Add(invoiceDetail);
        //                    }

        //                    foreach (InvoiceDetail detail in invoice.InvoiceDetail)
        //                    {
        //                        Item item = db.Item.FirstOrDefault(i => i.id == detail.id_item);
        //                        foreach (ItemTaxation tax in item.ItemTaxation)
        //                        {
        //                            InvoiceTotalTaxes totalTaxInvoice = invoice.InvoiceTotalTaxes.FirstOrDefault(tt => tt.id_taxType == tax.id_taxType && tt.id_rate == tax.id_rate);
        //                            if (totalTaxInvoice == null)
        //                            {
        //                                totalTaxInvoice = new InvoiceTotalTaxes
        //                                {
        //                                    Invoice = invoice,
        //                                    id_taxType = tax.id_taxType,
        //                                    TaxType = tax.TaxType,
        //                                    id_rate = tax.id_rate,
        //                                    Rate1 = tax.Rate,
        //                                    aditionalDiscount = 0.0M, //_detail.discount,
        //                                    taxableBase = detail.totalPriceWithoutTax,
        //                                    rate = tax.Rate.percentage * 100.0M,
        //                                    value = detail.totalPriceWithoutTax * tax.Rate.percentage
        //                                };
        //                                invoice.InvoiceTotalTaxes.Add(totalTaxInvoice);
        //                            }
        //                            else
        //                            {
        //                                // totalTaxInvoice.aditionalDiscount = totalTaxInvoice.aditionalDiscount + _detail.discount;
        //                                totalTaxInvoice.taxableBase = totalTaxInvoice.taxableBase + detail.totalPriceWithoutTax;
        //                                totalTaxInvoice.value = totalTaxInvoice.value + detail.totalPriceWithoutTax * tax.Rate.percentage;

        //                                if (totalTaxInvoice.id_invoice != 0)
        //                                {
        //                                    db.InvoiceTotalTaxes.Attach(totalTaxInvoice);
        //                                    db.Entry(totalTaxInvoice).State = EntityState.Modified;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    //Actualiza Secuencial
        //                    if (documentType != null)
        //                    {
        //                        documentType.currentNumber = documentType.currentNumber + 1;
        //                        db.DocumentType.Attach(documentType);
        //                        db.Entry(documentType).State = EntityState.Modified;
        //                    }

        //                    db.Invoice.Add(invoice);

        //                    db.SaveChanges();
        //                    trans.Commit();

        //                    return invoice;
        //                }
        //                catch (Exception /*e*/)
        //                {
        //                    trans.Rollback();
        //                    return null;
        //                }
        //            }
        //        }

        //        #endregion

        //        #region AXILIAR FUNCTIONS

        //        private SalesOrderTotal SalesOrderTotals(int id_salesOrder, List<SalesOrderDetail> orderDetails)
        //        {
        //            SalesOrderTotal salesOrderTotal = db.SalesOrderTotal.FirstOrDefault(t => t.id_salesOrder == id_salesOrder);

        //            salesOrderTotal = salesOrderTotal ?? new SalesOrderTotal
        //            {
        //                id_salesOrder = id_salesOrder
        //            };

        //            decimal subtotalIVA12Percent = 0.0M;
        //            decimal subtotalIVA14Percent = 0.0M;
        //            decimal subtotalIVA0Percent = 0.0M;
        //            decimal subtotalIVANoObjectIVA = 0.0M;
        //            decimal subtotalExentedIVA = 0.0M;

        //            decimal subtotal = 0.0M;

        //            decimal discount = 0.0M;
        //            decimal valueICE = 0.0M;
        //            decimal valueIRBPNR = 0.0M;

        //            decimal totalIVA12 = 0.0M;
        //            decimal totalIVA14 = 0.0M;

        //            decimal total = 0.0M;

        //            foreach (var detail in orderDetails)
        //            {
        //                if (detail.Item != null && detail.isActive)
        //                {
        //                    ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//Taxtype: "2": Impuesto IVA con Rate: "2": tarifa 12% 
        //            ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//Taxtype: "2": Impuesto IVA con Rate: "5": tarifa 14% 

        //            if (rateIVA12 != null)
        //                    {
        //                        subtotalIVA12Percent += detail.quantityApproved * detail.price;
        //                    }
        //            if (rateIVA14 != null)
        //            {
        //                subtotalIVA14Percent += detail.quantityApproved * detail.price;
        //            }


        //            subtotal += detail.quantityApproved * detail.price;
        //            discount += detail.discount;
        //        }

        //            }

        //    //totalIVA12 = subtotalIVA12Percent * 0.12M;
        //    //totalIVA14 = subtotalIVA14Percent * 0.14M;
        //    var percent12 = db.Rate.FirstOrDefault(fod => fod.code.Equals("2"))?.percentage / 100 ?? 0.12M; //"2" Tarifa 12%
        //    var percent14 = db.Rate.FirstOrDefault(fod => fod.code.Equals("5"))?.percentage / 100 ?? 0.14M; //"5" Tarifa 14%
        //    totalIVA12 = subtotalIVA12Percent * percent12;// 0.12M;
        //    totalIVA14 = subtotalIVA14Percent * percent14;//0.14M;

        //    total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;

        //            salesOrderTotal.subtotalIVA12Percent = subtotalIVA12Percent;
        //            salesOrderTotal.subtotalIVA14Percent = subtotalIVA14Percent;
        //            salesOrderTotal.subtotalIVA0Percent = subtotalIVA0Percent;
        //            salesOrderTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
        //            salesOrderTotal.subtotalExentedIVA = subtotalExentedIVA;

        //            salesOrderTotal.subtotal = subtotal;
        //            salesOrderTotal.discount = discount;
        //            salesOrderTotal.valueICE = valueICE;
        //            salesOrderTotal.valueIRBPNR = valueIRBPNR;

        //            salesOrderTotal.totalIVA12 = totalIVA12;
        //            salesOrderTotal.totalIVA14 = totalIVA14;

        //            salesOrderTotal.total = total;

        //            return salesOrderTotal;
        //        }

        //[HttpPost, ValidateInput(false)]
        //public JsonResult OrderTotals()
        //{
        //    SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //    salesOrder = salesOrder ?? new SalesOrder();
        //    salesOrder.SalesOrderDetail = salesOrder.SalesOrderDetail ?? new List<SalesOrderDetail>();

        //    salesOrder.SalesOrderTotal = SalesOrderTotals(salesOrder.id, salesOrder.SalesOrderDetail.ToList());

        //    TempData["salesOrder"] = salesOrder;
        //    TempData.Keep("salesOrder");

        //    var result = new
        //    {
        //        orderSubtotal = salesOrder.SalesOrderTotal.subtotal,
        //        orderSubtotalIVA12Percent = salesOrder.SalesOrderTotal.subtotalIVA12Percent,
        //        orderTotalIVA12 = salesOrder.SalesOrderTotal.totalIVA12,
        //        orderDiscount = salesOrder.SalesOrderTotal.discount,
        //        orderSubtotalIVA14Percent = salesOrder.SalesOrderTotal.subtotalIVA14Percent,
        //        orderTotalIVA14 = salesOrder.SalesOrderTotal.totalIVA14,
        //        orderTotal = salesOrder.SalesOrderTotal.total
        //    };

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //        [HttpPost, ValidateInput(false)]
        //public JsonResult ItemDetailData(int? id, int? id_item, string quantityApproved, string discount, int? id_metricUnitTypeUMPresentation)
        //{
        //    decimal _quantityApproved = Convert.ToDecimal(quantityApproved);
        //    decimal _discount = Convert.ToDecimal(discount);

        //    SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //    SalesOrder salesOrder2 = (TempData["salesOrder2"] as SalesOrder);

        //    salesOrder2 = salesOrder2 ?? Copy(salesOrder, salesOrder2);

        //    Item item = db.Item.FirstOrDefault(i => i.id == id_item);

        //    MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);


        //    //Item item = db.Item.FirstOrDefault(i => i.id == id_itemRequest);
        //    decimal quantityTypeUMSale = 0;
        //    string metricUnitSale = "";
        //    string msgErrorConversion = "";
        //    string msgErrorDiscount = "";
        //    //decimal _quantitySchedule = Convert.ToDecimal(quantitySchedule ?? "0");

        //    if (item != null)
        //    {
        //        metricUnitSale = item.ItemSaleInformation?.MetricUnit?.code ?? "";

        //        //metricUnitTypeUMPresentation var metricUnitRequest = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitRequest);

        //        if (metricUnitTypeUMPresentation != null)
        //        {
        //            var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
        //            var factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
        //                                                                                 muc.id_metricOrigin == metricUnitTypeUMPresentation.id &&
        //                                                                                 muc.id_metricDestiny == id_metricUnitPresentation);
        //            if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
        //            {
        //                factorConversion = new MetricUnitConversion() { factor = 1 };
        //            }
        //            if (factorConversion == null)
        //            {
        //                //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
        //                msgErrorConversion = ("Falta el Factor de Conversión entre : " + (metricUnitTypeUMPresentation.code) + " y " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
        //            }
        //            else
        //            {
        //                var quantityAux = _quantityApproved * factorConversion.factor;
        //                quantityAux /= (item.Presentation?.minimum ?? 1);
        //                quantityTypeUMSale = TruncateDecimal(quantityAux);
        //                //var truncateQuantityAux = decimal.Truncate(quantityAux);
        //                //if ((quantityAux - truncateQuantityAux) > 0)
        //                //{
        //                //    quantityAux = truncateQuantityAux + 1;
        //                //};
        //                //quantitySale = quantityAux;

        //                //var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
        //                factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
        //                                                                                     muc.id_metricOrigin == id_metricUnitPresentation &&
        //                                                                                     muc.id_metricDestiny == metricUnitTypeUMPresentation.id);
        //                if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
        //                {
        //                    factorConversion = new MetricUnitConversion() { factor = 1 };
        //                }
        //                if (factorConversion == null)
        //                {
        //                    //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
        //                    msgErrorConversion = ("Falta el Factor de Conversión entre : " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + " y " + (metricUnitTypeUMPresentation.code) + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
        //                }
        //                else
        //                {
        //                    quantityAux = (quantityTypeUMSale * (item.Presentation?.minimum ?? 1)) * factorConversion.factor;

        //                    _quantityApproved = quantityAux;
        //                }
        //            }
        //        }
        //    }

        //    decimal _price;
        //    try
        //    {
        //        _price = SaleDetailPrice(item.id, salesOrder2, id_metricUnitTypeUMPresentation);
        //    }
        //    catch (Exception e)
        //    {
        //        _price = 0;
        //        msgErrorConversion = e.Message;
        //    }


        //    decimal iva = SaleDetailIVA(item.id, _quantityApproved, _price);

        //    SalesOrderDetail detail = salesOrder2.SalesOrderDetail.FirstOrDefault(d => d.id_item == id_item && d.id == id);
        //    if ((_price * _quantityApproved + iva) <= _discount && _discount != 0)
        //    {
        //        msgErrorDiscount = "El descuento no puede ser mayor o igual a la suma de Subtotal mas el Iva";
        //        _discount = 0;
        //    }
        //    if (detail != null)
        //    {
        //        detail.price = _price;
        //        detail.quantityApproved = _quantityApproved;
        //        detail.discount = _discount;
        //        salesOrder2.SalesOrderTotal = SalesOrderTotals(salesOrder2.id, salesOrder2.SalesOrderDetail.ToList());
        //    }
        //    else
        //    {
        //        detail = new SalesOrderDetail()
        //        {
        //            id = salesOrder2.SalesOrderDetail.Count() > 0 ? salesOrder2.SalesOrderDetail.Max(ppd => ppd.id) + 1 : 1,
        //            //id_item = id_item.Value,
        //            Item = db.Item.FirstOrDefault(i => i.id == id_item),
        //            isActive = true,
        //            price = _price,
        //            iva = iva,
        //            subtotal = _price * _quantityApproved,
        //            discount = _discount,
        //            total = (_price * _quantityApproved + iva) - _discount,
        //            SalesOrder = salesOrder2,
        //            id_salesOrder = salesOrder2.id,
        //            quantityApproved = _quantityApproved,
        //            //id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value,
        //            MetricUnit = metricUnitTypeUMPresentation
        //        };
        //        if (id_item != null) detail.id_item = id_item.Value;
        //        if (id_metricUnitTypeUMPresentation != null) detail.id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value;

        //        salesOrder2.SalesOrderDetail.Add(detail);
        //        salesOrder2.SalesOrderTotal = SalesOrderTotals(salesOrder2.id, salesOrder2.SalesOrderDetail.ToList());

        //    }

        //    //var metricUnits = item?.Presentation.MetricUnit.MetricType.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId) || w.id == id_metricUnitTypeUMPresentation).ToList() ?? new List<MetricUnit>();


        //    var result = new
        //    {
        //        ItemDetailData = new
        //        {
        //            masterCode = item.masterCode,
        //            um = item.ItemSaleInformation.MetricUnit.code,
        //            //currentStock = item.ItemInventory.currentStock,
        //            price = _price,
        //            iva = iva,
        //            subtotal = _price * _quantityApproved,
        //            total = (_price * _quantityApproved + iva) - _discount
        //        },
        //        OrderTotal = new
        //        {
        //            subtotal = salesOrder2.SalesOrderTotal.subtotal,
        //            subtotalIVA12Percent = salesOrder2.SalesOrderTotal.subtotalIVA12Percent,
        //            totalIVA12 = salesOrder2.SalesOrderTotal.totalIVA12,
        //            discount = salesOrder2.SalesOrderTotal.discount,
        //            subtotalIVA14Percent = salesOrder2.SalesOrderTotal.subtotalIVA14Percent,
        //            totalIVA14 = salesOrder2.SalesOrderTotal.totalIVA14,
        //            total = salesOrder2.SalesOrderTotal.total
        //        },
        //        quantityTypeUMSale = quantityTypeUMSale,
        //        metricUnitSale = metricUnitSale,
        //        quantityApproved = _quantityApproved,
        //        msgErrorConversion = msgErrorConversion,
        //        msgErrorDiscount = msgErrorDiscount,
        //        //id_metricUnitTypeUMPresentationDefault = item?.Presentation.id_metricUnit
        //    };

        //    TempData["salesOrder2"] = salesOrder2;
        //    TempData.Keep("salesOrder2");

        //    TempData["salesOrder"] = salesOrder;
        //    TempData.Keep("salesOrder");

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //private decimal SaleDetailPrice(int id_item, SalesOrder salesOrder, int? id_metricUnitTypeUMPresentation)
        //{
        //            Item item = db.Item.FirstOrDefault(i => i.id == id_item);
        //    MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);

        //    if (item == null || metricUnitTypeUMPresentation == null)
        //            {
        //                return 0.0M;
        //            }

        //            PriceList list = salesOrder.PriceList;

        //    decimal priceUMPresentation = item.ItemSaleInformation.salePrice / ((item.Presentation?.maximum ?? 1) * (item.Presentation?.minimum ?? 1)) ?? 0.0M;

        //    decimal priceAux = 0;
        //    var metricUnitPresentation = item.Presentation?.MetricUnit;

        //    if (metricUnitPresentation?.id != null)
        //    {
        //        var factorConversion = GetFactorConversion(metricUnitTypeUMPresentation, metricUnitPresentation, "Falta el Factor de Conversión entre : " + metricUnitTypeUMPresentation.code + " y " + (metricUnitPresentation.code) + ".Necesario para obtener el precio del detalle de la Orden de Produdcción Configúrelo, e intente de nuevo", db);
        //        //var factorConversion = (metricUnitTypeUMPresentation.id != metricUnitPresentation.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitTypeUMPresentation.id &&
        //        //                                                                                                                             fod.id_metricDestiny == metricUnitPresentation.id)?.factor ?? 0 : 1;
        //        //priceAux = 0;
        //        //if (factorConversion == 0)
        //        //{
        //        //    throw new Exception("Falta el Factor de Conversión entre : " + metricUnitTypeUMPresentation.code + " y " + (metricUnitPresentation.code) + ".Necesario para obtener el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

        //        //}
        //        //else
        //        //{
        //        priceAux = priceUMPresentation * factorConversion;
        //        //}
        //    }


        //    if (list != null)
        //            {
        //                PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);

        //        var priceListDetail = listDetail?.salePrice;

        //        if (priceListDetail != null)
        //        {
        //            var detailtPriceList = list?.PriceListDetail.FirstOrDefault(fod => fod.id_item == item.id);
        //            var metricUnitPriceList = detailtPriceList?.MetricUnit;

        //            if (metricUnitPriceList != null)
        //            {
        //                var metricUnitSale = item.ItemSaleInformation.MetricUnit;

        //                var metricUnitAux = metricUnitPriceList;
        //                decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

        //                if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
        //                {
        //                    metricUnitAux = item.Presentation?.MetricUnit;

        //                    //metricUnitDetail = metricUnitTypeUMPresentation;
        //                    var factorConversionAux = GetFactorConversion(metricUnitSale, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Orden de Producción Configúrelo, e intente de nuevo", db);
        //                    //var factorConversionAux = (metricUnitSale.id != metricUnitPriceList.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitSale.id &&
        //                    //                                                                                                                 fod.id_metricDestiny == metricUnitPriceList.id)?.factor ?? 0 : 1;
        //                    //priceAux = 0;
        //                    //if (factorConversionAux == 0)
        //                    //{
        //                    //    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

        //                    //}
        //                    //else
        //                    //{
        //                    priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (item.Presentation?.minimum ?? 1);
        //                    //}

        //                    //priceMetricUnitPresentation = detailtPriceList.salePrice / (item.Presentation?.minimum ?? 1);
        //                }

        //                var metricUnitDetail = metricUnitTypeUMPresentation;

        //                var factorConversion = GetFactorConversion(metricUnitDetail, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle de la Orden de Producción Configúrelo, e intente de nuevo", db);
        //                //var factorConversion = (metricUnitDetail.id != metricUnitAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitDetail.id &&
        //                //                                                                                                                 fod.id_metricDestiny == metricUnitAux.id)?.factor ?? 0 : 1;
        //                //priceAux = 0;
        //                //if (factorConversion == 0)
        //                //{
        //                //    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

        //                //}
        //                //else
        //                //{
        //                priceAux = priceMetricUnitPresentation * factorConversion;
        //                //}

        //        }
        //            else
        //            {
        //                priceAux = 0;
        //            }

        //        }
        //        else
        //        {
        //            priceAux = 0;
        //        }
        //    }

        //    return priceAux;
        //}

        //private decimal SaleDetailIVA(int id_item, decimal quantity, decimal price)
        //        {
        //            decimal iva = 0.0M;

        //            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

        //            if (item == null)
        //            {
        //                return 0.0M;
        //            }

        //            List<ItemTaxation> taxations = item.ItemTaxation.Where(w => w.TaxType.code.Equals("2")).ToList();//"2" Es el Impuesto de IVA

        //    foreach (var taxation in taxations)
        //            {
        //                iva += quantity * price * taxation.Rate.percentage / 100.0M;
        //            }

        //            return iva;
        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public JsonResult SalesOrderDetails()
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //            salesOrder = salesOrder ?? new SalesOrder();
        //            salesOrder.SalesOrderDetail = salesOrder.SalesOrderDetail ?? new List<SalesOrderDetail>();

        //            TempData.Keep("salesOrder");

        //            return Json(salesOrder.SalesOrderDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        //        }

        //        [HttpPost]
        //        public JsonResult ValidateSelectedRowsSalesRequest(int[] ids)
        //        {
        //            var result = new
        //            {
        //                Message = "OK"
        //            };

        //            PriceList priceListFirst = null;
        //            PriceList priceListCurrent = null;
        //            Customer customerFirst = null;
        //            Customer customerCurrent = null;
        //            int count = 0;
        //            foreach (var i in ids)
        //            {
        //                priceListCurrent = db.SalesRequestDetail.FirstOrDefault(fod => fod.id == i)?.SalesRequest.PriceList;
        //                customerCurrent = db.SalesRequestDetail.FirstOrDefault(fod => fod.id == i)?.SalesRequest.Customer;

        //                if (count == 0)
        //                {
        //                    priceListFirst = priceListCurrent;
        //                    customerFirst = customerCurrent;
        //                }
        //                //else
        //                //{
        //                //    if (providerFirst == null)
        //                //    {
        //                //        providerFirst = providerCurrent;
        //                //    }
        //                //}



        //                if (priceListFirst != null && priceListCurrent != null && priceListCurrent != priceListFirst)
        //                {
        //                    result = new
        //                    {
        //                        Message = ErrorMessage("No se pueden mezclar detalles con Lista de Precio diferente")
        //                    };
        //                    TempData.Keep("salesOrder");
        //                    return Json(result, JsonRequestBehavior.AllowGet);
        //                }

        //                if (customerFirst != null && customerCurrent != null && customerCurrent != customerFirst)
        //                {
        //                    result = new
        //                    {
        //                        Message = ErrorMessage("No se pueden mezclar detalles con clientes diferentes")
        //                    };
        //                    TempData.Keep("salesOrder");
        //                    return Json(result, JsonRequestBehavior.AllowGet);
        //                }
        //                count++;
        //            }

        //            TempData.Keep("salesOrder");
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }

        //        private decimal SalesOrderDetailIVA(int id_item, decimal quantity, decimal price)
        //        {
        //            decimal iva = 0.0M;

        //            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

        //            if (item == null)
        //            {
        //                return 0.0M;
        //            }

        //            List<ItemTaxation> taxations = item.ItemTaxation.ToList();

        //            foreach (var taxation in taxations)
        //            {
        //                iva += quantity * price * taxation.Rate.percentage / 100.0M;
        //            }

        //            return iva;
        //        }

        //        [HttpPost]
        //        public JsonResult OnPriceList_SelectedIndexChanged(int? id_priceList)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);
        //            salesOrder = salesOrder ?? new SalesOrder();

        //            var priceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
        //            salesOrder.PriceList = priceList;
        //            var listSalesOrderDetail = salesOrder.SalesOrderDetail.ToList();
        //            decimal priceAux = 0;

        //            foreach (var salesOrderDetail in listSalesOrderDetail)
        //            {

        //                if (priceList != null)
        //                {
        //                    var detailtPriceList = priceList?.PriceListDetail.FirstOrDefault(fod => fod.id_item == salesOrderDetail.id_item);
        //                    var metricUnitPriceList = detailtPriceList?.MetricUnit;

        //                    if (metricUnitPriceList != null)
        //                    {
        //                        var metricUnitSale = salesOrderDetail.Item.ItemSaleInformation.MetricUnit;

        //                        var metricUnitAux = metricUnitPriceList;
        //                        decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

        //                        if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
        //                        {
        //                            metricUnitAux = salesOrderDetail.Item.Presentation?.MetricUnit;
        //                            var factorConversionAux = GetFactorConversion(metricUnitSale, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Orden de Producción Configúrelo, e intente de nuevo", db);

        //                            priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (salesOrderDetail.Item.Presentation?.minimum ?? 1);

        //                        }

        //                        var metricUnitDetail = salesOrderDetail.MetricUnit;

        //                        var factorConversion = GetFactorConversion(metricUnitDetail, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para recoste el detalle de la Orden de Producción Configúrelo, e intente de nuevo", db);

        //                        priceAux = priceMetricUnitPresentation * factorConversion;
        //                    }
        //                    else
        //                    {
        //                        priceAux = 0;
        //                    }

        //                }
        //                else
        //                {
        //                    priceAux = 0;
        //                }

        //                salesOrderDetail.price = priceAux;
        //            }

        //            var result = new
        //            {
        //                Message = "Ok"
        //            };

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }

        //        [HttpPost]
        //        public ActionResult GetPriceList(int? id_customer)
        //        {
        //            TempData.Keep("salesOrder");
        //            var salesOrderAux = id_customer == null ? new SalesOrder() : new SalesOrder { id_customer = id_customer.Value };
        //            return PartialView("Component/_ComboBoxPriceList", salesOrderAux);
        //        }

        //        [HttpPost]
        //        public ActionResult GetItem(int? id_item)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);
        //            salesOrder = salesOrder ?? new SalesOrder();
        //            TempData.Keep("salesOrder");

        //            return GridViewExtension.GetComboBoxCallbackResult(p => {
        //                //settings.Name = "id_person";
        //                p.ClientInstanceName = "id_item";
        //                p.Width = Unit.Percentage(100);

        //                p.DropDownStyle = DropDownStyle.DropDownList;
        //                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
        //                p.EnableSynchronization = DefaultBoolean.False;
        //                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

        //                p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "GetItem" };
        //                p.CallbackPageSize = 5;
        //                //settings.Properties.EnableCallbackMode = true;
        //                //settings.Properties.TextField = "CityName";
        //                p.ClientSideEvents.BeginCallback = "SalesOrderItem_BeginCallback";
        //                p.ClientSideEvents.EndCallback = "SalesOrderItem_EndCallback";

        //                p.DataSource = DataProviderItem.SalesItemsPTByCompanyAndCurrent(this.ActiveCompanyId, id_item);
        //                p.ValueField = "id";
        //                //p.TextField = "name";
        //                p.TextFormatString = "{1}";
        //                p.ValueType = typeof(int);

        //                p.Columns.Add("masterCode", "Código", 70);
        //                p.Columns.Add("name", "Nombre del Producto", 300);
        //                p.Columns.Add("ItemSaleInformation.MetricUnit.code", "UM", 50);
        //                //p.ClientSideEvents.Init = "ItemCombo_OnInit";
        //                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
        //                p.ClientSideEvents.SelectedIndexChanged = "ItemCombo_SelectedIndexChanged";
        //                p.ClientSideEvents.Validation = "OnItemValidation";

        //                //p.TextField = textField;
        //                //p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

        //            });

        //            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        //        }

        //        [HttpPost]
        //        public ActionResult GetMetricUnitTypeUMPresentation(int? id_item, int? id_metricUnitTypeUMPresentation)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);
        //            salesOrder = salesOrder ?? new SalesOrder();
        //            TempData.Keep("salesOrder");

        //            return GridViewExtension.GetComboBoxCallbackResult(p => {
        //                //settings.Name = "id_person";
        //                p.ClientInstanceName = "id_metricUnitTypeUMPresentation";
        //                p.Width = Unit.Percentage(100);

        //                p.DropDownStyle = DropDownStyle.DropDownList;
        //                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
        //                p.EnableSynchronization = DefaultBoolean.False;
        //                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

        //                p.CallbackRouteValues = new { Controller = "SalesOrder", Action = "GetMetricUnitTypeUMPresentation" };
        //                p.CallbackPageSize = 5;
        //                //settings.Properties.EnableCallbackMode = true;
        //                //settings.Properties.TextField = "CityName";
        //                p.ClientSideEvents.BeginCallback = "SalesOrderMetricUnitTypeUMPresentation_BeginCallback";
        //                p.ClientSideEvents.EndCallback = "SalesOrderMetricUnitTypeUMPresentation_EndCallback";

        //                p.DataSource = DataProviderMetricUnit.MetricUnitTypeUMPresentation(this.ActiveCompanyId, id_item, id_metricUnitTypeUMPresentation);
        //                p.ValueField = "id";
        //                p.TextField = "code";
        //                p.Width = Unit.Percentage(100);
        //                p.ValueType = typeof(int);
        //                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
        //                p.DropDownStyle = DropDownStyle.DropDownList;
        //                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
        //                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
        //                p.ClientSideEvents.SelectedIndexChanged = "MetricUnitTypeUMPresentationCombo_SelectedIndexChanged";
        //                p.ClientSideEvents.Validation = "OnMetricUnitTypeUMPresentationValidation";

        //                //p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

        //            });

        //        }


        //        [HttpPost, ValidateInput(false)]
        //        public JsonResult ItsRepeatedItemDetail(int? id_itemNew)
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //            salesOrder = salesOrder ?? new SalesOrder();

        //            var result = new
        //            {
        //                itsRepeated = 0,
        //                Error = ""

        //            };


        //            var salesOrderDetailAux = salesOrder.SalesOrderDetail.FirstOrDefault(w => w.id_item == id_itemNew && w.isActive &&
        //                                                                                 (w.SalesOrderDetailSalesRequest == null || w.SalesOrderDetailSalesRequest.Count() <= 0) &&
        //                                                                                 (w.ProductionScheduleProductionOrderDetailSalesOrderDetail == null || w.ProductionScheduleProductionOrderDetailSalesOrderDetail.Count() <= 0));
        //            if (salesOrderDetailAux != null)
        //            {
        //                var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
        //                result = new
        //                {
        //                    itsRepeated = 1,
        //                    Error = "No se puede repetir el Ítem: " + itemNewAux.name + " en los detalles."

        //                };

        //            }

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");

        //            return Json(result, JsonRequestBehavior.AllowGet);

        //        }

        //        [HttpPost, ValidateInput(false)]
        //        public JsonResult UpdateSalesOrder2()
        //        {
        //            SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //            SalesOrder salesOrder2 = (TempData["salesOrder2"] as SalesOrder);

        //            salesOrder2 = Copy(salesOrder, salesOrder2);

        //            var result = new
        //            {
        //                Message = "OK"
        //            };

        //            TempData["salesOrder2"] = salesOrder2;
        //            TempData.Keep("salesOrder2");

        //            TempData["salesOrder"] = salesOrder;
        //            TempData.Keep("salesOrder");

        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }


        //        //[HttpPost, ValidateInput(false)]
        //        //public JsonResult ItemDetailData(int id_item, string quantityOrdered, string price)
        //        //{
        //        //    decimal _quantityOrdered = decimal.Parse(quantityOrdered, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));
        //        //    decimal _price = decimal.Parse(price, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));


        //        //    SalesOrder salesOrder = (TempData["salesOrder"] as SalesOrder);

        //        //    Item item = db.Item.FirstOrDefault(i => i.id == id_item);

        //        //    if (item == null)
        //        //    {
        //        //        return Json(null, JsonRequestBehavior.AllowGet);
        //        //    }

        //        //    if (_price == 0.0M)
        //        //    {
        //        //        _price = SalesDetailPrice(item.id, salesOrder);
        //        //    }

        //        //    decimal iva = SalesDetailIVA(item.id, _quantityOrdered, _price);

        //        //    SalesOrderDetail detail = salesOrder.SalesOrderDetail.FirstOrDefault(d => d.id_item == id_item);

        //        //    if (detail != null)
        //        //    {
        //        //        detail.price = _price;
        //        //        detail.quantityOrdered = _quantityOrdered;
        //        //        salesOrder.SalesOrderTotal = SalesOrderTotals(salesOrder.id, salesOrder.SalesOrderDetail.ToList());
        //        //    }

        //        //    var result = new
        //        //    {
        //        //        ItemDetailData = new
        //        //        {
        //        //            masterCode = item.masterCode,
        //        //            um = item.ItemSaleInformation.MetricUnit.code,
        //        //            price = _price,
        //        //            iva = iva,
        //        //            subtotal = _price * _quantityOrdered,
        //        //            total = _price * _quantityOrdered + iva
        //        //        },
        //        //        OrderTotal = new
        //        //        {
        //        //            subtotal = salesOrder.SalesOrderTotal.subtotal,
        //        //            subtotalIVA12Percent = salesOrder.SalesOrderTotal.subtotalIVA12Percent,
        //        //            totalIVA12 = salesOrder.SalesOrderTotal.totalIVA12,
        //        //            discount = salesOrder.SalesOrderTotal.discount,
        //        //            subtotalIVA14Percent = salesOrder.SalesOrderTotal.subtotalIVA14Percent,
        //        //            totalIVA14 = salesOrder.SalesOrderTotal.totalIVA14,
        //        //            total = salesOrder.SalesOrderTotal.total
        //        //        }
        //        //    };

        //        //    TempData["salesOrder"] = salesOrder;
        //        //    TempData.Keep("salesOrder");

        //        //    return Json(result, JsonRequestBehavior.AllowGet);
        //        //}


        //        private SalesOrder Copy(SalesOrder salesOrder, SalesOrder salesOrder2)
        //        {
        //            salesOrder2 = new SalesOrder()
        //            {
        //                id = 0,
        //                id_customer = salesOrder.id_customer,
        //                Customer = salesOrder.Customer,
        //                id_employeeSeller = salesOrder.id_employeeSeller,
        //                Employee = salesOrder.Employee,
        //                id_priceList = salesOrder.id_priceList,
        //                PriceList = salesOrder.PriceList,
        //            };
        //            foreach (var detail in salesOrder.SalesOrderDetail)
        //            {
        //                if (detail.Item != null && detail.isActive)
        //                {
        //                    var tempItemSODetail = new SalesOrderDetail
        //                    {
        //                        id = detail.id,
        //                        id_item = detail.id_item,
        //                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

        //                        quantityTypeUMSale = detail.quantityTypeUMSale,
        //                        quantityRequested = detail.quantityRequested,
        //                        quantityOrdered = detail.quantityOrdered,
        //                        quantityApproved = detail.quantityApproved,
        //                        quantityDelivered = detail.quantityDelivered,
        //                        id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
        //                        MetricUnit = detail.MetricUnit,

        //                        price = detail.price,
        //                        discount = detail.discount,
        //                        iva = detail.iva,

        //                        subtotal = detail.subtotal,
        //                        total = detail.total,

        //                        isActive = detail.isActive,
        //                        id_userCreate = detail.id_userCreate,
        //                        dateCreate = detail.dateCreate,
        //                        id_userUpdate = detail.id_userUpdate,
        //                        dateUpdate = detail.dateUpdate,

        //                        SalesOrderDetailSalesRequest = detail.SalesOrderDetailSalesRequest,
        //                        ProductionScheduleProductionOrderDetailSalesOrderDetail = detail.ProductionScheduleProductionOrderDetailSalesOrderDetail

        //                    };
        //                    salesOrder2.SalesOrderDetail.Add(tempItemSODetail);
        //                }

        //            }
        //            return salesOrder2;
        //        }

        //        #endregion



    }
}