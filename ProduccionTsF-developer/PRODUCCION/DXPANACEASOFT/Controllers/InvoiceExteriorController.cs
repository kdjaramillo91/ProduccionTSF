using DevExpress.CodeParser.VB;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.Native;
using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using System.Xml;

namespace DXPANACEASOFT.Controllers
{
    public class InvoiceExteriorController : DefaultController
    {
        private const int LOGON_TYPE_NEW_CREDENTIALS = 9;
        private const int LOGON32_PROVIDER_WINNT50 = 3;
        private const string DOCUMENT_STATE_APROVEE = "03";
        private const string PARAMETRO_VALIDAFACTURA_INVENTARIO = "INVFACT";

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        private void UpdateStateInvoiceExteriorAutorize()
        {
            string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];

            if (!string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    LogController.tratarFicheroLog();
                    bool change = false;
                    //bool changeLista = false;
                    try
                    {
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada
                        ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03"); //Autorizada
                        String[] dirs = System.IO.Directory.GetDirectories(routePathB1AutorizadoActualizado);
                        var todayAux = DateTime.Today;
                        var yearMothCurrent = todayAux.Year.ToString() + todayAux.Month.ToString("D2");
                        var yearMothCurrentPlus = todayAux.AddMonths(-1).Year.ToString() + todayAux.AddMonths(-1).Month.ToString("D2");
                        foreach (String dir in dirs)
                        {
                            var directoryNameAux = dir.Substring(dir.Length - 8, 6);
                            if (directoryNameAux == yearMothCurrent ||
                                directoryNameAux == yearMothCurrentPlus)
                            {
                                String[] files = System.IO.Directory.GetFileSystemEntries(dir);
                                foreach (String file in files)
                                {
                                    string[] words = Path.GetFileNameWithoutExtension(file).Split('_');
                                    string accessKeyAux = words[0];
                                    var aDocument = db.Document.FirstOrDefault(fod => fod.accessKey == accessKeyAux);
                                    if (aDocument != null && aDocument.DocumentState.code == "09")//Estado de PRE-AUTORIZADA
                                    {
                                        aDocument.dateUpdate = DateTime.Now;
                                        aDocument.id_userUpdate = ActiveUser.id;
                                        aDocument.id_documentState = documentState.id;
                                        aDocument.DocumentState = documentState;
                                        aDocument.authorizationNumber = aDocument.accessKey;

                                        db.Document.Attach(aDocument);
                                        db.Entry(aDocument).State = EntityState.Modified;

                                        var aElectronicDocument = db.ElectronicDocument.FirstOrDefault(fod => fod.Document.accessKey == accessKeyAux);
                                        if (aElectronicDocument != null)
                                        {
                                            aElectronicDocument.id_electronicDocumentState = electronicDocumentState.id;
                                            aElectronicDocument.ElectronicDocumentState = electronicDocumentState;

                                            db.ElectronicDocument.Attach(aElectronicDocument);
                                            db.Entry(aElectronicDocument).State = EntityState.Modified;
                                        }
                                        if (!change) change = true;
                                    }
                                }
                            };
                        }
                        if (change)
                        {
                            db.SaveChanges();
                            trans.Commit();
                            LogController.WriteLog("Actualización de Estados de las Facturas del Exterior a Estado Autorizado Satisfactoriamente(Con Cambios)");
                            LogController.WriteLog("Actualización de la Lista de Carpetas de XML Autorizadas Satisfactoriamente");
                        }
                        else
                        {
                            LogController.WriteLog("Actualización de Estados de las Facturas del Exterior a Estado Autorizado Satisfactoriamente(Sin Cambios)");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (change)
                        {
                            trans.Rollback();
                        }
                        LogController.WriteLog(ex.Message);
                    }
                }
            }
        }

        #region {RA} - Transacion Invoice

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorPartial()
        {
            var modelFX = (TempData["modelFX"] as List<Invoice>);
            modelFX = modelFX ?? new List<Invoice>();
            var dd = ViewData;
            TempData.Keep("modelFX");
            TempData.Keep("parametersSeekInvoiceExterior");

            return PartialView("_InvoiceexteriorPartial", modelFX.OrderByDescending(o => o.id).ToList());
        }

        private void SaveInvoiceCommercial(int id_invoice)
        {
            Invoice _invoice = db.Invoice.FirstOrDefault(r => r.id == id_invoice);
            int id_company = (int)ViewData["id_company"];

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"));
                    DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                    tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

                    List<SettingDetail> settingsInvoiceCommercial = db.Setting.FirstOrDefault(r => r.code == "INPFC").SettingDetail.ToList();
                    string codeCompany = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CCIA").valueAux;
                    string codeBranchOffice = settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CESTB").valueAux;

                    int codeEmissionPoint = int.Parse(settingsInvoiceCommercial.FirstOrDefault(rr => rr.value == "CPTOE").valueAux);
                    Company companyInvoiceCommercial = db.Company.FirstOrDefault(r => r.code == codeCompany);
                    BranchOffice branchOfficeInvoiceCommercial = db.BranchOffice.FirstOrDefault(r => r.code == codeBranchOffice && r.id_company == companyInvoiceCommercial.id);
                    EmissionPoint emissionPointInvoiceCommercial = db.EmissionPoint.FirstOrDefault(r => r.code == codeEmissionPoint);

                    string emissionDate = _invoice.Document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                    string documentNumber = string.Empty;

                    string numberString = GetDocumentSequential(documentType.id).ToString().PadLeft(9, '0');
                    documentNumber = $"{branchOfficeInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{emissionPointInvoiceCommercial.code.ToString().PadLeft(3, '0')}-{numberString}";

                    #region InvoiceCommercial Head

                    Document _document = new Document();

                    _document.id = 0;

                    _document.id_documentType = documentType.id;
                    _document.id_documentState = documentState.id;
                    _document.id_emissionPoint = emissionPointInvoiceCommercial.id;
                    _document.emissionDate = _invoice.Document.emissionDate;
                    _document.sequential = GetDocumentSequential(documentType.id);
                    _document.number = documentNumber;
                    _document.description = _invoice.Document.description;
                    _document.reference = _invoice.Document.reference;
                    _document.id_userCreate = ActiveUser.id;
                    _document.dateCreate = DateTime.Now;
                    _document.id_userUpdate = ActiveUser.id;
                    _document.dateUpdate = DateTime.Now;
                    _document.accessKey = null;

                    InvoiceCommercial NewInvoiceCommercial = new InvoiceCommercial();

                    NewInvoiceCommercial.Document = _document;

                    NewInvoiceCommercial.id_documentInvoice = _invoice.id;
                    NewInvoiceCommercial.id_ForeignCustomer = _invoice.id_buyer;
                    NewInvoiceCommercial.id_Consignee = _invoice.InvoiceExterior.id_consignee;
                    NewInvoiceCommercial.id_Notifier = _invoice.InvoiceExterior.id_notifier;
                    NewInvoiceCommercial.id_InvoiceType = invoiceType.id;
                    NewInvoiceCommercial.id_metricUnitInvoice = (int)_invoice.InvoiceExterior.id_metricUnitInvoice;
                    NewInvoiceCommercial.purchaseOrder = _invoice.InvoiceExterior.purchaseOrder;
                    NewInvoiceCommercial.id_termsNegotiation = (int)_invoice.InvoiceExterior.id_termsNegotiation;
                    NewInvoiceCommercial.id_PaymentMethod = (int)_invoice.InvoiceExterior.id_PaymentMethod;
                    NewInvoiceCommercial.id_PaymentTerm = (int)_invoice.InvoiceExterior.id_PaymentTerm;
                    NewInvoiceCommercial.dateShipment = (DateTime)_invoice.InvoiceExterior.dateShipment;
                    NewInvoiceCommercial.id_shippingAgency = (int)_invoice.InvoiceExterior.id_shippingAgency;
                    NewInvoiceCommercial.id_shippingLine = (int)_invoice.InvoiceExterior.id_ShippingLine;
                    NewInvoiceCommercial.id_portShipment = (int)_invoice.InvoiceExterior.id_portShipment;
                    NewInvoiceCommercial.id_portDischarge = (int)_invoice.InvoiceExterior.id_portDischarge;
                    NewInvoiceCommercial.id_portDestination = (int)_invoice.InvoiceExterior.id_portDestination;
                    NewInvoiceCommercial.shipName = _invoice.InvoiceExterior.shipName;
                    NewInvoiceCommercial.daeNumber = _invoice.InvoiceExterior.daeNumber;
                    NewInvoiceCommercial.BLNumber = _invoice.InvoiceExterior.BLNumber;
                    NewInvoiceCommercial.numeroContenedores = (int)_invoice.InvoiceExterior.numeroContenedores;
                    NewInvoiceCommercial.id_capacityContainer = (int)_invoice.InvoiceExterior.id_capacityContainer;
                    NewInvoiceCommercial.totalBoxesOrigen = (int)_invoice.InvoiceExterior.totalBoxes;
                    NewInvoiceCommercial.totalWeightOrigen = (decimal)_invoice.InvoiceDetail.Sum(r => r.id_amountInvoice);
                    NewInvoiceCommercial.totalValueOrigen = _invoice.InvoiceExterior.valuetotalCIF;

                    NewInvoiceCommercial.totalBoxes = (int)_invoice.InvoiceExterior.totalBoxes;
                    NewInvoiceCommercial.totalWeight = (decimal)_invoice.InvoiceDetail.Sum(r => r.id_amountInvoice);
                    NewInvoiceCommercial.totalValue = _invoice.InvoiceExterior.valuetotalCIF;

                    NewInvoiceCommercial.valueInternationalFreight = _invoice.InvoiceExterior.valueInternationalFreight;
                    NewInvoiceCommercial.valueInternationalInsurance = _invoice.InvoiceExterior.valueInternationalInsurance;
                    NewInvoiceCommercial.valueCustomsExpenditures = _invoice.InvoiceExterior.valueCustomsExpenditures;
                    NewInvoiceCommercial.valueTransportationExpenses = _invoice.InvoiceExterior.valueTransportationExpenses;

                    #endregion InvoiceCommercial Head

                    #region PreProrrateo

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

                        foreach (InvoiceDetail detail in _invoice.InvoiceDetail)
                        {
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

                            foreach (InvoiceDetail detail in _invoice.InvoiceDetail)
                            {
                                InvoiceCommercialDetail invoiceCommercial = new InvoiceCommercialDetail
                                {
                                    // si existe descuento cammbiar el precio unitario
                                    id_invoiceCommercial = detail.Invoice.id,
                                    id_itemOrigen = detail.id_item,
                                    id_metricUnitOrigen = (int)detail.id_metricUnitInvoiceDetail,
                                    amountOrigen = (int)detail.id_amountInvoice,
                                    codePresentationOrigen = detail.codePresentation,
                                    presentationMinimumOrigen = detail.presentationMinimum,
                                    presentationMaximumOrigen = detail.presentationMaximum,
                                    numBoxesOrigen = (int)detail.numBoxes,
                                    unitPriceOrigen = detail.unitPrice,
                                    totalOrigen = detail.total,
                                    id_item = detail.id_item,
                                    id_metricUnit = detail.id_metricUnitInvoiceDetail,
                                    amount = (int)detail.id_amountInvoice,
                                    codePresentation = detail.codePresentation,
                                    presentationMinimum = detail.presentationMinimum,
                                    presentationMaximum = detail.presentationMaximum,
                                    amountInvoice = (int)detail.id_amountInvoice,
                                    numBoxes = (int)detail.numBoxes,
                                    unitPrice = detail.unitPrice,
                                    total = detail.total,
                                    isActive = true,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = ActiveUser.id,
                                    dateUpdate = DateTime.Now,
                                };

                                NewInvoiceCommercial.InvoiceCommercialDetail.Add(invoiceCommercial);
                            }

                            #endregion ForEachDetail Insert Detail
                        }
                    }

                    #endregion if condicion total

                    db.InvoiceCommercial.Add(NewInvoiceCommercial);

                    documentType.currentNumber = documentType.currentNumber + 1;
                    db.DocumentType.Attach(documentType);
                    db.Entry(documentType).State = EntityState.Modified;

                    db.SaveChanges();
                    trans.Commit();
                }
                catch (DbEntityValidationException e)
                {
                    string msgErr = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        msgErr += "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + " " + eve.Entry.State;

                        foreach (var ve in eve.ValidationErrors)
                        {
                            msgErr += "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + " " + ve.ErrorMessage;
                        }
                    }
                    throw;
                }
                catch
                {
                    trans.Rollback();
                }
                finally
                {
                    TempData.Keep("invoiceExterior");
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorPartialAddNew(bool partialApprove, bool approve, Invoice invoice, InvoiceExterior invoiceExterior, Document document,
            string jsonPaymentTermDetails)
        {
            //Person newBuyerData = null;
            Invoice _invoice = ObtainInvoice(invoice.id);
            Document Newdocument = new Document();
            if (invoice == null || invoiceExterior == null)
            {
                TempData.Keep("invoiceExterior");
                // return con error indicando que faltan datos
                ViewData["EditError"] = "No existen datos de Factura Fiscal.";
                return PartialView("_InvoiceExteriorMainFormPartial", invoice);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                 try
                 {
                     #region Invoice

                        invoice.id_InvoiceType = _invoice.id_InvoiceType;
                        invoice.id_InvoiceMode = _invoice.id_InvoiceMode;

                        /*  Estandar Document */
                        document.id_userCreate = ActiveUser.id;
                        document.dateCreate = DateTime.Now;
                        document.id_userUpdate = ActiveUser.id;
                        document.dateUpdate = DateTime.Now;
                        document.PendingDocument(ActiveUser);
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "07");
                        document.DocumentType = documentType;
                        document.id_documentOrigen = _invoice.Document.id_documentOrigen;
                        document.Document2 = db.Document.FirstOrDefault(d => d.id == document.id_documentOrigen);
                        invoice.Document = document;

                        #endregion Invoice

                     #region Invoice Exterior

                        invoice.InvoiceExterior = new InvoiceExterior();
                        SalesQuotationExterior salesQuotationExterior = null;
                        if (document.Document2 != null)
                        {
                            salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(s => s.id == document.Document2.id);
                        }

                        Person consignee = null;
                        if (invoiceExterior.id_consignee != 0)
                        {
                            consignee = db.Person.FirstOrDefault(r => r.id == invoiceExterior.id_consignee);
                            if (consignee != null) invoiceExterior.Person = consignee;
                            if (salesQuotationExterior != null)
                            {
                                invoiceExterior.id_addressCustomer = salesQuotationExterior?.id_addressCustomer ?? 0;
                                invoiceExterior.ForeignCustomerIdentification = salesQuotationExterior.ForeignCustomerIdentification;
                            }
                        }

                        invoice.InvoiceExterior.updateInvoiceExterior(invoiceExterior, true, consignee, ActiveUser);

                        #endregion Invoice Exterior

                     #region Invoice Details

                        if (_invoice?.InvoiceDetail != null)
                        {
                            if (_invoice.InvoiceDetail.FirstOrDefault(fod => fod.isActive && (fod.id_tariffHeadingDetail == null || fod.id_tariffHeadingDetail == 0)) != null)
                                throw new Exception("Existe detalle sin asignar Partida Arancelaria(Item). Configúrela e inténtelo de nuevo");

                            invoice.addBulkDetail(_invoice.InvoiceDetail.ToList(), ActiveUser);
                        }

                        #endregion Invoice Details

                     #region Campos Calculados

                        // campos calculados
                        invoice.calculateTotales();
                        invoice.calculateTotalBoxes();
                        invoice.calculateTotalesInvoiceExterior();
                        invoice.saveWeight(db);
                        invoice.GetTariffHeadingDescription();

                        #endregion Campos Calculados

                     // Preparar los plazos de pago...
                     this.PrepareInvoiceToSave(invoice, jsonPaymentTermDetails);

                     TempData["invoiceExterior"] = invoice;

                     #region Seleccion punto de Emisión

                        int id_ep = 0;
                        if (TempData["id_ep"] != null)
                        {
                            id_ep = (int)TempData["id_ep"];
                        }
                        id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                        #endregion Seleccion punto de Emisión

                     #region Document

                        var isReasignacion = false;
                        if (TempData["isReasignacion"] != null)
                        {
                            isReasignacion = (bool)TempData["isReasignacion"];
                        }

                        if (!isReasignacion)
                        {
                            invoice.Document.sequential = GetDocumentSequentialEmissionPoint(_invoice.Document.id_documentType, id_ep);
                            invoice.Document.number = GetDocumentNumberEmissionPoint(_invoice.Document.id_documentType, id_ep);
                        }
                        else
                        {
                            Document documentAsignado = db.Document.FirstOrDefault(r => r.accessKey == document.accessKey && r.DocumentState.code == "05");
                            Document documentGenerado = db.Document.FirstOrDefault(r => r.number == documentAsignado.number && r.DocumentType.code == documentType.code && r.DocumentState.code != "05");
                            if (documentAsignado != null)
                            {
                                invoice.Document.sequential = documentAsignado.sequential;
                                invoice.Document.number = documentAsignado.number;
                                if (document.emissionDate.Date == documentAsignado.emissionDate.Date)
                                    _invoice.Document.emissionDate = documentAsignado.emissionDate;
                            }
                            if (documentGenerado != null)
                            {
                                throw new Exception("Ya existen datos generados para la Factura del Exterior: " + documentAsignado.number + ", estado: " + documentGenerado.DocumentState.name + " ");
                            }
                        }

                        var aId_documentOrigen = invoice?.Document.id_documentOrigen;
                        if (db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen) != null && invoice.valuetotalCIFTruncate != invoice.valuetotalProformaTruncate)
                        {
                            throw new Exception("Valor factura es diferente a la proforma, recalcule los precios. Verifique el caso e inténtelo de nuevo.");
                        }

                        if (partialApprove)
                        {
                            var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                            if (proforma != null)
                            {
                                foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(d => d.isActive))
                                {
                                    var invoiceDetail = invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                                    if (invoiceDetail != null)
                                    {
                                        if ((proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus) < (proformaDetail.proformaUsedNumBoxes + invoiceDetail.numBoxes))
                                        {
                                            var aProformaPendingNumBoxes = (proformaDetail.numBoxes ?? 0) + (proformaDetail.proformaNumBoxesPlusMinus ?? 0) - (proformaDetail.proformaUsedNumBoxes ?? 0);
                                            if (aProformaPendingNumBoxes > 0)
                                            {
                                                throw new Exception("En el Detalle del Producto: " + proformaDetail.Item.name + ", no se puede facturar mas de: " + aProformaPendingNumBoxes + " carton(es). Verifique el caso e inténtelo de nuevo.");
                                            }
                                            else
                                            {
                                                throw new Exception("En el Detalle del Producto: " + proformaDetail.Item.name + ", no existe cartones pendiente para facturar. Verifique el caso e inténtelo de nuevo.");
                                            }
                                        }

                                        if (proformaDetail.proformaPendingDiscount < invoiceDetail.discount)
                                            if (proformaDetail.proformaPendingDiscount == 0)
                                            {
                                                throw new Exception("Del Item: " + proformaDetail.Item.name +
                                                                " no quedan Descuento pendiente en la Proforma. Debe verificar el caso.");
                                            }
                                            else
                                            {
                                                throw new Exception("Descuento del Item: " + proformaDetail.Item.name +
                                                                " debe ser menor e igual a: " + (proformaDetail.proformaPendingDiscount).ToString("#0.00") +
                                                                " que es el descuento pendiente en la Proforma.");
                                            }
                                    }
                                }
                            }

                            invoice.Document.PartialApprove(ActiveUser);
                        }
                        else
                        {
                            if (approve)
                            {
                                invoice.Document.Approve(ActiveUser);

                                if (invoice.Document.id_documentOrigen != null)
                                {
                                    var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
                                    if (proforma != null)
                                    {
                                        foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(d => d.isActive))
                                        {
                                            var invoiceDetail = _invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                                            if (invoiceDetail != null)
                                            {
                                                if (proformaDetail.proformaPendingNumBoxes < invoiceDetail.numBoxes)
                                                    if (proformaDetail.proformaPendingNumBoxes.Value == 0)
                                                    {
                                                        throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                                        " no quedan pendiente en la Proforma. Debe eliminar este detalle.");
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                                        " debe ser menor e igual a: " + proformaDetail.proformaPendingNumBoxes.Value.ToString("#") +
                                                                        " que son los cartones pendiente en la Proforma.");
                                                    }

                                                if (proformaDetail.proformaPendingDiscount < invoiceDetail.discount)
                                                    if (proformaDetail.proformaPendingDiscount == 0)
                                                    {
                                                        throw new Exception("Del Item: " + proformaDetail.Item.name +
                                                                        " no quedan Descuento pendiente en la Proforma. Debe verificar el caso.");
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Descuento del Item: " + proformaDetail.Item.name +
                                                                        " debe ser menor e igual a: " + (proformaDetail.proformaPendingDiscount).ToString("#0.00") +
                                                                        " que es el descuento pendiente en la Proforma.");
                                                    }
                                            }
                                        }
                                        ServiceSalesQuotationExterior.UpdateValuesFromInvoiceExterior(invoice, db);
                                    }
                                }
                            }
                            else
                            {
                                invoice.Document.PendingDocument(ActiveUser);
                            }
                        }

                        // TODO  DocumentTypeExtendedDefinition
                        // Definicion de Dinamica de Jerarquia y Propiedades
                        String LabelAmbiente = ConfigurationManager.AppSettings["PruebasInvoiceExterior"];
                        String CodeEnviromentType = "1";
                        if (LabelAmbiente == "NO")
                        {
                            CodeEnviromentType = "2";
                        }

                        EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);
                        invoice.Document.EmissionPoint = emissionPoint;
                        invoice.Document.id_emissionPoint = emissionPoint.id;

                        string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

                        invoice.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                                                        _invoice.Document.DocumentType.codeSRI,
                                                                        emissionPoint.BranchOffice.Division.Company.ruc,
                                                                        CodeEnviromentType,
                                                                        emissionPoint.BranchOffice.code.ToString().PadLeft(3, '0') + emissionPoint.code.ToString().PadLeft(3, '0'),
                                                                        document.sequential.ToString("D9"),
                                                                        document.sequential.ToString("D8"),
                                                                        "1");

                        //Actualiza Secuencial
                        if (documentType != null && !isReasignacion)
                        {
                            documentType.currentNumber = documentType.currentNumber + 1;
                            invoice.Document.EmissionPoint.secuenciaValor = invoice.Document.EmissionPoint.secuenciaValor + 1;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        #endregion Document

                     #region Actualizamos los datos de la factura comercial

                        var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                        if (puedeModDatosProforma)
                        {
                            UpdateSalesQuotationExterior(invoice.id, document, invoice, invoiceExterior);
                            UpdateInvoicesCommercial(invoice.id, document.id_documentOrigen, invoice, invoiceExterior);
                        }

                        #endregion Actualizamos los datos de la factura comercial

                     invoice.ValidateInfo();

                     db.Invoice.Add(invoice);
                     db.SaveChanges();
                     trans.Commit();

                     /* Add Document State */
                     if (invoice?.Document?.id_documentState != null && invoice?.Document?.DocumentState == null)
                     {
                         DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.id == invoice.Document.id_documentState);
                         invoice.Document.DocumentState = documentState;
                     }

                     TempData["invoiceExterior"] = invoice;

                     ViewData["EditMessage"] = SuccessMessage("Factura del Exterior: " + invoice.Document.number + " guardada exitosamente");
                 }
                 catch (DbEntityValidationException e)
                 {
                     string msgErr = "";
                     foreach (var eve in e.EntityValidationErrors)
                     {
                         msgErr += "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + " " + eve.Entry.State;

                         foreach (var ve in eve.ValidationErrors)
                         {
                             msgErr += "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + " " + ve.ErrorMessage;
                         }
                     }
                     invoice = (Invoice)TempData["invoiceExterior"];
                     invoice.Document.accessKey = null;
                     invoice.Document.number = null;
                     invoice.Document.sequential = 0;
                     invoice.Document.PendingDocument(ActiveUser);
                     ViewData["EditError"] = ErrorMessage(msgErr + e.Message);
                 }
                 catch (Exception e)
                 {
                     
                     string estatusDesc = (partialApprove) ? "Aprobar Parcialmente " : ((approve) ? "Aprobar " : null);
                     string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;
                     
                     if (e.Data["source"] != null)
                     {
                         if ((string)e.Data["source"] == "modelDocumentValidation" && !string.IsNullOrEmpty(estatusDesc)) msgErr += "Para " + estatusDesc + " el presente documento los siguientes campos son requeridos:<br>" + Environment.NewLine;
                     }
                     // "source", "modelDocumentValidation"
                     ViewData["EditError"] = ErrorMessage(msgErr + e.Message);
                     trans.Rollback();
                     invoice = (Invoice)TempData["invoiceExterior"];
                     Person consignee = null;
                     if (invoiceExterior.id_consignee != 0)
                     {
                         consignee = db.Person.FirstOrDefault(r => r.id == invoiceExterior.id_consignee);
                         if (consignee != null) invoiceExterior.Person = consignee;
                     }
                     invoice.InvoiceExterior.updateInvoiceExterior(invoiceExterior, true, consignee, ActiveUser);
                     invoice.Document.accessKey = null;
                     invoice.Document.number = null;
                     invoice.Document.sequential = 0;
                     invoice.Document.PendingDocument(ActiveUser);
                     invoice.Document.DocumentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                 }
                 finally
                 {
                     TempData.Keep("invoiceExterior");
                 }
             }
             
            

            ViewBag.id_documentState = invoice.Document.id_documentState;

            #region Preparamos el tipo de documento de origen

            var documentoOrigen = db.Document
               .FirstOrDefault(e => e.id == invoice.Document.id_documentOrigen && e.DocumentState.code != "05");
            if (documentoOrigen != null)
            {
                if (documentoOrigen.DocumentType.code == "70") // Factura Comercial
                {
                    this.ViewBag.EtiquetaExterna = "Factura Comercial";
                }
                else if (documentoOrigen.DocumentType.code == "131") // Proforma
                {
                    this.ViewBag.EtiquetaExterna = "Proforma";
                }
            }
            else
            {
                this.ViewBag.EtiquetaExterna = "";
            }

            #endregion Preparamos el tipo de documento de origen

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorPartialUpdate(bool partialApprove, bool approve, Invoice invoice, InvoiceExterior invoiceExterior, Document document,
            string jsonPaymentTermDetails)
        {
            Person newConsigneeData = null;
            Invoice editInvoice = null;
            Invoice _invoice = ObtainInvoice(invoice.id);
            int id_documentState = _invoice.Document.id_documentState;
            document.id_documentOrigen = _invoice.Document.id_documentOrigen;
            Boolean changeEmissionPoint = false;
            string valInvFact = DataProviderSetting.ValueSetting("INVFACT");

            if (invoice == null || invoiceExterior == null)
            {
                // return con error indicando que faltan datos
            }
            try
            {
                #region -- Validacion Vinculacion Movimiento Inventario --
                int? id_inventoryMove = validateIfHasValidate(invoice.id);
                if (id_inventoryMove.HasValue)
                {
                    string messageValidate = validateIfInvoiceInventoryMove(id_inventoryMove.Value, _invoice.InvoiceDetail.ToArray());

                    if (!string.IsNullOrEmpty(messageValidate))
                    {
                        throw new Exception(messageValidate);
                    }
                }
                #endregion

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //if (valInvFact == "SI" && !partialApprove && !approve)
                        //{
                        //    //Verifico si existe egreso Relacionado
                        //    var lstInvoiceInventory = db.InventoryMove.FirstOrDefault(w => w.id_Invoice != null && w.Document.DocumentState.code != "05" && w.id_Invoice == document.id);
                        //    if (lstInvoiceInventory != null)
                        //    {
                        //        throw new Exception("Existe movimiento de inventario: " + lstInvoiceInventory.natureSequential + "  relacionado a la Factura, deberá primero anularse el movimiento.");
                        //    }
                        //}
                        #region Obtener Punto de Emision

                        int _id_PuntoEmision = _invoice?.Document?.id_emissionPoint ?? 0;
                        if (_id_PuntoEmision == 0)
                        {
                            changeEmissionPoint = true;
                        }

                        #endregion Obtener Punto de Emision

                        // TODO: Al cambiar el punto de emision todos los documentos se deben filtrar

                        #region Validacion Punto de Emision

                        //Obtener punto de emision de Environment
                        int id_ep = 0;
                        if (changeEmissionPoint)
                        {
                            if (TempData["id_ep"] != null)
                            {
                                id_ep = (int)TempData["id_ep"];
                            }
                            id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);
                        }
                        else
                        {
                            id_ep = _id_PuntoEmision;
                        }

                        #endregion Validacion Punto de Emision

                        #region Cambio de Componentes Clave de Acceso

                        String LabelAmbiente = ConfigurationManager.AppSettings["Pruebas"];
                        String CodeEnviromentType = "1";
                        if (LabelAmbiente == "NO")
                        {
                            CodeEnviromentType = "2";
                        }

                        EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);
                        //string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

                        editInvoice = db.Invoice.FirstOrDefault(r => r.id == invoice.id);
                        //editInvoice.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                        //                                                editInvoice.Document.DocumentType.codeSRI,
                        //                                                emissionPoint.BranchOffice.Division.Company.ruc,
                        //                                                CodeEnviromentType,
                        //                                                emissionPoint.BranchOffice.code.ToString().PadLeft(3, '0') + emissionPoint.code.ToString().PadLeft(3, '0'),
                        //                                                editInvoice.Document.sequential.ToString("D9"),
                        //                                                editInvoice.Document.sequential.ToString("D8"),
                        //                                                "1");

                        #endregion Cambio de Componentes Clave de Acceso

                        editInvoice.Document.description = document.description;
                        //editInvoice.Document.emissionDate = document.emissionDate;
                        editInvoice.Document.id_userUpdate = ActiveUser.id;
                        editInvoice.Document.dateUpdate = DateTime.Now;

                        if (editInvoice != null)
                        {
                            #region invoice

                            if (editInvoice.InvoiceExterior.id_consignee != invoiceExterior.id_consignee)
                            {
                                newConsigneeData = db.Person.FirstOrDefault(r => r.id == invoiceExterior.id_consignee);
                                editInvoice.InvoiceExterior.Person = newConsigneeData;
                            }
                            if (document.id_documentOrigen != null)
                            {
                                invoiceExterior.id_addressCustomer = _invoice.InvoiceExterior.id_addressCustomer;
                                invoiceExterior.ForeignCustomerIdentification = _invoice.InvoiceExterior.ForeignCustomerIdentification;
                            }
                            editInvoice.id_buyer = invoice.id_buyer;

                            #endregion invoice

                            #region invoice Exterior

                            var _proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == _invoice.Document.id_documentOrigen);

                            if (invoiceExterior.id_termsNegotiation != 0 && _proforma != null)
                            {
                                invoiceExterior.id_termsNegotiation = _proforma.id_termsNegotiation;
                            }

                            editInvoice.InvoiceExterior.updateInvoiceExterior(invoiceExterior, false, newConsigneeData, ActiveUser);

                            #endregion invoice Exterior
                            var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";

                            #region Invoice Detail

                            if (_invoice?.InvoiceDetail != null)
                            {
                                if (_invoice.InvoiceDetail.FirstOrDefault(fod => fod.isActive && (fod.id_tariffHeadingDetail == null || fod.id_tariffHeadingDetail == 0)) != null)
                                    throw new Exception("Existe detalle sin asignar Partida Arancelaria(Item). Configúrela e inténtelo de nuevo");

                                editInvoice.addBulkDetail(_invoice.InvoiceDetail.ToList(), ActiveUser);
                            }

                            #endregion Invoice Detail

                            TempData["invoiceExterior"] = editInvoice;

                            #region Campos Calculados

                            foreach (InvoiceExteriorWeight _invoiceExteriorWeight in editInvoice.InvoiceExteriorWeight.Where(r => (Boolean)r.isActive).ToList())
                            {
                                InvoiceExteriorWeight _forDeleteInvoiceExteriorWeight = db.InvoiceExteriorWeight
                                                                                                .FirstOrDefault(r => r.id == _invoiceExteriorWeight.id);
                                db.InvoiceExteriorWeight.Attach(_forDeleteInvoiceExteriorWeight);
                                db.Entry(_forDeleteInvoiceExteriorWeight).State = EntityState.Deleted;

                                editInvoice.InvoiceExteriorWeight.Remove(_invoiceExteriorWeight);
                            }

                            // campos calculados
                            editInvoice.calculateTotales();
                            editInvoice.calculateTotalBoxes();
                            editInvoice.calculateTotalesInvoiceExterior();
                            editInvoice.saveWeight(db);
                            editInvoice.GetTariffHeadingDescription();

                            #endregion Campos Calculados

                            // Preparar los plazos de pago...
                            this.PrepareInvoiceToSave(editInvoice, jsonPaymentTermDetails);
                            var aId_documentOrigen = editInvoice?.Document.id_documentOrigen;
                            if (db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen) != null && editInvoice.valuetotalCIFTruncate != editInvoice.valuetotalProformaTruncate)
                            {
                                throw new Exception("Valor factura es diferente a la proforma, recalcule los precios. Verifique el caso e inténtelo de nuevo.");
                            }
                            // TODO: condicionamiento actualizacion
                            TempData["invoiceExterior"] = editInvoice;

                            #region Document

                            if (partialApprove)
                            {
                                var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                                if (proforma != null)
                                {
                                    foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(d => d.isActive))
                                    {
                                        var invoiceDetail = editInvoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                                        if (invoiceDetail != null)
                                        {
                                            if ((proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus) < (proformaDetail.proformaUsedNumBoxes + invoiceDetail.numBoxes))
                                            {
                                                var aProformaPendingNumBoxes = (proformaDetail.numBoxes ?? 0) + (proformaDetail.proformaNumBoxesPlusMinus ?? 0) - (proformaDetail.proformaUsedNumBoxes ?? 0);
                                                if (aProformaPendingNumBoxes > 0)
                                                {
                                                    throw new Exception("En el Detalle del Producto: " + proformaDetail.Item.name + ", no se puede facturar mas de: " + aProformaPendingNumBoxes + " carton(es). Verifique el caso e inténtelo de nuevo.");
                                                }
                                                else
                                                {
                                                    throw new Exception("En el Detalle del Producto: " + proformaDetail.Item.name + ", no existe cartones pendiente para facturar. Verifique el caso e inténtelo de nuevo.");
                                                }
                                            }

                                            if (proformaDetail.proformaPendingDiscount < invoiceDetail.discount)
                                                if (proformaDetail.proformaPendingDiscount == 0)
                                                {
                                                    throw new Exception("Del Item: " + proformaDetail.Item.name +
                                                                    " no quedan Descuento pendiente en la Proforma. Debe verificar el caso.");
                                                }
                                                else
                                                {
                                                    throw new Exception("Descuento del Item: " + proformaDetail.Item.name +
                                                                    " debe ser menor e igual a: " + (proformaDetail.proformaPendingDiscount).ToString("#0.00") +
                                                                    " que es el descuento pendiente en la Proforma.");
                                                }
                                        }
                                    }
                                }

                                editInvoice.Document.PartialApprove(ActiveUser);
                            }
                            else
                            {
                                #region Validación Permiso

                                if (approve)
                                {
                                    int id_user = (int)ViewData["id_user"];
                                    int id_menu = (int)ViewData["id_menu"];

                                    User user = DataProviderUser.UserById(id_user);
                                    UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                                    if (userMenu != null)
                                    {
                                        Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Aprobar");
                                        if (permission == null)
                                        {
                                            throw new Exception("No tiene Permiso para Aprobar la Factura");
                                        }
                                    }
                                }

                                #endregion Validación Permiso

                                if (approve)
                                {
                                    if (editInvoice.Document.id_documentOrigen != null) //&& editInvoice.Document.DocumentState.code != "02")//02: Partial Approve
                                    {
                                        var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == editInvoice.Document.id_documentOrigen);
                                        if (proforma != null)
                                        {
                                            foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(d => d.isActive))
                                            {
                                                var invoiceDetail = _invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                                                if (invoiceDetail != null)
                                                {
                                                    if (proformaDetail.proformaPendingNumBoxes < invoiceDetail.numBoxes)
                                                        if (proformaDetail.proformaPendingNumBoxes.Value == 0)
                                                        {
                                                            throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                                            " no quedan pendiente en la Proforma. Debe eliminar este detalle.");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception("Cartones del Item: " + proformaDetail.Item.name +
                                                                            " debe ser menor e igual a: " + proformaDetail.proformaPendingNumBoxes.Value.ToString("#") +
                                                                            " que son los cartones pendiente en la Proforma.");
                                                        }

                                                    if (proformaDetail.proformaPendingDiscount < invoiceDetail.discount)
                                                        if (proformaDetail.proformaPendingDiscount == 0)
                                                        {
                                                            throw new Exception("Del Item: " + proformaDetail.Item.name +
                                                                            " no quedan Descuento pendiente en la Proforma. Debe verificar el caso.");
                                                        }
                                                        else
                                                        {
                                                            throw new Exception("Descuento del Item: " + proformaDetail.Item.name +
                                                                            " debe ser menor e igual a: " + (proformaDetail.proformaPendingDiscount).ToString("#0.00") +
                                                                            " que es el descuento pendiente en la Proforma.");
                                                        }
                                                }
                                            }
                                        }

                                        //TODO validar algun valor necesario
                                        #region Verificamos si existe alguna factura comercial relacionada a la factura fiscal

                                        var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
                                        var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;
                                        var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;

                                        var facturasOrigenDoc = db.Document
                                            .FirstOrDefault(e => e.id == editInvoice.Document.id_documentOrigen
                                                && e.id_documentState != idEstadoAnulado);

                                        SalesQuotationExterior salesQuotationExterior = null;
                                        if ((facturasOrigenDoc != null) && (facturasOrigenDoc?.DocumentType?.code == "70")) // Factura Comercial
                                        {
                                            salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(r => r.id == facturasOrigenDoc.id_documentOrigen);
                                        }
                                        else if ((facturasOrigenDoc != null) && (facturasOrigenDoc?.DocumentType?.code == "131")) //Proforma
                                        {
                                            salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(r => r.id == editInvoice.Document.id_documentOrigen);
                                        }
                                        #endregion Verificamos si existe alguna factura comercial relacionada a la factura fiscal

                                        if (editInvoice.InvoiceExterior.totalBoxes > salesQuotationExterior.pendingBoxes)
                                            throw new Exception("La cantidad de cartones a facturar debe de ser menor o igual al saldo pendiente de la proforma.");

                                        //if (editInvoice.Document.id_documentOrigen != null && editInvoice.Document.DocumentState.code != "02")//02: Partial Approve
                                        ServiceSalesQuotationExterior.UpdateValuesFromInvoiceExterior(editInvoice, db);
                                    }
                                    editInvoice.Document.Approve(ActiveUser);
                                }
                                if (valInvFact == "SI")
                                {
                                    //Verifico si existe egreso Relacionado
                                    var lstInvoiceInventory = db.InventoryMove.FirstOrDefault(w => w.id_Invoice != null && w.Document.DocumentState.code != "05" && w.id_Invoice == document.id);
                                    if (lstInvoiceInventory == null)
                                    {
                                        throw new Exception("La factura debe de tener un movimiento de inventario relacionado.");
                                    }
                                }
                            }

                            #endregion Document

                            editInvoice.ValidateInfo();

                            #region Actualizamos los datos de la factura comercial


                            if (puedeModDatosProforma)
                            {
                                UpdateSalesQuotationExterior(editInvoice.id, editInvoice.Document, editInvoice, editInvoice.InvoiceExterior);
                                UpdateInvoicesCommercial(editInvoice.id, document.id_documentOrigen, editInvoice, editInvoice.InvoiceExterior);
                            }

                            #endregion Actualizamos los datos de la factura comercial

                            db.Invoice.Attach(editInvoice);
                            db.Entry(editInvoice).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Factura del Exterior: " + editInvoice.Document.number + " actualizada exitosamente");
                            TempData["invoiceExterior"] = editInvoice;
                        }
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        throw;
                    }

                }
            }
            catch (Exception e)
            {
                string estatusDesc = (partialApprove) ? "Aprobar Parcialmente " : ((approve) ? "Aprobar " : null);
                string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;

                if (e.Data["source"] != null)
                {
                    if ((string)e.Data["source"] == "modelDocumentValidation" && !string.IsNullOrEmpty(estatusDesc)) msgErr += "Para " + estatusDesc + " el presente documento los siguientes campos son requeridos: <br>" + Environment.NewLine;
                }
                // "source", "modelDocumentValidation"
                ViewData["EditError"] = ErrorMessage(msgErr + e.Message);
                editInvoice = (Invoice)TempData["invoiceExterior"];
                editInvoice.InvoiceExterior.numberRemissionGuide = invoiceExterior.numberRemissionGuide;
                editInvoice.InvoiceExterior.PO = invoiceExterior.PO;
                editInvoice.InvoiceExterior.noContrato = invoiceExterior.noContrato;
                editInvoice.InvoiceExterior.BLNumber = invoiceExterior.BLNumber;
                editInvoice.InvoiceExterior.daeNumber = invoiceExterior.daeNumber;
                editInvoice.InvoiceExterior.daeNumber2 = invoiceExterior.daeNumber2;
                editInvoice.InvoiceExterior.daeNumber3 = invoiceExterior.daeNumber3;
                editInvoice.InvoiceExterior.daeNumber4 = invoiceExterior.daeNumber4;
                editInvoice.InvoiceExterior.containers = invoiceExterior.containers;
                editInvoice.InvoiceExterior.tariffHeadingDescription = invoiceExterior.tariffHeadingDescription;


                editInvoice.Document.id_documentState = id_documentState;
                editInvoice.Document.DocumentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState);
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            
            ViewBag.id_documentState = editInvoice.Document.id_documentState;

            #region Preparamos el tipo de documento de origen

            var documentoOrigen = db.Document
               .FirstOrDefault(e => e.id == editInvoice.Document.id_documentOrigen && e.DocumentState.code != "05");
            if (documentoOrigen != null)
            {
                if (documentoOrigen.DocumentType.code == "70") // Factura Comercial
                {
                    this.ViewBag.EtiquetaExterna = "Factura Comercial";
                }
                else if (documentoOrigen.DocumentType.code == "131") // Proforma
                {
                    this.ViewBag.EtiquetaExterna = "Proforma";
                }
            }
            else
            {
                this.ViewBag.EtiquetaExterna = "";
            }

            #endregion Preparamos el tipo de documento de origen

            return PartialView("_InvoiceExteriorMainFormPartial", editInvoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorPartialDelete(int id)
        {
            var model = db.Invoice.Where(r => r.Document.DocumentState.code != "05");

            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var deleteInvoice = db.Invoice.FirstOrDefault(r => r.id == id);
                        if (deleteInvoice != null)
                        {
                            deleteInvoice.Document.RemoveDocument(ActiveUser);
                            db.Invoice.Attach(deleteInvoice);
                            db.Entry(deleteInvoice).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Factura del Exterior: " + deleteInvoice.Document.number + " anulada.");
                        }
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }

            return PartialView("_InvoiceExteriorMainFormPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            XmlDocument xmlFEX = new XmlDocument();

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Autorizada

                    if (invoice != null && documentState != null)
                    {
                        #region Regeneramos la clave de acceso

                        #region Seleccion punto de Emisión
                        int id_ep = 0;
                        if (TempData["id_ep"] != null)
                        {
                            id_ep = (int)TempData["id_ep"];
                        }
                        id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                        #endregion Seleccion punto de Emisión

                        EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);
                        invoice.Document.EmissionPoint = emissionPoint;
                        invoice.Document.id_emissionPoint = emissionPoint.id;
                        string emissionDate = invoice.Document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

                        String LabelAmbiente = ConfigurationManager.AppSettings["PruebasInvoiceExterior"];
                        String CodeEnviromentType = "1";
                        if (LabelAmbiente == "NO")
                        {
                            CodeEnviromentType = "2";
                        }

                        var accessKey = AccessKey.GenerateAccessKey(emissionDate,
                            invoice.Document.DocumentType.codeSRI,
                            emissionPoint.BranchOffice.Division.Company.ruc,
                            CodeEnviromentType,
                            emissionPoint.BranchOffice.code.ToString().PadLeft(3, '0') + emissionPoint.code.ToString().PadLeft(3, '0'),
                            invoice.Document.sequential.ToString("D9"),
                            invoice.Document.sequential.ToString("D8"),
                            "1");
                        #endregion

                        invoice.Document.accessKey = accessKey;
                        invoice.Document.authorizationDate = DateTime.Now;
                        invoice.ValidateStateChange("09");
                        invoice.Document.PreAutorize(ActiveUser);
                        invoice.ValidateInfo();

                        // Generamos el XML
                        xmlFEX = invoice.GenerateXML((int)ViewData["id_company"]);

                        string routePath = ConfigurationManager.AppSettings["rutaXmlFEX"];
                        string routePathA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];

                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceExterior"] = invoice;
                        ViewData["EditMessage"] = SuccessMessage("Factura : " + invoice.Document.number + " en proceso de ser autorizada");

                        try
                        {
                            //User token that represents the authorized user account
                            IntPtr token = IntPtr.Zero;

                            string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                            USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                            string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                            PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                            string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                            DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                            bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    if (!string.IsNullOrEmpty(routePath))
                                    {
                                        if (!(Directory.Exists(routePath)))
                                        {
                                            Directory.CreateDirectory(routePath);
                                            LogController.WriteLog(routePath + ": Creado Exitosamente");
                                        }
                                        if (Directory.Exists(routePath))
                                        {
                                            string pathFileXmlFileName = routePath + "\\" + invoice.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de XML de FEX.");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }

                            //User token that represents the authorized user account
                            //IntPtr token = IntPtr.Zero;
                            string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                            USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                            string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                            PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                            string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                            DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                            result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    if (!string.IsNullOrEmpty(routePathA1Firmar))
                                    {
                                        if (!(Directory.Exists(routePathA1Firmar)))
                                        {
                                            Directory.CreateDirectory(routePathA1Firmar);
                                            LogController.WriteLog(routePathA1Firmar + ": Creado Exitosamente");
                                        }
                                        if (Directory.Exists(routePathA1Firmar))
                                        {
                                            string pathFileXmlFileName = routePathA1Firmar + "\\" + invoice.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                            if (!string.IsNullOrEmpty(routePath))
                                            {
                                                if (Directory.Exists(routePath))
                                                {
                                                    string pathFileXmlFileName2 = routePath + "\\" + invoice.Document.accessKey + ".xml";
                                                    System.IO.File.Delete(pathFileXmlFileName2);
                                                    LogController.WriteLog(pathFileXmlFileName2 + ": Eliminado Exitosamente");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.WriteLog(ex.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;

                    if (e.Data["source"] != null)
                    {
                        if ((string)e.Data["source"] == "modelDocumentValidation") msgErr += "Para Autorizar el presente documento los siguientes campos son requeridos: <br>" + Environment.NewLine;
                    }
                    // "source", "modelDocumentValidation"
                    ViewData["EditError"] = ErrorMessage(msgErr + e.Message);

                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
                finally
                {
                    TempData.Keep("invoiceExterior");
                }
            }

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        //Impersonation functionality
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //Disconnection after file operations
        [DllImport("kernel32.dll")]
        private static extern Boolean CloseHandle(IntPtr hObject);

        [HttpPost]
        public ActionResult CheckAutorizeRSI(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            string msgXtraInfo = "";
            //string currentInvoiceNumber = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            List<Invoice> _invoiceList = new List<Invoice>();

            msgXtraInfo = "Obtener Ruta XML Factura Fiscal(B1.AutorizadoActualizado)";
            string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];
            if (string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
            {
                throw new Exception("No definida Configuración Ruta B1.AutorizadoActualizado.");
            }
            msgXtraInfo = "Obtener Factura";
            DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Estado de PRE-AUTORIZADA
            if (invoice == null) throw new Exception("Factura:" + id + " ,no se ha encontrado.");

            LogController.tratarFicheroLog();
            bool change = false;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //User token that represents the authorized user account
                    IntPtr token = IntPtr.Zero;
                    string USER_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["USER_rutaXmlB1AutorizadoActualizado"];
                    USER_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(USER_rutaXmlB1AutorizadoActualizado) ? "admin" : USER_rutaXmlB1AutorizadoActualizado;
                    string PASS_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["PASS_rutaXmlB1AutorizadoActualizado"];
                    PASS_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(PASS_rutaXmlB1AutorizadoActualizado) ? "admin" : PASS_rutaXmlB1AutorizadoActualizado;
                    string DOMAIN_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["DOMAIN_rutaXmlB1AutorizadoActualizado"];
                    DOMAIN_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(DOMAIN_rutaXmlB1AutorizadoActualizado) ? "" : DOMAIN_rutaXmlB1AutorizadoActualizado;

                    bool result = LogonUser(USER_rutaXmlB1AutorizadoActualizado, DOMAIN_rutaXmlB1AutorizadoActualizado, PASS_rutaXmlB1AutorizadoActualizado, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                    if (result == true)
                    {
                        //Use token to setup a WindowsImpersonationContext
                        using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                        {
                            String[] dirs = System.IO.Directory.GetDirectories(routePathB1AutorizadoActualizado);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada
                            ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03"); //Autorizada
                            var yearMothCurrent = invoice.Document.accessKey.Substring(4, 4).ToString() +
                                                    invoice.Document.accessKey.Substring(2, 2).ToString();
                            foreach (String dir in dirs)
                            {
                                var directoryNameAux = dir.Substring(dir.Length - 8, 6);
                                if (directoryNameAux == yearMothCurrent)
                                {
                                    String[] files = System.IO.Directory.GetFileSystemEntries(dir);
                                    foreach (String file in files)
                                    {
                                        string[] words = Path.GetFileNameWithoutExtension(file).Split('_');
                                        string accessKeyAux = words[0];
                                        var aDocument = invoice.Document;
                                        if (accessKeyAux == aDocument.accessKey)
                                        {
                                            aDocument.dateUpdate = DateTime.Now;
                                            aDocument.id_userUpdate = ActiveUser.id;
                                            aDocument.id_documentState = documentState.id;
                                            aDocument.DocumentState = documentState;
                                            aDocument.authorizationNumber = aDocument.accessKey;

                                            db.Document.Attach(aDocument);
                                            db.Entry(aDocument).State = EntityState.Modified;

                                            var aElectronicDocument = db.ElectronicDocument.FirstOrDefault(fod => fod.Document.accessKey == accessKeyAux);
                                            if (aElectronicDocument != null)
                                            {
                                                aElectronicDocument.id_electronicDocumentState = electronicDocumentState.id;
                                                aElectronicDocument.ElectronicDocumentState = electronicDocumentState;

                                                db.ElectronicDocument.Attach(aElectronicDocument);
                                                db.Entry(aElectronicDocument).State = EntityState.Modified;
                                            }
                                            if (!change) change = true;
                                        }
                                    }
                                };
                            }

                            ctx.Undo();
                            CloseHandle(token);
                        }
                    }

                    if (change)
                    {
                        db.SaveChanges();
                        trans.Commit();
                        TempData["invoiceExterior"] = invoice;
                        ViewData["EditMessage"] = SuccessMessage("Factura : " + invoice.Document.number + " verificada y actualizada la autorización del SRI");
                        LogController.WriteLog("Actualización de Estado de la Factura del Exterior a Estado Autorizado Satisfactoriamente(Con Cambio)");
                    }
                    else
                    {
                        ViewData["EditMessage"] = WarningMessage("Factura : " + invoice.Document.number + " aun no esta autorizada por el SRI");
                        LogController.WriteLog("Actualización de Estado de las Factura del Exterior a Estado Autorizado Satisfactoriamente(Sin Cambio)");
                    }
                }
                catch (Exception e)
                {
                    if (change)
                    {
                        trans.Rollback();
                    }
                    LogController.WriteLog(e.Message);
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    LogWrite(e, null, "CheckAutorizeRSIDocument=>" + msgXtraInfo);
                }
                finally
                {
                    TempData.Keep("invoiceExterior");
                }
            }

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        [HttpPost]
        public ActionResult DesvincularFactura(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (invoice != null && documentState != null)
                    {
                        invoice.ValidateStateChange("05");
                        invoice.Document.RemoveDocument(ActiveUser);

                        string codeState = invoice.Document?.DocumentState?.code ?? "01";

                        if (invoice.Document.id_documentOrigen != null && codeState != "01")
                            ServiceSalesQuotationExterior.UpdateValuesFromInvoiceExteriorAnul(invoice, db);

                        invoice.InvoiceExterior.dismissalreason = "Reasignación de Proforma";

                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceExterior"] = invoice;
                        TempData.Keep("invoiceExterior");

                        ViewData["EditMessage"] = SuccessMessage("Factura: " + invoice.Document.number + " anulada.");
                    }
                }
                catch (Exception e)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("invoiceExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            ViewBag.id_documentState = invoice.Document.id_documentState;

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (invoice != null && documentState != null)
                    {
                        invoice.ValidateStateChange("05");
                        invoice.Document.RemoveDocument(ActiveUser);

                        string codeState = invoice.Document?.DocumentState?.code ?? "01";

                        if (invoice.Document.id_documentOrigen != null && codeState != "01")
                            ServiceSalesQuotationExterior.UpdateValuesFromInvoiceExteriorAnul(invoice, db);

                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceExterior"] = invoice;
                        TempData.Keep("invoiceExterior");

                        ViewData["EditMessage"] = SuccessMessage("Factura: " + invoice.Document.number + " anulada.");
                    }
                }
                catch (Exception e)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("invoiceExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            ViewBag.id_documentState = invoice.Document.id_documentState;

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        /* Bulk Options */

        [HttpPost, ValidateInput(false)]
        public JsonResult ApprovePartialDocuments(int[] ids)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            string currentInvoiceNumber = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            if (ids != null && ids.Count() > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "02");

                            if (invoice != null && documentState != null)
                            {
                                currentInvoiceNumber = invoice.Document.number;

                                invoice.ValidateStateChange("02");
                                invoice.Document.PartialApprove(ActiveUser);
                                invoice.ValidateInfo();

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Facturas aprobadas parcialmente con éxito.");
                    }
                    catch (Exception e)
                    {
                        string msgErr = "Se presentaron el(los) siguiente(s) errores al Aprobar Parcialmente la Factura #" + currentInvoiceNumber + "<br>";// + Environment.NewLine;

                        if (e.Data["source"] != null)
                        {
                            if ((string)e.Data["source"] == "modelDocumentValidation") msgErr += "Los siguientes campos son requeridos: <br>";// + Environment.NewLine;
                        }
                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage(msgErr + e.Message);

                        trans.Rollback();
                    }
                }
            }

            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = (parametersSeekInvoiceExterior)TempData["parametersSeekInvoiceExterior"];
            var modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ApproveDocuments(int[] ids)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            string currentInvoiceNumber = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

                            if (invoice != null && documentState != null)
                            {
                                currentInvoiceNumber = invoice.Document.number;
                                invoice.ValidateStateChange("03");
                                invoice.Document.Approve(ActiveUser);
                                invoice.ValidateInfo();

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Facturas aprobadas con éxito.");
                    }
                    catch (Exception e)
                    {
                        string msgErr = "Se presentaron el(los) siguientes(s) error(es) al Aprobar la Factura #" + currentInvoiceNumber + "<br>";

                        if (e.Data["source"] != null)
                        {
                            if ((string)e.Data["source"] == "modelDocumentValidation") msgErr += "Los siguientes campos son requeridos: <br>";
                        }

                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage(msgErr + e.Message);
                        trans.Rollback();
                    }
                }
            }

            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = (parametersSeekInvoiceExterior)TempData["parametersSeekInvoiceExterior"];
            var modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AutorizeDocuments(int[] ids)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            string msgXtraInfo = "";
            string currentInvoiceNumber = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            List<Invoice> _invoiceList = new List<Invoice>();
            string msgErr = "Se presentaron el(los) siguiente(s) error(es) al Autorizar Parcialmente ";

            try
            {
                msgXtraInfo = "Obtener Ruta XML Factura Fiscal";
                string routePath = ConfigurationManager.AppSettings["rutaXmlFEX"];
                if (routePath == null)
                {
                    throw new Exception("No definida Configuración Ruta Factura Fiscal.");
                }

                msgXtraInfo = "Obtener Ruta XML Factura Fiscal(A1.Firmar)";
                string routePathA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];
                if (routePath == null)
                {
                    throw new Exception("No definida Configuración Ruta A1.Firmar.");
                }

                msgXtraInfo = "Obtener Configuración Adicional XML Factura Fiscal";
                string stringDelayFEX = ConfigurationManager.AppSettings["delayFEX"];
                int intDelayFEX = 0;
                if (stringDelayFEX == null || !(int.TryParse(stringDelayFEX, out intDelayFEX)))
                {
                    throw new Exception("No definida Configuración Adicional Factura Fiscal.");
                }

                if (ids != null && ids.Length > 0)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var id in ids)
                            {
                                msgXtraInfo = "Obtener Factura";
                                Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
                                if (invoice == null) throw new Exception("Documento con identificador:" + id + " ,no se ha encontrado.");

                                //_invoiceList.Add(invoice);

                                msgXtraInfo = "Obtener Estado";
                                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09");
                                if (documentState == null) throw new Exception("Estado con identificador: '09' ,no se ha encontrado.");

                                invoice.Document.authorizationDate = DateTime.Now;
                                invoice.ValidateStateChange("09");
                                invoice.Document.PreAutorize(ActiveUser);

                                currentInvoiceNumber = invoice.Document.number;

                                invoice.ValidateInfo();
                                _invoiceList.Add(invoice);

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                            trans.Commit();
                        }
                        catch (Exception e)
                        {
                            if (e.Data["source"] != null)
                            {
                                msgErr += "la Factura # " + currentInvoiceNumber + "<br>" + Environment.NewLine;
                                if ((string)e.Data["source"] == "modelDocumentValidation") msgErr += "Los siguientes campos son requeridos: <br>" + Environment.NewLine;
                            }

                            trans.Rollback();

                            throw new Exception(msgErr + e.Message);
                        }
                    }
                }

                // Llamada Async
                System.Threading.Tasks.Task.Run(() =>
                    ServiceInvoiceExteriorPartial.CallXML(db,
                                                        _invoiceList,
                                                          (int)ViewData["id_company"],
                                                          routePath,
                                                          routePathA1Firmar,
                                                          intDelayFEX
                                                      ));

                oJsonResult.codeReturn = 1;
                oJsonResult.message = SuccessMessage("Facturas en proceso de ser autorizadas.");
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(msgErr + e.Message);
                LogWrite(e, null, "AutorizeDocuments=>" + msgXtraInfo);
            }

            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = (parametersSeekInvoiceExterior)TempData["parametersSeekInvoiceExterior"];
            var modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CheckAutorizeRSIDocuments(int[] ids)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            string msgXtraInfo = "";
            //string currentInvoiceNumber = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            List<Invoice> _invoiceList = new List<Invoice>();
            //string msgErr = "Se presentaron el(los) siguiente(s) error(es) al Autorizar Parcialmente ";

            try
            {
                msgXtraInfo = "Obtener Ruta XML Factura Fiscal(B1.AutorizadoActualizado)";
                string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];
                if (string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
                {
                    throw new Exception("No definida Configuración Ruta B1.AutorizadoActualizado.");
                }

                if (ids != null && ids.Length > 0)
                {
                    msgXtraInfo = "Obtener Factura";
                    DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Estado de PRE-AUTORIZADA
                    Invoice invoice = db.Invoice.FirstOrDefault(r => ids.Contains(r.id) && r.Document.DocumentState.code != "09");
                    if (invoice != null) throw new Exception("Factura: " + invoice.Document.number + " ,tiene estado: " + invoice.Document.DocumentState.name +
                                                             ". Las facturas selecionadas deben tener estado: " + documentState09.name);

                    LogController.tratarFicheroLog();
                    bool change = false;
                    //bool changeLista = false;

                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //const int LOGON_TYPE_NEW_CREDENTIALS = 9;
                            //const int LOGON32_PROVIDER_WINNT50 = 3;

                            //User token that represents the authorized user account
                            IntPtr token = IntPtr.Zero;
                            string USER_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["USER_rutaXmlB1AutorizadoActualizado"];
                            USER_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(USER_rutaXmlB1AutorizadoActualizado) ? "admin" : USER_rutaXmlB1AutorizadoActualizado;
                            string PASS_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["PASS_rutaXmlB1AutorizadoActualizado"];
                            PASS_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(PASS_rutaXmlB1AutorizadoActualizado) ? "admin" : PASS_rutaXmlB1AutorizadoActualizado;
                            string DOMAIN_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["DOMAIN_rutaXmlB1AutorizadoActualizado"];
                            DOMAIN_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(DOMAIN_rutaXmlB1AutorizadoActualizado) ? "" : DOMAIN_rutaXmlB1AutorizadoActualizado;

                            bool result = LogonUser(USER_rutaXmlB1AutorizadoActualizado, DOMAIN_rutaXmlB1AutorizadoActualizado, PASS_rutaXmlB1AutorizadoActualizado, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    String[] dirs = System.IO.Directory.GetDirectories(routePathB1AutorizadoActualizado);

                                    foreach (var id in ids)
                                    {
                                        invoice = db.Invoice.FirstOrDefault(r => r.id == id);

                                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada
                                        ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03"); //Autorizada
                                        var yearMothCurrent = invoice.Document.accessKey.Substring(4, 4).ToString() +
                                                              invoice.Document.accessKey.Substring(2, 2).ToString();
                                        foreach (String dir in dirs)
                                        {
                                            var directoryNameAux = dir.Substring(dir.Length - 8, 6);
                                            if (directoryNameAux == yearMothCurrent)
                                            {
                                                String[] files = System.IO.Directory.GetFileSystemEntries(dir);
                                                foreach (String file in files)
                                                {
                                                    string[] words = Path.GetFileNameWithoutExtension(file).Split('_');
                                                    string accessKeyAux = words[0];
                                                    var aDocument = invoice.Document;
                                                    if (accessKeyAux == aDocument.accessKey)
                                                    {
                                                        aDocument.dateUpdate = DateTime.Now;
                                                        aDocument.id_userUpdate = ActiveUser.id;
                                                        aDocument.id_documentState = documentState.id;
                                                        aDocument.DocumentState = documentState;
                                                        aDocument.authorizationNumber = aDocument.accessKey;

                                                        db.Document.Attach(aDocument);
                                                        db.Entry(aDocument).State = EntityState.Modified;

                                                        var aElectronicDocument = db.ElectronicDocument.FirstOrDefault(fod => fod.Document.accessKey == accessKeyAux);
                                                        if (aElectronicDocument != null)
                                                        {
                                                            aElectronicDocument.id_electronicDocumentState = electronicDocumentState.id;
                                                            aElectronicDocument.ElectronicDocumentState = electronicDocumentState;

                                                            db.ElectronicDocument.Attach(aElectronicDocument);
                                                            db.Entry(aElectronicDocument).State = EntityState.Modified;
                                                        }
                                                        if (!change) change = true;
                                                    }
                                                }
                                            };
                                        }
                                    }
                                    if (change)
                                    {
                                        db.SaveChanges();
                                        trans.Commit();
                                        oJsonResult.codeReturn = 1;
                                        oJsonResult.message = SuccessMessage("Facturas verificadas y actualizadas de la autorización del SRI.");
                                        LogController.WriteLog("Actualización de Estados de las Facturas del Exterior a Estado Autorizado Satisfactoriamente(Con Cambios)");
                                    }
                                    else
                                    {
                                        oJsonResult.codeReturn = 1;
                                        oJsonResult.message = WarningMessage("Facturas verificadas y aun no estan autorizadas por el SRI.");
                                        LogController.WriteLog("Actualización de Estados de las Facturas del Exterior a Estado Autorizado Satisfactoriamente(Sin Cambios)");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (change /*|| changeLista*/)
                            {
                                trans.Rollback();
                            }
                            LogController.WriteLog(e.Message);
                            throw new Exception(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "CheckAutorizeRSIDocuments=>" + msgXtraInfo);
            }

            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = (parametersSeekInvoiceExterior)TempData["parametersSeekInvoiceExterior"];
            var modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void CancelDocuments(int[] ids)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            string currentInvoiceNumber = "";
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                            if (invoice != null && documentState != null)
                            {
                                currentInvoiceNumber = invoice.Document.number;

                                invoice.ValidateStateChange("05");
                                invoice.Document.RemoveDocument(ActiveUser);
                                // invoice.ValidateInfo();

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Facturas anuladas.");
                    }
                    catch (Exception e)
                    {
                        string msgError = "Existen inconvenientes al anular la Factura " + currentInvoiceNumber + ", revise los siguientes errores:";
                        ViewData["EditError"] = msgError + e.Message;
                        trans.Rollback();
                    }
                }
            }

            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = (parametersSeekInvoiceExterior)TempData["parametersSeekInvoiceExterior"];
            var modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

            if (invoice.Document.DocumentState.code != "02")
            {
                ViewData["EditError"] = "Documento no puede ser reversado";
                TempData["invoiceExterior"] = invoice;
                TempData.Keep("invoiceExterior");
                return PartialView("_InvoiceExteriorMainFormPartial", invoice);
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Anulado

                    if (invoice != null && documentState != null)
                    {
                        invoice.Document.Reverse(ActiveUser, "01");

                        if (invoice.Document.id_documentOrigen != null)
                            ServiceSalesQuotationExterior.UpdateValuesFromInvoiceExteriorAnul(invoice, db);

                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["invoiceExterior"] = invoice;
                        TempData.Keep("invoiceExterior");

                        ViewData["EditMessage"] = SuccessMessage("Factura: " + invoice.Document.number + " reversada.");
                    }
                }
                catch (Exception e)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("invoiceExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            //TempData["purchaseOrder"] = purchaseOrder;
            //TempData.Keep("purchaseOrder");

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
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
        public JsonResult RecalculatePrices(int id, decimal? valueInternationalFreight, decimal? valueInternationalInsurance, decimal? valueCustomsExpenditures, decimal? valueTransportationExpenses)
        {
            var result = new { Message = "OK" };

            Invoice _invoice = ObtainInvoice(id);
            if (_invoice.Document.id_documentOrigen != null)
            {
                var aInvoiceDetail = _invoice.InvoiceDetail.Where(w => w.isActive).ToList();
                //TOTAL GASTOS.-Obtenemos el valor total de los gastos sumando: flete(4000), seguro(5000), gastos aduaneros(0) , gastos de transporte(0) = 9000
                decimal expenses = (valueInternationalFreight ?? 0) + (valueInternationalInsurance ?? 0) + (valueCustomsExpenditures ?? 0) + (valueTransportationExpenses ?? 0);

                //Distribución de Gastos en función de los kilos de los productos.
                #region Validar que el producto exista en la profroma
                var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                if (puedeModDatosProforma)
                {
                    var detallesProforma = new InvoiceDetail[] { };
                    var documentOrigin = db.Document.FirstOrDefault(e => e.id == _invoice.Document.id_documentOrigen);

                    if (documentOrigin.DocumentType?.code == "131") // Proforma
                    {
                        detallesProforma = db.InvoiceDetail.Where(e => e.id_invoice == documentOrigin.id && e.isActive).ToArray();
                    }
                    else if (documentOrigin.DocumentType?.code == "70") // Factura Comercial
                    {
                        var documentoOrigen2 = db.Document.FirstOrDefault(e => e.id == documentOrigin.id_documentOrigen);
                        if (documentoOrigen2.DocumentType?.code == "131") // Si el origen es proforma se llena
                        {
                            detallesProforma = db.InvoiceDetail.Where(e => e.id_invoice == documentoOrigen2.id && e.isActive).ToArray();
                        }
                    }

                    foreach (var invoiceDetail in aInvoiceDetail)
                    {
                        var detalleProforma = detallesProforma.FirstOrDefault(e => e.id_item == invoiceDetail.id_item);
                        if (detalleProforma == null)
                        {
                            result = new { Message = $"El producto {invoiceDetail.Item.name} no existe en la proforma." };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if(detalleProforma.numBoxes != invoiceDetail.numBoxes)
                            {
                                result = new { Message = $"La cantidad de cartones difiere a la proforma, producto {invoiceDetail.Item.name}." };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    foreach (var detalleProforma in detallesProforma)
                    {
                        var detalleFactura = aInvoiceDetail.FirstOrDefault(e => e.id_item == detalleProforma.id_item);
                        if (detalleFactura == null)
                        {
                            result = new { Message = $"El producto {detalleProforma.Item.name} no existe en la factura." };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                #endregion

                decimal totalKilos = 0.00M;
                foreach (var detail in aInvoiceDetail)
                {
                    //1 Para cada producto obtiene el factor de conversión a kilos (Productos – Pesos – Conversión a Kilos)
                    var aConversionToKilos = db.ItemWeightConversionFreezen.FirstOrDefault(r => r.id_Item == detail.id_item).conversionToKilos;
                    //3.3.2   Obtiene el total de kilos para cada producto multiplicando cantidad factura por su respectiva conversión.
                    //detail.totalKiloProforma = ((detail.amountproforma ?? 0m) * aConversionToKilos);
                    detail.totalKiloProforma = ((detail.id_amountInvoice ?? 0) * aConversionToKilos ?? 0);
                    totalKilos += detail.totalKiloProforma;
                }

                decimal totalExpensesAux = 0.00M;
                int id_detailAux = 0;
                //3.3.3   Determina el porcentaje dividiendo la cantidad factura kgs de cada producto para el total de kilos.
                foreach (var detail in aInvoiceDetail)
                {
                    detail.percentageKiloProforma = detail.totalKiloProforma / totalKilos;
                    //3.3.4   Distribuye el TOTAL DE GASTOS(2) para cada producto en función del porcentaje obtenido en 3.3.3
                    detail.expensesProforma = Math.Round(expenses * detail.percentageKiloProforma, 2);
                    //detail.expensesProforma = expenses * detail.percentageKiloProforma;
                    totalExpensesAux += detail.expensesProforma;
                    id_detailAux = detail.id;
                    //Se obtiene el nuevo total para cada producto
                    //Nuevo Total = Valor Proforma (Cantidad factura * precio proforma) menos los gastos (obtenido en el punto 3.4)
                    detail.total = Math.Round((Math.Round(((detail.id_amountInvoice ?? 0) * detail.unitPriceProforma), 2) - detail.expensesProforma - detail.discount), 2);
                    //detail.total = ((detail.id_amountInvoice ?? 0) * detail.unitPriceProforma) - detail.expensesProforma;
                    //Si el valor que se obtiene es cero o es menor a cero dar un mensaje de alerta indicando que deben revisarse los gastos y NO se realiza el recalculo de precios.
                    if (detail.total <= 0)
                    {
                        result = new { Message = "Deben revisarse los gastos." };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    detail.totalPriceWithoutTax = Math.Round(detail.total + detail.discount, 2);
                    //detail.totalPriceWithoutTax = detail.total;
                    //Se divide el nuevo total para cantidad factura(redondear a 8 decimales)
                    detail.unitPrice = Math.Round((detail.total + detail.discount) / (detail.id_amountInvoice ?? 0.00M), 8);
                    var totalAux = Math.Round(detail.unitPrice * (detail.id_amountInvoice ?? 0.00M), 2);
                    //if (totalAux != detail.total) detail.unitPrice += 0.000001M;
                    var paso = totalAux > (detail.total + detail.discount) ? -0.0000001M : 0.0000001M;
                    while (totalAux != (detail.total + detail.discount))
                    {
                        detail.unitPrice += paso;
                        totalAux = Math.Round(detail.unitPrice * (detail.id_amountInvoice ?? 0.00M), 2);
                    }
                    //detail.unitPrice = Math.Round(detail.total / (detail.id_amountInvoice ?? 0), 8);
                }

                //la suma de los valores debe ser igual a la cantidad distribuida. Si existe diferencia de centavos se ajusta al último item
                if (totalExpensesAux != expenses)
                {
                    var aAInvoiceDetail = aInvoiceDetail.FirstOrDefault(fod => fod.id == id_detailAux);
                    aAInvoiceDetail.expensesProforma += (expenses - totalExpensesAux);
                    //aAInvoiceDetail.total = aAInvoiceDetail.subTotalIVA0Proforma - aAInvoiceDetail.expensesProforma;
                    aAInvoiceDetail.total = ((aAInvoiceDetail.id_amountInvoice ?? 0) * aAInvoiceDetail.unitPriceProforma) - aAInvoiceDetail.expensesProforma - aAInvoiceDetail.discount;
                    //aAInvoiceDetail.total = ((aAInvoiceDetail.id_amountInvoice ?? 0) * aAInvoiceDetail.unitPriceProforma) - aAInvoiceDetail.expensesProforma;
                    //Si el valor que se obtiene es cero o es menor a cero dar un mensaje de alerta indicando que deben revisarse los gastos y NO se realiza el recalculo de precios.
                    if (aAInvoiceDetail.total <= 0)
                    {
                        result = new { Message = "Deben revisarse los gastos." };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    aAInvoiceDetail.totalPriceWithoutTax = aAInvoiceDetail.total;
                    aAInvoiceDetail.unitPrice = Math.Round(aAInvoiceDetail.total / (aAInvoiceDetail.id_amountInvoice ?? 0), 8);

                    //Se divide el nuevo total para cantidad factura(redondear a 8 decimales)
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion {RA} - Transacion Invoice

        #region {RA} Invoice Edit

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInvoiceExterior(int id, int[] requestDetails)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            if(id != 0)
            {
                invoice.InvoiceDetail = invoice?.InvoiceDetail.Where(v => v.isActive).ToList();
            }
            var valInvFact = db.Setting.FirstOrDefault(e => e.code == "INVFACT")?.value == "SI";
            ViewBag.Movimiento = false;
            if (invoice == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                tbsysInvoiceMode invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
                tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

                int id_company = (int)ViewData["id_company"];
                invoice = new Invoice
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
                    id_InvoiceMode = invoiceMode?.id ?? 0,
                    tbsysInvoiceMode = invoiceMode,
                    id_InvoiceType = invoiceType?.id ?? 0,
                    tbsysInvoiceType = invoiceType,
                    InvoiceExterior = new InvoiceExterior
                    {
                        id = 0
                    },
                    InvoiceDetail = new List<InvoiceDetail>(),
                    InvoiceWeightList = new List<Models.ModelExtension.InvoiceWeightView>(),
                    InvoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>(),
                };
            }
            else
            {
                var salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
                var aValueSubscribed = invoice.InvoiceExterior.valueSubscribed ?? 0;
                if (salesQuotationExterior != null && aValueSubscribed <= 0)
                {
                    invoice.InvoiceExterior.valueSubscribed = salesQuotationExterior.valueSubscribed;
                    invoice.InvoiceExterior.balance = (invoice.InvoiceExterior.valuetotalCIF - (invoice.InvoiceExterior.valueSubscribed ?? 0) - (invoice.InvoiceExterior.finalPayment ?? 0));
                }

                #region Preparamos el tipo de documento de origen

                var documentoOrigen = db.Document
                   .FirstOrDefault(e => e.id == invoice.Document.id_documentOrigen && e.DocumentState.code != "05");
                if (documentoOrigen != null)
                {
                    if (documentoOrigen.DocumentType.code == "70") // Factura Comercial
                    {
                        this.ViewBag.EtiquetaExterna = "Factura Comercial";
                        salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(s => s.id == documentoOrigen.id_documentOrigen);
                    }
                    else if (documentoOrigen.DocumentType.code == "131") // Proforma
                    {
                        this.ViewBag.EtiquetaExterna = "Proforma";
                    }
                }
                else
                {
                    this.ViewBag.EtiquetaExterna = "";
                }
                ViewBag.movimiento = false;
                if (valInvFact)
                {
                    //Verifico si existe egreso Relacionado
                    var lstInvoiceInventory = db.InventoryMove.FirstOrDefault(w => w.id_Invoice != null && w.Document.DocumentState.code != "05" && w.id_Invoice == id);
                    if (lstInvoiceInventory != null)
                    {
                        ViewBag.movimiento = true;
                        ViewBag.movimientoInventario = lstInvoiceInventory.natureSequential;
                        ViewBag.naturalSequential = "No se puede agregar porque está realacionado a un movimiento de inventario " + lstInvoiceInventory.natureSequential;
                    }
                    else
                    {
                        ViewBag.movimiento = false;
                    }


                }
                invoice.SalesQuotationExterior = salesQuotationExterior;
                invoice.calculateTotales();
                invoice.calculateTotalesInvoiceExterior();
                invoice.ViewWeight();

                #endregion Preparamos el tipo de documento de origen
            }

            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            ViewBag.id_documentState = invoice.Document.id_documentState;

            ViewBag.IsManual = true;
            //

            return PartialView("_FormEditInvoiceExterior", invoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationList(bool isCallback = false)
        {
            var model = SalesQuotationExteriorDapper
                .RecuperarProformasPorFacturar();

            if (isCallback)
            {
                return PartialView("_SalesQuotationGridView", model);
            }
            else
            {
                return PartialView("_SalesQuotationList", model);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInvoiceExteriorFromSalesQuotation(int id, Boolean isReasignacion)
        {
            var documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"));
            var documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
            var invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
            var invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

            var invoice = new Invoice
            {
                Document = new Document
                {
                    id = 0,
                    id_documentType = documentType?.id ?? 0,
                    DocumentType = documentType,
                    id_documentState = documentState?.id ?? 0,
                    DocumentState = documentState,
                    emissionDate = DateTime.Now,
                    id_documentOrigen = id,
                    Document2 = db.Document.FirstOrDefault(d => d.id == id)
                },
                id_InvoiceMode = invoiceMode?.id ?? 0,
                tbsysInvoiceMode = invoiceMode,
                id_InvoiceType = invoiceType?.id ?? 0,
                tbsysInvoiceType = invoiceType,
                InvoiceExterior = new InvoiceExterior
                {
                    id = 0
                },
                InvoiceDetail = new List<InvoiceDetail>(),
                InvoiceWeightList = new List<Models.ModelExtension.InvoiceWeightView>(),
                InvoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>(),
            };

            var salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(s => s.id == id);
            if ((salesQuotationExterior != null) && (salesQuotationExterior.Invoice != null)
                && (salesQuotationExterior.pendingBoxes > 0))
            {
                //TODO: cargar datos desde proforma
                //Datos Factura
                invoice.id_buyer = salesQuotationExterior.Invoice.id_buyer;
                invoice.id_remissionGuide = salesQuotationExterior.Invoice.id_remissionGuide;
                invoice.Person = salesQuotationExterior.Invoice.Person;
                invoice.Person1 = salesQuotationExterior.Invoice.Person1;
                invoice.RemissionGuide = salesQuotationExterior.Invoice.RemissionGuide;
                invoice.subtotalIVA = 0;//salesQuotationExterior.Invoice.subtotalIVA;
                invoice.subTotalIVA0 = 0;//salesQuotationExterior.Invoice.subTotalIVA0;
                invoice.subTotalNoObjectIVA = 0;//salesQuotationExterior.Invoice.subTotalNoObjectIVA;
                invoice.subTotalExentIVA = 0;//salesQuotationExterior.Invoice.subTotalExentIVA;
                invoice.subTotal = 0;//salesQuotationExterior.Invoice.subTotal;
                invoice.totalDiscount = 0;//salesQuotationExterior.Invoice.totalDiscount;
                invoice.valueICE = 0;//salesQuotationExterior.Invoice.valueICE;
                invoice.valueIRBPNR = 0;//salesQuotationExterior.Invoice.valueIRBPNR;
                invoice.IVA = 0;//salesQuotationExterior.Invoice.IVA;
                invoice.tip = 0;//salesQuotationExterior.Invoice.tip;
                invoice.totalValue = 0;//salesQuotationExterior.Invoice.totalValue;
                invoice.subtotalNoTaxes = 0;//salesQuotationExterior.Invoice.subtotalNoTaxes;

                //Datos Factura Exterior
                invoice.InvoiceExterior.id_termsNegotiation = salesQuotationExterior.id_termsNegotiation;
                invoice.InvoiceExterior.TermsNegotiation = salesQuotationExterior.TermsNegotiation;
                invoice.InvoiceExterior.id_PaymentMethod = salesQuotationExterior.id_PaymentMethod;
                invoice.InvoiceExterior.PaymentMethod = salesQuotationExterior.PaymentMethod;
                invoice.InvoiceExterior.id_PaymentTerm = salesQuotationExterior.id_PaymentTerm;
                invoice.InvoiceExterior.PaymentTerm = salesQuotationExterior.PaymentTerm;
                invoice.InvoiceExterior.id_portDestination = salesQuotationExterior.id_portDestination;
                invoice.InvoiceExterior.Port1 = salesQuotationExterior.Port;
                invoice.InvoiceExterior.id_portDischarge = salesQuotationExterior.id_portDischarge;
                invoice.InvoiceExterior.Port2 = salesQuotationExterior.Port1;
                invoice.InvoiceExterior.dateShipment = salesQuotationExterior.dateShipment;
                invoice.InvoiceExterior.id_addressCustomer = salesQuotationExterior.id_addressCustomer;
                invoice.InvoiceExterior.ForeignCustomerIdentification = salesQuotationExterior.ForeignCustomerIdentification;
                //invoice.InvoiceExterior.direccion = salesQuotationExterior.direccion;
                //invoice.InvoiceExterior.email = salesQuotationExterior.email;
                invoice.InvoiceExterior.id_metricUnitInvoice = salesQuotationExterior.id_metricUnitInvoice;
                invoice.InvoiceExterior.MetricUnit = salesQuotationExterior.MetricUnit;
                invoice.InvoiceExterior.id_consignee = salesQuotationExterior.id_consignee;
                invoice.InvoiceExterior.Person = salesQuotationExterior.Person;
                invoice.InvoiceExterior.id_notifier = salesQuotationExterior.id_notifier;
                invoice.InvoiceExterior.Person1 = salesQuotationExterior.Person1;
                invoice.InvoiceExterior.idVendor = salesQuotationExterior.idVendor;
                invoice.InvoiceExterior.Person2 = salesQuotationExterior.Person2;
                invoice.InvoiceExterior.totalBoxes = 0;//salesQuotationExterior.pendingBoxes ?? 0;
                invoice.InvoiceExterior.purchaseOrder = salesQuotationExterior.purchaseOrder;
                invoice.InvoiceExterior.id_bank = salesQuotationExterior.id_bank;
                invoice.InvoiceExterior.id_portShipment = salesQuotationExterior.id_portShipment;

                invoice.InvoiceExterior.valueCustomsExpenditures = salesQuotationExterior.valueCustomsExpenditures;
                invoice.InvoiceExterior.valueInternationalFreight = salesQuotationExterior.valueInternationalFreight;
                invoice.InvoiceExterior.valueInternationalInsurance = salesQuotationExterior.valueInternationalInsurance;
                invoice.InvoiceExterior.valueTransportationExpenses = salesQuotationExterior.valueTransportationExpenses;
                invoice.InvoiceExterior.valuetotalCIF = salesQuotationExterior.valuetotalCIF;
                invoice.InvoiceExterior.valueTotalFOB = salesQuotationExterior.valueTotalFOB;

                invoice.InvoiceExterior.shipName = salesQuotationExterior.vessel;
                invoice.InvoiceExterior.shipNumberTrip = salesQuotationExterior.trip;
                invoice.InvoiceExterior.valueSubscribed = salesQuotationExterior.valueSubscribed;
                if (salesQuotationExterior.numeroContenedores != null) invoice.InvoiceExterior.numeroContenedores = salesQuotationExterior.numeroContenedores.Value;
                invoice.InvoiceExterior.id_capacityContainer = salesQuotationExterior.id_capacityContainer;
                invoice.InvoiceExterior.temperatureInstruction = salesQuotationExterior.temperatureInstruction;
                invoice.InvoiceExterior.idTipoTemperatura = salesQuotationExterior.idTipoTemperatura;
                invoice.InvoiceExterior.temperatureInstrucDate = salesQuotationExterior.temperatureInstrucDate;
                invoice.InvoiceExterior.id_BankTransfer = salesQuotationExterior.id_BankTransfer;
                invoice.InvoiceExterior.PO = salesQuotationExterior.PO;
                invoice.InvoiceExterior.noContrato = salesQuotationExterior.noContrato;

                //Detalles
                foreach (var detail in salesQuotationExterior.Invoice.InvoiceDetail)
                {
                    if (!(detail.proformaPendingNumBoxes > 0))
                        continue;

                    var _metricUnitOrigen = db.ItemWeightConversionFreezen.FirstOrDefault(r => r.id_Item == detail.id_item).MetricUnit;
                    var _amountInfo = CalculaCantidadInt(detail.id_item, detail.numBoxes.Value, _metricUnitOrigen.id);

                    detail.isActive = false;

                    detail.id_userCreate = ActiveUserId;
                    detail.dateCreate = DateTime.Now;
                    detail.id_userUpdate = ActiveUserId;
                    detail.dateUpdate = DateTime.Now;
                    detail.numBoxes = detail.proformaPendingNumBoxes;
                    detail.amount = _amountInfo.cantidadItem;
                    detail.id_amountInvoice = _amountInfo.cantidadItem;
                    detail.amountproforma = _amountInfo.cantidadProforma;
                    detail.total = (detail.amount * detail.unitPrice);// - detail.discount;
                    detail.totalPriceWithoutTax = detail.total;
                    detail.unitPriceProforma = detail.unitPrice;
                    detail.subTotalIVA0Proforma = detail.total;

                    invoice.InvoiceDetail.Add(detail);
                }

                //PaymentTerm
                foreach (var paymentTerm in salesQuotationExterior.Invoice.InvoiceExteriorPaymentTerm)
                {
                    invoice.InvoiceExteriorPaymentTerm.Add(paymentTerm);
                }

                //ExteriorWeight
                foreach (var exteriorWeight in salesQuotationExterior.Invoice.InvoiceExteriorWeight)
                {
                    invoice.InvoiceExteriorWeight.Add(exteriorWeight);
                }

                invoice.SalesQuotationExterior = salesQuotationExterior;
                invoice.calculateTotales();
                invoice.calculateTotalBoxes();
                invoice.calculateTotalesInvoiceExterior();
                invoice.GetTariffHeadingDescription();
            }

            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            ViewBag.id_documentState = invoice.Document.id_documentState;
            ViewBag.DocumentOrigen = id;
            ViewBag.IsManual = false;
            ViewBag.IsReasignacion = isReasignacion;
            TempData["isReasignacion"] = isReasignacion;
            this.ViewBag.EtiquetaExterna = "Proforma";

            return PartialView("_FormEditInvoiceExterior", invoice);
        }

        #region Crear desde Factura Comercial

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCommercialList(bool callBack = false)
        {
            var model = db.InvoiceCommercial.Where(s =>
                s.Document.DocumentTransactionState.code != "02" &&
                (s.Document.DocumentState.code == "03" || s.Document.DocumentState.code == "01")).ToList();

            if (callBack)
            {
                return PartialView("_InvoiceCommercialGridView", model);
            }
            else
            {
                return PartialView("_InvoiceCommercialList", model);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInvoiceExteriorFromInvoiceCommercial(int id, Boolean isReasignacion)
        {
            var documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"));
            var documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
            var invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
            var invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);
            var salesQuotationExterior = new SalesQuotationExterior();
            var invoice = new Invoice
            {
                Document = new Document
                {
                    id = 0,
                    id_documentType = documentType?.id ?? 0,
                    DocumentType = documentType,
                    id_documentState = documentState?.id ?? 0,
                    DocumentState = documentState,
                    emissionDate = DateTime.Now,
                    id_documentOrigen = id,
                    Document2 = db.Document.FirstOrDefault(d => d.id == id)
                },
                id_InvoiceMode = invoiceMode?.id ?? 0,
                tbsysInvoiceMode = invoiceMode,
                id_InvoiceType = invoiceType?.id ?? 0,
                tbsysInvoiceType = invoiceType,
                InvoiceExterior = new InvoiceExterior
                {
                    id = 0
                },
                InvoiceDetail = new List<InvoiceDetail>(),
                InvoiceWeightList = new List<Models.ModelExtension.InvoiceWeightView>(),
                InvoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>(),
            };

            var invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(s => s.id == id);
            if (invoiceCommercial != null)
            {
                //TODO: cargar datos desde proforma
                //Datos Factura
                invoice.id_buyer = invoiceCommercial.id_ForeignCustomer;
                invoice.Person = invoiceCommercial.Person;
                invoice.Person1 = invoiceCommercial.Person1;
                invoice.subtotalIVA = 0;
                invoice.subTotalIVA0 = 0;
                invoice.subTotalNoObjectIVA = 0;
                invoice.subTotalExentIVA = 0;
                invoice.subTotal = 0;
                invoice.totalDiscount = 0;
                invoice.valueICE = 0;
                invoice.valueIRBPNR = 0;
                invoice.IVA = 0;
                invoice.tip = 0;
                invoice.totalValue = 0;
                invoice.subtotalNoTaxes = 0;

                //Datos Factura Exterior
                invoice.InvoiceExterior.id_CityDelivery = invoiceCommercial.id_CityDelivery;
                invoice.InvoiceExterior.blDate = invoiceCommercial.blDate;
                invoice.InvoiceExterior.idPortfolioFinancing = invoiceCommercial.idPortfolioFinancing;
                invoice.InvoiceExterior.id_termsNegotiation = invoiceCommercial.id_termsNegotiation;
                invoice.InvoiceExterior.TermsNegotiation = invoiceCommercial.TermsNegotiation;
                invoice.InvoiceExterior.id_PaymentMethod = invoiceCommercial.id_PaymentMethod;
                invoice.InvoiceExterior.PaymentMethod = invoiceCommercial.PaymentMethod;
                invoice.InvoiceExterior.id_PaymentTerm = invoiceCommercial.id_PaymentTerm;
                invoice.InvoiceExterior.PaymentTerm = invoiceCommercial.PaymentTerm;
                invoice.InvoiceExterior.id_portDestination = invoiceCommercial.id_portDestination;
                invoice.InvoiceExterior.Port1 = invoiceCommercial.Port;
                invoice.InvoiceExterior.id_portDischarge = invoiceCommercial.id_portDischarge;
                invoice.InvoiceExterior.Port2 = invoiceCommercial.Port1;
                invoice.InvoiceExterior.dateShipment = invoiceCommercial.dateShipment;
                invoice.InvoiceExterior.id_metricUnitInvoice = invoiceCommercial.id_metricUnitInvoice;
                invoice.InvoiceExterior.MetricUnit = invoiceCommercial.MetricUnit;
                invoice.InvoiceExterior.id_consignee = invoiceCommercial.id_Consignee;
                invoice.InvoiceExterior.Person = invoiceCommercial.Person;
                invoice.InvoiceExterior.id_notifier = invoiceCommercial.id_Notifier;
                invoice.InvoiceExterior.Person1 = invoiceCommercial.Person1;
                invoice.InvoiceExterior.idVendor = invoiceCommercial.idVendor;
                invoice.InvoiceExterior.totalBoxes = 0;
                invoice.InvoiceExterior.purchaseOrder = invoiceCommercial.purchaseOrder;
                invoice.InvoiceExterior.id_BankTransfer = invoiceCommercial.id_BankTransfer;
                invoice.InvoiceExterior.id_portShipment = invoiceCommercial.id_portShipment;

                invoice.InvoiceExterior.valueCustomsExpenditures = invoiceCommercial.valueCustomsExpenditures;
                invoice.InvoiceExterior.valueInternationalFreight = invoiceCommercial.valueInternationalFreight;
                invoice.InvoiceExterior.valueInternationalInsurance = invoiceCommercial.valueInternationalInsurance;
                invoice.InvoiceExterior.valueTransportationExpenses = invoiceCommercial.valueTransportationExpenses;
                invoice.InvoiceExterior.valuetotalCIF = invoiceCommercial.totalValueOrigen;

                invoice.InvoiceExterior.shipName = invoiceCommercial.shipName;
                invoice.InvoiceExterior.id_shippingAgency = invoiceCommercial.id_shippingAgency;
                invoice.InvoiceExterior.id_ShippingLine = invoiceCommercial.id_shippingLine;
                invoice.InvoiceExterior.shipNumberTrip = invoiceCommercial.shipNumberTrip;
                invoice.InvoiceExterior.BLNumber = invoiceCommercial.BLNumber;
                invoice.InvoiceExterior.containers = invoiceCommercial.containers;
                invoice.InvoiceExterior.daeNumber = invoiceCommercial.daeNumber;
                invoice.InvoiceExterior.daeNumber2 = invoiceCommercial.daeNumber2;
                invoice.InvoiceExterior.daeNumber3 = invoiceCommercial.daeNumber3;
                invoice.InvoiceExterior.daeNumber4 = invoiceCommercial.daeNumber4;

                invoice.InvoiceExterior.id_tariffHeading = invoiceCommercial.id_tariffHeading;
                invoice.InvoiceExterior.tariffHeadingDescription = invoiceCommercial?.TariffHeading?.code;
                invoice.InvoiceExterior.TariffHeading = invoiceCommercial.TariffHeading;

                if (invoiceCommercial.numeroContenedores != null) invoice.InvoiceExterior.numeroContenedores = invoiceCommercial.numeroContenedores.Value;
                invoice.InvoiceExterior.id_capacityContainer = invoiceCommercial.id_capacityContainer;
                invoice.InvoiceExterior.etaDate = invoiceCommercial.etaDate;

                // Datos desde proforma
                IEnumerable<InvoiceDetail> detallesProforma = null;
                var proformaFacComercial = db.Document.FirstOrDefault(e => e.id == invoiceCommercial.Document.id_documentOrigen);
                if ((proformaFacComercial != null) && (proformaFacComercial.DocumentState?.code != "05")
                    && (proformaFacComercial.DocumentType?.code == "131"))
                {
                    invoice.InvoiceExterior.id_bank = proformaFacComercial.Invoice.SalesQuotationExterior.id_bank;
                    invoice.InvoiceExterior.id_addressCustomer = proformaFacComercial.Invoice.SalesQuotationExterior.id_addressCustomer;
                    invoice.InvoiceExterior.ForeignCustomerIdentification = proformaFacComercial.Invoice.SalesQuotationExterior.ForeignCustomerIdentification;

                    invoice.InvoiceExterior.valueSubscribed = proformaFacComercial.Invoice.SalesQuotationExterior.valueSubscribed;
                    invoice.InvoiceExterior.temperatureInstruction = proformaFacComercial.Invoice.SalesQuotationExterior.temperatureInstruction;
                    invoice.InvoiceExterior.idTipoTemperatura = proformaFacComercial.Invoice.SalesQuotationExterior.idTipoTemperatura;
                    invoice.InvoiceExterior.temperatureInstrucDate = proformaFacComercial.Invoice.SalesQuotationExterior.temperatureInstrucDate;
                    invoice.InvoiceExterior.valueTotalFOB = proformaFacComercial.Invoice.SalesQuotationExterior.valueTotalFOB;
                    invoice.InvoiceExterior.numeroContenedores = proformaFacComercial.Invoice.SalesQuotationExterior.numeroContenedores.Value;
                    invoice.InvoiceExterior.id_capacityContainer = proformaFacComercial.Invoice.SalesQuotationExterior.id_capacityContainer;
                    foreach (var paymentTerm in proformaFacComercial.Invoice.InvoiceExteriorPaymentTerm)
                    {
                        invoice.InvoiceExteriorPaymentTerm.Add(paymentTerm);
                    }

                    //ExteriorWeight
                    foreach (var exteriorWeight in proformaFacComercial.Invoice.InvoiceExteriorWeight)
                    {
                        invoice.InvoiceExteriorWeight.Add(exteriorWeight);
                    };

                    detallesProforma = proformaFacComercial.Invoice.InvoiceDetail;
                }

                //Detalles
                foreach (var detail in invoiceCommercial.InvoiceCommercialDetail.Where(e => e.isActive))
                {
                    var _metricUnitOrigen = db.ItemWeightConversionFreezen.FirstOrDefault(r => r.id_Item == detail.id_item).MetricUnit;
                    var _amountInfo = CalculaCantidadInt(detail.id_item, detail.numBoxes, _metricUnitOrigen.id);
                    var id_tariffHeadingDetail = invoiceCommercial.id_tariffHeading ?? db.TariffHeading.FirstOrDefault(e => e.isActive)?.id;

                    var invoiceDetail = new InvoiceDetail()
                    {
                        id = detail.id,
                        id_item = detail.id_item,
                        id_invoice = detail.id_invoiceCommercial,
                        auxCode_Inf = detail.auxCode_Inf,
                        cantidad_DetailOperation = detail.cantidad_DetailOperation,
                        codeMetricUnitOrigin_Inf = detail.codeMetricUnitOrigin_Inf,
                        codeMetricUnit_Inf = detail.codeMetricUnit_Inf,
                        codePresentation = detail.codePresentation,
                        db_DetailOperation = detail.db_DetailOperation,
                        factor_DetailOperation = detail.factor_DetailOperation,
                        foreignName_Inf = detail.foreignName_Inf,
                        hasGlaze_DetailOperation = detail.hasGlaze_DetailOperation,
                        idCompany = detail.idCompany,
                        idItem_DetailOperation = detail.idItem_DetailOperation,
                        id_itemMarked = detail.id_itemMarked,
                        id_metricUnit = detail.id_metricUnit,
                        id_MetricUnitInvoice_DetailOperation = detail.id_MetricUnitInvoice_DetailOperation,
                        id_MetricUnit_DetailOperation = detail.id_MetricUnit_DetailOperation,
                        Item = detail.Item,
                        Item1 = detail.Item1,
                        masterCode_Inf = detail.masterCode_Inf,
                        MetricUnit = detail.MetricUnit,
                        MetricUnit1 = detail.MetricUnit1,
                        numBoxes = detail.numBoxes,
                        pesoBasic_DetailOperation = detail.pesoBasic_DetailOperation,
                        pesoProformaTotal_DetailOperation = detail.pesoProformaTotal_DetailOperation,
                        pesoProforma_DetailOperation = detail.pesoProforma_DetailOperation,
                        pesoTotal_DetailOperation = detail.pesoTotal_DetailOperation,
                        peso_DetailOperation = detail.peso_DetailOperation,
                        precio_DetailOperation = detail.precio_DetailOperation,
                        presentationMaximum = detail.presentationMaximum,
                        presentationMinimum = detail.presentationMinimum,
                        discount = detail.discount.Value,
                        total = detail.total,
                        total_DetailOperation = detail.total_DetailOperation,
                        unitPrice = detail.unitPrice,
                        weightBoxUM = detail.weightBoxUM,
                        //id_amountInvoice = detail.amount,
                        id_metricUnitInvoiceDetail = detail.id_metricUnitOrigen,
                        amountproforma = _amountInfo.cantidadItem,
                        id_amountInvoice = _amountInfo.cantidadItem,
                        amount = _amountInfo.cantidadItem,
                        id_tariffHeadingDetail = id_tariffHeadingDetail,


                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        isActive = true,

                        // Inicializacion de campos
                        //amountproforma = 0,
                        expensesProforma = 0m,
                        id_attribute1 = 0,
                        netWeight = 0,
                        proformaPendingNumBoxes = 0,
                        proformaNumBoxesPlusMinus = 0,
                        proformaPorcVariationPlusMinus = 0,
                        proformaUsedNumBoxes = 0,
                        proformaWeight = 0,
                        totalProforma = 0,
                    };

                    // Completamos los detalles del producto desde el detalle de profroma
                    var detalleProducto = (detallesProforma != null)
                        ? detallesProforma.FirstOrDefault(e => e.id_item == detail.id_item && e.isActive)
                        : null;

                    if (detalleProducto != null)
                    {
                        invoiceDetail.id_amountInvoice = detalleProducto.id_amountInvoice;
                        invoiceDetail.id_tariffHeadingDetail = detalleProducto.id_tariffHeadingDetail ?? invoiceCommercial.id_tariffHeading;
                        invoiceDetail.id_metricUnitInvoiceDetail = detalleProducto.id_metricUnitInvoiceDetail;
                        invoiceDetail.amountDisplay = detalleProducto.amountDisplay;
                        invoiceDetail.amountInvoiceDisplay = detalleProducto.amountInvoiceDisplay;
                        invoiceDetail.proformaNumBoxesPlusMinus = detalleProducto.proformaNumBoxesPlusMinus;
                        invoiceDetail.proformaPendingDiscount = detalleProducto.proformaPendingDiscount;
                        invoiceDetail.proformaPendingNumBoxes = detalleProducto.proformaPendingNumBoxes;
                        invoiceDetail.proformaPorcVariationPlusMinus = detalleProducto.proformaPorcVariationPlusMinus;
                        invoiceDetail.proformaUsedDiscount = detalleProducto.proformaUsedDiscount;
                        invoiceDetail.proformaUsedNumBoxes = detalleProducto.proformaUsedNumBoxes;
                        invoiceDetail.proformaWeight = detalleProducto.proformaWeight;
                        invoiceDetail.amountProformaDisplay = detalleProducto.amountProformaDisplay;
                        invoiceDetail.expensesProforma = detalleProducto.expensesProforma;
                        invoiceDetail.percentageKiloProforma = detalleProducto.percentageKiloProforma;
                        invoiceDetail.pesoProformaTotal_DetailOperation = detalleProducto.pesoProformaTotal_DetailOperation;
                        invoiceDetail.pesoProforma_DetailOperation = detalleProducto.pesoProforma_DetailOperation;
                        invoiceDetail.subTotalIVA0Proforma = detalleProducto.subTotalIVA0Proforma;
                        invoiceDetail.totalKiloProforma = detalleProducto.totalKiloProforma;
                        invoiceDetail.unitPriceProforma = detalleProducto.unitPriceProforma;
                        invoiceDetail.discount = detalleProducto.discount;
                        invoiceDetail.total = (detail.amount * detalleProducto.unitPrice) - detalleProducto.discount;
                        invoiceDetail.totalPriceWithoutTax = detail.total;
                        invoiceDetail.unitPriceProforma = detalleProducto.unitPrice;
                        invoiceDetail.subTotalIVA0Proforma = invoiceDetail.total;
                        invoiceDetail.totalProforma = detalleProducto.total;
                    }

                    invoiceDetail.calculateTotal();
                    invoice.InvoiceDetail.Add(invoiceDetail);
                }
                salesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(a => a.id == invoice.Document.Document2.id_documentOrigen);
                invoice.SalesQuotationExterior = salesQuotationExterior;

                invoice.calculateTotales();
                invoice.calculateTotalBoxes();
                invoice.calculateTotalesInvoiceExterior();
                invoice.GetTariffHeadingDescription();
            }

            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            ViewBag.id_documentState = invoice.Document.id_documentState;
            ViewBag.DocumentOrigen = id;
            ViewBag.IsManual = false;
            ViewBag.IsReasignacion = isReasignacion;
            TempData["isReasignacion"] = isReasignacion;
            this.ViewBag.EtiquetaExterna = "Factura Comercial";

            return PartialView("_FormEditInvoiceExterior", invoice);
        }

        #endregion Crear desde Factura Comercial

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCopy(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(o => o.id == id);

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code.Equals("01"));
            tbsysInvoiceMode invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
            tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.isExterior && r.isActive);

            #region Seleccion punto de Emisión

            int id_ep = 0;
            if (TempData["id_ep"] != null)
            {
                id_ep = (int)TempData["id_ep"];
            }
            id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

            #endregion Seleccion punto de Emisión

            if (invoice != null)
            {
                invoice.id = 0;
                invoice.Document = new Document
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
                    sequential = GetDocumentSequentialEmissionPoint(documentType.id, id_ep),
                    number = GetDocumentNumberEmissionPoint(documentType.id, id_ep)
                };

                if (invoice.InvoiceExterior == null) invoice.InvoiceExterior = db.InvoiceExterior.FirstOrDefault(r => r.id == id);
                if (invoice.InvoiceDetail.Count() == 0) invoice.InvoiceDetail = db.InvoiceDetail.Where(r => r.id_invoice == id).ToList();

                invoice.calculateTotales();
                invoice.calculateTotalesInvoiceExterior();
                invoice.ViewWeight();

                //  invoice.InvoiceExterior.idStatusDocument = documentState?.id ?? 0;
            }

            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            return PartialView("_FormEditInvoiceExterior", invoice);
        }

        [HttpPost]
        public ActionResult GetPaymentTerm()
        {
            Invoice invoice = ObtainInvoice(0);
            TempData.Keep("invoiceExterior");
            return PartialView("_InvoiceExteriorMainFormTabInvoiceExteriorPaymentTerm", invoice);
        }

        [HttpPost]
        public ActionResult GetShippingLine()
        {
            Invoice invoice = ObtainInvoice(0);
            TempData.Keep("invoiceExterior");
            return PartialView("_InvoiceExteriorMainFormTabInvoiceExteriorShippingLine", invoice);
        }

        [HttpPost]
        public ActionResult InvoiceExternalTotales(
                                                    decimal valueTransportationExpenses,
                                                    decimal valueCustomsExpenditures,
                                                    decimal valueInternationalInsurance,
                                                    decimal valueInternationalFreight
                                                    )
        {
            Invoice invoice = ObtainInvoice(0);

            invoice.InvoiceExterior = invoice.InvoiceExterior ?? new InvoiceExterior();
            invoice.InvoiceExterior.valueTransportationExpenses = valueTransportationExpenses;
            invoice.InvoiceExterior.valueCustomsExpenditures = valueCustomsExpenditures;
            invoice.InvoiceExterior.valueInternationalInsurance = valueInternationalInsurance;
            invoice.InvoiceExterior.valueInternationalFreight = valueInternationalFreight;

            string[] configDec = new string[2];
            invoice.valueTransportationExpensesTruncate = valueTransportationExpenses.ToAdvanceDecimal(configDec, out configDec);  //(decimal)(Math.Truncate(valueTransportationExpenses * 100) / 100);
            invoice.valueCustomsExpendituresTruncate = valueCustomsExpenditures.ToAdvanceDecimal(configDec, out configDec); //(decimal)(Math.Truncate(valueCustomsExpenditures * 100) / 100);
            invoice.valueInternationalInsuranceTruncate = valueInternationalInsurance.ToAdvanceDecimal(configDec, out configDec); //(decimal)(Math.Truncate(valueInternationalInsurance * 100) / 100);
            invoice.valueInternationalFreightTruncate = valueInternationalFreight.ToAdvanceDecimal(configDec, out configDec);  //(decimal)(Math.Truncate(valueInternationalFreight * 100) / 100);

            invoice.calculateTotales();
            invoice.calculateTotalesInvoiceExterior();
            // invoice.saveWeight();
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");
            return PartialView("_InvoiceExternalTotales", invoice);
        }

        [HttpPost]
        public ActionResult InvoiceExternalWeight()
        {
            Invoice invoice = ObtainInvoice(0);

            if (invoice != null)
            {
                invoice.saveWeight(db);
                invoice.calculateTotalBoxes();
            }

            // invoice.saveWeight();
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");
            return PartialView("_InvoiceExternalInfoAdicional", invoice);
        }

        #endregion {RA} Invoice Edit

        #region Busqueda

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorResults
            (
               string fullname_businessName,
               string identity,
               string number,
               string motive,
               DateTime? fechaEmisionDesde,
               DateTime? fechaEmisionHasta,
               DateTime? fechaEmbarqueDesde,
               DateTime? fechaEmbarqueHasta,
               int? id_DocumentState,
               int? id_shippingAgency,
               int? id_portDischarge,
               int? id_portDestination)
        {
            parametersSeekInvoiceExterior _parametersSeekInvoiceExterior = new parametersSeekInvoiceExterior();
            _parametersSeekInvoiceExterior.nombreCliente = fullname_businessName;
            _parametersSeekInvoiceExterior.identificacion = identity;
            _parametersSeekInvoiceExterior.invoiceNumber = number;
            _parametersSeekInvoiceExterior.motive = motive;
            _parametersSeekInvoiceExterior.fechaEmisionDesde = fechaEmisionDesde;
            _parametersSeekInvoiceExterior.fechaEmisionHasta = fechaEmisionHasta;
            _parametersSeekInvoiceExterior.fechaEmbarqueDesde = fechaEmbarqueDesde;
            _parametersSeekInvoiceExterior.fechaEmbarqueHasta = fechaEmbarqueHasta;
            _parametersSeekInvoiceExterior.id_DocumentState = id_DocumentState;
            _parametersSeekInvoiceExterior.id_AgenciaNaviera = id_shippingAgency;
            _parametersSeekInvoiceExterior.id_PortDestiny = id_portDestination;
            _parametersSeekInvoiceExterior.id_PortDiscarge = id_portDischarge;

            TempData["parametersSeekInvoiceExterior"] = _parametersSeekInvoiceExterior;
            TempData.Keep("parametersSeekInvoiceExterior");

            List<Invoice> modelFX = null;

            modelFX = SeekInvoiceExterior(_parametersSeekInvoiceExterior);
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return PartialView("_InvoiceExteriorResultsPartial", modelFX.OrderByDescending(o => o.id).ToList());
        }

        #endregion Busqueda

        #region Paginacion

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_invoice)
        {
            TempData.Keep("invoiceExterior");
            TempData.Keep("modelFX");
            TempData.Keep("parametersSeekInvoiceExterior");

            List<Invoice> invoices = (List<Invoice>)TempData["modelFX"];

            int index = invoices.Where(d => d.InvoiceExterior != null).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_invoice);

            var result = new
            {
                maximunPages = invoices.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            TempData.Keep("parametersSeekInvoiceExterior");
            List<Invoice> invoices = (List<Invoice>)TempData["modelFX"];

            Invoice invoice = invoices.Where(d => d.InvoiceExterior != null).OrderByDescending(p => p.id).Take(page).ToList().Last();
            if (invoice != null)
            {
                TempData["invoiceExterior"] = invoice;
            }
            invoice = (invoice == null) ? new Invoice() : invoice;

            invoice.calculateTotales();
            invoice.calculateTotalesInvoiceExterior();
            invoice.ViewWeight();

            TempData.Keep("invoiceExterior");
            TempData.Keep("modelFX");

            return PartialView("_InvoiceExteriorMainFormPartial", invoice);
        }

        #endregion Paginacion

        #region InvoiceDetail

        [ValidateInput(false)]
        public ActionResult InvoiceExteriorDetail(int? id_invoice)
        {
            Invoice invoice = ObtainInvoice(id_invoice);

            var model = invoice.InvoiceDetail?.ToList() ?? new List<InvoiceDetail>();

            ExludeItemByEditRow(Request.Params["__DXCallbackArgument"], model, invoice);

            TempData["invoiceExterior"] = TempData["invoiceExterior"] ?? invoice;
            TempData.Keep("invoiceExterior");

            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            model = invoice.InvoiceDetail?.Where(r => r.isActive).ToList();

            ViewBag.DocumentOrigen = invoice.Document.id_documentOrigen;
            ViewBag.IsManual = (invoice.Document?.Document2?.number == null);

            return PartialView("_InvoiceExteriorMainFormTabDetailsProduct", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorDetailAddNew(int? id_invoice, InvoiceDetail invoiceDetail)
        {
            Invoice invoice = ObtainInvoice(id_invoice);

            var aInvoiceDetail = invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == invoiceDetail.id_item);

            amoutInfoTransmit amountData = ((amoutInfoTransmit)TempData["amountDetail"]) ?? CalculaCantidadInt(invoiceDetail.id_item, (int)invoiceDetail.numBoxes, (int)invoiceDetail.id_metricUnitInvoiceDetail);

            List<InvoiceDetail> model = new List<InvoiceDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    if (id_invoice.HasValue)
                    {
                        int? id_inventoryMove = validateIfHasValidate(id_invoice.Value);
                        if (id_inventoryMove.HasValue)
                        {
                            string messageValidate = validateIfInvoiceInventoryMove(id_inventoryMove.Value, new InvoiceDetail[]
                            {
                                invoiceDetail
                            });

                            if (!string.IsNullOrEmpty(messageValidate))
                            {
                                throw new Exception(messageValidate);
                            }
                        }
                    }
                    
                    if (aInvoiceDetail == null)
                    {
                        Random rnd = new Random();

                        int? newId = rnd.Next(-9999999, -999);

                        invoiceDetail.addNew((int)newId, ActiveUser);
                        invoiceDetail.amount = amountData.cantidadItem;
                        invoiceDetail.id_amountInvoice = amountData.cantidadFactura;
                        invoiceDetail.amountproforma = amountData.cantidadProforma;
                        invoiceDetail.id_metricUnitInvoiceDetail = (invoiceDetail.id_metricUnitInvoiceDetail == 999) ? invoiceDetail.id_metricUnit : invoiceDetail.id_metricUnitInvoiceDetail;

                        List<InvoiceDetail> _invoiceDetailList = new List<InvoiceDetail>();
                        _invoiceDetailList.Add(invoiceDetail);
                        invoice.addBulkDetail(_invoiceDetailList, ActiveUser);
                    }
                    else
                    {
                        aInvoiceDetail.isActive = true;
                        model = InvoiceDetailUpdateDelete(invoice, aInvoiceDetail.id, false, invoiceDetail.discount, amountData);
                    }

                    invoice.calculateTotales();
                    invoice.calculateTotalesInvoiceExterior();
                    invoice.GetTariffHeadingDescription();

                    TempData["id_Items"] = invoice.getId_Items(null);
                    TempData.Keep("id_Items");

                    TempData["invoiceExterior"] = invoice;

                    TempData.Remove("amountDetail");
                }
                catch (Exception e)
                {
                    TempData.Keep("amountDetail");
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    TempData.Keep("invoiceExterior");
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            if (aInvoiceDetail == null)
            {
                model = invoice.InvoiceDetail.Where(r => r.isActive)?.ToList() ?? new List<InvoiceDetail>();
            }
            ViewBag.IsManual = (invoice.Document?.Document2?.number == null);

            return PartialView("_InvoiceExteriorMainFormTabDetailsProduct", model);
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorDetailUpdate(InvoiceDetail invoiceDetail)
        {
            List<InvoiceDetail> model = new List<InvoiceDetail>();
            amoutInfoTransmit amountData = ((amoutInfoTransmit)TempData["amountDetail"]) ?? CalculaCantidadInt(invoiceDetail.id_item, (int)invoiceDetail.numBoxes, (int)invoiceDetail.id_metricUnitInvoiceDetail);
            Invoice invoice = ObtainInvoice(invoiceDetail.id_invoice);
            try
            {
                int? id_inventoryMove = validateIfHasValidate(invoiceDetail.id_invoice);
                if (id_inventoryMove.HasValue)
                {
                    string messageValidate = validateIfInvoiceInventoryMove(id_inventoryMove.Value, new InvoiceDetail[]
                    {
                       invoiceDetail
                    });

                    if (!string.IsNullOrEmpty(messageValidate))
                    {
                        throw new Exception(messageValidate);
                    }
                }

                model = InvoiceDetailUpdateDelete(invoice, invoiceDetail.id, false, invoiceDetail.discount, amountData);

                invoice.calculateTotales();
                invoice.calculateTotalesInvoiceExterior();
                invoice.GetTariffHeadingDescription();

                TempData["invoiceExterior"] = invoice;
                TempData["id_Items"] = invoice.getId_Items(null);
                TempData.Keep("id_Items");
                TempData.Remove("amountDetail");
            }
            catch (Exception e)
            {
                TempData.Keep("amountDetail");
                ViewData["EditError"] = e.Message;
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            ViewBag.IsManual = (invoice.Document?.Document2?.number == null);

            return PartialView("_InvoiceExteriorMainFormTabDetailsProduct", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceExteriorDetailDelete(int? id_invoice, int id)
        {
            List<InvoiceDetail> model = new List<InvoiceDetail>();
            Invoice invoice = ObtainInvoice(id_invoice);
            try
            {

                model = InvoiceDetailUpdateDelete(invoice, id, true, 0, null);
                invoice.calculateTotales();
                invoice.calculateTotalesInvoiceExterior();

                TempData["invoiceExterior"] = invoice;
                TempData.Keep("invoiceExterior");

                TempData["id_Items"] = invoice.getId_Items(null);
                TempData.Keep("id_Items");
            }
            catch (Exception e)
            {
                TempData.Keep("amountDetail");
                ViewData["EditError"] = e.Message;
            }

            ViewBag.IsManual = (invoice.Document?.Document2?.number == null);

            return PartialView("_InvoiceExteriorMainFormTabDetailsProduct", model);
        }

        private List<InvoiceDetail> InvoiceDetailUpdateDelete(Invoice invoice,
            int id_invoiceDetail, Boolean isDelete, decimal discount, amoutInfoTransmit amountData = null)
        {
            if (ModelState.IsValid && invoice != null)
            {
                try
                {
                    var modelinvoiceDetail = invoice.InvoiceDetail.FirstOrDefault(i => i.id == id_invoiceDetail);
                    if (modelinvoiceDetail != null)
                    {
                        modelinvoiceDetail.id_userUpdate = ActiveUser.id;
                        modelinvoiceDetail.dateUpdate = DateTime.Now;
                        if (amountData != null)
                        {
                            modelinvoiceDetail.amount = amountData.cantidadItem;
                            modelinvoiceDetail.discount = discount;
                            modelinvoiceDetail.id_amountInvoice = amountData.cantidadFactura;
                            modelinvoiceDetail.amountproforma = amountData.cantidadProforma;
                            modelinvoiceDetail.id_metricUnitInvoiceDetail = (modelinvoiceDetail.id_metricUnitInvoiceDetail == 999) ? modelinvoiceDetail.id_metricUnit : modelinvoiceDetail.id_metricUnitInvoiceDetail;
                        }

                        if (isDelete) modelinvoiceDetail.isActive = false;
                        this.UpdateModel(modelinvoiceDetail);
                    }

                    TempData["invoiceExterior"] = invoice;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("invoiceExterior");
            TempData.Keep("id_Items");

            List<InvoiceDetail> model = invoice.InvoiceDetail?.Where(x => x.isActive).ToList() ?? new List<InvoiceDetail>();
            model = (model.Count() != 0) ? model : new List<InvoiceDetail>();

            return model;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemCombobox(int? id_itemCurrent)
        {
            MVCxColumnComboBoxProperties p;
            Invoice invoice = ObtainInvoice(null);
            if (!invoice.Document.id_documentOrigen.HasValue)
            {
                int? id_company = (int?)ViewData["id_company"];
                List<int> id_items = (List<int>)ViewData["id_items"];

                p = CreateComboBoxColumnProperties(id_company, id_items, null);
            }
            else
            {
                var aItems = new List<Item>();
                var aId_documentOrigen = invoice.Document.id_documentOrigen;
                var document = db.Document.FirstOrDefault(e => e.id == aId_documentOrigen);
                if (document.DocumentType.code == "131") // Si el documento fue creado a partir de una proforma
                {
                    aItems = document.Invoice.InvoiceDetail
                        .Where(w => w.id_item == id_itemCurrent || (w.isActive && w.proformaPendingNumBoxes > 0 &&
                                invoice.InvoiceDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == w.id_item) == null))
                        .Select(s => s.Item1).Distinct()
                        .ToList();
                }
                else if (document.DocumentType.code == "70")// Si el documento fue creado a partir de una factura comercial
                {
                    aItems = document.InvoiceCommercial.InvoiceCommercialDetail
                        .Where(w => w.id_item == id_itemCurrent || (w.isActive &&
                                invoice.InvoiceDetail.FirstOrDefault(fod => fod.isActive && fod.id_item == w.id_item) == null))
                        .Select(s => s.Item1).Distinct()
                        .ToList();
                }

                p = CreateComboBoxColumnProperties(null, null, aItems);
            }

            TempData.Keep("amountDetail");
            TempData.Keep("invoiceExterior");
            TempData.Keep("id_Items");
            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        public static MVCxColumnComboBoxProperties CreateComboBoxColumnProperties(int? id_company, List<int> id_items, List<Item> aItems)
        {
            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "InvoiceExterior", Action = "LoadItemCombobox" };
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
            p.ClientSideEvents.SelectedIndexChanged = "ItemCombo_SelectedIndexChanged";
            p.ClientSideEvents.BeginCallback = "ItemCombo_OnBeginCallback";
            p.ClientSideEvents.EndCallback = "ItemCombo_OnEndCallback";
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

        [HttpPost, ValidateInput(false)]
        public void InvoiceExteriorDetailsDeleteSeleted(int[] ids)
        {
            Invoice invoice = ObtainInvoice(0);

            if (ids != null)
            {
                try
                {
                    var invoiceDetail = invoice.InvoiceDetail.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in invoiceDetail)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["invoiceExterior"] = invoice;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("invoiceExterior");
        }

        #endregion InvoiceDetail

        #region Clases Aux.

        public class amoutInfoTransmit
        {
            public bool isError { get; set; }
            public string ErrorMessage { get; set; }
            public decimal cantidadItem { get; set; }
            public decimal cantidadFactura { get; set; }
            public decimal cantidadDescuento { get; set; }
            public string cantidadDisplay { get; set; }
            public string cantidadInvoiceDisplay { get; set; }
            public decimal cantidadProforma { get; set; }
            public string cantidadProformaDisplay { get; set; }
            public decimal totalProforma { get; set; }
        }

        private class parametersSeekInvoiceExterior
        {
            public string nombreCliente { get; set; }
            public string identificacion { get; set; }
            public string invoiceNumber { get; set; }
            public string motive { get; set; }
            public DateTime? fechaEmisionDesde { get; set; }
            public DateTime? fechaEmisionHasta { get; set; }
            public DateTime? fechaEmbarqueDesde { get; set; }
            public DateTime? fechaEmbarqueHasta { get; set; }

            public int? id_DocumentState { get; set; }
            public int? id_AgenciaNaviera { get; set; }
            public int? id_PortDiscarge { get; set; }
            public int? id_PortDestiny { get; set; }
        }

        #endregion Clases Aux.

        #region Auxiliar

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateWithSalesQuotationExterior(int? id_item)
        {
            string smensaje = "OK";
            var aInvoice = ObtainInvoice(null);
            var aId_documentOrigen = aInvoice?.Document.id_documentOrigen;
            var documentoOrigen = db.Document.FirstOrDefault(e => e.id == aId_documentOrigen && e.DocumentState.code != "05");

            int numBoxes_Aux;
            decimal proformaWeight_Aux, unitPrice_Aux, totalProforma_Aux, amount, id_amountInvoice, total,
                    totalPriceWithoutTax, discount;
            if (documentoOrigen.DocumentType.code == "131")
            {
                var aId_item = documentoOrigen
                    .Invoice.InvoiceDetail
                    .FirstOrDefault(fod => fod.id_item == id_item && fod.isActive);

                amount = (aId_item?.amount ?? 0);
                id_amountInvoice = (aId_item?.id_amountInvoice ?? 0);
                numBoxes_Aux = (aId_item?.proformaPendingNumBoxes ?? 0);
                proformaWeight_Aux = aId_item?.proformaWeight ?? 0;
                unitPrice_Aux = aId_item?.unitPrice ?? 0;
                totalProforma_Aux = (numBoxes_Aux * proformaWeight_Aux) * unitPrice_Aux;
                total = aId_item?.total ?? 0;
                totalPriceWithoutTax = aId_item?.totalPriceWithoutTax ?? 0;
                discount = aId_item?.discount ?? 0;
            }
            else if (documentoOrigen.DocumentType.code == "70")
            {
                var proformaFacturaComercial = db.Document
                    .FirstOrDefault(e => e.id == documentoOrigen.id_documentOrigen
                         && e.DocumentState.code != "05");

                if (proformaFacturaComercial != null)
                {
                    var aId_item = proformaFacturaComercial
                        .Invoice.InvoiceDetail
                        .FirstOrDefault(fod => fod.id_item == id_item && fod.isActive);

                    amount = (aId_item?.amount ?? 0);
                    id_amountInvoice = (aId_item?.id_amountInvoice ?? 0);
                    numBoxes_Aux = (aId_item?.proformaPendingNumBoxes ?? 0);
                    proformaWeight_Aux = aId_item?.proformaWeight ?? 0;
                    unitPrice_Aux = aId_item?.unitPrice ?? 0;
                    totalProforma_Aux = (numBoxes_Aux * proformaWeight_Aux) * unitPrice_Aux;
                    total = aId_item?.total ?? 0;
                    totalPriceWithoutTax = aId_item?.totalPriceWithoutTax ?? 0;
                    discount = aId_item?.discount ?? 0;
                }
                else
                {
                    var aId_item = documentoOrigen
                        .InvoiceCommercial.InvoiceCommercialDetail
                        .FirstOrDefault(fod => fod.id_item == id_item && fod.isActive);

                    amount = (aId_item?.amount ?? 0);
                    id_amountInvoice = (aId_item?.amount ?? 0);
                    numBoxes_Aux = 0;
                    totalPriceWithoutTax = discount = proformaWeight_Aux = 0;
                    unitPrice_Aux = aId_item?.unitPrice ?? 0;
                    totalProforma_Aux = (numBoxes_Aux * proformaWeight_Aux) * unitPrice_Aux;
                    total = aId_item?.total ?? 0;
                }
            }
            else
            {
                numBoxes_Aux = 0;
                proformaWeight_Aux = unitPrice_Aux = totalProforma_Aux = amount = id_amountInvoice = total = totalPriceWithoutTax = discount = 0m;
            }

            var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);

            TempData.Keep("invoiceExterior");

            var result = new
            {
                numBoxes = numBoxes_Aux,
                amount = amount,
                id_amountInvoice = id_amountInvoice,
                total = total, 
                totalPriceWithoutTax = totalPriceWithoutTax,
                proformaWeight = proformaWeight_Aux,
                unitPrice = unitPrice_Aux,
                totalProforma = totalProforma_Aux,
                discount = discount,
                mensaje = smensaje
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Invoice ObtainInvoice(int? id_invoice)
        {
            Invoice invoice = (TempData["invoiceExterior"] as Invoice);

            invoice = invoice ?? db.Invoice.FirstOrDefault(i => i.id == id_invoice);
            invoice = invoice ?? new Invoice();

            TempData["invoiceExterior"] = invoice;
            return invoice;
        }

        private void ExludeItemByEditRow(string RequestArgument, List<InvoiceDetail> invoiceDetail, Invoice invoice)
        {
            if (!RequestArgument.Contains("STARTEDIT"))
            {
                TempData["id_Items"] = invoice.getId_Items(null);
                return;
            }

            try
            {
                string partialArgument = RequestArgument.Split(';')[3];
                string _estatusEdit = partialArgument.Split('|')[1];
                if (!_estatusEdit.Contains("STARTEDIT")) return;

                string _rowEdit = partialArgument.Split('|')[2];
                int rowEdit = int.Parse(_rowEdit);
                int _id_item = invoiceDetail.FirstOrDefault(r => r.id == rowEdit).id_item;
                TempData["id_Items"] = invoice.getId_Items(_id_item);
            }
            catch
            {
                TempData.Keep("id_Items");
            }
        }

        private List<Invoice> SeekInvoiceExterior(parametersSeekInvoiceExterior parameterToSeek)
        {
            int countParameters = 0;
            List<Invoice> modelFX = null;

            if (!string.IsNullOrEmpty(parameterToSeek.nombreCliente))
            {
                countParameters++;
                if (!string.IsNullOrEmpty(parameterToSeek.nombreCliente)) modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.Person.fullname_businessName.Contains(parameterToSeek.nombreCliente)).ToList() : modelFX.Where(r => r.Person.fullname_businessName.Contains(parameterToSeek.nombreCliente)).ToList();
            }

            if (!string.IsNullOrEmpty(parameterToSeek.identificacion))
            {
                countParameters++;
                if (!string.IsNullOrEmpty(parameterToSeek.identificacion)) modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.Person.identification_number.Contains(parameterToSeek.identificacion)).ToList() : modelFX.Where(r => r.Person.identification_number.Contains(parameterToSeek.identificacion)).ToList();
            }

            if (parameterToSeek.id_DocumentState != null && parameterToSeek.id_DocumentState != 0)
            {
                countParameters++;
                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.Document.id_documentState == parameterToSeek.id_DocumentState).ToList() : modelFX.Where(r => r.Document.id_documentState == parameterToSeek.id_DocumentState).ToList();
            }

            if (!string.IsNullOrEmpty(parameterToSeek.invoiceNumber))
            {
                countParameters++;
                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.Document.number.Contains(parameterToSeek.invoiceNumber)).ToList()
                                             : modelFX.Where(r => r.Document.number.Contains(parameterToSeek.invoiceNumber)).ToList();
            }

            if (!string.IsNullOrEmpty(parameterToSeek.motive))
            {
                countParameters++;
                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.dismissalreason.Contains(parameterToSeek.motive)).ToList()
                                             : modelFX.Where(r => r.InvoiceExterior.dismissalreason.Contains(parameterToSeek.motive)).ToList();
            }

            if (parameterToSeek.id_AgenciaNaviera != null && parameterToSeek.id_AgenciaNaviera != 0)
            {
                countParameters++;
                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null
                                                                   && r.InvoiceExterior.id_shippingAgency == parameterToSeek.id_AgenciaNaviera
                                                               ).ToList()
                                                               : modelFX.Where(r => r.InvoiceExterior.id_shippingAgency == parameterToSeek.id_AgenciaNaviera).ToList();
            }

            if (parameterToSeek.id_PortDestiny != null && parameterToSeek.id_PortDestiny != 0)
            {
                countParameters++;

                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.id_portDestination == parameterToSeek.id_PortDestiny).ToList() : modelFX.Where(r => r.InvoiceExterior.id_portDestination == parameterToSeek.id_PortDestiny).ToList();
            }

            if (parameterToSeek.id_PortDiscarge != null && parameterToSeek.id_PortDiscarge != 0)
            {
                countParameters++;
                modelFX = (modelFX == null || modelFX.Count() == 0) ? db.Invoice.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.id_portDischarge == parameterToSeek.id_PortDiscarge).ToList()
                                            : modelFX.Where(r => r.InvoiceExterior.id_portDischarge == parameterToSeek.id_PortDiscarge).ToList();
            }

            if (parameterToSeek.fechaEmisionDesde != null)
            {
                countParameters++;
                if (modelFX == null || modelFX.Count() == 0)
                {
                    List<Invoice> modelFX_Date = db.Invoice.Where(r => r.InvoiceExterior != null && r.Document.emissionDate != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(parameterToSeek.fechaEmisionDesde.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
                }
                else
                {
                    List<Invoice> modelFX_Date = modelFX.Where(r => r.InvoiceExterior != null && r.Document.emissionDate != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(parameterToSeek.fechaEmisionDesde.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
                }
            }

            if (parameterToSeek.fechaEmisionHasta != null)
            {
                countParameters++;
                if (modelFX == null || modelFX.Count() == 0)
                {
                    List<Invoice> modelFX_Date = db.Invoice.Where(r => r.InvoiceExterior != null && r.Document.emissionDate != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(o.Document.emissionDate.Date, parameterToSeek.fechaEmisionHasta.Value.Date) <= 0).ToList();
                }
                else
                {
                    List<Invoice> modelFX_Date = modelFX.Where(r => r.InvoiceExterior != null && r.Document.emissionDate != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(o.Document.emissionDate.Date, parameterToSeek.fechaEmisionHasta.Value.Date) <= 0).ToList();
                }
            }

            if (parameterToSeek.fechaEmbarqueDesde != null)
            {
                countParameters++;
                DateTime dateTimeNull = DateTime.Parse("01/01/1900");

                if (modelFX == null || modelFX.Count() == 0)
                {
                    List<Invoice> modelFX_Date = db.Invoice.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.dateShipment != null).ToList();
                    modelFX = modelFX_Date
                                .Where(o => DateTime.Compare(parameterToSeek.fechaEmbarqueDesde.Value.Date, o.InvoiceExterior.dateShipment.Value.Date) <= 0)
                                .ToList();
                }
                else
                {
                    List<Invoice> modelFX_Date = modelFX.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.dateShipment != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(parameterToSeek.fechaEmbarqueDesde.Value.Date, o.InvoiceExterior.dateShipment.Value.Date) <= 0)
                                                                  .ToList();
                }
            }

            if (parameterToSeek.fechaEmbarqueHasta != null)
            {
                countParameters++;
                if (modelFX == null || modelFX.Count() == 0)
                {
                    modelFX = db.Invoice.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.dateShipment != null).ToList();
                    modelFX = modelFX.Where(o => DateTime.Compare(o.InvoiceExterior.dateShipment.Value.Date, parameterToSeek.fechaEmbarqueHasta.Value.Date) <= 0).ToList();
                }
                else
                {
                    List<Invoice> modelFX_Date = modelFX.Where(r => r.InvoiceExterior != null && r.InvoiceExterior.dateShipment != null).ToList();
                    modelFX = modelFX_Date.Where(o => DateTime.Compare(o.InvoiceExterior.dateShipment.Value.Date, parameterToSeek.fechaEmbarqueHasta.Value.Date) <= 0).ToList();
                }
            }

            if ((modelFX == null || modelFX.Count() == 0) && countParameters == 0)
            {
                modelFX = db.Invoice.Where(r => r.InvoiceExterior != null).ToList();
            }
            else if ((modelFX == null || modelFX.Count() == 0) && countParameters > 0)
            {
                modelFX = new List<Invoice>();
            }

            return modelFX;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetInfoBank(int id_BankTransfer)
        {
            TempData.Keep("invoiceExterior");
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
                TempData.Keep("invoiceExterior");
            }

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }

        #endregion Auxiliar

        #region Ajax-Request

        [HttpPost]
        public JsonResult CalculaCantidad(int id_item, int numCajas, int id_metricUnitInvoice)
        {
            var JsonReturn = CalculaCantidadInt(id_item, numCajas, id_metricUnitInvoice);

            TempData["amountDetail"] = JsonReturn;
            TempData.Keep("amountDetail");
            TempData.Keep("invoiceExterior");
            TempData.Keep("id_Items");

            return Json(JsonReturn, JsonRequestBehavior.AllowGet);
        }

        private amoutInfoTransmit CalculaCantidadInt(int id_item,
                                                      int numCajas,
                                                      int id_metricUnitInvoice)
        {
            TempData.Keep("invoiceExterior");
            amoutInfoTransmit CalculaCantidadIntReturn = null;
            decimal _cantidad = 0;
            decimal _cantidadInvoice = 0;
            string _cantidadDisplay = "";
            string _cantidadInvoiceDisplay = "";
            decimal _cantidadProforma = 0;
            string _cantidadProformaDisplay = "";
            decimal _totalProforma = 0;

            CalculaCantidadIntReturn = new amoutInfoTransmit
            {
                isError = true,
                ErrorMessage = "",
                cantidadItem = _cantidad,
                cantidadFactura = _cantidadInvoice,
                cantidadDisplay = _cantidadDisplay,
                cantidadInvoiceDisplay = _cantidadInvoiceDisplay,
                cantidadProforma = _cantidadProforma,
                cantidadProformaDisplay = _cantidadProformaDisplay,
                totalProforma = _totalProforma,
            };

            Invoice invoice = (TempData["invoiceExterior"] as Invoice);
            invoice = invoice ?? new Invoice();

            try
            {
                InvoiceDetail invoiceDetail = invoice
                                                .InvoiceDetail
                                                .FirstOrDefault(r => r.id_item == id_item);

                if (invoiceDetail == null)
                {
                    invoiceDetail = new InvoiceDetail();
                    invoiceDetail.id_item = id_item;
                }

                MetricUnit _metricUnitOrigen = db.ItemWeightConversionFreezen.FirstOrDefault(r => r.id_Item == id_item).MetricUnit;
                MetricUnit _metricUnitDestino = new MetricUnit();

                if (id_metricUnitInvoice == 999)
                {
                    id_metricUnitInvoice = _metricUnitOrigen.id;
                }

                _metricUnitDestino = db.MetricUnit.FirstOrDefault(r => r.id == id_metricUnitInvoice);

                invoiceDetail.hasGlaze_DetailOperation = false;
                invoiceDetail.numBoxes = Convert.ToInt32(numCajas);

                var aId_documentOrigen = invoice?.Document?.id_documentOrigen;

                var documentOrigin = db.Document.FirstOrDefault(e => e.id == aId_documentOrigen);

                InvoiceDetail aId_item = null;
                if (documentOrigin?.DocumentType?.code == "131")
                {
                    var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                    aId_item = proforma?.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == id_item && fod.isActive);
                }
                else if (documentOrigin?.DocumentType?.code == "70")
                {
                    var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == documentOrigin.id_documentOrigen);
                    aId_item = proforma?.Invoice.InvoiceDetail.FirstOrDefault(fod => fod.id_item == id_item && fod.isActive);
                }

                var unitPrice_Aux = aId_item?.unitPrice ?? 0;
                invoiceDetail.unitPrice = unitPrice_Aux;
                invoiceDetail.proformaWeight = aId_item?.proformaWeight ?? 0;
                invoiceDetail.id_MetricUnitInvoice_DetailOperation = id_metricUnitInvoice;

                _totalProforma = (numCajas * invoiceDetail.proformaWeight.Value) * unitPrice_Aux;// - (aId_item?.discount ?? 0);

                invoiceDetail.CalculateDetailInvoiceCommercialDetail();

                _cantidadDisplay = invoiceDetail.pesoBasic_DetailOperation.ToString("N2") + " " + _metricUnitOrigen.code;
                _cantidadInvoiceDisplay = invoiceDetail.pesoProformaTotal_DetailOperation.ToString("N2") + " " + _metricUnitDestino.code;
                _cantidadProformaDisplay = invoiceDetail.pesoProformaTotal_DetailOperation.ToString("N2") + " " + _metricUnitDestino.code;

                CalculaCantidadIntReturn.isError = false;
                CalculaCantidadIntReturn.ErrorMessage = "";
                CalculaCantidadIntReturn.cantidadItem = invoiceDetail.pesoBasic_DetailOperation;
                if(documentOrigin != null)
                {
                    CalculaCantidadIntReturn.cantidadFactura = invoiceDetail.pesoTotal_DetailOperation > 0 ? invoiceDetail.pesoProformaTotal_DetailOperation : (decimal)(invoiceDetail.numBoxes ?? 0.00M);
                }
                else
                {
                    CalculaCantidadIntReturn.cantidadFactura = invoiceDetail.pesoTotal_DetailOperation > 0 ? invoiceDetail.pesoTotal_DetailOperation : (decimal)(invoiceDetail.numBoxes ?? 0.00M);
                }
                CalculaCantidadIntReturn.cantidadDisplay = _cantidadDisplay;
                CalculaCantidadIntReturn.cantidadInvoiceDisplay = _cantidadInvoiceDisplay;
                CalculaCantidadIntReturn.cantidadProforma = invoiceDetail.pesoProformaTotal_DetailOperation;
                CalculaCantidadIntReturn.cantidadProformaDisplay = _cantidadProformaDisplay;
                CalculaCantidadIntReturn.totalProforma = _totalProforma;
            }
            catch
            {
            }

            return CalculaCantidadIntReturn;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SetPaymentMethod(int? id_invoice, int? id_paymentMethod)
        {
            Invoice invoice = null;
            PaymentMethod paymentMethod = null;
            var jresult = new { error = true, msgerr = "" };

            try
            {
                invoice = ObtainInvoice(id_invoice);
                if (id_paymentMethod != 0)
                {
                    paymentMethod = db.PaymentMethod.FirstOrDefault(r => r.id == id_paymentMethod);

                    invoice.InvoiceExterior.PaymentMethod = paymentMethod;
                    invoice.InvoiceExterior.id_PaymentMethod = (int)id_paymentMethod;
                }
                else
                {
                    invoice.InvoiceExterior.PaymentMethod = null;
                    invoice.InvoiceExterior.id_PaymentMethod = 0;
                }

                jresult = new { error = false, msgerr = "" };
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
                TempData["invoiceExterior"] = invoice;
                TempData.Keep("invoiceExterior");
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SetShippingAgency(int? id_invoice, int? id_shippingAgency)
        {
            Invoice invoice = null;
            ShippingAgency shippingAgency = null;
            var jresult = new { error = true, msgerr = "" };

            try
            {
                invoice = ObtainInvoice(id_invoice);
                if (id_shippingAgency != 0)
                {
                    shippingAgency = db.ShippingAgency.FirstOrDefault(r => r.id == id_shippingAgency);

                    invoice.InvoiceExterior.ShippingAgency = shippingAgency;
                    invoice.InvoiceExterior.id_shippingAgency = (int)id_shippingAgency;
                }
                else
                {
                    invoice.InvoiceExterior.ShippingAgency = null;
                    invoice.InvoiceExterior.id_shippingAgency = 0;
                }

                jresult = new { error = false, msgerr = "" };
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
                TempData["invoiceExterior"] = invoice;
                TempData.Keep("invoiceExterior");
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var aInvoice = ObtainInvoice(id);

            var actions = new
            {
                btnSave = true,
                btnApprovePartial = true,
                btnApprove = true,
                btnAutorize = false,
                btnCheckAutorizeRSI = false,
                btnCancel = false,
                btnPrint = true,
                btnPrintPartial = true,
                btnRevert = false,
                btnDesvincular = false,
                btnExportExcel = true,
                btnRecalculatePrices = aInvoice?.Document.id_documentOrigen != null,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            string state = invoice.Document.DocumentState.code;

            var aId_documentOrigen = invoice?.Document.id_documentOrigen;

            if (state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnSave = true,
                    btnApprovePartial = true,
                    btnApprove = true,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnCancel = true,
                    btnPrint = true,
                    btnPrintPartial = true,
                    btnRevert = false,
                    btnDesvincular = true,
                    btnExportExcel = true,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            else if (state == "02") // APROBADA PARCIAL
            {
                actions = new
                {
                    btnSave = true,
                    btnApprovePartial = false,
                    btnApprove = true,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnCancel = true,
                    btnPrint = true,
                    btnPrintPartial = true,
                    btnRevert = true,
                    btnDesvincular = true,
                    btnExportExcel = true,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            else if (state == "03") // APROBADA
            {
                actions = new
                {
                    btnSave = false,
                    btnApprovePartial = false,
                    btnApprove = false,
                    btnAutorize = true,
                    btnCheckAutorizeRSI = false,
                    btnCancel = true,
                    btnPrint = true,
                    btnPrintPartial = true,
                    btnRevert = false,
                    btnDesvincular = true,
                    btnExportExcel = true,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            else if (state == "05") // CERRADA O ANULADA
            {
                actions = new
                {
                    btnSave = false,
                    btnApprovePartial = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnCancel = false,
                    btnPrint = false,
                    btnPrintPartial = false,
                    btnRevert = false,
                    btnDesvincular = false,
                    btnExportExcel = false,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            else if (state == "06") // AUTORIZADA o Pre Autorizada
            {
                actions = new
                {
                    btnSave = false,
                    btnApprovePartial = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnCancel = false,
                    btnPrint = true,
                    btnPrintPartial = true,
                    btnRevert = false,
                    btnDesvincular = false,
                    btnExportExcel = true,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            else if (state == "09") // AUTORIZADA o Pre Autorizada
            {
                actions = new
                {
                    btnSave = false,
                    btnApprovePartial = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = true,
                    btnCancel = false,
                    btnPrint = true,
                    btnPrintPartial = true,
                    btnRevert = false,
                    btnDesvincular = false,
                    btnExportExcel = true,
                    btnRecalculatePrices = aId_documentOrigen != null
                };
            }
            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeMetricUnitInvoiceMaster(int id_MetricUnitInvoice, string accion)
        {
            var jresult = new { error = false, msgerr = "" };
            Invoice invoice = null;

            try
            {
                invoice = ObtainInvoice(0);

                switch (accion)
                {
                    case "PRICE":
                        invoice.ConversionMetricUnitCorrectUnitPrice(id_MetricUnitInvoice);
                        break;

                    case "TOTAL":
                        invoice.ConversionMetricUnitCorrectTotalValue(id_MetricUnitInvoice);
                        break;
                }

                TempData["invoiceExterior"] = invoice;
                TempData.Keep("invoiceExterior");
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        [ActionName("tariffheadingdesc")]
        public JsonResult GetTariffHeadingDescription(int? id_invoice)
        {
            GenericResultJson oJsonResult = new GenericResultJson();
            string msgXtraInfo = "";

            try
            {
                msgXtraInfo = "Obtener Factura";

                Invoice _invoice = ObtainInvoice(id_invoice);
                if (_invoice == null)
                {
                    throw new Exception("Factura con identificador " + id_invoice + " , No Definido.");
                }

                msgXtraInfo = "Asignar Descripción Partida Arancelaria";
                oJsonResult.codeReturn = 1;
                oJsonResult.ValueDataList = new List<ValueData>();
                oJsonResult.ValueDataList.Add(new ValueData
                {
                    CodeObject = "tariffHeadingDescription",
                    valueObject = _invoice.InvoiceExterior.tariffHeadingDescription
                });
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialConfigControl==>" + msgXtraInfo);
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidAndUpdateDetailFromProforma(int id_metricUnitInvoice)
        {
            var result = new { Message = "OK" };
            Invoice invoice = null;
            try
            {
                invoice = ObtainInvoice(0);
                var aId_documentOrigen = invoice?.Document.id_documentOrigen;
                var nameItem = "";
                var cantItemActualizado = 0;

                SalesQuotationExterior proforma = null;
                var documentOrigin = db.Document.FirstOrDefault(e => e.id == aId_documentOrigen);
                if (documentOrigin?.DocumentType?.code == "131")
                {
                    proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == aId_documentOrigen);
                }
                else if (documentOrigin?.DocumentType?.code == "70")
                {
                    proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == documentOrigin.id_documentOrigen);
                }

                #region Validar que el producto exista en la profroma
                var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                if (puedeModDatosProforma && documentOrigin != null)
                {
                    var detallesProforma = new InvoiceDetail[] { };
                    if (documentOrigin.DocumentType?.code == "131") // Proforma
                    {
                        detallesProforma = db.InvoiceDetail.Where(e => e.id_invoice == documentOrigin.id && e.isActive).ToArray();
                    }
                    else if (documentOrigin.DocumentType?.code == "70") // Factura Comercial
                    {
                        var documentoOrigen2 = db.Document.FirstOrDefault(e => e.id == documentOrigin.id_documentOrigen);
                        if (documentoOrigen2.DocumentType?.code == "131") // Si el origen es proforma se llena
                        {
                            detallesProforma = db.InvoiceDetail.Where(e => e.id_invoice == documentoOrigen2.id && e.isActive).ToArray();
                        }
                    }
                    var aInvoiceDetail = invoice.InvoiceDetail.Where(w => w.isActive).ToList();
                    foreach (var invoiceDetail in aInvoiceDetail)
                    {
                        var detalleProforma = detallesProforma.FirstOrDefault(e => e.id_item == invoiceDetail.id_item);
                        if (detalleProforma == null)
                        {
                            throw new Exception($"El producto {invoiceDetail.Item.name} no existe en la proforma.");
                        }
                        else
                        {
                            if (detalleProforma.numBoxes != invoiceDetail.numBoxes)
                            {
                                throw new Exception($"La cantidad de cartones difiere a la proforma, producto {invoiceDetail.Item.name}.");
                            }
                        }
                    }

                    foreach (var detalleProforma in detallesProforma)
                    {
                        var detalleFactura = aInvoiceDetail.FirstOrDefault(e => e.id_item == detalleProforma.id_item);
                        if (detalleFactura == null)
                        {
                            throw new Exception($"El producto {detalleProforma.Item.name} no existe en la factura.");
                        }
                        
                    }
                }
                #endregion


                if (proforma != null)
                {
                    foreach (var proformaDetail in proforma.Invoice.InvoiceDetail.Where(d => d.isActive))
                    {
                        var invoiceDetail = invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
                        if (invoiceDetail != null)
                        {
                            if (proformaDetail.proformaWeight != invoiceDetail.proformaWeight || proformaDetail.unitPrice != invoiceDetail.unitPriceProforma || proformaDetail.discount != invoiceDetail.discount)
                            {
                                invoiceDetail.proformaWeight = proformaDetail.proformaWeight;
                                invoiceDetail.pesoProformaTotal_DetailOperation = decimal.Round((invoiceDetail.numBoxes ?? 0) * (proformaDetail.proformaWeight ?? 0.00M), 2);
                                invoiceDetail.amountproforma = invoiceDetail.pesoProformaTotal_DetailOperation;
                                MetricUnit _metricUnitOrigen = db.ItemWeightConversionFreezen.FirstOrDefault(r => r.id_Item == proformaDetail.id_item).MetricUnit;
                                MetricUnit _metricUnitDestino = new MetricUnit();
                                if (id_metricUnitInvoice == 999)
                                {
                                    id_metricUnitInvoice = _metricUnitOrigen.id;
                                }
                                _metricUnitDestino = db.MetricUnit.FirstOrDefault(r => r.id == id_metricUnitInvoice);

                                invoiceDetail.amountProformaDisplay = invoiceDetail.pesoProformaTotal_DetailOperation.ToString("N2") + " " + _metricUnitDestino.code;

                                invoiceDetail.unitPriceProforma = proformaDetail.unitPrice;
                                invoiceDetail.totalProforma = decimal.Round(invoiceDetail.unitPriceProforma * invoiceDetail.pesoProformaTotal_DetailOperation, 2);

                                //invoiceDetail.discount = proformaDetail.discount;
                                cantItemActualizado++;
                                if (nameItem == "")
                                {
                                    nameItem = proformaDetail.Item.name;
                                }
                                else
                                {
                                    nameItem += ", " + proformaDetail.Item.name;
                                }
                            }
                        }
                    }
                }
                if (cantItemActualizado > 0)
                {
                    if (cantItemActualizado > 1)
                    {
                        result = new { Message = "Precio, peso o descuento de los ítems " + nameItem + " han sido modificado en la proforma" };
                    }
                    else
                    {
                        result = new { Message = "Precio, peso o descuento del ítem " + nameItem + " ha sido modificado en la proforma" };
                    }
                }
            }
            catch (Exception e)
            {
                result = new { Message = e.Message };
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Ajax-Request

        #region Métodos de actualización de datos en facturas / proformas

        private void UpdateInvoicesCommercial(int id_invoice, int? id_documentoOrigen,
            Invoice invoice, InvoiceExterior invoiceExterior)
        {
            var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
            var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;

            var facturasComercialesDoc = db.Document.Where(e => e.id_documentOrigen == id_invoice &&
                    e.id_documentType == idTipoDocFactComercial && e.id_documentState != idEstadoAnulado)
                .Union(db.Document.Where(e => e.id == id_documentoOrigen &&
                    e.id_documentType == idTipoDocFactComercial && e.id_documentState != idEstadoAnulado))
                .ToList();

            if (facturasComercialesDoc.Any(e => e.id_documentState != idEstadoPendiente))
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

                UpdateInvoiceComercialData(facturaComercialDoc, invoice, invoiceExterior);
            }
        }

        private void UpdateSalesQuotationExterior(int id_invoice,
            Document document, Invoice invoice, InvoiceExterior invoiceExterior)
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

                proformaDoc.Invoice.SalesQuotationExterior.id_consignee = invoiceExterior.id_consignee;  // consignatario
                proformaDoc.Invoice.id_buyer = invoice.id_buyer; // Cliente del exterior
                proformaDoc.Invoice.SalesQuotationExterior.id_notifier = invoiceExterior.id_notifier; // Notificador
                proformaDoc.Invoice.SalesQuotationExterior.purchaseOrder = invoiceExterior.purchaseOrder; // Orden de pedido
                proformaDoc.Invoice.SalesQuotationExterior.transport = invoiceExterior.transport; // Transporte
                proformaDoc.Invoice.SalesQuotationExterior.idVendor = invoiceExterior.idVendor; // Vendedor
                proformaDoc.Invoice.SalesQuotationExterior.id_termsNegotiation = invoiceExterior.id_termsNegotiation; //Termino de Negociación
                proformaDoc.Invoice.SalesQuotationExterior.id_PaymentMethod = invoiceExterior.id_PaymentMethod; // Forma de Pago
                proformaDoc.Invoice.SalesQuotationExterior.id_PaymentTerm = invoiceExterior.id_PaymentTerm; // Plazo de Pago
                proformaDoc.Invoice.SalesQuotationExterior.id_bank = invoiceExterior.id_bank; //Banco Beneficiario
                proformaDoc.Invoice.SalesQuotationExterior.valueSubscribed = invoiceExterior.valueSubscribed; //Banco Beneficiario
                proformaDoc.Invoice.SalesQuotationExterior.dateShipment = invoiceExterior.dateShipment; // Fecha de Embarque
                proformaDoc.Invoice.SalesQuotationExterior.id_portShipment = invoiceExterior.id_portShipment; // Puerto de Embarque
                proformaDoc.Invoice.SalesQuotationExterior.id_portDischarge = invoiceExterior.id_portDischarge; // Puerto Descarga
                proformaDoc.Invoice.SalesQuotationExterior.id_portDestination = invoiceExterior.id_portDestination; //Puerto Destino
                proformaDoc.Invoice.SalesQuotationExterior.temperatureInstruction = invoiceExterior.temperatureInstruction; //Instrucciones de Temperatura
                proformaDoc.Invoice.SalesQuotationExterior.id_BankTransfer = invoiceExterior.id_BankTransfer; //Banco de Transferencia

                proformaDoc.dateUpdate = DateTime.Now;
                proformaDoc.id_userUpdate = this.ActiveUserId;

                db.Document.Attach(proformaDoc);
                db.Entry(proformaDoc).State = EntityState.Modified;

                #region buscamos las facturas fiscales relacionadas a la proforma

                var facturasFiscalesDoc = db.Document
                   .Where(e => e.id_documentOrigen == proformaDoc.id && e.id_documentType == idTipoDocFactFiscal
                        && e.id_documentState != idEstadoAnulado && e.id != invoice.id)
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

                    UpdateInvoiceExteriorData(facturaFiscalDoc, invoice, invoiceExterior);

                    #region Verificamos existencia de una faactura comercial Asociada

                    var facturasComercialesFiscalesDoc = db.Document
                       .Where(e => e.id_documentOrigen == facturaFiscalDoc.id && e.id_documentType == idTipoDocFactComercial
                            && e.id_documentState != idEstadoAnulado);

                    if (facturasComercialesFiscalesDoc != null)
                    {
                        foreach (var facturaComercialDoc in facturasComercialesFiscalesDoc)
                        {
                            UpdateInvoiceComercialData(facturaComercialDoc, invoice, invoiceExterior);
                        }
                    }

                    #endregion
                }

                #endregion

                #region buscamos las facturas comerciales relacionadas a la proforma

                var facturasComercialesDoc = db.Document
                   .Where(e => e.id_documentOrigen == proformaDoc.id && e.id_documentType == idTipoDocFactComercial
                        && e.id_documentState != idEstadoAnulado && e.id != invoice.id)
                   .ToList();

                if (facturasComercialesDoc
                    .Any(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial))
                {
                    var facturaComercialDoc = facturasComercialesDoc
                        .FirstOrDefault(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial);

                    throw new Exception(
                        $"No se pueden actualizar datos en la factura comercial {facturaComercialDoc.number} porque está está {facturaComercialDoc.DocumentState.name}");
                }

                // Actualizamos las facturas comerciales
                foreach (var facturaComercialDoc in facturasComercialesDoc)
                {
                    var invoiceFacComercial = facturaComercialDoc.InvoiceCommercial;
                    if (invoiceFacComercial == null) continue; // Si invoice es null, no actualizamos nada

                    UpdateInvoiceComercialData(facturaComercialDoc, invoice, invoiceExterior);

                    #region Verificamos existencia de una factura fiscal Asociada

                    var facturasFiscalesComercialesDoc = db.Document
                       .Where(e => e.id_documentOrigen == facturaComercialDoc.id && e.id_documentType == idTipoDocFactFiscal
                            && e.id_documentState != idEstadoAnulado);

                    if (facturasFiscalesComercialesDoc != null)
                    {
                        foreach (var facturaFiscalDoc in facturasFiscalesComercialesDoc)
                        {
                            UpdateInvoiceExteriorData(facturaFiscalDoc, invoice, invoiceExterior);
                        }
                    }

                    #endregion
                }

                #endregion
            }
        }

        private void UpdateInvoiceExteriorData(Document facturaFiscalDoc,
            Invoice invoice, InvoiceExterior invoiceExterior)
        {
            facturaFiscalDoc.Invoice.InvoiceExterior.id_consignee = invoiceExterior.id_consignee; // consignatario
            facturaFiscalDoc.Invoice.id_buyer = invoice.id_buyer; // Cliente del exterior
            facturaFiscalDoc.Invoice.InvoiceExterior.id_notifier = invoiceExterior.id_notifier; // Notificador
            facturaFiscalDoc.Invoice.InvoiceExterior.purchaseOrder = invoiceExterior.purchaseOrder; // Orden de pedido
            facturaFiscalDoc.Invoice.InvoiceExterior.transport = invoiceExterior.transport; // Transporte
            facturaFiscalDoc.Invoice.InvoiceExterior.idVendor = invoiceExterior.idVendor; // Vendedor
            facturaFiscalDoc.Invoice.InvoiceExterior.id_termsNegotiation = invoiceExterior.id_termsNegotiation; //Termino de Negociación
            facturaFiscalDoc.Invoice.InvoiceExterior.id_PaymentMethod = invoiceExterior.id_PaymentMethod; // Forma de Pago
            facturaFiscalDoc.Invoice.InvoiceExterior.id_PaymentTerm = invoiceExterior.id_PaymentTerm; // Plazo de Pago
            facturaFiscalDoc.Invoice.InvoiceExterior.id_bank = invoiceExterior.id_bank; //Banco Beneficiario
            facturaFiscalDoc.Invoice.InvoiceExterior.valueSubscribed = invoiceExterior.valueSubscribed; //Banco Beneficiario
            facturaFiscalDoc.Invoice.InvoiceExterior.dateShipment = invoiceExterior.dateShipment; // Fecha de Embarque
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portShipment = invoiceExterior.id_portShipment; // Puerto de Embarque
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portDischarge = invoiceExterior.id_portDischarge; // Puerto Descarga
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portDestination = invoiceExterior.id_portDestination; //Puerto Destino
            facturaFiscalDoc.Invoice.InvoiceExterior.temperatureInstruction = invoiceExterior.temperatureInstruction; //Instrucciones de Temperatura
            facturaFiscalDoc.Invoice.InvoiceExterior.temperatureInstrucDate = invoiceExterior.temperatureInstrucDate; //Fecha Emisión Instrucciones de Temperatura
            facturaFiscalDoc.Invoice.InvoiceExterior.id_BankTransfer = invoiceExterior.id_BankTransfer; //Banco de Transferencia

            facturaFiscalDoc.dateUpdate = DateTime.Now;
            facturaFiscalDoc.id_userUpdate = this.ActiveUserId;

            db.Document.Attach(facturaFiscalDoc);
            db.Entry(facturaFiscalDoc).State = EntityState.Modified;
        }

        private void UpdateInvoiceComercialData(Document facturaComercialDoc,
            Invoice invoice, InvoiceExterior invoiceExterior)
        {
            var invoiceFacComercial = facturaComercialDoc.InvoiceCommercial;
            invoiceFacComercial.id_Consignee = invoiceExterior.id_consignee; // consignatario
            invoiceFacComercial.id_ForeignCustomer = invoice.id_buyer; // Cliente del exterior
            invoiceFacComercial.id_Notifier = invoiceExterior.id_notifier; // Notificador
            invoiceFacComercial.purchaseOrder = invoiceExterior.purchaseOrder; // Orden de pedido
            invoiceFacComercial.idVendor = invoiceExterior.idVendor; // Vendedor
            invoiceFacComercial.id_termsNegotiation = invoiceExterior.id_termsNegotiation;// Término de Negociación
            invoiceFacComercial.idPortfolioFinancing = invoiceExterior.idPortfolioFinancing; //Financiamiento de Cartera
            invoiceFacComercial.id_PaymentMethod = invoiceExterior.id_PaymentMethod; // Forma de Pago
            invoiceFacComercial.id_PaymentTerm = invoiceExterior.id_PaymentTerm; // Plazo de Pago
            invoiceFacComercial.id_BankTransfer = invoiceExterior.id_BankTransfer; // Banco de transferencia
            invoiceFacComercial.dateShipment = invoiceExterior.dateShipment; // Fecha de Embarque
            invoiceFacComercial.id_portShipment = invoiceExterior.id_portShipment; // Puerto de Embarque
            invoiceFacComercial.id_portDischarge = invoiceExterior.id_portDischarge; // Puerto Descarga
            invoiceFacComercial.id_portDestination = invoiceExterior.id_portDestination; //Puerto Destino
            invoiceFacComercial.etaDate = invoiceExterior.etaDate; // Fecha de Arribo y/o Fecha ETA
            invoiceFacComercial.id_shippingAgency = invoiceExterior.id_shippingAgency; //Agencia Naviera
            invoiceFacComercial.id_shippingLine = invoiceExterior.id_ShippingLine;// Línea Naviera
            invoiceFacComercial.shipName = invoiceExterior.shipName; //Buque
            invoiceFacComercial.shipNumberTrip = invoiceExterior.shipNumberTrip; // Viaje

            // Partida arrancelarias
            var id_tariffHeading = db.TariffHeading
                .FirstOrDefault(e => e.code == invoiceExterior.tariffHeadingDescription)?
                .id;
            invoiceFacComercial.id_tariffHeading = id_tariffHeading;

            invoiceFacComercial.BLNumber = invoiceExterior.BLNumber; // Número de BL
            invoiceFacComercial.blDate = invoiceExterior.blDate; // Fecha de BL
            invoiceFacComercial.daeNumber = invoiceExterior.daeNumber; // DAE
            invoiceFacComercial.daeNumber2 = invoiceExterior.daeNumber2;
            invoiceFacComercial.daeNumber3 = invoiceExterior.daeNumber3;
            invoiceFacComercial.daeNumber4 = invoiceExterior.daeNumber4;
            invoiceFacComercial.containers = invoiceExterior.containers;

            invoiceFacComercial.id_CityDelivery = invoiceExterior.id_CityDelivery; // Ciudad de Entrega
            invoiceFacComercial.seals = invoiceExterior.seals; // Sellos

            db.InvoiceCommercial.Attach(invoiceFacComercial);
            db.Entry(invoiceFacComercial).State = EntityState.Modified;
        }

        #endregion Métodos de actualización de datos en facturas / proformas

        #region PRINT REPORT OPTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult PRInvoiceFiscal(string codeReport, DateTime? fechaEmisionDesde,
                DateTime? fechaEmisionHasta,
                DateTime? fechaEmbarqueDesde,
                DateTime? fechaEmbarqueHasta)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            string str_starReceptionDate = "";
            if (fechaEmisionDesde != null) { str_starReceptionDate = fechaEmisionDesde.Value.Date.ToString("yyyy/MM/dd"); }
            _param.Nombre = "@str_startEmissionDate";
            _param.Valor = str_starReceptionDate;
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (fechaEmisionHasta != null) { str_endReceptionDate = fechaEmisionHasta.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_endEmissionDate";
            _param.Valor = str_endReceptionDate;
            paramLst.Add(_param);

            string str_startDateShipment = "";
            if (fechaEmbarqueDesde != null) { str_startDateShipment = fechaEmbarqueDesde.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_startDateShipment";
            _param.Valor = str_startDateShipment;
            paramLst.Add(_param);

            string str_endDateShipment = "";
            if (fechaEmbarqueHasta != null) { str_endDateShipment = fechaEmbarqueHasta.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_endDateShipment";
            _param.Valor = str_endDateShipment;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorReport(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "IEFE1";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorPropioReport(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "IEFEP";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintInvoiceExteriorCertificadaReport(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "FECER";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        //kpi
        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorHeight(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "FACPES";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorISFTempReport(int id, string codeReport)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        //-------------------------------------------------------

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorPakinListReport(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "PAKINL";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintInvoiceExteriorNonWoodReport(int id_inv)
        {
            Invoice invoice = ObtainInvoice(id_inv);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "NONWOD";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InvoiceExteriorExporExcel(int id_invoice)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            //_param.Valor = id_invoice;
            _param.Valor = id_invoice;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "FACPE";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;
            //_repMod.nameReport = "Proforma";

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InvoiceExteriorReportFilterPartial(int id_invoice)
        {
            Invoice invoice = ObtainInvoice(id_invoice);
            TempData["invoiceExterior"] = invoice;
            TempData.Keep("invoiceExterior");

            string callIdentity = ServiceInvoiceExteriorPartial.generateInvoicePartial(db, id_invoice);

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();

            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_invoice;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@callIdentity";
            _param.Valor = callIdentity;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "FEXPAR";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        #endregion PRINT REPORT OPTIONS

        [HttpPost, ValidateInput(false)]
        public PartialViewResult InvoiceExteriorPaymentTermViewDetailsPartial(
            int? idPaymentTerm, DateTime? emissionDate, decimal? invoiceTotal, bool? canEditPaymentTerm,
            List<InvoiceExteriorPaymentTerm> currentPaymentTermDetails)
        {
            ICollection<InvoiceExteriorPaymentTerm> invoiceExteriorPaymentTerm = null;

            try
            {
                // Recuperamos la factura actual, y nos aseguramos de que tiene la lista de plazos
                var invoice = this.ObtainInvoice(0);

                invoiceExteriorPaymentTerm = invoice.InvoiceExteriorPaymentTerm;

                if (invoiceExteriorPaymentTerm == null)
                {
                    invoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>();
                    invoice.InvoiceExteriorPaymentTerm = invoice.InvoiceExteriorPaymentTerm;
                }

                // Removemos los detalles anteriores del elemento
                invoiceExteriorPaymentTerm.Clear();

                // Verificamos el monto de la factura
                var montoTotalFactura = Decimal.Round(
                    invoiceTotal ?? 0m,
                    2,
                    MidpointRounding.AwayFromZero);

                if (montoTotalFactura > 0m)
                {
                    // Recuperamos el objeto con los términos de pago
                    var paymentTerm = (idPaymentTerm.HasValue)
                        ? db.PaymentTerm.FirstOrDefault(t => t.id == idPaymentTerm.Value)
                        : null;

                    if (paymentTerm != null)
                    {
                        var fechaEmision = (emissionDate ?? DateTime.Today).Date;

                        if ((paymentTerm.firstPaymentDays <= 0)
                            || (paymentTerm.PercentAnticipation >= 100m))
                        {
                            // Si solo va a existir un pago, o si el anticipo es 100%,
                            // va un único pago a la vista
                            invoiceExteriorPaymentTerm.Add(new InvoiceExteriorPaymentTerm()
                            {
                                idInvoiceExterior = invoice.id,
                                orderPayment = 0,
                                dueDate = fechaEmision,
                                porcentaje = 100m,
                                valuePayment = montoTotalFactura,
                            });
                        }
                        else if ((paymentTerm.PercentAnticipation > 0m)
                            || (paymentTerm.PercentAnticipation < 100m))
                        {
                            // Va a existir un anticipo y un pago en fecha posterior
                            var montoAnticipo = Decimal.Round(
                                montoTotalFactura * paymentTerm.PercentAnticipation / 100m,
                                2,
                                MidpointRounding.AwayFromZero);
                            var montoSaldoFactura = montoTotalFactura - montoAnticipo;

                            if (montoAnticipo > 0m)
                            {
                                invoiceExteriorPaymentTerm.Add(new InvoiceExteriorPaymentTerm()
                                {
                                    idInvoiceExterior = invoice.id,
                                    orderPayment = 0,
                                    dueDate = fechaEmision,
                                    porcentaje = paymentTerm.PercentAnticipation,
                                    valuePayment = montoAnticipo,
                                });
                            }

                            if (montoSaldoFactura > 0m)
                            {
                                var fechaPagoSaldo = currentPaymentTermDetails?
                                    .FirstOrDefault(d => d.orderPayment == 1)?
                                    .dueDate ?? fechaEmision.AddDays(paymentTerm.firstPaymentDays);

                                invoiceExteriorPaymentTerm.Add(new InvoiceExteriorPaymentTerm()
                                {
                                    idInvoiceExterior = invoice.id,
                                    orderPayment = 1,
                                    dueDate = fechaPagoSaldo,
                                    porcentaje = 100m - paymentTerm.PercentAnticipation,
                                    valuePayment = montoSaldoFactura,
                                });
                            }
                        }
                        else
                        {
                            invoiceExteriorPaymentTerm.Add(new InvoiceExteriorPaymentTerm()
                            {
                                idInvoiceExterior = invoice.id,
                                orderPayment = 1,
                                dueDate = fechaEmision.AddDays(paymentTerm.firstPaymentDays),
                                porcentaje = 100m,
                                valuePayment = montoTotalFactura,
                            });
                        }
                    }
                }
            }
            finally
            {
                TempData.Keep("invoiceExterior");
            }

            this.ViewBag.CanEditPaymentTerm = canEditPaymentTerm;

            return this.PartialView(
                "_InvoiceExteriorMainFormTabInvoiceExteriorPaymentTermDetails",
                invoiceExteriorPaymentTerm);
        }

        private void PrepareInvoiceToSave(Invoice invoice, string jsonPaymentTermDetails)
        {
            // Preparamos los detalles de edición y el documento recibido
            var paymentTermDetails = Newtonsoft.Json.JsonConvert
                .DeserializeObject<InvoiceExteriorPaymentTerm[]>(jsonPaymentTermDetails);
            var numDetallesEdicion = paymentTermDetails?.Length ?? 0;

            // Preparamos los detalles del documento a guardar
            if (invoice.InvoiceExteriorPaymentTerm == null)
            {
                invoice.InvoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>();
            }
            var numDetallesActuales = invoice.InvoiceExteriorPaymentTerm.Count;

            // Preparamos las variables de control para la validción
            var numDetallesProcesar = Math.Max(numDetallesEdicion, numDetallesActuales);
            var minFechaPlazo = invoice.Document.emissionDate.Date;
            var totalDocumento = 0m;

            for (var i = 0; i < numDetallesProcesar; i++)
            {
                if (i < numDetallesEdicion)
                {
                    var detalleEdicion = paymentTermDetails[i];

                    // Validamos la fecha del pago
                    detalleEdicion.dueDate = detalleEdicion.dueDate.Date;

                    if (detalleEdicion.dueDate < minFechaPlazo)
                    {
                        throw new ApplicationException(
                            $"Fecha de plazo de pago para cuota {i + 1} debe ser mayor o igual a {minFechaPlazo:dd/MM/yyyy}.");
                    }
                    minFechaPlazo = detalleEdicion.dueDate.AddDays(1);

                    // Validamos el monto del pago
                    if (detalleEdicion.valuePayment < 0m)
                    {
                        throw new ApplicationException(
                            $"Monto de plazo de pago para cuota {i + 1} debe ser mayor a cero.");
                    }
                    totalDocumento += detalleEdicion.valuePayment;

                    // Agregamos el detalle al documento existente
                    if (i < numDetallesActuales)
                    {
                        // Actualizamos el elemento actual
                        var detalleActual = invoice.InvoiceExteriorPaymentTerm.ElementAt(i);

                        detalleActual.orderPayment = detalleEdicion.orderPayment;
                        detalleActual.dueDate = detalleEdicion.dueDate;
                        detalleActual.porcentaje = detalleEdicion.porcentaje;
                        detalleActual.valuePayment = detalleEdicion.valuePayment;

                        db.Entry(detalleActual).State = EntityState.Modified;
                    }
                    else
                    {
                        // Agregamos un nuevo elemento
                        var detalleActual = new InvoiceExteriorPaymentTerm()
                        {
                            idInvoiceExterior = invoice.id,
                            Invoice = invoice,
                            orderPayment = detalleEdicion.orderPayment,
                            dueDate = detalleEdicion.dueDate,
                            porcentaje = detalleEdicion.porcentaje,
                            valuePayment = detalleEdicion.valuePayment,
                        };

                        invoice.InvoiceExteriorPaymentTerm.Add(detalleActual);

                        db.Entry(detalleActual).State = EntityState.Added;
                    }
                }
                else
                {
                    // Detalle actual sobrante... debe eliminarse
                    var detalleActual = invoice.InvoiceExteriorPaymentTerm.ElementAt(i);
                    invoice.InvoiceExteriorPaymentTerm.Remove(detalleActual);
                    db.Entry(detalleActual).State = EntityState.Deleted;
                }
            }

            // Validar el total del documento
            if (invoice.valuetotalCIFTruncate != totalDocumento)
            {
                throw new ApplicationException(
                    "Total del documento no coincide con la suma de los plazos de pago.");
            }
        }

        #region -- Validacion Movimiento Inventario Relacionado --
        private string validateIfInvoiceInventoryMove(int id_inventorymove, InvoiceDetail[] invoiceDetails)
        {
            // 1- obtener cantidad de los detalles del movimiento, se va asumir que es solo un movimiento
            // Si es solo un movimiento se debe validar en el ingreso del movimiento
            // 2- obtener cantidad de  los detalles de la factura (parametro)
            var inventoryMoveDetails = db.InventoryMoveDetail
                                                    .Include("Item")
                                                    .Where(r => r.id_inventoryMove == id_inventorymove)
                                                    .Select(r => new
                                                    {
                                                        nombre = r.Item.masterCode,
                                                        r.id_item,
                                                        r.exitAmount
                                                    }).ToArray();



            var inventoryMoveDetailsGroupItem = inventoryMoveDetails
                                                        .GroupBy(r => r.id_item)
                                                        .Select(r => new
                                                        {
                                                            item = r.Key,
                                                            cantidad = r.Sum(t => t.exitAmount),
                                                            nombre = r.Max(t => t.nombre)
                                                        })
                                                        .ToArray();

            var invoiceDetailGroupItem = invoiceDetails
                                                .Where(r => r.isActive)
                                                .GroupBy(r => r.id_item)
                                                .Select(r => new
                                                {
                                                    item = r.Key,
                                                    cantidad = r.Sum(t => t.numBoxes)
                                                }).ToArray();

            int[] itemsInvoice = invoiceDetailGroupItem
                                        .Select(r => r.item)
                                        .ToArray();
            var itemsInvoiceData = db.Item
                                        .Where(r => itemsInvoice.Contains(r.id))
                                        .Select(r => new
                                        {
                                            r.id,
                                            nombre = r.masterCode
                                        }).ToArray();

            var invoiceDetailGroupItemWithData = (from invoice in invoiceDetailGroupItem
                                                  join item in itemsInvoiceData
                                                  on invoice.item equals item.id
                                                  select new
                                                  {
                                                      item = invoice.item,
                                                      cantidad = invoice.cantidad,
                                                      nombre = item.nombre

                                                  })
                                                  .ToArray();



            // validar item que esten en la factura y no en el movimiento
            // validar cantidades de facturas si son superiores a las cantidades del movimiento
            var validaFacturaInventario = (from factura in invoiceDetailGroupItemWithData
                                           join inventario in inventoryMoveDetailsGroupItem
                                           on factura.item equals inventario.item
                                           into inv
                                           from definventario in inv.DefaultIfEmpty()
                                           select new
                                           {
                                               item = factura.item,
                                               name = factura.nombre,
                                               existe = (definventario != null),
                                               diferencia = ((definventario == null) ? 0 : definventario.cantidad) - factura.cantidad
                                           });
            //var noExisteInventario = validaFacturaInventario.Where(r => !r.existe).ToArray();
            //if (noExisteInventario.Count() > 0)
            //{
            //    return $"Items: {noExisteInventario.Select(r => r.name).Aggregate((i, j) => $"{i},{j}")}, no estan en el movimiento de inventario relacionado.";
            //}
            var cantidadFacturaMayor = validaFacturaInventario.Where(r => r.diferencia < 0).ToArray();
            if (cantidadFacturaMayor.Count() > 0)
            {
                return $"Items: {cantidadFacturaMayor.Select(r => r.name).Aggregate((i, j) => $"{i},{j}")}, tienen cantidad mayor en la factura que en el movimiento de inventario relacionado.";
            }
            // validar que item este en el movimiento y no en la factura

            //var validaInventarioFactura = (from inventario in inventoryMoveDetailsGroupItem
            //                               join factura in invoiceDetailGroupItemWithData
            //                               on inventario.item equals factura.item
            //                               into fac
            //                               from deffactura in fac.DefaultIfEmpty()
            //                               select new
            //                               {
            //                                   item = inventario.item,
            //                                   name = inventario.nombre,
            //                                   existe = (deffactura != null)
            //                               });
            //var noExisteFactura = validaInventarioFactura.Where(r => !r.existe).ToArray();
            //if (noExisteFactura.Count() > 0)
            //{
            //    return $"Items: {noExisteFactura.Select(r => r.name).Aggregate((i, j) => $"{i},{j}")}, no estan en la factura y si en el movimiento de inventario relacionado.";
            //}

            return null;
        }
        private int? validateIfHasValidate(int id_invoice)
        {
            // validar que el parametro este activo
            // validar que la factura este en un movimiento y este Aprobado
            Setting setting = db.Setting.FirstOrDefault(r => r.code == PARAMETRO_VALIDAFACTURA_INVENTARIO);
            if ((setting?.value ?? "NO") == "SI")
            {
                var inventoryMoveInvoice = db.InventoryMove.FirstOrDefault(r => r.id_Invoice == id_invoice);
                if (inventoryMoveInvoice != null)
                {
                    Document documentInventoryMove = db.Document.Include("DocumentState").FirstOrDefault(r => r.id == inventoryMoveInvoice.id);
                    if (documentInventoryMove != null)
                    {
                        if (documentInventoryMove.DocumentState.code == DOCUMENT_STATE_APROVEE)
                        {
                            return inventoryMoveInvoice.id;
                        }


                    }
                }
            }
            return null;
        }
        #endregion
    }
}

