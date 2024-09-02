using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
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
    [Authorize]
    public class InvoiceCommercialController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region Invoice Commercial EDITFORM

        private InvoiceCommercial GetInvoiceCommercial()
        {
            if (!(Session["invoiceCommercial"] is InvoiceCommercial invoiceCommercial))
                invoiceCommercial = new InvoiceCommercial();
            return invoiceCommercial;
        }

        private InvoiceCommercial saveInvoiceCommercialFromProforma(int id_invoice)
        {
            InvoiceCommercial NewInvoiceCommercial = new InvoiceCommercial();
            int id_company = (int)ViewData["id_company"];
            Invoice _invoice = new Invoice();
            string msgXtraInfo = "";

            try
            {
                msgXtraInfo = "Obtener Proforma";
                _invoice = db.Invoice.FirstOrDefault(r => r.id == id_invoice);

                msgXtraInfo = "Obtener Información Documento";
                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

                msgXtraInfo = "Obtener información Emisión Factura Comercial";
                List<SettingDetail> settingsInvoiceCommercial = db.Setting.FirstOrDefault(r => r.code == "INPFC").SettingDetail.ToList();
                String codeCompany = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CCIA").valueAux;
                String codeBranchOffice = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CESTB").valueAux;
                int codeEmissionPoint = int.Parse(settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CPTOE").valueAux);
                Company companyInvoiceCommercial = db.Company.FirstOrDefault(r => r.code == codeCompany);
                BranchOffice branchOfficeInvoiceCommercial = db.BranchOffice.FirstOrDefault(r => r.code == codeBranchOffice && r.id_company == companyInvoiceCommercial.id);
                EmissionPoint emissionPointInvoiceCommercial = db.EmissionPoint.FirstOrDefault(r => r.code == codeEmissionPoint);
                string emissionDate = _invoice.Document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                string numberString = GetDocumentSequential(documentType.id).ToString().PadLeft(9, '0');
                string documentNumber = $"{branchOfficeInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{emissionPointInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{numberString}";

                #region InvoiceCommercial Head

                msgXtraInfo = "Creación Documento";
                var _document = new Document
                {
                    id = 0,
                    id_documentType = documentType.id,
                    id_documentState = documentState.id,
                    DocumentType = documentType,
                    DocumentState = documentState,
                    id_emissionPoint = emissionPointInvoiceCommercial.id,
                    emissionDate = _invoice.Document.emissionDate,
                    description = _invoice.Document.description,
                    reference = _invoice.Document.reference,
                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    id_documentOrigen = id_invoice,
                    Document2 = db.Document.FirstOrDefault(s => s.id == id_invoice)
                };

                var foreignCustomer = db.ForeignCustomer.FirstOrDefault(r => r.id == _invoice.id_buyer);
                var foreignCustomerIdentification = db.ForeignCustomerIdentification.FirstOrDefault(r => r.id == _invoice.SalesQuotationExterior.id_addressCustomer);
                NewInvoiceCommercial.ForeignCustomer = foreignCustomer;
                NewInvoiceCommercial.Document = _document;
                NewInvoiceCommercial.id_documentInvoice = _invoice.id;
                NewInvoiceCommercial.id_ForeignCustomer = _invoice.id_buyer;
                NewInvoiceCommercial.id_Consignee = _invoice.SalesQuotationExterior.id_consignee;
                NewInvoiceCommercial.id_Notifier = _invoice.SalesQuotationExterior.id_notifier;
                NewInvoiceCommercial.idVendor = _invoice.SalesQuotationExterior.idVendor;
                NewInvoiceCommercial.id_InvoiceType = invoiceType.id;
                NewInvoiceCommercial.id_metricUnitInvoice = _invoice.SalesQuotationExterior.id_metricUnitInvoice;
                NewInvoiceCommercial.purchaseOrder = _invoice.SalesQuotationExterior.purchaseOrder;
                NewInvoiceCommercial.id_termsNegotiation = _invoice.SalesQuotationExterior.id_termsNegotiation;
                NewInvoiceCommercial.id_PaymentMethod = _invoice.SalesQuotationExterior.id_PaymentMethod;
                NewInvoiceCommercial.id_PaymentTerm = _invoice.SalesQuotationExterior.id_PaymentTerm;
                NewInvoiceCommercial.dateShipment = _invoice.SalesQuotationExterior.dateShipment;
                NewInvoiceCommercial.id_portDischarge = _invoice.SalesQuotationExterior.id_portDischarge;
                NewInvoiceCommercial.id_portDestination = _invoice.SalesQuotationExterior.id_portDestination;
                NewInvoiceCommercial.totalBoxesOrigen = (int)_invoice.SalesQuotationExterior.totalBoxes;
                NewInvoiceCommercial.totalWeightOrigen = (decimal)_invoice.InvoiceDetail.Sum(r => r.id_amountInvoice);
                NewInvoiceCommercial.totalBoxes = _invoice.SalesQuotationExterior.totalBoxes;
                NewInvoiceCommercial.totalWeight = (decimal)_invoice.InvoiceDetail.Sum(r => r.id_amountInvoice);
                NewInvoiceCommercial.id_portShipment = _invoice.SalesQuotationExterior.id_portShipment;
                NewInvoiceCommercial.numeroContenedores = _invoice.SalesQuotationExterior.numeroContenedores;
                NewInvoiceCommercial.id_capacityContainer = _invoice.SalesQuotationExterior.id_capacityContainer;
                NewInvoiceCommercial.shipName = _invoice.SalesQuotationExterior.vessel;
                NewInvoiceCommercial.shipNumberTrip = _invoice.SalesQuotationExterior.trip;
                NewInvoiceCommercial.id_BankTransfer = _invoice.SalesQuotationExterior.id_BankTransfer;
                NewInvoiceCommercial.id_addressCustomer = _invoice.SalesQuotationExterior.id_addressCustomer;
                NewInvoiceCommercial.ForeignCustomerIdentification = foreignCustomerIdentification;

                #endregion InvoiceCommercial Head

                foreach (var detail in _invoice.InvoiceDetail.Where(a => a.isActive))
                {
                    if (!(detail.proformaPendingNumBoxes > 0))
                        continue;
                    var invoiceCommercialDetail = new InvoiceCommercialDetail
                    {
                        // si existe descuento cammbiar el precio unitario
                        id = detail.id,
                        id_invoiceCommercial = detail.Invoice.id,
                        id_itemOrigen = detail.id_item,
                        Item = detail.Item,
                        id_metricUnitOrigen = (int)detail.id_metricUnitInvoiceDetail,
                        amountOrigen = detail.id_amountInvoice ?? 0,  // objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).,
                        codePresentationOrigen = detail.codePresentation,
                        presentationMinimumOrigen = detail.presentationMinimum,
                        presentationMaximumOrigen = detail.presentationMaximum,
                        numBoxesOrigen = (int)detail.numBoxes,
                        unitPriceOrigen = detail.unitPrice,
                        totalOrigen = detail.total,
                        id_item = detail.id_item,
                        Item1 = detail.Item,
                        id_metricUnit = detail.id_metricUnitInvoiceDetail,
                        amount = detail.id_amountInvoice ?? 0,
                        codePresentation = detail.codePresentation,
                        presentationMinimum = detail.presentationMinimum,
                        presentationMaximum = detail.presentationMaximum,
                        amountInvoice = detail.id_amountInvoice ?? 0,
                        id_itemMarked = detail.id_itemMarked,
                        Item2 = detail.Item1,

                        numBoxes = (int)detail.proformaPendingNumBoxes,
                        unitPrice = detail.unitPrice,
                        discount = detail.discount,
                        total = detail.total,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                    };

                    NewInvoiceCommercial.InvoiceCommercialDetail.Add(invoiceCommercialDetail);
                }
            }
            catch (Exception e)
            {
                LogWrite(e, null, "saveInvoiceCommercial==>" + msgXtraInfo);
                throw new Exception("Error al generar Factura Comercial desde Proforma");
            }

            return NewInvoiceCommercial;
        }

        private InvoiceCommercial saveInvoiceCommercial(int id_invoice)
        {
            InvoiceCommercial NewInvoiceCommercial = new InvoiceCommercial();
            int id_company = (int)ViewData["id_company"];
            Invoice _invoice = new Invoice();
            string msgXtraInfo = "";

            try
            {
                msgXtraInfo = "Obtener Factura Fiscal";
                _invoice = db.Invoice.FirstOrDefault(r => r.id == id_invoice);

                msgXtraInfo = "Obtener Información Documento";
                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

                msgXtraInfo = "Obtener información Emisión Factura Comercial";
                List<SettingDetail> settingsInvoiceCommercial = db.Setting.FirstOrDefault(r => r.code == "INPFC").SettingDetail.ToList();
                String codeCompany = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CCIA").valueAux;
                String codeBranchOffice = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CESTB").valueAux;
                int codeEmissionPoint = int.Parse(settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CPTOE").valueAux);
                Company companyInvoiceCommercial = db.Company.FirstOrDefault(r => r.code == codeCompany);
                BranchOffice branchOfficeInvoiceCommercial = db.BranchOffice.FirstOrDefault(r => r.code == codeBranchOffice && r.id_company == companyInvoiceCommercial.id);
                EmissionPoint emissionPointInvoiceCommercial = db.EmissionPoint.FirstOrDefault(r => r.code == codeEmissionPoint);
                string emissionDate = _invoice.Document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                string numberString = GetDocumentSequential(documentType.id).ToString().PadLeft(9, '0');
                string documentNumber = $"{branchOfficeInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{emissionPointInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{numberString}";

                #region InvoiceCommercial Head

                msgXtraInfo = "Creación Documento";
                Document _document = new Document();

                _document.id = 0;
                _document.id_documentType = documentType.id;
                _document.id_documentState = documentState.id;
                _document.DocumentType = documentType;
                _document.DocumentState = documentState;
                _document.id_emissionPoint = emissionPointInvoiceCommercial.id;
                _document.emissionDate = _invoice.Document.emissionDate;
                _document.description = _invoice.Document.description;
                _document.reference = _invoice.Document.reference;
                _document.id_userCreate = ActiveUser.id;
                _document.dateCreate = DateTime.Now;
                _document.id_userUpdate = ActiveUser.id;
                _document.dateUpdate = DateTime.Now;

                // Asignación de origen desde proforma
                _document.id_documentOrigen = _invoice.Document.id;
                _document.Document2 = _invoice.Document;

                ForeignCustomer foreignCustomer = db.ForeignCustomer.FirstOrDefault(r => r.id == _invoice.id_buyer);
                NewInvoiceCommercial.ForeignCustomer = foreignCustomer;
                NewInvoiceCommercial.Document = _document;

                NewInvoiceCommercial.id_documentInvoice = _invoice.id;
                NewInvoiceCommercial.id_ForeignCustomer = _invoice.id_buyer;
                NewInvoiceCommercial.id_Consignee = _invoice.InvoiceExterior.id_consignee;
                NewInvoiceCommercial.id_Notifier = _invoice.InvoiceExterior.id_notifier;
                NewInvoiceCommercial.id_InvoiceType = invoiceType.id;
                NewInvoiceCommercial.id_metricUnitInvoice = _invoice.InvoiceExterior.id_metricUnitInvoice;
                NewInvoiceCommercial.purchaseOrder = _invoice.InvoiceExterior.purchaseOrder;
                NewInvoiceCommercial.id_termsNegotiation = _invoice.InvoiceExterior.id_termsNegotiation;
                NewInvoiceCommercial.id_PaymentMethod = _invoice.InvoiceExterior.id_PaymentMethod;
                NewInvoiceCommercial.id_PaymentTerm = _invoice.InvoiceExterior.id_PaymentTerm;
                NewInvoiceCommercial.dateShipment = _invoice.InvoiceExterior.dateShipment;
                NewInvoiceCommercial.id_shippingAgency = _invoice.InvoiceExterior.id_shippingAgency;
                NewInvoiceCommercial.id_shippingLine = _invoice.InvoiceExterior.id_ShippingLine;
                NewInvoiceCommercial.id_portShipment = _invoice.InvoiceExterior.id_portShipment;
                NewInvoiceCommercial.id_portDischarge = _invoice.InvoiceExterior.id_portDischarge;
                NewInvoiceCommercial.id_portDestination = _invoice.InvoiceExterior.id_portDestination;
                NewInvoiceCommercial.idVendor = _invoice.InvoiceExterior.idVendor;
                NewInvoiceCommercial.shipName = _invoice.InvoiceExterior.shipName;
                NewInvoiceCommercial.shipNumberTrip = _invoice.InvoiceExterior.shipNumberTrip;
                NewInvoiceCommercial.daeNumber = _invoice.InvoiceExterior.daeNumber;
                NewInvoiceCommercial.daeNumber2 = _invoice.InvoiceExterior.daeNumber2;
                NewInvoiceCommercial.daeNumber3 = _invoice.InvoiceExterior.daeNumber3;
                NewInvoiceCommercial.daeNumber4 = _invoice.InvoiceExterior.daeNumber4;
                NewInvoiceCommercial.BLNumber = _invoice.InvoiceExterior.BLNumber;
                NewInvoiceCommercial.seals = _invoice.InvoiceExterior.seals;
                NewInvoiceCommercial.containers = _invoice.InvoiceExterior.containers;
                NewInvoiceCommercial.numeroContenedores = (int)_invoice.InvoiceExterior.numeroContenedores;
                NewInvoiceCommercial.id_capacityContainer = _invoice.InvoiceExterior.id_capacityContainer;
                NewInvoiceCommercial.id_tariffHeading = _invoice.InvoiceExterior.id_tariffHeading;
                NewInvoiceCommercial.totalBoxesOrigen = (int)_invoice.InvoiceExterior.totalBoxes;
                NewInvoiceCommercial.totalWeightOrigen = (decimal)_invoice.InvoiceDetail.Where(e => e.isActive).Sum(r => r.id_amountInvoice);
                NewInvoiceCommercial.totalValueOrigen = _invoice.InvoiceExterior.valuetotalCIF;

                NewInvoiceCommercial.valueDiscount = (decimal)_invoice.InvoiceDetail.Where(e => e.isActive).Sum(r => r.discount);
                NewInvoiceCommercial.totalBoxes = _invoice.InvoiceExterior.totalBoxes;
                NewInvoiceCommercial.totalWeight = (decimal)_invoice.InvoiceDetail.Where(e => e.isActive).Sum(r => r.id_amountInvoice);
                NewInvoiceCommercial.totalValue = _invoice.InvoiceExterior.valuetotalCIF;

                NewInvoiceCommercial.valueInternationalFreight = _invoice.InvoiceExterior.valueInternationalFreight;
                NewInvoiceCommercial.valueInternationalInsurance = _invoice.InvoiceExterior.valueInternationalInsurance;
                NewInvoiceCommercial.valueCustomsExpenditures = _invoice.InvoiceExterior.valueCustomsExpenditures;
                NewInvoiceCommercial.valueTransportationExpenses = _invoice.InvoiceExterior.valueTransportationExpenses;
                NewInvoiceCommercial.id_addressCustomer = _invoice.InvoiceExterior.id_addressCustomer;
                NewInvoiceCommercial.ForeignCustomerIdentification = _invoice.InvoiceExterior.ForeignCustomerIdentification;

                #endregion InvoiceCommercial Head

                #region PreProrrateo
                msgXtraInfo = "Prorrateo Incoterms";
                Decimal totalIncotermsValue = (_invoice.InvoiceExterior.valueInternationalFreight +
                                                _invoice.InvoiceExterior.valueInternationalInsurance +
                                                _invoice.InvoiceExterior.valueCustomsExpenditures +
                                                _invoice.InvoiceExterior.valueTransportationExpenses);

                var invoiceDetailCount = _invoice.InvoiceDetail?.Count() ?? 0;
                #endregion PreProrrateo

                // recalcular
                #region if condicion total
                if (totalIncotermsValue > 0 && invoiceDetailCount > 0)
                {
                    decimal fullTotal = totalIncotermsValue + _invoice.totalValue;
                    decimal totalDoc = _invoice.totalValue;

                    var objCalculateLine
                                        = _invoice.InvoiceDetail
                                                    .Where(r => r.isActive)
                                                    .Select(s => new
                                                    {
                                                        idLine = s.id,
                                                        newTotalLine = (((s.total * 100) / totalDoc) * (fullTotal / 100)),
                                                        cantidad = s.id_amountInvoice
                                                    }).Select(s1 => new
                                                    {
                                                        idLine = s1.idLine,
                                                        newTotalLine = s1.newTotalLine,
                                                        newPrecio = (s1.newTotalLine / s1.cantidad)
                                                    }).ToList();

                    #region ForEachDetail New calculo
                    foreach (InvoiceDetail detail in _invoice.InvoiceDetail.Where(r => r.isActive).ToList())
                    {
                        Item item = db.Item.FirstOrDefault(r => r.id == detail.id_item);

                        InvoiceCommercialDetail invoiceCommercial = new InvoiceCommercialDetail
                        {
                            id_invoiceCommercial = detail.Invoice.id,
                            id_itemOrigen = detail.id_item,
                            id_metricUnitOrigen = (int)detail.id_metricUnitInvoiceDetail,
                            amountOrigen = (int)detail.id_amountInvoice,  // objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).,
                            codePresentationOrigen = detail.codePresentation,
                            presentationMinimumOrigen = detail.presentationMinimum,
                            presentationMaximumOrigen = detail.presentationMaximum,
                            numBoxesOrigen = (int)detail.numBoxes,
                            unitPriceOrigen = (decimal)objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).newPrecio,
                            totalOrigen = (decimal)objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).newTotalLine,
                            id_item = detail.id_item,
                            id_itemMarked = detail.id_itemMarked,
                            id_metricUnit = detail.id_metricUnitInvoiceDetail,
                            amount = (int)detail.id_amountInvoice,
                            codePresentation = detail.codePresentation,
                            presentationMinimum = detail.presentationMinimum,
                            presentationMaximum = detail.presentationMaximum,
                            amountInvoice = (int)detail.id_amountInvoice,
                            numBoxes = (int)detail.numBoxes,
                            unitPrice = (decimal)objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).newPrecio,
                            total = (decimal)objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).newTotalLine,
                            isActive = true,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            Item2 = item,
                            Item1 = item,
                            Item = item
                        };

                        NewInvoiceCommercial.InvoiceCommercialDetail.Add(invoiceCommercial);
                    }

                    #endregion ForEachDetail New calculo
                }
                else
                {
                    if (invoiceDetailCount > 0)
                    {
                        #region ForEachDetail Insert Detail
                        foreach (var detail in _invoice.InvoiceDetail.Where(r => r.isActive).ToList())
                        {
                            var invoiceCommercialDetail = new InvoiceCommercialDetail
                            {
                                // si existe descuento cammbiar el precio unitario
                                id = detail.id,
                                id_invoiceCommercial = detail.Invoice.id,
                                id_itemOrigen = detail.id_item,
                                Item = detail.Item,
                                id_metricUnitOrigen = (int)detail.id_metricUnitInvoiceDetail,
                                amountOrigen = (int)detail.id_amountInvoice,  // objCalculateLine.FirstOrDefault(r => r.idLine == detail.id).,
                                codePresentationOrigen = detail.codePresentation,
                                presentationMinimumOrigen = detail.presentationMinimum,
                                presentationMaximumOrigen = detail.presentationMaximum,
                                numBoxesOrigen = (int)detail.numBoxes,
                                unitPriceOrigen = detail.unitPrice,
                                totalOrigen = detail.total,
                                id_item = detail.id_item,
                                Item1 = detail.Item,
                                id_itemMarked = detail.id_itemMarked,
                                Item2 = detail.Item,
                                id_metricUnit = detail.id_metricUnitInvoiceDetail,
                                amount = (int)detail.id_amountInvoice,
                                codePresentation = detail.codePresentation,
                                presentationMinimum = detail.presentationMinimum,
                                presentationMaximum = detail.presentationMaximum,
                                amountInvoice = (int)detail.id_amountInvoice,
                                numBoxes = (int)detail.numBoxes,
                                unitPrice = detail.unitPrice,
                                discount = detail.discount,
                                total = detail.total,
                                isActive = true,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now,
                            };

                            NewInvoiceCommercial.InvoiceCommercialDetail.Add(invoiceCommercialDetail);
                        }
                        #endregion ForEachDetail Insert Detail
                    }
                }
                #endregion if condicion total
            }
            catch (Exception e)
            {
                LogWrite(e, null, "saveInvoiceCommercial==>" + msgXtraInfo);
                throw new Exception("Error al generar Factura Fiscal desde Comercial");
            }

            return NewInvoiceCommercial;
        }

        private InvoiceCommercial getInvoiceCommercial(int id_invoice)
        {
            InvoiceCommercial invoiceCommercial = new InvoiceCommercial();
            string msgXtraInfo = "";
            try
            {
                msgXtraInfo = "Obtener Factura Comercial";
                invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id_invoice);

                if (invoiceCommercial == null)
                {
                    msgXtraInfo = "Obtener Tipos Documento";
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("70"));//Factura Comercial 70
                    DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//Estado Pendiente 01

                    msgXtraInfo = "Creación Factura Comercial";
                    invoiceCommercial = new InvoiceCommercial
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
                        //id_responsable = employee?.id ?? 0,
                        //Employee = employee,
                        InvoiceCommercialDetail = new List<InvoiceCommercialDetail>()
                    };
                    invoiceCommercial.hasGlaze = true;
                }
                else
                {
                    msgXtraInfo = "Descartar Detalles Eliminados";
                    List<InvoiceCommercialDetail> lstInvComDetail = new List<InvoiceCommercialDetail>();
                    // TODO:2018-07-12:{RA} : Eliminar el detalle de la factura comercial
                    lstInvComDetail = invoiceCommercial.InvoiceCommercialDetail.Where(w => w.isActive).ToList();
                    invoiceCommercial.InvoiceCommercialDetail = lstInvComDetail;

                    invoiceCommercial.hasGlaze = (invoiceCommercial.hasGlaze == null) ? true : invoiceCommercial.hasGlaze;
                }

                msgXtraInfo = "Unidad de Medida Predeterminada";
                if (invoiceCommercial.id_metricUnitInvoice == 0 || invoiceCommercial.id_metricUnitInvoice == null)
                {
                    /*
						string codeMetricUnitDefault = db.Setting.FirstOrDefault(r=> r.code  == "FECUM")?.value ?? "";
						if( !String.IsNullOrEmpty(codeMetricUnitDefault))
						{
							MetricUnit metricUnitDefault = db.MetricUnit.FirstOrDefault(r => r.code  == codeMetricUnitDefault);
							if (metricUnitDefault != null) invoiceCommercial.id_metricUnitInvoice = metricUnitDefault.id;
						}
					*/
                    // TODO:2018-07-12:{RA} : Configurar la unidad de medida predeterminada
                    invoiceCommercial.id_metricUnitInvoice = 999;
                }
            }
            catch (Exception e)
            {
                LogWrite(e, null, "getInvoiceCommercial==>" + msgXtraInfo);
                throw new Exception("Error al crear Factura Comercial");
            }

            return invoiceCommercial;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialFormEditPartial(int id, int? id_InvoiceExterior)
        {
            InvoiceCommercial invoiceCommercial = new InvoiceCommercial();
            try
            {
                if (id_InvoiceExterior != null) invoiceCommercial = saveInvoiceCommercial((int)id_InvoiceExterior);
                if (id_InvoiceExterior == null) invoiceCommercial = getInvoiceCommercial(id);

                string codeDocumentState = db.Document.FirstOrDefault(r => r.id == id)?.DocumentState?.code ?? "01";

                invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);

                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "InvoiceCommercialFormEditPartial");
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            return PartialView("_FormEditInvoiceCommercial", invoiceCommercial);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationList(bool isCallback = false)
        {
            var idsInvoiceComercial = db.Document.Where(e => e.DocumentType.code == "70" && e.DocumentState.code != "05" && e.id_documentOrigen != null).Select(a => a.id_documentOrigen).ToArray();

            var model = SalesQuotationExteriorDapper
                .RecuperarProformasPorFacturar()
                .Where(s => !idsInvoiceComercial.Contains(s.id)).ToList();

            if (isCallback)
            {
                return PartialView("_SalesQuotationGridView", model);
            }
            else
            {
                return PartialView("_SalesQuotationList", model);
            }
        }

        public JsonResult SetAddressCustomer(int? id_consignee)
        {
            var result = db.ForeignCustomerIdentification.Where(fod => fod.id_ForeignCustomer == id_consignee).Select(s => new
            {
                id = s.id,
                tipoDireccion = db.tbsysCatalogueDetail.FirstOrDefault(a => a.id == s.AddressType).name,
                name = s.addressForeign,
                emailInterno = s.emailInterno,
                emailInternoCC = s.emailInternoCC,
                phone1FC = s.phone1FC,
                fax1FC = s.fax1FC
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInvoiceExteriorFromSalesQuotation(int id)
        {
            var invoiceCommercial = new InvoiceCommercial();
            try
            {
                invoiceCommercial = saveInvoiceCommercialFromProforma(id);

                invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", "01", ActiveUser.id);

                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "InvoiceCommercialFormEditPartial");
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            return PartialView("_FormEditInvoiceCommercial", invoiceCommercial);
        }

        #endregion Invoice Commercial EDITFORM

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult InvoiceCommercialResultsPartial(int[] customers, string identification, int[] consignees, int[] notifiers, int[] notifiers2, //Cliente
                                                            int? id_documentState, DateTime? startEmissionDate, DateTime? endEmissionDate, string numberInvoiceFiscal, //Document
                                                            DateTime? startDateShipment, DateTime? endDateShipment, int?[] id_shippingAgencys, int?[] id_portDischarges, int?[] id_portDestinations, string BLNumber,
                                                            string referenceInvoice
                                                            )//Embarque
        {
            List<InvoiceCommercial> model = db.InvoiceCommercial.ToList();
            string msgXtraInfo = "";

            try
            {
                model = findResult
                        (
                            customers,
                            identification,
                            consignees,
                            notifiers,
                            notifiers2,
                             //Cliente
                             id_documentState,
                            startEmissionDate,
                            endEmissionDate,
                            numberInvoiceFiscal,
                            //Document
                            startDateShipment,
                            endDateShipment,
                            id_shippingAgencys,
                            id_portDischarges,
                            id_portDestinations,
                            BLNumber,
                            referenceInvoice
                        );

                TempData["model"] = model;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "InvoiceCommercialResultsPartial==>" + msgXtraInfo);
            }
            finally
            {
                TempData.Keep("model");
            }

            return PartialView("_InvoiceCommercialResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        private Boolean validateHavingParameter
        (
                int[] customers,
                string identification,
                int[] consignees,
                int[] notifiers,
                int[] notifiers2,
                //Cliente
                int? id_documentState,
                DateTime? startEmissionDate,
                DateTime? endEmissionDate,
                string numberInvoiceFiscal,
                //Document
                DateTime? startDateShipment,
                DateTime? endDateShipment,
                int?[] id_shippingAgencys,
                int?[] id_portDischarges,
                int?[] id_portDestinations,
                string BLNumber,
                string referenceInvoice

        )
        {
            Boolean havingParameter = false;

            if (customers != null && customers.Length > 0) return true;
            if (!string.IsNullOrEmpty(identification)) return true;
            if (consignees != null && consignees.Length > 0) return true;
            if (notifiers != null && notifiers.Length > 0) return true;
            if (notifiers2 != null && notifiers2.Length > 0) return true;
            if (id_documentState != null && id_documentState != 0) return true;
            if (startEmissionDate != null) return true;
            if (endEmissionDate != null) return true;
            if (!string.IsNullOrEmpty(numberInvoiceFiscal)) return true;
            if (startDateShipment != null) return true;
            if (endDateShipment != null) return true;

            if (id_shippingAgencys != null && id_shippingAgencys.Length > 0) return true;
            if (id_portDischarges != null && id_portDischarges.Length > 0) return true;
            if (id_portDestinations != null && id_portDestinations.Length > 0) return true;
            if (!string.IsNullOrEmpty(BLNumber)) return true;
            if (!string.IsNullOrEmpty(referenceInvoice)) return true;

            return havingParameter;
        }

        private List<InvoiceCommercial> findResult
            (
                int[] customers,
                string identification,
                int[] consignees,
                int[] notifiers,
                int[] notifiers2,
                //Cliente
                int? id_documentState,
                DateTime? startEmissionDate,
                DateTime? endEmissionDate,
                string numberInvoiceFiscal,
                //Document
                DateTime? startDateShipment,
                DateTime? endDateShipment,
                int?[] id_shippingAgencys,
                int?[] id_portDischarges,
                int?[] id_portDestinations,
                string BLNumber,
                string referenceInvoice
            )
        {
            List<InvoiceCommercial> model = new List<InvoiceCommercial>();

            string msgXtraInfo = "";

            try
            {
                model = db.InvoiceCommercial.ToList();

                #region Customer

                msgXtraInfo = "Cliente";
                //consignees
                if (customers != null && customers.Length > 0)
                {
                    model = model.Where(r => customers.Contains(r.id_ForeignCustomer)).ToList();
                }

                msgXtraInfo = "Identficación";
                //identification
                if (!string.IsNullOrEmpty(identification))
                {
                    model = model.Where(o => o.ForeignCustomer.Person.identification_number.Contains(identification)).ToList();
                }

                msgXtraInfo = "Consignatario";
                //consignees
                if (consignees != null && consignees.Length > 0)
                {
                    model = model.Where(r => r.id_Consignee.HasValue && consignees.Contains(r.id_Consignee.Value)).ToList();
                }

                msgXtraInfo = "Notificador 1";
                //notifiers
                if (notifiers != null && notifiers.Length > 0)
                {
                    model = model.Where(r => r.id_Notifier.HasValue && notifiers.Contains(r.id_Notifier.Value)).ToList();
                }

                msgXtraInfo = "Notificador 2";
                //notifiers2
                if (notifiers2 != null && notifiers2.Length > 0)
                {
                    model = model.Where(r => r.id_Notifier2.HasValue && notifiers2.Contains(r.id_Notifier2.Value)).ToList();
                }

                #endregion Customer

                #region Document

                msgXtraInfo = "Estado del Documento";
                //id_documentState
                if (id_documentState != 0 && id_documentState != null)
                {
                    model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
                }

                //startEmissionDate
                if (startEmissionDate != null)
                {
                    model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
                }

                //endEmissionDate
                if (endEmissionDate != null)
                {
                    model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
                }

                //numberInvoiceFiscal
                if (!string.IsNullOrEmpty(numberInvoiceFiscal))
                {
                    model = model.Where(o => o.Document.number.Contains(numberInvoiceFiscal)).ToList();
                }

                if (!string.IsNullOrEmpty(referenceInvoice))
                {
                    model = model.Where(o => o.referenceInvoice?.Contains(referenceInvoice) ?? false).ToList();
                }

                #endregion Document

                #region Shipment
                msgXtraInfo = "Datos de Embarque";
                //startDateShipment
                if (startDateShipment != null)
                {
                    model = model
                                .Where(o => o.dateShipment.HasValue
                                            && DateTime.Compare(startDateShipment.Value.Date, o.dateShipment.Value.Date) <= 0)
                                 .ToList();
                }

                //endDateShipment
                if (endDateShipment != null)
                {
                    model = model
                               .Where(o => o.dateShipment.HasValue
                                            && DateTime.Compare(o.dateShipment.Value.Date, endDateShipment.Value.Date) <= 0)
                               .ToList();
                }

                //id_shippingAgencys
                if (id_shippingAgencys != null && id_shippingAgencys.Length > 0)
                {
                    var tempModel = new List<InvoiceCommercial>();
                    foreach (var m in model)
                    {
                        if (id_shippingAgencys.Contains(m.id_shippingAgency))
                        {
                            tempModel.Add(m);
                        }
                    }
                    model = model
                                .Where(r => r.id_shippingAgency.HasValue
                                            && id_shippingAgencys.Contains(r.id_shippingAgency.Value))
                                 .ToList();
                }

                //id_portDischarges
                if (id_portDischarges != null && id_portDischarges.Length > 0)
                {
                    // && id_portDischarges.Contains(r.id_portDischarge )
                    model = model
                              .Where(r => r.id_portDischarge.HasValue && id_portDischarges.Contains(r.id_portDischarge))
                              .ToList();
                }

                //id_portDestinations
                if (id_portDestinations != null && id_portDestinations.Length > 0)
                {
                    model = model
                                .Where(r => r.id_portDestination.HasValue && id_portDestinations.Contains(r.id_portDestination.Value))
                                .ToList();
                }

                //BLNumber
                if (!string.IsNullOrEmpty(BLNumber))
                {
                    model = model.Where(o => o.BLNumber?.Contains(BLNumber) ?? false).ToList();
                }

                #endregion Shipment
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "findResult==>" + msgXtraInfo);
            }

            return model;
        }

        #endregion ResultGridView

        #region Invoice Commercial

        [HttpPost]
        public ActionResult InvoiceCommercialsPartial()
        {
            List<InvoiceCommercial> model = new List<InvoiceCommercial>();
            try
            {
                model = (TempData["model"] as List<InvoiceCommercial>);
                model = model ?? new List<InvoiceCommercial>();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "InvoiceCommercialsPartial");
            }
            finally
            {
                TempData.Keep("model");
            }

            return PartialView("_InvoiceCommercialsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialsAddNew
            (bool approve,
            InvoiceCommercial item,
            string descriptionInvoiceCommercial,
            Document itemDoc,
            string descriptionDocument,
            string purchaseOrderInvoiceCommercial)
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            string msgXtraInfo = "";
            string msgOK = " guardada exitosamente";

            try
            {
                //if (!ModelState.IsValid) throw new Exception("Error en los datos ingresados.");

                #region Invoice Commercial Head

                msgXtraInfo = "Datos Cabecera Comercial";
                int id_InvoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.code == "EXT")?.id ?? 0;
                item.description = descriptionInvoiceCommercial;
                item.id_metricUnitInvoice = (invoiceCommercial.id_metricUnitInvoice == 999) ? null : invoiceCommercial.id_metricUnitInvoice;
                //item.purchaseOrder = purchaseOrderInvoiceCommercial;
                item.isChargeInUnitPrice = true;
                item.id_InvoiceType = id_InvoiceType;

                #endregion Invoice Commercial Head

                #region Document

                // Obtener Setting Compañía y Punto de Emision Predeterminada para Factura Comercial
                string codeFacturaComercialCompany = db.Setting.FirstOrDefault(r => r.code == "CIAFEC")?.value;
                string codeFacturaComercialEmissionPoint = db.Setting.FirstOrDefault(r => r.code == "EMPFEC")?.value;
                if (codeFacturaComercialCompany == null && codeFacturaComercialEmissionPoint == null)
                {
                    throw new Exception("No existe valores de configuración para Empresa y Punto de Emisión prederminado para Factura Comercial");
                }

                int intCodeFacturaComercialEmissionPoint = int.Parse(codeFacturaComercialEmissionPoint);

                int? id_facturaComercialCompany = db.Company.FirstOrDefault(r => r.code == codeFacturaComercialCompany)?.id;
                if (id_facturaComercialCompany == null || id_facturaComercialCompany == 0)
                {
                    throw new Exception("La configuración para Empresa prederminado para Factura Comercial es incorrecta");
                }

                int? id_emissionPoint = db.EmissionPoint.FirstOrDefault(r => r.code == intCodeFacturaComercialEmissionPoint && r.id_company == id_facturaComercialCompany)?.id;
                if (id_emissionPoint == null || id_emissionPoint == 0)
                {
                    throw new Exception("No existe Punto de Emision correspondiente con la configuración prederminado para Factura Comercial");
                }

                item.Document = item.Document ?? new Document();
                item.Document.id_userCreate = ActiveUser.id;
                item.Document.dateCreate = DateTime.Now;
                item.Document.id_userUpdate = ActiveUser.id;
                item.Document.dateUpdate = DateTime.Now;

                InvoiceCommercial invoiceTemp = null;
                if (TempData["invoiceCommercial"] is InvoiceCommercial)
                    invoiceTemp = (TempData["invoiceCommercial"] as InvoiceCommercial);

                if (invoiceTemp != null)
                {
                    item.Document.id_documentOrigen = invoiceTemp.Document.id_documentOrigen;
                    item.Document.Document2 = db.Document.FirstOrDefault(s => s.id == item.Document.id_documentOrigen);
                }

                //Factura Comercial 70
                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "70");
                if (documentType == null)
                {
                    throw new Exception("No se puede guardar la Factura Comercial porque no existe el Tipo de Documento: Factura Commercial con Código: 70, configúrelo e inténtelo de nuevo");
                }

                item.Document.id_documentType = documentType.id;
                item.Document.DocumentType = documentType;

                item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                item.Document.number = GetDocumentNumber(item.Document.id_documentType, (int)id_emissionPoint);

                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                if (documentState == null)
                {
                    throw new Exception("No se puede guardar la Factura Comercial porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                }

                item.Document.id_documentState = documentState.id;
                item.Document.DocumentState = documentState;

                item.Document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_emissionPoint);
                item.Document.id_emissionPoint = (int)id_emissionPoint; //ActiveEmissionPoint.id;

                item.Document.emissionDate = itemDoc.emissionDate;
                item.Document.description = descriptionDocument;

                //Actualiza Secuencial
                if (documentType != null)
                {
                    documentType.currentNumber = documentType.currentNumber + 1;
                    db.DocumentType.Attach(documentType);
                    db.Entry(documentType).State = EntityState.Modified;
                }

                #endregion Document

                #region InvoiceCommercial

                //item.Employee = db.Employee.FirstOrDefault(fod => fod.id == item.id_responsable);
                //openingClosingPlateLying.Employee = item.Employee;

                //item.Language = db.Language.FirstOrDefault(fod=> fod.id == item.id_Language);
                //invoiceCommercial.Language = item.Language;

                item.purchaseOrder = invoiceCommercial.purchaseOrder;
                item.id_termsNegotiation = invoiceCommercial.id_termsNegotiation;
                if (item.idVendor == null)
                {
                    item.idVendor = invoiceCommercial.idVendor;
                }
                //item.id_ForeignCustomer = invoiceCommercial.id_ForeignCustomer;
                if (item.id_Consignee == null)
                {
                    item.id_Consignee = invoiceCommercial.id_Consignee;
                }
                item.ForeignCustomer1 = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Consignee);//id_Consignee
                item.ForeignCustomer = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_ForeignCustomer);//id_Consignee
                                                                                                                   //item.id_Notifier = invoiceCommercial.id_Notifier;
                item.ForeignCustomer2 = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Notifier);//id_Notifier
                if (item.id_BankTransfer == null)
                {
                    item.id_BankTransfer = invoiceCommercial.id_BankTransfer;
                }
                item.id_addressCustomer = invoiceCommercial.id_addressCustomer;
                item.ForeignCustomerIdentification = db.ForeignCustomerIdentification.FirstOrDefault(fod => fod.id == item.id_addressCustomer);

                //item.id_company = this.ActiveCompanyId;
                //invoiceCommercial.id_company = item.id_company;

                #endregion InvoiceCommercial

                #region InvoiceCommercialContainer

                item.InvoiceCommercialContainer = new List<InvoiceCommercialContainer>();
                if (invoiceCommercial.InvoiceCommercialContainer != null)
                {
                    foreach (var detail in invoiceCommercial.InvoiceCommercialContainer)
                    {
                        var invoiceCommercialContainer = new InvoiceCommercialContainer();
                        invoiceCommercialContainer.id_invoiceCommercial = item.id;
                        invoiceCommercialContainer.numberContainer = detail.numberContainer;
                        invoiceCommercialContainer.isActive = true;
                        invoiceCommercialContainer.dateCreate = DateTime.Now;
                        invoiceCommercialContainer.id_userUpdate = ActiveUser.id;
                        invoiceCommercialContainer.dateUpdate = DateTime.Now;
                        invoiceCommercialContainer.id_userCreate = ActiveUser.id;

                        item.InvoiceCommercialContainer.Add(invoiceCommercialContainer);
                    }
                }
                /*
				if (item.InvoiceCommercialContainer.Count == 0)
				{
					//TempData.Keep("openingClosingPlateLying");
					//ViewData["EditMessage"] = ErrorMessage("No se puede guardar una tumbada sin detalles");
					//return PartialView("_OpeningClosingPlateLyingEditFormPartial", item);
					throw new Exception("No se puede guardar una Factura Comercial sin detalles de Contenedores.");
				}*/

                #endregion InvoiceCommercialContainer

                #region InvoiceCommercialDetail

                item.InvoiceCommercialDetail = new List<InvoiceCommercialDetail>();
                if (invoiceCommercial.InvoiceCommercialDetail != null)
                {
                    foreach (var detail in invoiceCommercial.InvoiceCommercialDetail)
                    {
                        var invoiceCommercialDetail = new InvoiceCommercialDetail();
                        invoiceCommercialDetail.id_invoiceCommercial = item.id;
                        invoiceCommercialDetail.id_item = detail.id_item;

                        invoiceCommercialDetail.codePresentation = detail.codePresentation;
                        invoiceCommercialDetail.presentationMinimum = detail.presentationMinimum;
                        invoiceCommercialDetail.presentationMaximum = detail.presentationMaximum;
                        invoiceCommercialDetail.numBoxes = detail.numBoxes;
                        invoiceCommercialDetail.id_metricUnit = detail.id_metricUnit;
                        invoiceCommercialDetail.amount = detail.amount;
                        invoiceCommercialDetail.amountInvoice = detail.amountInvoice;
                        invoiceCommercialDetail.unitPrice = detail.unitPrice;
                        invoiceCommercialDetail.discount = detail.discount;
                        invoiceCommercialDetail.total = detail.total;
                        invoiceCommercialDetail.glazePercentageDetail = detail.glazePercentageDetail;
                        invoiceCommercialDetail.isActive = detail.isActive;
                        invoiceCommercialDetail.dateCreate = DateTime.Now;
                        invoiceCommercialDetail.id_userUpdate = ActiveUser.id;
                        invoiceCommercialDetail.dateUpdate = DateTime.Now;
                        invoiceCommercialDetail.id_userCreate = ActiveUser.id;

                        invoiceCommercialDetail.id_itemOrigen = detail.id_itemOrigen;
                        invoiceCommercialDetail.numBoxesOrigen = detail.numBoxesOrigen;
                        invoiceCommercialDetail.amountOrigen = detail.amountOrigen;
                        invoiceCommercialDetail.presentationMaximumOrigen = detail.presentationMaximumOrigen;
                        invoiceCommercialDetail.presentationMinimumOrigen = detail.presentationMinimumOrigen;
                        invoiceCommercialDetail.unitPriceOrigen = detail.unitPriceOrigen;
                        invoiceCommercialDetail.id_metricUnitOrigen = detail.id_metricUnitOrigen;
                        invoiceCommercialDetail.codePresentationOrigen = detail.codePresentationOrigen;
                        invoiceCommercialDetail.totalOrigen = detail.totalOrigen;
                        invoiceCommercialDetail.weightBox = detail.weightBox;
                        invoiceCommercialDetail.factorMetricUnit = detail.factorMetricUnit;
                        invoiceCommercialDetail.id_itemMarked = detail.id_itemMarked;

                        /*invoiceCommercialDetail.Item = detail.Item;
						invoiceCommercialDetail.Item1 = detail.Item1;*/
                        item.InvoiceCommercialDetail.Add(invoiceCommercialDetail);
                    }
                }

                if (item.InvoiceCommercialDetail.Where(d => d.isActive).ToList().Count == 0)
                {
                    //TempData.Keep("openingClosingPlateLying");
                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar una tumbada sin detalles");
                    //return PartialView("_OpeningClosingPlateLyingEditFormPartial", item);
                    throw new Exception("No se puede guardar una Factura Comercial sin detalles de Producto.");
                }

                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == item.Document.id_documentOrigen);
                if (proforma != null)
                {
                    foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(s => s.isActive))
                    {
                        var invoiceDetail = item.InvoiceCommercialDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                        if (invoiceDetail != null)
                        {
                            if (proformaDetail.proformaPendingNumBoxes < invoiceDetail.numBoxes)
                                throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                    " debe ser menor e igual a: " + proformaDetail.proformaPendingNumBoxes.Value.ToString("#,##0") +
                                                    " que son los cartones pendiente en la Proforma.");
                        }
                    }
                }
                #endregion InvoiceCommercialDetail

                #region InvoiceCommercialDocument

                if (invoiceCommercial?.InvoiceCommercialDocument != null)
                {
                    item.InvoiceCommercialDocument = new List<InvoiceCommercialDocument>();
                    var itemInvoiceCommercialDocument = invoiceCommercial.InvoiceCommercialDocument.ToList();

                    foreach (var detail in itemInvoiceCommercialDocument)
                    {
                        var tempItemInvoiceCommercialDocument = new InvoiceCommercialDocument
                        {
                            guid = detail.guid,
                            url = detail.url,
                            attachment = detail.attachment,
                            referenceDocument = detail.referenceDocument,
                            descriptionDocument = detail.descriptionDocument
                        };

                        item.InvoiceCommercialDocument.Add(tempItemInvoiceCommercialDocument);
                    }
                }

                #endregion InvoiceCommercialDocument

                UpdateInvoiceCommercialTotals(item);
                TempData["invoiceCommercial"] = item;

                if (approve)
                {
                    item.ValidateInfo("03");
                    //ServiceInventoryMove.UpdateInventaryMoveTransferOpeningClosingPlateLying(ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                    item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    msgOK = " Aprobada Exitosamente";

                    if (item.Document.id_documentOrigen != null)
                        ServiceSalesQuotationExterior.UpdateValuesFromInvoiceComercial(item, db);
                }
                else
                {
                    item.ValidateInfo("01");
                }

                #region TRANSACCION
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.InvoiceCommercial.Add(item);

                        #region Actualizamos los datos de las proformas
                        var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                        if (puedeModDatosProforma)
                        {
                            UpdateSalesQuotationExterior(item.Document, item);
                            UpdateInvoicesExterior(item.Document.id, item.Document.id_documentOrigen, item);
                        } 
                        #endregion

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceCommercial"] = item;
                        ViewData["EditMessage"] = SuccessMessage("Factura Comercial: " + item.Document.number + msgOK);
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();

                        item.Document.accessKey = null;
                        item.Document.number = null;
                        item.Document.sequential = 0;
                        TempData["invoiceCommercial"] = item;

                        /*Prepar mensaje de error Post-Validacion */
                        string msgErr = "";
                        if (e.Data["source"] != null)
                        {
                            if ((string)e.Data["source"] == "modelDocumentValidation" && approve)
                                msgErr = "Para Aprobar el presente documento los siguientes campos son requeridos:<br>"
                                                + Environment.NewLine
                                                + e.Message;
                        }
                        else
                        {
                            msgErr = e.Message;
                        }

                        throw new Exception(msgErr);
                    }
                }
                #endregion TRANSACCION
            }
            catch (Exception e)
            {
                LogWrite(e, null, "InvoiceCommercialsAddNew==>" + msgXtraInfo);
                ViewData["EditError"] = ErrorMessage(e.Message);
            }
            finally
            {
                item.InvoiceCommercialDetail.ToList().ForEach(l =>
                {
                    InvoiceCommercialDetail invoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(r => r.id_itemMarked == l.id_itemMarked); ;
                    if (invoiceCommercialDetail != null)
                    {
                        l.Item = invoiceCommercialDetail.Item;
                        l.Item2 = invoiceCommercialDetail.Item2;
                    }
                });
                item.id_metricUnitInvoice = (item.id_metricUnitInvoice == null) ? 999 : item.id_metricUnitInvoice;

                string codeDocumentState = db.Document.FirstOrDefault(r => r.id == item.id)?.DocumentState?.code ?? "01";

                item = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(item, "70", codeDocumentState, ActiveUser.id);

                TempData.Keep("invoiceCommercial");
            }

            return PartialView("_InvoiceCommercialEditFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialsUpdate
            (
                bool approve,
                InvoiceCommercial item,
                string descriptionInvoiceCommercial,
                Document itemDoc,
                string descriptionDocument,
                string purchaseOrderInvoiceCommercial
            )
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            InvoiceCommercial modelItem = new InvoiceCommercial();
            string msgXtraInfo = "";
            string msgOK = " guardada exitosamente";
            try
            {
                modelItem = db.InvoiceCommercial.FirstOrDefault(p => p.id == item.id);
                if (ModelState.IsValid && modelItem == null) throw new Exception("Error en los datos ingresados.");
                var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                #region DOCUMENT

                msgXtraInfo = "Información Documento";
                modelItem.Document.description = descriptionDocument;
                modelItem.Document.id_userUpdate = ActiveUser.id;
                modelItem.Document.dateUpdate = DateTime.Now;
                modelItem.Document.emissionDate = itemDoc.emissionDate;
                modelItem.Document.description = descriptionDocument;

                #endregion DOCUMENT

                #region InvoiceCommercial
                msgXtraInfo = "Informción Cabecera Factura Comercial";
                //item.Employee = db.Employee.FirstOrDefault(fod => fod.id == item.id_responsable);
                //openingClosingPlateLying.Employee = item.Employee;

                modelItem.id_Language = item.id_Language;
                //modelItem.Language = db.Language.FirstOrDefault(fod => fod.id == item.id_Language);
                //invoiceCommercial.Language = item.Language;
                modelItem.id_ForeignCustomer = item.id_ForeignCustomer;
                //modelItem.ForeignCustomer = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_ForeignCustomer);//id_Consignee
                //modelItem.id_Consignee = item.id_Consignee;
                //modelItem.ForeignCustomer1 = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Consignee);//id_Consignee
                modelItem.id_Notifier = item.id_Notifier;
                modelItem.ForeignCustomer2 = db.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Notifier);//id_Notifier
                                                                                                                  //modelItem.BLNumber = item.BLNumber;
                modelItem.description = descriptionInvoiceCommercial;
                modelItem.glazePercentage = item.glazePercentage;
                modelItem.BLNumber = item.BLNumber;
                //modelItem.purchaseOrder = purchaseOrderInvoiceCommercial;
                modelItem.idPortfolioFinancing = item.idPortfolioFinancing;
                modelItem.id_PaymentMethod = item.id_PaymentMethod;
                modelItem.id_PaymentTerm = item.id_PaymentTerm;
                modelItem.dateShipment = item.dateShipment;
                modelItem.id_shippingAgency = item.id_shippingAgency;
                modelItem.id_shippingLine = item.id_shippingLine;
                modelItem.id_portShipment = item.id_portShipment;
                modelItem.id_portDischarge = item.id_portDischarge;
                modelItem.id_portDestination = item.id_portDestination;
                modelItem.shipName = item.shipName;
                modelItem.shipNumberTrip = item.shipNumberTrip;
                modelItem.daeNumber = item.daeNumber;
                modelItem.BLNumber = item.BLNumber;
                modelItem.numeroContenedores = item.numeroContenedores;
                modelItem.id_capacityContainer = item.id_capacityContainer;
                modelItem.id_Notifier2 = item.id_Notifier2;
                modelItem.letterCredit = item.letterCredit;
                modelItem.daeNumber2 = item.daeNumber2;
                modelItem.daeNumber3 = item.daeNumber3;
                modelItem.daeNumber4 = item.daeNumber4;
                modelItem.id_CityDelivery = item.id_CityDelivery;
                modelItem.id_metricUnitInvoice = (item.id_metricUnitInvoice == 999) ? null : item.id_metricUnitInvoice;
                modelItem.id_tariffHeading = item.id_tariffHeading;
                modelItem.id_BankTransfer = item.id_BankTransfer;
                modelItem.valueTotalFreight = item.valueTotalFreight;
                modelItem.valueDiscount = item.valueDiscount;
                modelItem.referenceInvoice = item.referenceInvoice;
                modelItem.hasGlaze = item.hasGlaze;
                modelItem.etaDate = item.etaDate;
                modelItem.blDate = item.blDate;
                modelItem.seals = item.seals;
                modelItem.containers = item.containers;
                modelItem.id_termsNegotiation = invoiceCommercial.id_termsNegotiation;
                if (modelItem.idVendor == null)
                {
                    modelItem.idVendor = invoiceCommercial.idVendor;
                }

                #endregion InvoiceCommercial

                #region InvoiceCommercialContainer
                msgXtraInfo = "Información Contenedores";
                for (int i = modelItem.InvoiceCommercialContainer.Count - 1; i >= 0; i--)
                {
                    var detail = modelItem.InvoiceCommercialContainer.ElementAt(i);

                    modelItem.InvoiceCommercialContainer.Remove(detail);
                    db.Entry(detail).State = EntityState.Deleted;
                }

                modelItem.InvoiceCommercialContainer = new List<InvoiceCommercialContainer>();
                if (invoiceCommercial.InvoiceCommercialContainer != null)
                {
                    foreach (var detail in invoiceCommercial.InvoiceCommercialContainer)
                    {
                        var invoiceCommercialContainer = new InvoiceCommercialContainer();
                        invoiceCommercialContainer.id_invoiceCommercial = modelItem.id;
                        invoiceCommercialContainer.numberContainer = detail.numberContainer;
                        invoiceCommercialContainer.isActive = true;
                        invoiceCommercialContainer.dateCreate = DateTime.Now;
                        invoiceCommercialContainer.id_userUpdate = ActiveUser.id;
                        invoiceCommercialContainer.dateUpdate = DateTime.Now;
                        invoiceCommercialContainer.id_userCreate = ActiveUser.id;

                        modelItem.InvoiceCommercialContainer.Add(invoiceCommercialContainer);
                    }
                }

                #endregion InvoiceCommercialContainer

                #region InvoiceCommercialDetail
                if (!puedeModDatosProforma) 
                {
                    msgXtraInfo = "Información Detalle Factura Comercial";
                    if (invoiceCommercial.InvoiceCommercialDetail != null)
                    {
                        foreach (var detail in invoiceCommercial.InvoiceCommercialDetail)
                        {
                            var invoiceCommercialDetail = modelItem.InvoiceCommercialDetail.FirstOrDefault(fod => fod.id == detail.id);
                            if (invoiceCommercialDetail != null)
                            {
                                if (modelItem.id_documentInvoice == null)
                                {
                                    invoiceCommercialDetail.id_itemOrigen = (int)detail.id_itemOrigen;
                                    invoiceCommercialDetail.codePresentationOrigen = detail.codePresentationOrigen;
                                    invoiceCommercialDetail.presentationMinimumOrigen = detail.presentationMinimumOrigen;
                                    invoiceCommercialDetail.presentationMaximumOrigen = detail.presentationMaximumOrigen;
                                    invoiceCommercialDetail.numBoxesOrigen = detail.numBoxesOrigen;
                                    invoiceCommercialDetail.id_metricUnitOrigen = detail.id_metricUnitOrigen;
                                    invoiceCommercialDetail.amountOrigen = detail.amountOrigen;
                                    invoiceCommercialDetail.unitPriceOrigen = detail.unitPriceOrigen;
                                    invoiceCommercialDetail.totalOrigen = detail.totalOrigen;
                                }

                                invoiceCommercialDetail.id_item = detail.id_item;
                                invoiceCommercialDetail.codePresentation = detail.codePresentation;
                                invoiceCommercialDetail.presentationMinimum = detail.presentationMinimum;
                                invoiceCommercialDetail.presentationMaximum = detail.presentationMaximum;
                                invoiceCommercialDetail.numBoxes = detail.numBoxes;
                                invoiceCommercialDetail.id_metricUnit = detail.id_metricUnit;
                                invoiceCommercialDetail.amount = detail.amount;
                                invoiceCommercialDetail.amountInvoice = detail.amountInvoice;
                                invoiceCommercialDetail.unitPrice = detail.unitPrice;
                                invoiceCommercialDetail.discount = detail.discount;
                                invoiceCommercialDetail.total = detail.total;
                                invoiceCommercialDetail.glazePercentageDetail = detail.glazePercentageDetail;
                                invoiceCommercialDetail.isActive = detail.isActive;
                                invoiceCommercialDetail.id_userUpdate = ActiveUser.id;
                                invoiceCommercialDetail.dateUpdate = DateTime.Now;
                                invoiceCommercialDetail.weightBox = detail.weightBox;
                                invoiceCommercialDetail.factorMetricUnit = detail.factorMetricUnit;
                                invoiceCommercialDetail.id_itemMarked = detail.id_itemMarked;

                                db.InvoiceCommercialDetail.Attach(invoiceCommercialDetail);
                                db.Entry(invoiceCommercialDetail).State = EntityState.Modified;
                            }
                            else
                            {
                                invoiceCommercialDetail = new InvoiceCommercialDetail
                                {
                                    id = detail.id,
                                    amount = detail.amount,
                                    amountInvoice = detail.amountInvoice,
                                    id_item = detail.id_item,
                                    codePresentation = detail.codePresentation,
                                    id_metricUnit = detail.id_metricUnit,
                                    unitPrice = detail.unitPrice,
                                    discount = detail.discount,
                                    numBoxes = detail.numBoxes,
                                    presentationMinimum = detail.presentationMinimum,
                                    presentationMaximum = detail.presentationMaximum,
                                    total = detail.total,
                                    glazePercentageDetail = detail.glazePercentageDetail,

                                    numBoxesOrigen = detail.numBoxesOrigen,
                                    id_itemOrigen = detail.id_itemOrigen,
                                    codePresentationOrigen = detail.codePresentationOrigen,
                                    amountOrigen = detail.amountOrigen,
                                    id_metricUnitOrigen = detail.id_metricUnitOrigen,
                                    presentationMaximumOrigen = detail.presentationMaximumOrigen,
                                    presentationMinimumOrigen = detail.presentationMinimumOrigen,
                                    totalOrigen = detail.totalOrigen,
                                    unitPriceOrigen = detail.unitPriceOrigen,
                                    weightBox = detail.weightBox,
                                    factorMetricUnit = detail.factorMetricUnit,
                                    id_itemMarked = detail.id_itemMarked,

                                    dateCreate = DateTime.Now,
                                    dateUpdate = DateTime.Now,
                                    id_userCreate = ActiveUser.id,
                                    id_userUpdate = ActiveUser.id,
                                    isActive = detail.isActive
                                };
                                modelItem.InvoiceCommercialDetail.Add(invoiceCommercialDetail);
                            }

                            //item.InvoiceCommercialDetail.Add(invoiceCommercialDetail);
                        }
                    }
                }
                    

                #region InvoiceCommercialDocument

                if (invoiceCommercial.InvoiceCommercialDocument != null)
                {
                    var itemInvoiceCommercialDocument = invoiceCommercial.InvoiceCommercialDocument.ToList();

                    for (int i = modelItem.InvoiceCommercialDocument.Count - 1; i >= 0; i--)
                    {
                        var detail = modelItem.InvoiceCommercialDocument.ElementAt(i);

                        if (itemInvoiceCommercialDocument.FirstOrDefault(fod => fod.id == detail.id) == null)
                        {
                            DeleteAttachment(detail);
                            modelItem.InvoiceCommercialDocument.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }
                    }

                    foreach (var detail in itemInvoiceCommercialDocument)
                    {
                        var tempItemInvoiceCommercialDocument = modelItem.InvoiceCommercialDocument.FirstOrDefault(fod => fod.id == detail.id);
                        if (tempItemInvoiceCommercialDocument == null)
                        {
                            tempItemInvoiceCommercialDocument = new InvoiceCommercialDocument
                            {
                                guid = detail.guid,
                                url = detail.url,
                                attachment = detail.attachment,
                                referenceDocument = detail.referenceDocument,
                                descriptionDocument = detail.descriptionDocument
                            };
                            modelItem.InvoiceCommercialDocument.Add(tempItemInvoiceCommercialDocument);
                        }
                        else
                        {
                            if (tempItemInvoiceCommercialDocument.url != detail.url)
                            {
                                DeleteAttachment(tempItemInvoiceCommercialDocument);
                                tempItemInvoiceCommercialDocument.guid = detail.guid;
                                tempItemInvoiceCommercialDocument.url = detail.url;
                                tempItemInvoiceCommercialDocument.attachment = detail.attachment;
                            }
                            tempItemInvoiceCommercialDocument.referenceDocument = detail.referenceDocument;
                            tempItemInvoiceCommercialDocument.descriptionDocument = detail.descriptionDocument;
                            db.Entry(tempItemInvoiceCommercialDocument).State = EntityState.Modified;
                        }
                    }
                }

                #endregion InvoiceCommercialDocument

                if (modelItem.InvoiceCommercialDetail.Where(d => d.isActive).ToList().Count == 0)
                {
                    //TempData.Keep("openingClosingPlateLying");
                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar una tumbada sin detalles");
                    //return PartialView("_OpeningClosingPlateLyingEditFormPartial", item);
                    throw new Exception("No se puede guardar una Factura Comercial sin detalles de Producto.");
                }
                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == modelItem.Document.id_documentOrigen);
                if (proforma != null)
                {
                    foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(s => s.isActive))
                    {
                        var invoiceDetail = modelItem.InvoiceCommercialDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                        if (invoiceDetail != null)
                        {
                            if (proformaDetail.proformaPendingNumBoxes < invoiceDetail.numBoxes)
                                throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                    " debe ser menor e igual a: " + proformaDetail.proformaPendingNumBoxes.Value.ToString("#,##0") +
                                                    " que son los cartones pendiente en la Proforma.");
                        }
                    }
                }

                #endregion InvoiceCommercialDetail

                msgXtraInfo = "Cálculo de Totales";
                UpdateInvoiceCommercialTotals(modelItem);
                TempData["invoiceCommercial"] = modelItem;

                #region VALIDACION
                if (approve)
                {
                    modelItem.ValidateInfo("03");
                    //ServiceInventoryMove.UpdateInventaryMoveTransferOpeningClosingPlateLying(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                    modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    msgOK = " Aprobada Exitosamente";

                    if (modelItem.Document.id_documentOrigen != null)
                        ServiceSalesQuotationExterior.UpdateValuesFromInvoiceComercial(modelItem, db);
                }
                else
                {
                    modelItem.ValidateInfo("01");
                }
                #endregion VALIDACION

                #region TRANSACCION
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.InvoiceCommercial.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        #region Actualizamos los datos de las proformas
                        
                        if (puedeModDatosProforma)
                        {
                            UpdateSalesQuotationExterior(modelItem.Document, modelItem);
                            UpdateInvoicesExterior(modelItem.Document.id, modelItem.Document.id_documentOrigen, modelItem);
                        }
                        #endregion

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        TempData["invoiceCommercial"] = modelItem;
                        trans.Rollback();

                        /*Prepar mensaje de error Post-Validacion */
                        string msgErr = "";
                        if (e.Data["source"] != null)
                        {
                            if ((string)e.Data["source"] == "modelDocumentValidation" && approve)
                                msgErr = "Para Aprobar el presente documento los siguientes campos son requeridos:<br>"
                                                + Environment.NewLine
                                                + e.Message;
                        }
                        else
                        {
                            msgErr = e.Message;
                        }

                        throw new Exception(msgErr);
                    }
                }
                #endregion TRANSACCION

                modelItem.InvoiceCommercialDetail?.ToList().RemoveAll(r => !r.isActive);

                ViewData["EditMessage"] = SuccessMessage("Factura Comercial: " + modelItem.Document.number + msgOK);
            }
            catch (Exception e)
            {
                LogWrite(e, null, "InvoiceCommercialsUpdate==>" + msgXtraInfo);
                ViewData["EditError"] = ErrorMessage(e.Message);
            }
            finally
            {
                modelItem.InvoiceCommercialDetail.ToList().ForEach(l =>
                {
                    InvoiceCommercialDetail invoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(r => r.id_itemMarked == l.id_itemMarked); ;
                    if (invoiceCommercialDetail != null)
                    {
                        l.Item = invoiceCommercialDetail.Item;
                        l.Item2 = invoiceCommercialDetail.Item2;
                    }
                });

                modelItem.id_metricUnitInvoice = (modelItem.id_metricUnitInvoice == null) ? 999 : modelItem.id_metricUnitInvoice;

                string codeDocumentState = db.Document.FirstOrDefault(r => r.id == modelItem.id)?.DocumentState?.code ?? "01";

                modelItem = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(modelItem, "70", codeDocumentState, ActiveUser.id);

                TempData.Keep("invoiceCommercial");
            }

            return PartialView("_InvoiceCommercialEditFormPartial", modelItem);
        }

        #endregion Invoice Commercial

        private void UpdateInvoiceCommercialTotals(InvoiceCommercial invoiceCommercial)
        {
            decimal discounGlobal = invoiceCommercial.valueDiscount;
            invoiceCommercial.totalBoxes = 0;
            invoiceCommercial.totalWeight = 0.0M;
            invoiceCommercial.totalValue = 0.0M;
            invoiceCommercial.valueDiscount = 0.0M;
            decimal subtotal = 0;
            
            foreach (var invoiceCommercialDetail in invoiceCommercial.InvoiceCommercialDetail.Where(r => r.isActive))
            {
                invoiceCommercial.totalBoxes += invoiceCommercialDetail.numBoxes;
                invoiceCommercial.totalWeight += decimal.Round(invoiceCommercialDetail.amountInvoice, 2);
                invoiceCommercial.valueDiscount += invoiceCommercialDetail.discount.Value;
                subtotal += decimal.Round(invoiceCommercialDetail.total, 2);
                //invoiceCommercial.totalValue
            }

            invoiceCommercial.valueDiscount += discounGlobal;
            subtotal = (subtotal - invoiceCommercial.valueDiscount );
            
            int? id_termsNegotiation = invoiceCommercial.id_termsNegotiation;
            if (id_termsNegotiation != null)
            {
                TermsNegotiation _termsNegotiation = db.TermsNegotiation.FirstOrDefault(r => r.id == id_termsNegotiation);
                if ((_termsNegotiation?.code ?? "") == "FOBFLET")
                {
                    subtotal = subtotal + invoiceCommercial.valueTotalFreight;
                }
            }

            invoiceCommercial.totalValue = subtotal;
        }

        #region InvoiceCommercialContainer

        [ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormContainerPartial()
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            ViewData["codeState"] = invoiceCommercial?.Document?.DocumentState?.code ?? "";

            var model = invoiceCommercial?.InvoiceCommercialContainer.OrderBy(od => od.id).ToList() ?? new List<InvoiceCommercialContainer>();

            TempData["invoiceCommercial"] = TempData["invoiceCommercial"] ?? invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            return PartialView("_InvoiceCommercialEditFormContainerPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormContainerAddNew(InvoiceCommercialContainer item)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialContainer = invoiceCommercial.InvoiceCommercialContainer ?? new List<InvoiceCommercialContainer>();

            if (ModelState.IsValid)
            {
                item.id = invoiceCommercial.InvoiceCommercialContainer.Count() > 0 ? invoiceCommercial.InvoiceCommercialContainer.Max(ppd => ppd.id) + 1 : 1;
                invoiceCommercial.InvoiceCommercialContainer.Add(item);
                //UpdateProductionLotTotals(purchasePlanning);
            }

            TempData["invoiceCommercial"] = invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            ViewData["codeState"] = invoiceCommercial?.Document?.DocumentState?.code ?? "";

            var model = invoiceCommercial?.InvoiceCommercialContainer.OrderBy(od => od.id).ToList() ?? new List<InvoiceCommercialContainer>();

            return PartialView("_InvoiceCommercialEditFormContainerPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormContainerUpdate(InvoiceCommercialContainer item)
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialContainer = invoiceCommercial.InvoiceCommercialContainer ?? new List<InvoiceCommercialContainer>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = invoiceCommercial.InvoiceCommercialContainer.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        //UpdateProductionLotTotals(purchasePlanning);
                        TempData["invoiceCommercial"] = invoiceCommercial;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceCommercial");
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("invoiceCommercial");

            ViewData["codeState"] = invoiceCommercial?.Document.DocumentState.code ?? "";

            var model = invoiceCommercial?.InvoiceCommercialContainer.OrderBy(od => od.id).ToList() ?? new List<InvoiceCommercialContainer>();

            return PartialView("_InvoiceCommercialEditFormContainerPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormContainerDelete(System.Int32 id)
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialContainer = invoiceCommercial.InvoiceCommercialContainer ?? new List<InvoiceCommercialContainer>();

            //if (id_item >= 0)
            //{
            try
            {
                var invoiceCommercialContainers = invoiceCommercial.InvoiceCommercialContainer.FirstOrDefault(p => p.id == id);
                if (invoiceCommercialContainers != null)
                    invoiceCommercial.InvoiceCommercialContainer.Remove(invoiceCommercialContainers);

                //UpdateProductionLotTotals(purchasePlanning);
                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            TempData.Keep("invoiceCommercial");

            ViewData["codeState"] = invoiceCommercial?.Document.DocumentState.code ?? "";

            var model = invoiceCommercial?.InvoiceCommercialContainer.OrderBy(od => od.id).ToList() ?? new List<InvoiceCommercialContainer>();

            return PartialView("_InvoiceCommercialEditFormContainerPartial", model);
        }

        #endregion InvoiceCommercialContainer

        #region InvoiceCommercialDetail

        [ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormDetailPartial()
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            var model = invoiceCommercial?.InvoiceCommercialDetail.Where(r => r.isActive).CalulateInvoiceDetail().OrderBy(od => od.id).ToList() ?? new List<InvoiceCommercialDetail>();

            ExludeItemByEditRow(Request.Params["__DXCallbackArgument"], model, invoiceCommercial);
            TempData["invoiceCommercial"] = TempData["invoiceCommercial"] ?? invoiceCommercial;
            TempData.Keep("invoiceCommercial");
            TempData.Keep("id_Items");
            ViewBag.DocumentOrigen = invoiceCommercial.Document.id_documentOrigen;

            return PartialView("_InvoiceCommercialEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormDetailAddNew(InvoiceCommercialDetail invoiceCommercialDetail, Boolean hasGlazeValue)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            //invoiceCommercial = invoiceCommercial ?? db.InvoiceCommercial.FirstOrDefault(i => i.id == id_invoice);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    Item item = db.Item.FirstOrDefault(r => r.id == invoiceCommercialDetail.id_item);
                    Item item1 = db.Item.FirstOrDefault(r => r.id == invoiceCommercialDetail.id_itemMarked);
                    Presentation presentation = db.Presentation.FirstOrDefault(r => r.id == item.id_presentation);
                    int unidadMedidaOrigen = item?.ItemWeightConversionFreezen?.id_MetricUnit ?? 0;   //presentation.id_metricUnit;
                    int unidadMedidaOrigenInvoice
                                = (invoiceCommercial.id_metricUnitInvoice == 0 || invoiceCommercial.id_metricUnitInvoice == null || invoiceCommercial.id_metricUnitInvoice == 999)
                                  ? unidadMedidaOrigen : (int)invoiceCommercial.id_metricUnitInvoice;
                    //decimal minimoPresentation = presentation.minimum;
                    //decimal pesItemValue = (invoiceCommercialDetail.glazePercentageDetail == 0) ? minimoPresentation : ((100 * minimoPresentation) / invoiceCommercialDetail.glazePercentageDetail);
                    //int maximumPresentation = presentation.maximum;
                    /*
					 decimal pesItemValue = (item.glazePercentageDetail == 0) ? minimumItemValue : ((100 * minimumItemValue) / item.glazePercentageDetail);
						modelItem.presentationMinimum = minimumItemValue;
						decimal amount = item.numBoxes * modelItem.presentationMaximum * pesItemValue;
					 */
                    Random rnd = new Random();
                    int? newId = rnd.Next(-9999999, -999);
                    //decimal amount = invoiceCommercialDetail.numBoxes * maximumPresentation * pesItemValue;
                    //amount = decimal.Round(amount, 2);
                    //var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                    //                                                                    muc.id_metricOrigin == unidadMedidaOrigen  &&
                    //                                                                    muc.id_metricDestiny == unidadMedidaOrigenInvoice);
                    //var factor = (unidadMedidaOrigen == unidadMedidaOrigenInvoice) ? 1 : (metricUnitConversion?.factor ?? 0);

                    //var amountInvoice = decimal.Round((amount * factor), 2);
                    //decimal total = amountInvoice * invoiceCommercialDetail.unitPrice;
                    //total = decimal.Round(total, 2);

                    //decimal totalOrigen = amount * invoiceCommercialDetail.unitPrice;

                    invoiceCommercialDetail.id = (int)newId;
                    invoiceCommercialDetail.Item1 = item;
                    invoiceCommercialDetail.id_item = item.id;
                    invoiceCommercialDetail.id_metricUnit = unidadMedidaOrigenInvoice;

                    invoiceCommercialDetail.codePresentation = presentation.code;
                    invoiceCommercialDetail.presentationMinimum = presentation.minimum;
                    invoiceCommercialDetail.presentationMaximum = presentation.maximum;
                    //invoiceCommercialDetail.amount = amount;
                    //invoiceCommercialDetail.amountInvoice = amountInvoice;
                    //invoiceCommercialDetail.total = total;

                    /*  campos requerido control desde factura fiscal */
                    invoiceCommercialDetail.id_itemOrigen = item.id;
                    invoiceCommercialDetail.id_itemMarked = item1.id;
                    invoiceCommercialDetail.Item2 = item1;
                    invoiceCommercialDetail.id_metricUnitOrigen = unidadMedidaOrigen;
                    invoiceCommercialDetail.codePresentationOrigen = presentation.code;
                    invoiceCommercialDetail.presentationMinimumOrigen = presentation.minimum;
                    invoiceCommercialDetail.presentationMaximumOrigen = presentation.maximum;
                    invoiceCommercialDetail.numBoxesOrigen = invoiceCommercialDetail.numBoxes;
                    invoiceCommercialDetail.unitPriceOrigen = invoiceCommercialDetail.unitPrice;
                    //invoiceCommercialDetail.amountOrigen = amount;
                    //invoiceCommercialDetail.totalOrigen = totalOrigen;

                    invoiceCommercialDetail.dateCreate = DateTime.Now;
                    invoiceCommercialDetail.dateUpdate = DateTime.Now;
                    invoiceCommercialDetail.id_userCreate = ActiveUser.id;
                    invoiceCommercialDetail.id_userUpdate = ActiveUser.id;
                    invoiceCommercialDetail.isActive = true;
                    invoiceCommercialDetail.hasGlaze_DetailOperation = hasGlazeValue;
                    invoiceCommercialDetail.idCompany = this.ActiveCompanyId;
                    invoiceCommercialDetail.CalculateDetailInvoiceCommercialDetail();

                    invoiceCommercial.InvoiceCommercialDetail.Add(invoiceCommercialDetail);

                    TempData["invoiceCommercial"] = invoiceCommercial;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceCommercial");
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            var model = invoiceCommercial?.InvoiceCommercialDetail.Where(r => r.isActive).OrderByDescending(od => od.id).ToList() ?? new List<InvoiceCommercialDetail>();
            TempData.Keep("invoiceCommercial");
            TempData["id_Items"] = invoiceCommercial.getIds_Items(null);
            TempData.Keep("id_Items");
            return PartialView("_InvoiceCommercialEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormDetailUpdate(InvoiceCommercialDetail item, Boolean hasGlazeValue)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.numBoxes = Convert.ToInt32(item.numBoxes);
                        modelItem.unitPrice = decimal.Round(Convert.ToDecimal(item.unitPrice), 2);
                        modelItem.weightBox = decimal.Round(Convert.ToDecimal(item.weightBox), 2);

                        modelItem.Item1 = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                        modelItem.Item2 = db.Item.FirstOrDefault(fod => fod.id == item.id_itemMarked);
                        modelItem.codePresentation = modelItem.Item2?.Presentation?.code ?? "";

                        int metricUnitOrigen = modelItem.Item2?.ItemWeightConversionFreezen?.id_MetricUnit ?? 0;

                        modelItem.presentationMaximum = modelItem.Item2?.Presentation?.maximum ?? 1;
                        //decimal minimumItemValue = modelItem.Item1?.Presentation?.minimum ?? 1;
                        //decimal pesItemValue = (item.glazePercentageDetail == 0) ? minimumItemValue : ((100 * minimumItemValue) / item.glazePercentageDetail);

                        modelItem.presentationMinimum = modelItem.Item2?.Presentation?.minimum ?? 1;
                        //decimal amount = item.numBoxes * modelItem.presentationMaximum * pesItemValue;
                        //amount = decimal.Round(amount, 2);
                        //modelItem.amount = amount;
                        int unidadMedidaOrigen = (modelItem.id_metricUnit == null) ? 0 : (int)modelItem.id_metricUnit;

                        int unidadMedidaOrigenInvoice
                                = (invoiceCommercial.id_metricUnitInvoice == 0 || invoiceCommercial.id_metricUnitInvoice == null || invoiceCommercial.id_metricUnitInvoice == 999)
                                  ? unidadMedidaOrigen : (int)invoiceCommercial.id_metricUnitInvoice;

                        modelItem.id_metricUnit = unidadMedidaOrigenInvoice;
                        modelItem.hasGlaze_DetailOperation = hasGlazeValue;
                        modelItem.idCompany = this.ActiveCompanyId;
                        modelItem.CalculateDetailInvoiceCommercialDetail();
                        this.UpdateModel(modelItem);

                        TempData["invoiceCommercial"] = invoiceCommercial;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceCommercial");
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("invoiceCommercial");
            TempData["id_Items"] = invoiceCommercial.getIds_Items(null);
            TempData.Keep("id_Items");

            var model = invoiceCommercial?.InvoiceCommercialDetail.Where(r => r.isActive).OrderByDescending(od => od.id).ToList() ?? new List<InvoiceCommercialDetail>();

            return PartialView("_InvoiceCommercialEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialEditFormDetailDelete(int id)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            //if (id_item >= 0)
            //{
            try
            {
                var invoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(p => p.id == id);
                if (invoiceCommercialDetail != null)
                {
                    invoiceCommercialDetail.isActive = false;
                    invoiceCommercialDetail.dateUpdate = DateTime.Now;
                    invoiceCommercialDetail.id_userUpdate = ActiveUser.id;
                    this.UpdateModel(invoiceCommercialDetail);
                }
                //UpdateProductionLotTotals(purchasePlanning);
                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }
            //}

            TempData.Keep("invoiceCommercial");
            TempData["id_Items"] = invoiceCommercial.getIds_Items(null);
            TempData.Keep("id_Items");

            var model = invoiceCommercial?.InvoiceCommercialDetail.Where(r => r.isActive).OrderByDescending(od => od.id).ToList() ?? new List<InvoiceCommercialDetail>();
            return PartialView("_InvoiceCommercialEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public void InvoiceCommercialDetailsDeleteSeleted(int[] ids)
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            //invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            if (ids != null)
            {
                try
                {
                    var invoiceCommercialDetail = invoiceCommercial?.InvoiceCommercialDetail?.Where(i => ids.ToList().Contains((int)i.id_item)) ?? null;

                    foreach (var detail in invoiceCommercialDetail)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["invoiceCommercial"] = invoiceCommercial;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceExterior");
                }
            }

            TempData.Keep("invoiceExterior");
        }

        //private void UpdateProductionLotTotals(ProductionLot productionLot)
        //{
        //    productionLot.totalQuantityOrdered = 0.0M;
        //    productionLot.totalQuantityRemitted = 0.0M;
        //    productionLot.totalQuantityRecived = 0.0M;

        //    foreach (var productionLotDetail in productionLot.ProductionLotDetail)
        //    {
        //        productionLot.totalQuantityOrdered += productionLotDetail.quantityOrdered;
        //        productionLot.totalQuantityRemitted += productionLotDetail.quantityRemitted;
        //        productionLot.totalQuantityRecived += productionLotDetail.quantityRecived;
        //    }
        //}

        #endregion InvoiceCommercialDetail

        #region SALES INVOICE COMMERCIAL ATTACHMENT

        #region SALES INVOICE COMMERCIAL ATTACHED DOCUMENTS

        [ValidateInput(false)]
        public ActionResult InvoiceCommercialAttachedDocumentsPartial()
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            var model = invoiceCommercial.InvoiceCommercialDocument;
            TempData["invoiceCommercial"] = TempData["invoiceCommercial"] ?? invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            return PartialView("_InvoiceCommercialAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.InvoiceCommercialDocument item)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            if (ModelState.IsValid)
            {
                try
                {
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
                            var invoiceCommercialDocumentDetailAux = invoiceCommercial.
                                                    InvoiceCommercialDocument.
                                                    FirstOrDefault(fod => fod.attachment == item.attachment);
                            if (invoiceCommercialDocumentDetailAux != null)
                            {
                                throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachment + ", en el detalle de los Documentos Adjunto.");
                            }
                        }
                    }
                    item.id = invoiceCommercial.InvoiceCommercialDocument.Count() > 0 ? invoiceCommercial.InvoiceCommercialDocument.Max(pld => pld.id) + 1 : 1;
                    invoiceCommercial.InvoiceCommercialDocument.Add(item);
                    TempData["invoiceCommercial"] = invoiceCommercial;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            var model = invoiceCommercial.InvoiceCommercialDocument;
            TempData["invoiceCommercial"] = TempData["invoiceCommercial"] ?? invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            return PartialView("_InvoiceCommercialAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.InvoiceCommercialDocument item)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDocument = invoiceCommercial.InvoiceCommercialDocument ?? new List<InvoiceCommercialDocument>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = invoiceCommercial.InvoiceCommercialDocument.FirstOrDefault(i => i.id == item.id);
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
                            if (modelItem.attachment != item.attachment)
                            {
                                var invoiceCommercialDocumentDetailAux = invoiceCommercial.
                                                      InvoiceCommercialDocument.
                                                      FirstOrDefault(fod => fod.attachment == item.attachment);
                                if (invoiceCommercialDocumentDetailAux != null)
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
                        TempData["invoiceCommercial"] = invoiceCommercial;
                        //db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceCommercial");
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            var model = invoiceCommercial.InvoiceCommercialDocument;
            TempData.Keep("invoiceCommercial");

            return PartialView("_InvoiceCommercialAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialAttachedDocumentsPartialDelete(System.Int32 id)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            invoiceCommercial.InvoiceCommercialDetail = invoiceCommercial.InvoiceCommercialDetail ?? new List<InvoiceCommercialDetail>();

            try
            {
                var item = invoiceCommercial.InvoiceCommercialDocument.FirstOrDefault(it => it.id == id);
                if (item != null)
                    invoiceCommercial.InvoiceCommercialDocument.Remove(item);
                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }
            var model = invoiceCommercial.InvoiceCommercialDocument;
            TempData.Keep("invoiceCommercial");

            return PartialView("_InvoiceCommercialAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion SALES INVOICE COMMERCIAL ATTACHED DOCUMENTS

        private void UpdateAttachment(InvoiceCommercial invoiceCommercial)
        {
            List<InvoiceCommercialDocument> invoiceCommercialDocument = invoiceCommercial.InvoiceCommercialDocument.ToList() ?? new List<InvoiceCommercialDocument>();
            foreach (var item in invoiceCommercialDocument)
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
                        item.guid = FileUploadHelper.FileUploadProcessAttachment("/InvoiceCommercial/" + invoiceCommercial.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
                        item.url = urlAux;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
                    }
                }
            }
        }

        private void DeleteAttachment(InvoiceCommercialDocument invoiceCommercialDocument)
        {
            if (invoiceCommercialDocument.url != FileUploadHelper.UploadDirectoryDefaultTemp)
            {
                try
                {
                    // Carga el contenido guardado en el temp
                    FileUploadHelper.CleanUpUploadedFiles(invoiceCommercialDocument.url, invoiceCommercialDocument.guid);
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
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            try
            {
                InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

                List<InvoiceCommercialDocument> invoiceCommercialDocument = invoiceCommercial.InvoiceCommercialDocument.ToList() ?? new List<InvoiceCommercialDocument>();
                var invoiceCommercialDocumentAux = invoiceCommercialDocument.FirstOrDefault(fod => fod.id == id);
                if (invoiceCommercialDocumentAux != null)
                {
                    // Carga el contenido guardado en el temp
                    string nameAttachment;
                    string typeContentAttachment;
                    string guidAux = invoiceCommercialDocumentAux.guid;
                    string urlAux = invoiceCommercialDocumentAux.url;
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
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");
            UploadControlExtension.GetUploadedFiles(
                "attachmentUploadControl", UploadControlSettings.UploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        public class UploadControlSettings
        {
            public static readonly UploadControlValidationSettings UploadValidationSettings;

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

        #endregion UPLOAD FILE

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedAttachmentDetail(string attachmentNameNew)
        {
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var invoiceCommercialDocumentDetailAux = invoiceCommercial.
                                                      InvoiceCommercialDocument.
                                                      FirstOrDefault(fod => fod.attachment == attachmentNameNew);
            if (invoiceCommercialDocumentDetailAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Documento Adjunto: " + attachmentNameNew + ", en el detalle de los Documentos Adjunto."
                };
            }

            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion SALES INVOICE COMMERCIAL ATTACHMENT

        private void ExludeItemByEditRow(string RequestArgument, List<InvoiceCommercialDetail> invoiceCommercialDetail, InvoiceCommercial invoiceCommercial)
        {
            if (!RequestArgument.Contains("STARTEDIT"))
            {
                TempData["id_Items"] = invoiceCommercial.getIds_Items(null);
                return;
            }

            try
            {
                string partialArgument = RequestArgument.Split(';')[3];
                string _estatusEdit = partialArgument.Split('|')[1];

                if (!_estatusEdit.Contains("STARTEDIT")) return;

                string _rowEdit = partialArgument.Split('|')[2];
                int rowEdit = int.Parse(_rowEdit);
                int? _id_item = invoiceCommercialDetail.FirstOrDefault(r => r.id == rowEdit).id_item;
                TempData["id_Items"] = invoiceCommercial.getIds_Items(_id_item);
            }
            catch
            {
                TempData.Keep("id_Items");
            }
        }

        #region DETAILS VIEW

        public ActionResult InvoiceCommercialDetailPartial(int? id_invoiceCommercial)
        {
            TempData.Keep("invoiceCommercial");
            ViewData["id_invoiceCommercial"] = id_invoiceCommercial;
            var invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(p => p.id == id_invoiceCommercial);
            var model = invoiceCommercial?.InvoiceCommercialDetail.Where(r => r.isActive).ToList() ?? new List<InvoiceCommercialDetail>();
            return PartialView("_InvoiceCommercialDetailViewsPartial", model.ToList());
        }

        #endregion DETAILS VIEW

        #region SINGLE CHANGE InvoiceCommercial STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

                    if (purchasePlanning != null && documentState != null)
                    {
                        purchasePlanning.Document.id_documentState = documentState.id;
                        purchasePlanning.Document.DocumentState = documentState;

                        db.PurchasePlanning.Attach(purchasePlanning);
                        db.Entry(purchasePlanning).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " aprobada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");//06 Autorizado

                    if (invoiceCommercial != null && documentState != null)
                    {
                        invoiceCommercial.Document.id_documentState = documentState.id;
                        invoiceCommercial.Document.DocumentState = documentState;

                        db.InvoiceCommercial.Attach(invoiceCommercial);
                        db.Entry(invoiceCommercial).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        TempData["invoiceCommercial"] = invoiceCommercial;
                        ViewData["EditMessage"] = SuccessMessage("Factura Comercial: " + invoiceCommercial.Document1.number + " autorizada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    LogWrite(e, null, "Autorize==>");
                }
                finally
                {
                    string codeDocumentState = db.Document.FirstOrDefault(r => r.id == id)?.DocumentState?.code ?? "01";
                    invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);
                    TempData.Keep("invoiceCommercial");
                }
            }

            return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
                    var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;

                    var facturasFiscalesDoc = db.Document.FirstOrDefault(e => e.id_documentOrigen == id &&
                            e.id_documentType == idTipoDocFactFiscal && e.id_documentState != idEstadoAnulado);

                    if (facturasFiscalesDoc != null)
                    {
                        throw new Exception(
                            $"No se pueden anular la Factura Comercial porque tiene una Factura Fiscal {facturasFiscalesDoc.number} en estado {facturasFiscalesDoc.DocumentState.name}");
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (invoiceCommercial != null && documentState != null)
                    {
                        invoiceCommercial.Document.id_documentState = documentState.id;
                        invoiceCommercial.Document.DocumentState = documentState;

                        db.InvoiceCommercial.Attach(invoiceCommercial);
                        db.Entry(invoiceCommercial).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceCommercial"] = invoiceCommercial;
                        ViewData["EditMessage"] = SuccessMessage("Factura: " + invoiceCommercial.Document.number + " anulada.");
                    }
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    LogWrite(e, null, "Cancel==>");
                }
                finally
                {
                    string codeDocumentState = db.Document.FirstOrDefault(r => r.id == id)?.DocumentState?.code ?? "01";
                    invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);
                    TempData.Keep("invoiceCommercial");
                }
            }

            return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE

                    if (invoiceCommercial != null && documentState != null)
                    {
                        invoiceCommercial.Document.id_documentState = documentState.id;
                        invoiceCommercial.Document.DocumentState = documentState;

                        if (invoiceCommercial.Document.id_documentOrigen != null)
                            ServiceSalesQuotationExterior.UpdateValuesFromInvoiceComercialAnul(invoiceCommercial, db);

                        db.InvoiceCommercial.Attach(invoiceCommercial);
                        db.Entry(invoiceCommercial).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    TempData["invoiceCommercial"] = invoiceCommercial;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    ViewData["EditMessage"] = ErrorMessage("No se puede Reversar la Factura Comercial debido a: " + e.Message);
                    trans.Rollback();
                    LogWrite(e, null, "Revert==>");
                }
                finally
                {
                    string codeDocumentState = db.Document.FirstOrDefault(r => r.id == id)?.DocumentState?.code ?? "01";
                    invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);

                    TempData.Keep("invoiceCommercial");
                }
            }

            ViewData["EditMessage"] = SuccessMessage("Factura Comercial: " + invoiceCommercial.Document.number + " reversada exitosamente");

            return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
        }

        #endregion SINGLE CHANGE InvoiceCommercial STATE

        #region"SINGLE DOCUMENT"

        public JsonResult InvoiceCommercialReport(int id_invoice)
        {
            TempData.Keep("invoiceCommercial");
            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");

            ReportModel reportModel = new ReportModel();
            try
            {
                reportModel.ReportName = "InvoiceCommercialReport";
                reportModel.ReportDescription = "Factura Comercial";

                reportModel.ListReportParameter = new List<ReportParameter>();

                //ReportParameter reportParameter = new ReportParameter();

                //reportParameter.Name = "id";
                //reportParameter.Value = id_invoice.ToString();
                //reportModel.ListReportParameter.Add(reportParameter);

                isvalid = true;
            }
            catch //(Exception ex)
            {
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);
            TempData.Keep("invoiceCommercial");

            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCopy(int id)
        {
            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(o => o.id == id);

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code.Equals("01"));
            //tbsysInvoiceMode invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
            //tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

            if (invoiceCommercial != null)
            {
                invoiceCommercial.id = 0;
                invoiceCommercial.Document = new Document
                {
                    id = 0,
                    id_documentType = documentType?.id ?? 0,
                    DocumentType = documentType,
                    id_documentState = documentState?.id ?? 0,
                    DocumentState = documentState,
                    emissionDate = DateTime.Now,
                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    sequential = GetDocumentSequential(documentType.id),
                    number = GetDocumentNumber(documentType.id)
                };

                if (invoiceCommercial.InvoiceCommercialDetail.Count() == 0) invoiceCommercial.InvoiceCommercialDetail = db.InvoiceCommercialDetail.Where(r => r.id_invoiceCommercial == id).ToList();
                if (invoiceCommercial.InvoiceCommercialContainer.Count() == 0) invoiceCommercial.InvoiceCommercialContainer = db.InvoiceCommercialContainer.Where(r => r.id_invoiceCommercial == id).ToList();
                //  invoice.InvoiceExterior.idStatusDocument = documentState?.id ?? 0;
            }

            string codeDocumentState = db.Document.FirstOrDefault(r => r.id == id)?.DocumentState?.code ?? "01";

            invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);

            TempData["invoiceCommercial"] = invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            return PartialView("_FormEditInvoiceCommercial", invoiceCommercial);
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnApprove = true,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnSave = true,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id);
            string code_state = invoiceCommercial.Document.DocumentState.code;

            if (code_state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                    btnSave = true,
                };
            }
            else if (code_state == "03") // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = true,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnSave = false,
                };
            }
            else if (code_state == "05") // 06 Autorizado
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnSave = false,
                };
            }
            else if (code_state == "06") // 06 Autorizado
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnSave = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_invoiceCommercial)
        {
            TempData.Keep("invoiceCommercial");

            int index = db.InvoiceCommercial.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_invoiceCommercial);

            var result = new
            {
                maximunPages = db.InvoiceCommercial.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InvoiceCommercial invoiceCommercial = new InvoiceCommercial();

            try
            {
                invoiceCommercial = db.InvoiceCommercial.OrderByDescending(p => p.id).Take(page).ToList().Last();
                if (invoiceCommercial == null)
                {
                    invoiceCommercial = new InvoiceCommercial();
                }
            }
            catch (Exception e)
            {
                LogWrite(e, null, "Pagination==>");
                ViewData["EditError"] = ErrorMessage(e.Message);
            }
            finally
            {
                TempData["invoiceCommercial"] = invoiceCommercial;
                TempData.Keep("invoiceCommercial");

                int idInvoiceCommercial = (invoiceCommercial?.id ?? 0);
                string codeDocumentState = db.Document.FirstOrDefault(r => r.id == idInvoiceCommercial)?.DocumentState?.code ?? "01";

                invoiceCommercial = SecurityControl.SetSecurityControlDocument<InvoiceCommercial>(invoiceCommercial, "70", codeDocumentState, ActiveUser.id);
            }

            TempData.Keep("invoiceCommercial");
            TempData.Keep("model");
            return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
        }

        #endregion

        #region AXILIAR FUNCTIONS

        private class tmpItem
        {
            public int id { get; set; }
            public string auxCode { get; set; }
            public string name { get; set; }
            public string masterCode { get; set; }

            public tmpItem()
            { }
        }

        [HttpPost]
        public ActionResult GetDetailItemInvoiceCommercial(int? id_itemCurrent)
        {
            TempData.Keep("invoiceCommercial");

            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            Item itemAux = db.Item.FirstOrDefault(i => i.id == id_itemCurrent);
            itemAux = itemAux ?? new Item();
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            List<int> id_items = (List<int>)ViewData["id_items"];

            var codeFEXP = db.Setting.FirstOrDefault(fod => fod.code == "FEXP")?.value ?? "";
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";

            var aId_documentOrigen = invoiceCommercial.Document.id_documentOrigen;
            var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
            var aItems = proforma.Invoice.InvoiceDetail.Where(w => w.id_item == id_itemCurrent || (w.isActive && w.proformaPendingNumBoxes > 0 && invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == w.id_item) == null)).Select(s => s.Item).Distinct().ToList();

            List<tmpItem> items = new List<tmpItem>();
            if (invoiceCommercial.id_documentInvoice != null)
            {
                var _items = db.Item.AsEnumerable().Where(w => ((w.isSold && w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code == codeFEXP /*&& w?.Presentation.code == codePMASTER*/ &&
                                                         w.id_itemType == itemAux.id_itemType && w.id_itemTypeCategory == itemAux.id_itemTypeCategory && w.id_presentation == itemAux.id_presentation &&
                                                         w.ItemGeneral?.id_trademark == itemAux.ItemGeneral?.id_trademark && w.ItemGeneral?.id_color == itemAux?.ItemGeneral?.id_color) ||
                                                         w.id == id_itemCurrent))
                                              .Select(s => new { s.id, s.auxCode, s.name, s.masterCode }).ToList();
                _items.ForEach(r =>
                {
                    items.Add(new tmpItem
                    {
                        auxCode = r.auxCode,
                        id = r.id,
                        masterCode = r.masterCode,
                        name = r.name
                    });
                });
            }
            else
            {
                var _items = db.Item
                              .Where(w => w.isSold
                                         && w.isActive
                                         && w.id_company == this.ActiveCompanyId
                                         && w.InventoryLine.code == codeFEXP
                                         && w.Presentation.code.Substring(0, 1) == codeMaster
                                     ).Select(s => new { s.id, s.auxCode, s.name, s.masterCode }).ToList();

                _items.ForEach(r =>
                {
                    items.Add(new tmpItem
                    {
                        auxCode = r.auxCode,
                        id = r.id,
                        masterCode = r.masterCode,
                        name = r.name
                    });
                });
            }

            TempData.Keep("invoiceCommercial");
            TempData.Keep("id_Items");
            //ViewData["id_person"] = id_person;
            //var dataProvider = GetDataProvider(dataSource);
            return DevExpress.Web.Mvc.GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                //settings.Name = "id_person";

                p.ClientInstanceName = "id_item";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DevExpress.Web.DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = DevExpress.Web.ClearButtonDisplayMode.OnHover;
                //p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = DevExpress.Web.IncrementalFilteringMode.Contains;

                //p.NullDisplayText = "Todo";
                //p.NullText = "Todo";
                //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.CallbackRouteValues = new { Controller = "InvoiceCommercial", Action = "GetDetailItemInvoiceCommercial"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ItemComboBox_BeginCallback";
                p.ClientSideEvents.EndCallback = "ItemComboBox_EndCallback";
                p.CallbackPageSize = 20;
                //p.ClientSideEvents.Init = "ValueConditionSelect_Init";
                //p.ClientSideEvents.ValueChanged = "ValueConditionSelect_ValueChanged";
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                //p.ClientSideEvents.EndCallback = "ValueConditionSelect_EndCallback";
                p.DropDownStyle = DevExpress.Web.DropDownStyle.DropDownList;

                //p.ValueField = "id";
                //p.TextField = "name";
                p.TextFormatString = "{0} | {1}";
                p.ValueField = "id";
                p.ValueType = typeof(int);

                p.Columns.Add("masterCode", "Cod.", 50);
                p.Columns.Add("name", "Nombre", 200);
                p.Columns.Add("auxCode", "Cod. Aux.", 100);

                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                p.ClientSideEvents.SelectedIndexChanged = "ItemComboBox_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnItemComboBoxValidation";
                p.ClientSideEvents.Init = "ItemComboBox_Init";
                //p.TextField = textField;

                if (aItems == null)
                {
                    p.BindList(items);
                }
                else
                {
                    p.BindList(aItems);
                }
            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        public ActionResult LoadItemCombobox(int? id_itemCurrent)
        {
            MVCxColumnComboBoxProperties p;

            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            if (invoiceCommercial.Document.id_documentOrigen == null)
            {
                int? id_company = (int?)ViewData["id_company"];
                List<int> id_items = (List<int>)ViewData["id_items"];

                p = CreateComboBoxColumnProperties(id_company, id_items, null);
            }
            else
            {
                var aId_documentOrigen = invoiceCommercial.Document.id_documentOrigen;
                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                var aItems = proforma.Invoice.InvoiceDetail.Where(w => w.id_item == id_itemCurrent || (w.isActive && w.proformaPendingNumBoxes > 0 && invoiceCommercial.InvoiceCommercialDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == w.id_item) == null)).Select(s => s.Item1).Distinct().ToList();
                p = CreateComboBoxColumnProperties(null, null, aItems);
            }

            TempData.Keep("amountDetail");
            TempData.Keep("invoiceCommercial");
            TempData.Keep("id_Items");
            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        public static MVCxColumnComboBoxProperties CreateComboBoxColumnProperties(int? id_company, List<int> id_items, List<Item> aItems)
        {
            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "InvoiceCommercial", Action = "LoadItemCombobox" };
            p.ClientInstanceName = "id_item";
            p.ValueField = "id";
            p.TextFormatString = "{0} | {1}";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 20;
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            p.Columns.Add("masterCode", "Código.", 80);//, Unit.Percentage(50));
            p.Columns.Add("name", "Nombre del Producto", 300);
            p.Columns.Add("auxCode", "Cod.Aux.", 160);
            p.Columns.Add("Presentation.MetricUnit.code", "U.M.", 50);
            p.Columns.Add("description2", "Descripcion", 0);

            p.ClientSideEvents.Init = "ItemCombo_OnInit";
            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.ClientSideEvents.SelectedIndexChanged = "ItemComboBox_SelectedIndexChanged";
            p.ClientSideEvents.BeginCallback = "ItemComboBox_BeginCallback";
            p.ClientSideEvents.EndCallback = "ItemComboBox_EndCallback";
            //p.BindList(DXPANACEASOFT.DataProviders.DataProviderItem.SalesItemsByCompany(id_company, id_items));
            p.ClientSideEvents.Validation = "function (s, e) {e.isValid = (e.value != null ); e.errorText = 'Debe elegir un Item';}";
            if (aItems == null)
            {
                p.BindList(DXPANACEASOFT.DataProviders.DataProviderItem.SalesItemsByCompany(id_company, id_items));
            }
            else
            {
                p.BindList(aItems);
            }

            return p;
        }

        [HttpPost]
        public ActionResult GetDetailItemMarkedInvoiceCommercial(int? id_itemCurrent)
        {
            TempData.Keep("invoiceCommercial");

            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            Item itemAux = db.Item.FirstOrDefault(i => i.id == id_itemCurrent);
            itemAux = itemAux ?? new Item();
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            var codeFEXP = db.Setting.FirstOrDefault(fod => fod.code == "FEXP")?.value ?? "";
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
            List<tmpItem> items = new List<tmpItem>();
            if (invoiceCommercial.id_documentInvoice != null)
            {
                var _items = db.Item.AsEnumerable().Where(w => ((w.isSold && w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code == codeFEXP /*&& w?.Presentation.code == codePMASTER*/ &&
                                                         w.id_itemType == itemAux.id_itemType && w.id_itemTypeCategory == itemAux.id_itemTypeCategory && w.id_presentation == itemAux.id_presentation &&
                                                         w.ItemGeneral?.id_trademark == itemAux.ItemGeneral?.id_trademark && w.ItemGeneral?.id_color == itemAux?.ItemGeneral?.id_color) ||
                                                         w.id == id_itemCurrent))
                                              .Select(s => new { s.id, s.auxCode, s.name, s.masterCode }).ToList();
                _items.ForEach(r =>
                {
                    items.Add(new tmpItem
                    {
                        auxCode = r.auxCode,
                        id = r.id,
                        masterCode = r.masterCode,
                        name = r.name
                    });
                });
            }
            else
            {
                var _items = db.Item
                              .Where(w => w.isSold
                                         && w.isActive
                                         && w.id_company == this.ActiveCompanyId
                                         && w.InventoryLine.code == codeFEXP
                                         && w.Presentation.code.Substring(0, 1) == codeMaster
                                     ).Select(s => new { s.id, s.auxCode, s.name, s.masterCode }).ToList();

                _items.ForEach(r =>
                {
                    items.Add(new tmpItem
                    {
                        auxCode = r.auxCode,
                        id = r.id,
                        masterCode = r.masterCode,
                        name = r.name
                    });
                });
            }

            TempData.Keep("invoiceCommercial");

            return DevExpress.Web.Mvc.GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_itemMarked";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DevExpress.Web.DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = DevExpress.Web.ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = DevExpress.Web.IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "InvoiceCommercial", Action = "GetDetailItemMarkedInvoiceCommercial" };
                p.ClientSideEvents.BeginCallback = "ItemMarkedComboBox_BeginCallback";
                //p.ClientSideEvents.EndCallback = "ItemMarkedComboBox_EndCallback";
                p.CallbackPageSize = 20;

                p.DropDownStyle = DevExpress.Web.DropDownStyle.DropDownList;

                p.TextFormatString = "{0} | {1}";
                p.ValueField = "id";
                p.ValueType = typeof(int);

                p.Columns.Add("masterCode", "Cod.", 50);
                p.Columns.Add("name", "Nombre", 200);
                p.Columns.Add("auxCode", "Cod. Aux.", 100);

                p.ClientSideEvents.SelectedIndexChanged = "ItemMarkedComboBox_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnItemMarkedComboBoxValidation";
                p.ClientSideEvents.Init = "ItemMarkedComboBox_Init";

                p.BindList(items);
            });
        }

        [HttpPost]
        public JsonResult UpdateInvoiceCommercialDetail(
                                                            int id_itemCurrent,
                                                            string numBoxesCurrent,
                                                            string unitPriceCurrent,
                                                            int? id_MetricUnitInvoice,
                                                            //string  weightBox,
                                                            Boolean hasGlazeValue
                                                        )
        {
            GenericResultJson resultJson = new GenericResultJson();
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            MetricUnit metricUnitInvoice = null;
            var aId_documentOrigen = invoiceCommercial?.Document.id_documentOrigen;
            var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
            var aId_item = proforma.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == id_itemCurrent && fod.isActive);
            var numBoxes_Aux = (aId_item?.proformaPendingNumBoxes ?? 0);
            var codigoItemMarked = aId_item?.Item.masterCode ?? "";
            var unitPrice_Aux = aId_item?.unitPrice ?? 0;
            var nombreItemMarked = aId_item?.Item.name ?? "";

            try
            {
                resultJson.ValueDataList = new List<ValueData>();
                InvoiceCommercialDetail invoiceCommercialDetail = invoiceCommercial
                                                                        .InvoiceCommercialDetail
                                                                        .FirstOrDefault(r => r.id_item == id_itemCurrent);

                if (invoiceCommercialDetail == null)
                {
                    invoiceCommercialDetail = new InvoiceCommercialDetail();
                    invoiceCommercialDetail.id_item = id_itemCurrent;
                }

                invoiceCommercialDetail.hasGlaze_DetailOperation = hasGlazeValue;
                invoiceCommercialDetail.numBoxes = numBoxes_Aux;
                invoiceCommercialDetail.unitPrice = unitPrice_Aux;
                invoiceCommercialDetail.amountInvoice = aId_item?.amount ?? 0;
                invoiceCommercialDetail.total = aId_item?.total ?? 0;
                invoiceCommercialDetail.id_itemMarked = aId_item.id_itemMarked;
                invoiceCommercialDetail.id_MetricUnitInvoice_DetailOperation = id_MetricUnitInvoice;

                //invoiceCommercialDetail.weightBox = decimal.Round(Convert.ToDecimal(weightBox), 2);

                if (id_MetricUnitInvoice == 0 || id_MetricUnitInvoice == 999)
                {
                    invoiceCommercial.MetricUnit = null;
                    invoiceCommercial.id_metricUnitInvoice = null;
                }
                else
                {
                    metricUnitInvoice = db.MetricUnit.FirstOrDefault(r => r.id == id_MetricUnitInvoice);
                    invoiceCommercial.MetricUnit = metricUnitInvoice;
                    invoiceCommercial.id_metricUnitInvoice = id_MetricUnitInvoice;
                }

                invoiceCommercialDetail.CalculateDetailInvoiceCommercialDetail();

                #region Cambio por Metodo Extension InvoiceCommercialDetail

                #endregion

                resultJson.codeReturn = 1;
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialAuxCode", valueObject = invoiceCommercialDetail.auxCode_Inf });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialMasterCode", valueObject = invoiceCommercialDetail.masterCode_Inf });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialForeignName", valueObject = invoiceCommercialDetail.foreignName_Inf });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "amountInvoice", valueObject = invoiceCommercialDetail.amountInvoice });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialUM", valueObject = invoiceCommercialDetail.codeMetricUnit_Inf });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "invoiceCommercialTotal", valueObject = invoiceCommercialDetail.total });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "weightBox", valueObject = invoiceCommercialDetail.weightBox.ToString() });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "weightBoxUM", valueObject = invoiceCommercialDetail.codeMetricUnitOrigin_Inf.ToString() });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "unitPrice", valueObject = invoiceCommercialDetail.unitPrice });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "numBoxes", valueObject = invoiceCommercialDetail.numBoxes });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "id_itemMarked", valueObject = invoiceCommercialDetail.id_itemMarked });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "codigoItemMarked", valueObject = codigoItemMarked });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "nombreItemMarked", valueObject = nombreItemMarked });

                TempData["invoiceCommercial"] = invoiceCommercial;
            }
            catch (Exception e)
            {
                resultJson.codeReturn = -1;
                resultJson.message = e.Message;
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialAuxCode", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialMasterCode", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialForeignName", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "amountInvoice", valueObject = "0" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "itemInvoiceCommercialUM", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "invoiceCommercialTotal", valueObject = "0" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "weightBox", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "weightBoxUM", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "unitPrice", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "numBoxes", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "id_itemMarked", valueObject = null });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "codigoItemMarked", valueObject = "" });
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "nombreItemMarked", valueObject = "" });
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InvoiceCommercialExporExcel(int id_invoice)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            TempData["invoiceCommercial"] = invoiceCommercial;
            TempData.Keep("invoiceCommercial");

            List<ParamCR> paramLst = new List<ParamCR>();

            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            //_param.Valor = id_invoice;
            _param.Valor = id_invoice;
            paramLst.Add(_param);

            /*_param = new ParamCR();
			_param.Nombre = "@callIdentity";
			_param.Valor = "28fa6c0f-eb19-4758-83e6-b52aae1ca0aa";
			paramLst.Add(_param);
			*/

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "FECEXC";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;
            _repMod.nameReport = "Factura Comercial";

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;

            List<ResultInvoiceComercial> modelAux = new List<ResultInvoiceComercial>();
            modelAux = db.Database.SqlQuery<ResultInvoiceComercial>
                    ("exec par_InvoiceExteriorCommercialCR @id",
                    new SqlParameter("id", paramLst[0].Valor)
                    ).ToList();

            TempData["modelInvoiceComercial"] = modelAux;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Métodos de actualización entre facturas y proformas

        private void UpdateSalesQuotationExterior(
            Document document, InvoiceCommercial invoiceCommercial)
        {
            var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
            var idEstadoAprParcial = db.DocumentState.FirstOrDefault(e => e.code == "02")?.id;
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
            var idTipProforma = db.DocumentType.FirstOrDefault(r => r.code.Equals("131"))?.id;
            var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;
            var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;

            var proformasDoc = db.Document
                .Where(e => e.id == document.id_documentOrigen && e.id_documentType == idTipProforma && e.id_documentState != idEstadoAnulado)
                .ToList();

            if (proformasDoc
                .Any(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial))
            {
                var proformaDoc = proformasDoc.FirstOrDefault();
                throw new Exception(
                    $"No se pueden actualizar datos en la proforma {proformaDoc.number} porque está está {proformaDoc.DocumentState.name}");
            }

            foreach (var proformaDoc in proformasDoc)
            {
                // Actualizamos la proforma
                if (proformaDoc.Invoice == null) continue; // Si invoice es null, no actualizamos nada

                UpdateSalesQuotationData(proformaDoc, invoiceCommercial);

                #region buscamos las facturas fiscales relacionadas a la proforma
                var facturasFiscalesDoc = db.Document
                   .Where(e => e.id_documentOrigen == proformaDoc.id && e.id_documentType == idTipoDocFactFiscal
                        && e.id_documentState != idEstadoAnulado && e.id != invoiceCommercial.id)
                   .ToList();

                if (facturasFiscalesDoc
                    .Any(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial))
                {
                    var facturaFiscalDoc = facturasFiscalesDoc
                        .FirstOrDefault(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial);

                    throw new Exception(
                        $"No se pueden actualizar datos en la factura fiscal {facturaFiscalDoc.number} porque está está {facturaFiscalDoc.DocumentState.name}");
                }

                foreach (var facturaFiscalDoc in facturasFiscalesDoc)
                {
                    if (facturaFiscalDoc.Invoice == null) continue; // Si invoice es null, no actualizamos nada

                    UpdateInvoiceExteriorData(facturaFiscalDoc, invoiceCommercial);

                    #region Verificamos existencia de una faactura comercial Asociada
                    var facturasComercialesFiscalesDoc = db.Document
                       .Where(e => e.id_documentOrigen == facturaFiscalDoc.id && e.id_documentType == idTipoDocFactComercial
                            && e.id_documentState != idEstadoAnulado);

                    if (facturasComercialesFiscalesDoc != null)
                    {
                        foreach (var facturaComercialDoc in facturasComercialesFiscalesDoc)
                        {
                            UpdateInvoiceComercialData(facturaComercialDoc, invoiceCommercial);
                        }
                    }
                    #endregion
                }
                #endregion

                #region buscamos las facturas comerciales relacionadas a la proforma
                var facturasComercialesDoc = db.Document
                   .Where(e => e.id_documentOrigen == proformaDoc.id && e.id_documentType == idTipoDocFactComercial
                        && e.id_documentState != idEstadoAnulado && e.id != invoiceCommercial.id)
                   .ToList();

                if (facturasComercialesDoc
                    .Any(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial))
                {
                    var facturaComercialDoc = facturasComercialesDoc.FirstOrDefault();

                    throw new Exception(
                        $"No se pueden actualizar datos en la factura comercial {facturaComercialDoc.number} porque está está {facturaComercialDoc.DocumentState.name}");
                }

                // Actualizamos las facturas comerciales
                foreach (var facturaComercialDoc in facturasComercialesDoc)
                {
                    var invoiceFacComercial = facturaComercialDoc.InvoiceCommercial;
                    if (invoiceFacComercial == null) continue; // Si invoice es null, no actualizamos nada

                    UpdateInvoiceComercialData(facturaComercialDoc, invoiceCommercial);

                    #region Verificamos existencia de una faactura fiscal Asociada
                    var facturasFiscalesComercialDoc = db.Document
                       .Where(e => e.id_documentOrigen == facturaComercialDoc.id && e.id_documentType == idTipoDocFactFiscal
                            && e.id_documentState != idEstadoAnulado);

                    if (facturasFiscalesComercialDoc != null)
                    {
                        foreach (var facturaFiscalDoc in facturasFiscalesComercialDoc)
                        {
                            UpdateInvoiceExteriorData(facturaFiscalDoc, invoiceCommercial);
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }
        private void UpdateInvoicesExterior(int? idDocument, int? id_documentOrigin, InvoiceCommercial invoiceFacComercial)
        {
            var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
            var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;

            var facturasFiscalesDoc = db.Document.Where(e => e.id_documentOrigen == idDocument &&
                    e.id_documentType == idTipoDocFactFiscal && e.id_documentState != idEstadoAnulado)
                .Union(db.Document.Where(e => e.id == id_documentOrigin &&
                        e.id_documentType == idTipoDocFactFiscal && e.id_documentState != idEstadoAnulado))
                .ToList();

            if (facturasFiscalesDoc.Any(e => e.id_documentState != idEstadoPendiente))
            {
                var facturaFiscal = facturasFiscalesDoc.FirstOrDefault(e => e.id_documentState != idEstadoPendiente);

                throw new Exception(
                    $"No se pueden actualizar datos en la factura fiscal {facturaFiscal.number} porque está está {facturaFiscal.DocumentState.name}");
            }

            // Actualizamos las facturas comerciales
            foreach (var facturaFiscal in facturasFiscalesDoc)
            {
                var invoiceFacFiscal = facturaFiscal.Invoice;
                if (invoiceFacFiscal == null) continue; // Si invoice es null, no actualizamos nada

                UpdateInvoiceExteriorData(facturaFiscal, invoiceFacComercial);
            }
        }

        private void UpdateSalesQuotationData(Document proforma,
            InvoiceCommercial invoiceFacComercial)
        {
            var salesQuotationExterior = proforma.Invoice.SalesQuotationExterior;
            salesQuotationExterior.id_consignee = invoiceFacComercial.id_Consignee; // consignatario
            proforma.Invoice.id_buyer = invoiceFacComercial.id_ForeignCustomer; // Cliente del exterior
            salesQuotationExterior.id_notifier = invoiceFacComercial.id_Notifier; // Notificador
            salesQuotationExterior.purchaseOrder = invoiceFacComercial.purchaseOrder; // Orden de pedido
            salesQuotationExterior.idVendor = invoiceFacComercial.idVendor; // Vendedor
            salesQuotationExterior.id_termsNegotiation = invoiceFacComercial.id_termsNegotiation;// Término de Negociación
            salesQuotationExterior.id_PaymentMethod = invoiceFacComercial.id_PaymentMethod; // Forma de Pago
            salesQuotationExterior.id_PaymentTerm = invoiceFacComercial.id_PaymentTerm; // Plazo de Pago
            salesQuotationExterior.dateShipment = invoiceFacComercial.dateShipment; //  Fecha de Embarque
            salesQuotationExterior.id_portShipment = invoiceFacComercial.id_portShipment; // Puerto de Embarque
            salesQuotationExterior.id_portDischarge = invoiceFacComercial.id_portDischarge; // Puerto Descarga
            salesQuotationExterior.id_portDestination = invoiceFacComercial.id_portDestination; //Puerto Destino
            salesQuotationExterior.id_BankTransfer = invoiceFacComercial.id_BankTransfer; //Banco de Transferencia

            proforma.dateUpdate = DateTime.Today;
            proforma.id_userUpdate = this.ActiveUserId;

            db.Document.Attach(proforma);
            db.Entry(proforma).State = EntityState.Modified;
        }
        private void UpdateInvoiceExteriorData(Document facturaFiscalDoc,
           InvoiceCommercial invoiceFacComercial)
        {
            var invoiceExterior = facturaFiscalDoc.Invoice.InvoiceExterior;
            invoiceExterior.id_consignee = invoiceFacComercial.id_Consignee; // consignatario
            facturaFiscalDoc.Invoice.id_buyer = invoiceFacComercial.id_ForeignCustomer; // Cliente del exterior
            invoiceExterior.id_notifier = invoiceFacComercial.id_Notifier; // Notificador
            invoiceExterior.purchaseOrder = invoiceFacComercial.purchaseOrder; // Orden de pedido
            invoiceExterior.idVendor = invoiceFacComercial.idVendor; // Vendedor
            invoiceExterior.id_termsNegotiation = invoiceFacComercial.id_termsNegotiation;// Término de Negociación
            invoiceExterior.idPortfolioFinancing = invoiceFacComercial.idPortfolioFinancing; //Financiamiento de Cartera
            invoiceExterior.id_PaymentMethod = invoiceFacComercial.id_PaymentMethod; // Forma de Pago
            invoiceExterior.id_PaymentTerm = invoiceFacComercial.id_PaymentTerm; // Plazo de Pago
            invoiceExterior.id_BankTransfer = invoiceFacComercial.id_BankTransfer; // Banco de transferencia
            invoiceExterior.dateShipment = invoiceFacComercial.dateShipment; // Fecha de Embarque
            invoiceExterior.id_portShipment = invoiceFacComercial.id_portShipment; // Puerto de Embarque
            invoiceExterior.id_portDischarge = invoiceFacComercial.id_portDischarge; // Puerto Descarga
            invoiceExterior.id_portDestination = invoiceFacComercial.id_portDestination; //Puerto Destino
            invoiceExterior.etaDate = invoiceFacComercial.etaDate; // Fecha de Arribo y/o Fecha ETA
            invoiceExterior.id_shippingAgency = invoiceFacComercial.id_shippingAgency; //Agencia Naviera
            invoiceExterior.id_ShippingLine = invoiceFacComercial.id_shippingLine;// Línea Naviera
            invoiceExterior.shipName = invoiceFacComercial.shipName; //Buque
            invoiceExterior.shipNumberTrip = invoiceFacComercial.shipNumberTrip; // Viaje
            invoiceExterior.id_BankTransfer = invoiceFacComercial.id_BankTransfer; // Banco de Transferencia

            // Partida arrancelarias
            var id_tariffHeading = db.TariffHeading
                .FirstOrDefault(e => e.id == invoiceFacComercial.id_tariffHeading)?
                .id;
            invoiceExterior.id_tariffHeading = id_tariffHeading;

            invoiceExterior.BLNumber = invoiceFacComercial.BLNumber; // Número de BL
            invoiceExterior.blDate = invoiceFacComercial.blDate; // Fecha de BL
            invoiceExterior.daeNumber = invoiceFacComercial.daeNumber; // DAE
            invoiceExterior.daeNumber2 = invoiceFacComercial.daeNumber2; // DAE2
            invoiceExterior.daeNumber3 = invoiceFacComercial.daeNumber3; // DAE3
            invoiceExterior.daeNumber4 = invoiceFacComercial.daeNumber4; // DAE4
            invoiceExterior.containers = invoiceFacComercial.containers;


            invoiceExterior.id_CityDelivery = invoiceFacComercial.id_CityDelivery; // Ciudad de Entrega
            invoiceExterior.seals = invoiceFacComercial.seals; // Sellos

            facturaFiscalDoc.id_userUpdate = this.ActiveUserId;
            facturaFiscalDoc.dateUpdate = DateTime.Now;

            db.InvoiceExterior.Attach(invoiceExterior);
            db.Entry(invoiceExterior).State = EntityState.Modified;

            db.Document.Attach(facturaFiscalDoc);
            db.Entry(facturaFiscalDoc).State = EntityState.Modified;
        }
        private void UpdateInvoiceComercialData(Document facturaComercialDoc,
            InvoiceCommercial invoiceCommercial)
        {
            var invoiceFacComercial = facturaComercialDoc.InvoiceCommercial;
            invoiceFacComercial.id_Consignee = invoiceCommercial.id_Consignee; // consignatario
            invoiceFacComercial.id_ForeignCustomer = invoiceCommercial.id_ForeignCustomer; // Cliente del exterior
            invoiceFacComercial.id_Notifier = invoiceCommercial.id_Notifier; // Notificador
            invoiceFacComercial.purchaseOrder = invoiceCommercial.purchaseOrder; // Orden de pedido
            invoiceFacComercial.idVendor = invoiceCommercial.idVendor; // Vendedor
            invoiceFacComercial.id_termsNegotiation = invoiceCommercial.id_termsNegotiation;// Término de Negociación
            invoiceFacComercial.idPortfolioFinancing = invoiceCommercial.idPortfolioFinancing; //Financiamiento de Cartera
            invoiceFacComercial.id_PaymentMethod = invoiceCommercial.id_PaymentMethod; // Forma de Pago
            invoiceFacComercial.id_PaymentTerm = invoiceCommercial.id_PaymentTerm; // Plazo de Pago
            invoiceFacComercial.id_BankTransfer = invoiceCommercial.id_BankTransfer; // Banco de transferencia
            invoiceFacComercial.dateShipment = invoiceCommercial.dateShipment; // Fecha de Embarque
            invoiceFacComercial.id_portShipment = invoiceCommercial.id_portShipment; // Puerto de Embarque
            invoiceFacComercial.id_portDischarge = invoiceCommercial.id_portDischarge; // Puerto Descarga
            invoiceFacComercial.id_portDestination = invoiceCommercial.id_portDestination; //Puerto Destino
            invoiceFacComercial.etaDate = invoiceCommercial.etaDate; // Fecha de Arribo y/o Fecha ETA
            invoiceFacComercial.id_shippingAgency = invoiceCommercial.id_shippingAgency; //Agencia Naviera
            invoiceFacComercial.id_shippingLine = invoiceCommercial.id_shippingLine;// Línea Naviera
            invoiceFacComercial.shipName = invoiceCommercial.shipName; //Buque
            invoiceFacComercial.shipNumberTrip = invoiceCommercial.shipNumberTrip; // Viaje
            invoiceFacComercial.id_BankTransfer = invoiceCommercial.id_BankTransfer; // Banco de Transferencia

            // Partida arrancelarias
            invoiceFacComercial.id_tariffHeading = invoiceCommercial.id_tariffHeading;

            invoiceFacComercial.BLNumber = invoiceCommercial.BLNumber; // Número de BL
            invoiceFacComercial.blDate = invoiceCommercial.blDate; // Fecha de BL
            invoiceFacComercial.daeNumber = invoiceCommercial.daeNumber; // DAE
            invoiceFacComercial.daeNumber2 = invoiceCommercial.daeNumber2; // DAE2
            invoiceFacComercial.daeNumber3 = invoiceCommercial.daeNumber3; // DAE3
            invoiceFacComercial.daeNumber4 = invoiceCommercial.daeNumber4; // DAE4
            invoiceFacComercial.containers = invoiceCommercial.containers;


            invoiceFacComercial.id_CityDelivery = invoiceCommercial.id_CityDelivery; // Ciudad de Entrega
            invoiceFacComercial.seals = invoiceCommercial.seals; // Sellos

            facturaComercialDoc.dateUpdate = DateTime.Now;
            facturaComercialDoc.id_userUpdate = this.ActiveUserId;

            db.Document.Attach(facturaComercialDoc);
            db.Entry(facturaComercialDoc).State = EntityState.Modified;

            db.InvoiceCommercial.Attach(invoiceFacComercial);
            db.Entry(invoiceFacComercial).State = EntityState.Modified;
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public JsonResult SetPaymentMethod(int? id_invoice, int? id_paymentMethod)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = null;
            PaymentMethod paymentMethod = null;
            GenericResultJson resultJson = new GenericResultJson();
            resultJson.codeReturn = 1;
            resultJson.message = "";

            try
            {
                invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
                invoiceCommercial = invoiceCommercial ?? db.InvoiceCommercial.FirstOrDefault(i => i.id == id_invoice);
                invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
                if (id_paymentMethod != 0)
                {
                    paymentMethod = db.PaymentMethod.FirstOrDefault(r => r.id == id_paymentMethod);

                    invoiceCommercial.PaymentMethod = paymentMethod;
                    invoiceCommercial.id_PaymentMethod = (int)id_paymentMethod;
                }
                else
                {
                    invoiceCommercial.PaymentMethod = null;
                    invoiceCommercial.id_PaymentMethod = 0;
                }
            }
            catch (Exception e)
            {
                resultJson.codeReturn = -1;
                resultJson.message = e.Message;
            }
            finally
            {
                TempData["invoiceCommercial"] = invoiceCommercial;
                TempData.Keep("invoiceCommercial");
            }

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetInfoBank(int id_BankTransfer)
        {
            TempData.Keep("invoiceCommercial");
            GenericResultJson resultJson = new GenericResultJson();
            resultJson.codeReturn = 1;
            resultJson.message = "";

            try
            {
                tbsysCatalogueDetail oBankTransfer = db.tbsysCatalogueDetail.FirstOrDefault(r => r.id == id_BankTransfer);
                if (oBankTransfer == null)
                {
                    throw new Exception("No existe el Banco indicado");
                }

                string infoBankTransfer = oBankTransfer.fldFullText;
                if (infoBankTransfer == null)
                {
                    throw new Exception("No existe información de Transferencia  del Banco seleccionado");
                }
                resultJson.ValueDataList = new List<ValueData>();
                resultJson.ValueDataList.Add(new ValueData { CodeObject = "infoBankTransfer", valueObject = infoBankTransfer });
            }
            catch (Exception e)
            {
                resultJson.codeReturn = -1;
                resultJson.message = e.Message;
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPaymentTerm()
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            // invoiceCommercial = invoiceCommercial ?? db.InvoiceCommercial.FirstOrDefault(i => i.id == id_invoice);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            TempData.Keep("invoiceCommercial");
            return PartialView("_InvoiceCommercialMainFormTabInvoiceExteriorPaymentTerm", invoiceCommercial);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SetShippingAgency(int? id_invoice, int? id_shippingAgency)
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = null;
            ShippingAgency shippingAgency = null;
            var jresult = new { error = true, msgerr = "" };

            try
            {
                invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
                invoiceCommercial = invoiceCommercial ?? db.InvoiceCommercial.FirstOrDefault(i => i.id == id_invoice);
                invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
                if (id_shippingAgency != 0)
                {
                    shippingAgency = db.ShippingAgency.FirstOrDefault(r => r.id == id_shippingAgency);

                    invoiceCommercial.ShippingAgency = shippingAgency;
                    invoiceCommercial.id_shippingAgency = (int)id_shippingAgency;
                }
                else
                {
                    invoiceCommercial.ShippingAgency = null;
                    invoiceCommercial.id_shippingAgency = 0;
                }

                jresult = new { error = false, msgerr = "" };
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
                TempData["invoiceCommercial"] = invoiceCommercial;
                TempData.Keep("invoiceCommercial");
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetShippingLine()
        {
            TempData.Keep("invoiceCommercial");
            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
            // invoiceCommercial = invoiceCommercial ?? db.InvoiceCommercial.FirstOrDefault(i => i.id == id_invoice);
            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();
            TempData.Keep("invoiceCommercial");
            return PartialView("_InvoiceCommercialMainFormTabInvoiceCommercialShippingLine", invoiceCommercial);
        }

        /*  Cambio de Unidad de Medida */

        [HttpPost]
        public JsonResult ChangeMetricUnitInvoiceMaster(int id_MetricUnitInvoice)
        {
            var jresult = new { error = false, msgerr = "" };
            InvoiceCommercial invoiceCommercial = null;

            try
            {
                invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);
                invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

                invoiceCommercial.ConversionMetricUnitCorrectTotalValue(id_MetricUnitInvoice, (int)ViewData["id_company"]);

                TempData["invoiceCommercial"] = invoiceCommercial;
                TempData.Keep("invoiceCommercial");
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
                TempData.Keep("invoiceCommercial");
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        #region Generacion de Factura Commercial desde F. Exterior

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorDetailsResults()
        {
            //agregar campo Company / Estatus
            List<int> ids_InvoiceCommercial = db.InvoiceCommercial.Select(s => s.id).ToList();

            List<InvoiceExterior> model;
            var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
            if (puedeModDatosProforma)
            {
                model = db.InvoiceExterior
                    .Where(d => (d.Invoice.Document.DocumentState.code != "05") // "05" Anulado
                                && !ids_InvoiceCommercial.Contains(d.id)
                            )
                    .OrderByDescending(d => d.id).ToList();
            }
            else
            {
                model = db.InvoiceExterior
                    .Where(d => (
                                    d.Invoice.Document.DocumentState.code == "06" || //"06" AUTORIZADA
                                    d.Invoice.Document.DocumentState.code == "09"    //"09" Preautorizada
                                )
                                && !ids_InvoiceCommercial.Contains(d.id)
                            )
                    .OrderByDescending(d => d.id).ToList();
            }

            return PartialView("_InvoiceExteriorDetailsResultsPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorDetailsPartial()
        {
            //agregar campo Company / Estatus
            List<int> ids_InvoiceCommercial = db.InvoiceCommercial.Select(s => s.id).ToList();

            List<InvoiceExterior> model;
            var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
            if (puedeModDatosProforma)
            {
                model = db.InvoiceExterior
                    .Where(d => (d.Invoice.Document.DocumentState.code != "05") // "05" Anulado
                                && !ids_InvoiceCommercial.Contains(d.id)
                            )
                    .OrderByDescending(d => d.id).ToList();
            }
            else
            {
                model = db.InvoiceExterior
                    .Where(d => (
                                    d.Invoice.Document.DocumentState.code == "06" || //"06" AUTORIZADA
                                    d.Invoice.Document.DocumentState.code == "09"    //"09" Preautorizada
                                )
                                && !ids_InvoiceCommercial.Contains(d.id)
                            )
                    .OrderByDescending(d => d.id).ToList();
            }

            return PartialView("_InvoiceExteriorDetailsPartial", model);
        }

        [HttpPost]
        public JsonResult ValidateSelectedRowsPurchaseOrder(int[] ids)
        {
            var result = new
            {
                Message = "OK"
            };

            Provider providerFirst = null;
            Provider providerCurrent = null;
            ProductionUnitProvider productionUnitProviderFirst = null;
            ProductionUnitProvider productionUnitProviderCurrent = null;
            Person personBuyer = null;
            Person personBuyerCurrent = null;
            bool requiredLogistic = false;
            bool requiredLogisticCurrent = false;

            string codeState = "";
            string nameState = "";

            int count = 0;
            foreach (var i in ids)
            {
                codeState = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder?.Document?.DocumentState?.code ?? "";
                nameState = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder?.Document?.DocumentState?.name ?? "";

                if (codeState != "06")
                {
                    result = new
                    {
                        Message = ErrorMessage("La orden de compra debe estar en estado Autorizado y actualmente se encuentra en estado " + nameState + ".")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                providerCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.Provider;
                productionUnitProviderCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.ProductionUnitProvider;
                personBuyerCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.Person;
                requiredLogisticCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i).PurchaseOrder.requiredLogistic;

                if (count == 0)
                {
                    providerFirst = providerCurrent;
                    productionUnitProviderFirst = productionUnitProviderCurrent;
                    personBuyer = personBuyerCurrent;
                    requiredLogistic = requiredLogisticCurrent;
                }

                if (providerCurrent != providerFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con proveedores diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (productionUnitProviderFirst != productionUnitProviderCurrent)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con Unidades de Producción diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (personBuyer != personBuyerCurrent)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con Compradores diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (requiredLogisticCurrent != requiredLogistic)
                {
                    result = new
                    {
                        Message = ErrorMessage("Los Detalles seleccionados provienen de órdenes de compra con distinto tipo de Logística")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                count++;
            }

            TempData.Keep("remissionGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult PRInvoiceComercial
                            (
                               string codeReport,
                               int[] customers,
                               string identification,
                                int[] consignees,
                                int[] notifiers,
                                int[] notifiers2,
                                //Cliente
                                int? id_documentState,
                                DateTime? startEmissionDate,
                                DateTime? endEmissionDate,
                                string numberInvoiceFiscal,
                                //Document
                                DateTime? startDateShipment,
                                DateTime? endDateShipment,
                                int?[] id_shippingAgencys,
                                int?[] id_portDischarges,
                                int?[] id_portDestinations,
                                string BLNumber,
                                string referenceInvoice
                            )
        {
            List<InvoiceCommercial> invoiceCommercialList = new List<InvoiceCommercial>();
            ReportParanNameModel rep = new ReportParanNameModel();
            GenericResultJson JsonResult = new GenericResultJson();
            string idsInvoiceCommercial = "";

            try
            {
                List<ParamCR> paramLst = new List<ParamCR>();

                Boolean withParam = validateHavingParameter
                                    (
                                            customers,
                                            identification,
                                            consignees,
                                            notifiers,
                                            notifiers2,
                                             //Cliente
                                             id_documentState,
                                            startEmissionDate,
                                            endEmissionDate,
                                            numberInvoiceFiscal,
                                            //Document
                                            startDateShipment,
                                            endDateShipment,
                                            id_shippingAgencys,
                                            id_portDischarges,
                                            id_portDestinations,
                                            BLNumber,
                                            referenceInvoice
                                        );
                if (withParam)
                {
                    invoiceCommercialList = findResult
                                        (
                                            customers,
                                            identification,
                                            consignees,
                                            notifiers,
                                            notifiers2,
                                             //Cliente
                                             id_documentState,
                                            startEmissionDate,
                                            endEmissionDate,
                                            numberInvoiceFiscal,
                                            //Document
                                            startDateShipment,
                                            endDateShipment,
                                            id_shippingAgencys,
                                            id_portDischarges,
                                            id_portDestinations,
                                            BLNumber,
                                            referenceInvoice
                                        );

                    string delimiter = ",";
                    idsInvoiceCommercial = invoiceCommercialList
                                                    .Select(r => r.id.ToString())
                                                    .ToList()
                                                    .Aggregate((i, j) => i + delimiter + j);
                }

                ParamCR _param = new ParamCR();
                _param.Nombre = "@idsInvoicesList";
                _param.Valor = idsInvoiceCommercial;
                paramLst.Add(_param);
                #region "Armo Parametros"

                Conexion objConex = GetObjectConnection("DBContextNE");

                ReportProdModel _repMod = new ReportProdModel();
                _repMod.codeReport = codeReport;
                _repMod.conex = objConex;
                _repMod.paramCRList = paramLst;
                rep = GetTmpDataName(20);

                TempData[rep.nameQS] = _repMod;
                TempData.Keep(rep.nameQS);

                var result = rep;

                #endregion

                JsonResult.codeReturn = 1;
                JsonResult.ValueDataList = new List<ValueData>();
                JsonResult.ValueDataList.Add(new ValueData { CodeObject = "returnReport", valueObject = rep });
            }
            catch (Exception e)
            {
                LogWrite(e, null, "PRInvoiceComercial");
                JsonResult.codeReturn = -1;
                JsonResult.message = ErrorMessage(e.Message);
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}