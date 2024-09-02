using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Mvc;
using DevExpress.XtraGauges.Core.Model;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Operations;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilitarios.CorreoElectronico;
using Utilitarios.Encriptacion;

namespace DXPANACEASOFT.Controllers
{
    public class SalesQuotationExteriorController : DefaultController
    {
        private Invoice GetSalesQuotationExterior()
        {
            if (!(Session["salesQuotationExterior"] is Invoice salesQuotationExterior))
                salesQuotationExterior = new Invoice();
            return salesQuotationExterior;
        }

        private void SetSalesQuotationExterior(Invoice salesQuotationExterior)
        {
            Session["salesQuotationExterior"] = salesQuotationExterior;
        }

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region {RA} - Transacion Invoice

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorPartial()
        {
            var modelFX = (TempData["modelFX"] as List<Invoice>);
            modelFX = modelFX ?? new List<Invoice>();
            var dd = ViewData;
            TempData.Keep("modelFX");
            TempData.Keep("ParametersSeek");

            return PartialView("_IndexResultGridView", modelFX.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorPartialAddNew(bool approve, bool autorize, Invoice invoice, SalesQuotationExterior salesQuotationExterior, Document document,
            string jsonPaymentTermDetails)
        {
            //Person newBuyerData = null;
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var tempSalesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;
            Invoice _invoice = ObtainInvoice(invoice.id);
            FillViewBagSolicitante(_invoice);

            Document Newdocument = new Document();
            if (invoice == null || salesQuotationExterior == null)
            {
                //TempData.Keep("salesQuotationExterior");
                // return con error indicando que faltan datos
                ViewData["EditError"] = "No existen datos de Proforma.";
                return PartialView("_EditForm", invoice);
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
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
                    var documentType = db.DocumentType.FirstOrDefault(t => t.code == "131");
                    document.DocumentType = documentType;
                    var documentState = db.DocumentState.FirstOrDefault(t => t.code == "01");
                    document.DocumentState = documentState;
                    invoice.Document = document;

                    #endregion Invoice

                    #region Sales Quotation Exterior

                    invoice.SalesQuotationExterior = new SalesQuotationExterior();
                    var foreignCustomerIdentification = db.ForeignCustomerIdentification.FirstOrDefault(s => s.id == salesQuotationExterior.id_addressCustomer);
                    Person consignee = null;
                    if (salesQuotationExterior.id_consignee != 0)
                    {
                        consignee = db.Person.FirstOrDefault(r => r.id == salesQuotationExterior.id_consignee);
                        if (consignee != null) salesQuotationExterior.Person = consignee;
                        invoice.SalesQuotationExterior.ForeignCustomerIdentification = foreignCustomerIdentification;
                    }

                    invoice.SalesQuotationExterior.updateSalesQuotationExterior(salesQuotationExterior, true, consignee, ActiveUser);

                    #endregion Sales Quotation Exterior

                    #region Invoice Details

                    if (_invoice?.InvoiceDetail != null)
                    {
                        if (_invoice.InvoiceDetail.Count == 0)
                        {
                            throw new Exception("No puede guardar la proforma sin detalle.");
                        }
                        invoice.addBulkDetail(_invoice.InvoiceDetail.ToList(), ActiveUser);
                    }

                    #endregion Invoice Details

                    #region Campos Calculados

                    // campos calculados
                    invoice.calculateTotales();
                    calculateTotalesSalesQuotationExterior(invoice);
                    invoice.calculateTotalBoxes_SalesQuotationExterior();
                    invoice.saveWeight(db);

                    #endregion Campos Calculados

                    // Preparar los plazos de pago...
                    this.PrepareInvoiceToSave(invoice, jsonPaymentTermDetails);

                    SetSalesQuotationExterior(invoice);

                    #region Seleccion punto de Emisión

                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    #endregion Seleccion punto de Emisión

                    #region Document

                    invoice.Document.sequential = GetDocumentSequential(_invoice.Document.id_documentType);
                    invoice.Document.number = GetDocumentNumber(_invoice.Document.id_documentType, id_ep);

                    if (autorize)
                    {
                        invoice.Document.Autorize(ActiveUser);
                    }
                    if (approve)
                    {
                        invoice.Document.Approve(ActiveUser);
                    }

                    invoice.Document.PendingDocument(ActiveUser);
                    invoice.Document.DocumentTransactionState = db.DocumentTransactionState.FirstOrDefault(ds => ds.code == "00");

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);
                    invoice.Document.EmissionPoint = emissionPoint;
                    invoice.Document.id_emissionPoint = emissionPoint.id;

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }

                    #endregion Document

                    #region SalesQuotationExteriorDocument

                   
                    if (tempSalesQuotationExterior?.SalesQuotationExteriorDocument != null)
                    {
                        invoice.SalesQuotationExterior.SalesQuotationExteriorDocument = new List<SalesQuotationExteriorDocument>();
                        var itemSalesQuotationExteriorDocument = tempSalesQuotationExterior.SalesQuotationExteriorDocument.ToList();

                        foreach (var detail in itemSalesQuotationExteriorDocument)
                        {
                            var tempItemSalesQuotationExteriorDocument = new SalesQuotationExteriorDocument
                            {
                                guid = detail.guid,
                                url = detail.url,
                                attachment = detail.attachment,
                                referenceDocument = detail.referenceDocument,
                                descriptionDocument = detail.descriptionDocument
                            };

                            invoice.SalesQuotationExterior.SalesQuotationExteriorDocument.Add(tempItemSalesQuotationExteriorDocument);
                        }
                    }

                    #endregion SalesQuotationExteriorDocument

                    invoice.ValidateInfo_SalesQuotationExterior();

                    db.Invoice.Add(invoice);
                    db.SaveChanges();
                    trans.Commit();

                    /* Add Document State */
                    if (invoice?.Document?.id_documentState != null && invoice?.Document?.DocumentState == null)
                    {
                        var state = db.DocumentState.FirstOrDefault(r => r.id == invoice.Document.id_documentState);
                        invoice.Document.DocumentState = state;
                    }
                    SetSalesQuotationExterior(invoice);
                    //TempData["salesQuotationExterior"] = invoice;
                    ViewData["EditMessage"] = SuccessMessage("Proforma del Exterior: " + invoice.Document.number + " guardada exitosamente");
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

                    invoice.Document.accessKey = null;
                    invoice.Document.number = null;
                    invoice.Document.sequential = 0;
                    invoice.Document.PendingDocument(ActiveUser);
                    throw;
                }
                catch (Exception e)
                {
                    string estatusDesc = (autorize) ? "Autorizar " : ((approve) ? "Aprobar " : null);
                    string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;

                    if (e.Data["source"] != null)
                    {
                        if ((string)e.Data["source"] == "modelDocumentValidation" && !string.IsNullOrEmpty(estatusDesc)) msgErr += "Para " + estatusDesc + " el presente documento los siguientes campos son requeridos:<br>" + Environment.NewLine;
                    }
                    // "source", "modelDocumentValidation"
                    ViewData["EditError"] = ErrorMessage(msgErr + e.Message);
                    trans.Rollback();

                    invoice.Document.accessKey = null;
                    invoice.Document.number = null;
                    invoice.Document.sequential = 0;
                    invoice.Document.PendingDocument(ActiveUser);
                }
                finally
                {
                    SetSalesQuotationExterior(invoice);
                    //TempData.Keep("salesQuotationExterior");
                }
            }

            return PartialView("_EditForm", invoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorPartialUpdate(bool approve, bool autorize, Invoice invoice, SalesQuotationExterior salesQuotationExterior, Document document,
            string jsonPaymentTermDetails)
         {
            Person newConsigneeData = null;
            Invoice editInvoice = null;
            Invoice _invoice = ObtainInvoice(invoice.id);
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var tempSalesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;
            FillViewBagSolicitante(_invoice);
            //int id_DocumentState = document.id_documentState;
            int id_documentState = _invoice.Document.id_documentState;
            Boolean changeEmissionPoint = false;

            if (invoice == null || salesQuotationExterior == null)
            {
                // return con error indicando que faltan datos
            }
            Document documentEnvioNotificacion = null;
            string purchaseOrdeNotificacion = null;
            bool sendNotificacion = false;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
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

                    // Obtener punto de emision actual del Invoice->Document

                    #endregion Validacion Punto de Emision

                    editInvoice = db.Invoice.FirstOrDefault(r => r.id == invoice.id);
                    editInvoice.Document.description = document.description;
                    editInvoice.Document.emissionDate = document.emissionDate;
                    editInvoice.Document.id_userUpdate = ActiveUser.id;
                    editInvoice.Document.dateUpdate = DateTime.Now;
                    editInvoice.Document.reference = document.reference;

                    if (editInvoice != null)
                    {
                        #region invoice

                        if (editInvoice.SalesQuotationExterior.id_consignee != salesQuotationExterior.id_consignee)
                        {
                            newConsigneeData = db.Person.FirstOrDefault(r => r.id == salesQuotationExterior.id_consignee);
                            editInvoice.SalesQuotationExterior.Person = newConsigneeData;
                        }

                        editInvoice.id_buyer = invoice.id_buyer;

                        #endregion invoice

                        #region invoice Exterior

                        editInvoice.SalesQuotationExterior.updateSalesQuotationExterior(salesQuotationExterior, false, newConsigneeData, ActiveUser);

                        #endregion invoice Exterior

                        #region Invoice Detail

                        if (_invoice?.InvoiceDetail != null)
                        {
                            editInvoice.addBulkDetail(_invoice.InvoiceDetail.ToList(), ActiveUser);
                            if (_invoice.InvoiceDetail.Where(w => w.isActive).ToList().Count == 0)
                            {
                                throw new Exception("No puede guardar la proforma sin detalle.");
                            }
                        }

                        #endregion Invoice Detail

                        SetSalesQuotationExterior(editInvoice);
                        //TempData["salesQuotationExterior"] = editInvoice;

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
                        calculateTotalesSalesQuotationExterior(editInvoice);
                        editInvoice.calculateTotalBoxes_SalesQuotationExterior();
                        editInvoice.saveWeight(db);

                        #endregion Campos Calculados

                        // Preparar los plazos de pago...
                        this.PrepareInvoiceToSave(editInvoice, jsonPaymentTermDetails);

                        // TODO: condicionamiento actualizacion
                        SetSalesQuotationExterior(editInvoice);

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
                                    throw new Exception("No tiene Permiso para Aprobar la Proforma");
                                }
                            }
                        }

                        #endregion Validación Permiso

                        #region Document

                        if (approve)
                        {
                            editInvoice.Document.Approve(ActiveUser);
                        }
                        if (autorize)
                        {
                            editInvoice.Document.Autorize(ActiveUser);
                        }

                        #endregion Document

                        #region SalesQuotationExteriorDocument

                        if (tempSalesQuotationExterior.SalesQuotationExteriorDocument != null)
                        {
                            var itemSalesQuotationExteriorDocument = tempSalesQuotationExterior.SalesQuotationExteriorDocument.ToList();

                            for (int i = editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument.Count - 1; i >= 0; i--)
                            {
                                var detail = editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument.ElementAt(i);

                                if (itemSalesQuotationExteriorDocument.FirstOrDefault(fod => fod.id == detail.id) == null)
                                {
                                    DeleteAttachment(detail);
                                    editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }
                            }

                            foreach (var detail in itemSalesQuotationExteriorDocument)
                            {
                                var tempItemSalesQuotationExteriorDocument = editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument.FirstOrDefault(fod => fod.id == detail.id);
                                if (tempItemSalesQuotationExteriorDocument == null)
                                {
                                    tempItemSalesQuotationExteriorDocument = new SalesQuotationExteriorDocument
                                    {
                                        guid = detail.guid,
                                        url = detail.url,
                                        attachment = detail.attachment,
                                        referenceDocument = detail.referenceDocument,
                                        descriptionDocument = detail.descriptionDocument
                                    };
                                    editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument.Add(tempItemSalesQuotationExteriorDocument);
                                }
                                else
                                {
                                    if (tempItemSalesQuotationExteriorDocument.url != detail.url)
                                    {
                                        DeleteAttachment(tempItemSalesQuotationExteriorDocument);
                                        tempItemSalesQuotationExteriorDocument.guid = detail.guid;
                                        tempItemSalesQuotationExteriorDocument.url = detail.url;
                                        tempItemSalesQuotationExteriorDocument.attachment = detail.attachment;
                                    }
                                    tempItemSalesQuotationExteriorDocument.referenceDocument = detail.referenceDocument;
                                    tempItemSalesQuotationExteriorDocument.descriptionDocument = detail.descriptionDocument;
                                    db.Entry(tempItemSalesQuotationExteriorDocument).State = EntityState.Modified;
                                }
                            }
                        }

                        #endregion SalesQuotationExteriorDocument

                        #region Actualización de datos en facturas

                        var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                        if (puedeModDatosProforma)
                        {
                            UpdateAllInvoiceComercial(editInvoice, editInvoice.SalesQuotationExterior);
                            UpdateAllInvoiceExterior(editInvoice, editInvoice.SalesQuotationExterior);
                        }

                        #endregion Actualización de datos en facturas

                        editInvoice.ValidateInfo_SalesQuotationExterior();

                        db.Invoice.Attach(editInvoice);
                        db.Entry(editInvoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Proforma del Exterior: " + editInvoice.Document.number + " actualizada exitosamente");
                        SetSalesQuotationExterior(editInvoice);


                        documentEnvioNotificacion = editInvoice.Document;
                        purchaseOrdeNotificacion = editInvoice.SalesQuotationExterior.purchaseOrder;
                        sendNotificacion = true;

                    }
                }
                catch (Exception e)
                {
                    trans.Rollback();

                    string estatusDesc = (autorize) ? "Autorizar " : ((approve) ? "Aprobar " : null);
                    string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;

                    if (e.Data["source"] != null)
                    {
                        if ((string)e.Data["source"] == "modelDocumentValidation" && !string.IsNullOrEmpty(estatusDesc)) msgErr += "Para " + estatusDesc + " el presente documento los siguientes campos son requeridos: <br>" + Environment.NewLine;
                    }
                    ViewData["EditError"] = ErrorMessage(msgErr + e.Message);

                    editInvoice.Document.id_documentState = id_documentState;
                }
                finally
                {
                    SetSalesQuotationExterior(editInvoice);
                }

                if (sendNotificacion)
                {
                    var settingNotificacion = db.Setting.FirstOrDefault(r => r.code == "NOTPROC");
                    if (settingNotificacion != null && settingNotificacion.value == "SI")
                    {
                        Task.Run(() => notify(documentEnvioNotificacion, purchaseOrdeNotificacion)).ConfigureAwait(false);
                    }
                }
            }

            return PartialView("_EditForm", editInvoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorPartialDelete(int id)
        {
            var model = db.Invoice.Where(r => r.Document.DocumentState.code != "05");

            if (id >= 0)
            {
                using (var trans = db.Database.BeginTransaction())
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
                            ViewData["EditMessage"] = SuccessMessage("Proforma del Exterior: " + deleteInvoice.Document.number + " anulada.");
                        }
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }

            return PartialView("_EditForm", model.First());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateDivTabSalesQuotationExterior(Invoice invoice, SalesQuotationExterior salesQuotationExterior, Document document,
            string jsonPaymentTermDetails)
        {
            Person newConsigneeData = null;
            Invoice editInvoice = null;
            Invoice _invoice = ObtainInvoice(invoice.id);
            FillViewBagSolicitante(_invoice);
            //int id_DocumentState = document.id_documentState;
            int id_documentState = _invoice.Document.id_documentState;
            Boolean changeEmissionPoint = false;

            if (invoice == null || salesQuotationExterior == null)
            {
                // return con error indicando que faltan datos
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
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

                    // Obtener punto de emision actual del Invoice->Document

                    #endregion Validacion Punto de Emision

                    editInvoice = db.Invoice.FirstOrDefault(r => r.id == invoice.id);
                    editInvoice.Document.description = document.description;
                    editInvoice.Document.emissionDate = document.emissionDate;
                    editInvoice.Document.id_userUpdate = ActiveUser.id;
                    editInvoice.Document.dateUpdate = DateTime.Now;
                    editInvoice.Document.reference = document.reference;

                    if (editInvoice != null)
                    {
                        #region invoice

                        if (editInvoice.SalesQuotationExterior.id_consignee != salesQuotationExterior.id_consignee)
                        {
                            newConsigneeData = db.Person.FirstOrDefault(r => r.id == salesQuotationExterior.id_consignee);
                            editInvoice.SalesQuotationExterior.Person = newConsigneeData;
                        }

                        editInvoice.id_buyer = invoice.id_buyer;

                        #endregion invoice

                        #region invoice Exterior

                        editInvoice.SalesQuotationExterior.updateSalesQuotationExterior(salesQuotationExterior, false, newConsigneeData, ActiveUser);

                        #endregion invoice Exterior

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
                        calculateTotalesSalesQuotationExterior(editInvoice);
                        editInvoice.calculateTotalBoxes_SalesQuotationExterior();
                        editInvoice.saveWeight(db);

                        #endregion Campos Calculados

                        SetSalesQuotationExterior(editInvoice);
                    }
                }
                catch (Exception e)
                {
                    string msgErr = "Se presentaron el(los) siguientes(s) errores: <br>" + Environment.NewLine;
                    ViewData["EditError"] = ErrorMessage(msgErr + e.Message);

                    editInvoice.Document.id_documentState = id_documentState;
                }
                finally
                {
                    SetSalesQuotationExterior(editInvoice);
                }
            }

            return PartialView("_TabSalesQuotationExterior", editInvoice);
        }

        [HttpPost]
        public void CancelDocuments(int[] ids)
        {
            TempData.Keep("ParametersSeek");
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

                                if (invoice.InvoiceDetail.FirstOrDefault(fod => fod.proformaUsedNumBoxes > 0) != null)
                                {
                                    throw new Exception("No puede Anularse la Proforma porque tiene detalle Facturado");
                                }

                                invoice.ValidateStateChange("05");
                                invoice.Document.RemoveDocument(ActiveUser);

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Proformas anuladas.");
                    }
                    catch (Exception e)
                    {
                        string msgError = "Existen inconvenientes al anular la Proforma " + currentInvoiceNumber + ", revise los siguientes errores:";
                        ViewData["EditError"] = msgError + e.Message;
                        trans.Rollback();
                    }
                }
            }

            ParametersSeek _parametersSeekInvoiceExterior = (ParametersSeek)TempData["ParametersSeek"];
            var modelFX = SeekSalesQuotationExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ApproveDocuments(int[] ids)
        {
            TempData.Keep("ParametersSeek");
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
                                invoice.ValidateInfo_SalesQuotationExterior();

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Proformas aprobadas con éxito.");
                    }
                    catch (Exception e)
                    {
                        string msgErr = "Se presentaron el(los) siguientes(s) error(es) al Aprobar la Proforma #" + currentInvoiceNumber + "<br>";

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

            ParametersSeek _parametersSeek = (ParametersSeek)TempData["ParametersSeek"];
            var modelFX = SeekSalesQuotationExterior(_parametersSeek);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AutorizeDocuments(int[] ids)
        {
            TempData.Keep("ParametersSeek");
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

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");

                            if (invoice != null && documentState != null)
                            {
                                currentInvoiceNumber = invoice.Document.number;

                                invoice.ValidateStateChange("06");
                                invoice.Document.PartialApprove(ActiveUser);
                                invoice.ValidateInfo_SalesQuotationExterior();

                                db.Invoice.Attach(invoice);
                                db.Entry(invoice).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Proformas autorizadas con éxito.");
                    }
                    catch (Exception e)
                    {
                        string msgErr = "Se presentaron el(los) siguiente(s) errores al Autorizar la Proforma #" + currentInvoiceNumber + "<br>";// + Environment.NewLine;

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

            ParametersSeek _parametersSeekInvoiceExterior = (ParametersSeek)TempData["ParametersSeek"];
            var modelFX = SeekSalesQuotationExterior(_parametersSeekInvoiceExterior);
            modelFX = modelFX ?? new List<Invoice>();
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reverse(int id)
        {
            var invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            FillViewBagSolicitante(invoice);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (invoice != null && documentState != null)
                    {
                        invoice.Document.DocumentState = documentState;
                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        //TODO: Actualizar valores de cartones

                        db.SaveChanges();
                        trans.Commit();

                        invoice.calculateTotales();
                        calculateTotalesSalesQuotationExterior(invoice);
                        invoice.ViewWeight();

                        SetSalesQuotationExterior(invoice);

                        ViewData["EditMessage"] = SuccessMessage("Proforma: " + invoice.Document.number + " reversada.");
                    }
                }
                catch (Exception e)
                {
                    //TempData.Keep("salesQuotationExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_EditForm", invoice);
        }

        [HttpPost]
        public ActionResult Close(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            FillViewBagSolicitante(invoice);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04"); //Cerrado

                    if (invoice != null && documentState != null)
                    {
                        invoice.Document.DocumentState = documentState;
                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        invoice.calculateTotales();
                        calculateTotalesSalesQuotationExterior(invoice);
                        invoice.ViewWeight();

                        SetSalesQuotationExterior(invoice);

                        ViewData["EditMessage"] = SuccessMessage("Proforma: " + invoice.Document.number + " cerrada.");
                    }
                }
                catch (Exception e)
                {
                    //TempData.Keep("salesQuotationExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_EditForm", invoice);
        }

        [HttpPost]
        public ActionResult Annul(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            FillViewBagSolicitante(invoice);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var documentInvoice = db.Document.FirstOrDefault(fod => fod.id_documentOrigen == id && fod.DocumentState.code != "05");
                    if(documentInvoice != null)
                    {
                        if (invoice.InvoiceDetail.FirstOrDefault(fod => fod.proformaUsedNumBoxes > 0) != null)
                        {
                            throw new Exception("No puede Anularse la Proforma porque tiene detalle Facturado");
                        }
                    }
                       
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (invoice != null && documentState != null)
                    {
                        invoice.ValidateStateChange("05");

                        invoice.Document.DocumentState = documentState;
                        db.Invoice.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        SetSalesQuotationExterior(invoice);

                        ViewData["EditMessage"] = SuccessMessage("Proforma: " + invoice.Document.number + " anulada.");
                    }
                }
                catch (Exception e)
                {
                    //TempData.Keep("salesQuotationExterior");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_EditForm", invoice);
        }

        #endregion {RA} - Transacion Invoice

        #region Métodos de actualización de facturas / proformas

        private void UpdateAllInvoiceComercial(Invoice invoice, SalesQuotationExterior salesQuotationExterior)
        {
            var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
            var idEstadoAprParcial = db.DocumentState.FirstOrDefault(e => e.code == "02")?.id;
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
            var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;
            var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;

            var facturasComercialesDoc = db.Document
                .Where(e => e.id_documentOrigen == invoice.id && e.id_documentType == idTipoDocFactComercial && e.id_documentState != idEstadoAnulado)
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

                UpdateInvoiceComercialData(facturaComercialDoc, invoice, salesQuotationExterior);

                #region Verificamos si existe alguna factura Fiscal relacionada a la factura comercial

                var facturaesFiscalesDoc = db.Document
                    .Where(e => e.id_documentOrigen == facturaComercialDoc.id
                        && e.id_documentType == idTipoDocFactFiscal && e.id_documentState != idEstadoAnulado);

                if (facturaesFiscalesDoc != null)
                {
                    foreach (var facturaFiscalDoc in facturaesFiscalesDoc)
                    {
                        UpdateInvoiceExteriorData(facturaFiscalDoc, invoice, salesQuotationExterior);
                    }
                }

                #endregion Verificamos si existe alguna factura Fiscal relacionada a la factura comercial
            }
        }

        private void UpdateAllInvoiceExterior(Invoice invoice, SalesQuotationExterior salesQuotationExterior)
        {
            var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
            var idEstadoAprParcial = db.DocumentState.FirstOrDefault(e => e.code == "02")?.id;
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;
            var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;
            var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;

            var documentOrigin = db.Document.FirstOrDefault(e => e.id_documentOrigen == invoice.id && e.id_documentState != idEstadoAnulado);
            List<Document> facturasFiscalesDoc = new List<Document>();
            if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "70"))
            {
                facturasFiscalesDoc = db.Document.Where(e => e.id_documentOrigen == documentOrigin.id &&
                                           e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoAnulado))
                                       .ToList();
            }
            else
            {
                facturasFiscalesDoc = db.Document.Where(e => e.id_documentOrigen == invoice.id &&
                                           e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoAnulado))
                                       .ToList();
            }


            if (facturasFiscalesDoc
                .Any(e => e.id_documentState != idEstadoPendiente && e.id_documentState != idEstadoAprParcial))
            {
                var facturaFiscalDoc = facturasFiscalesDoc.FirstOrDefault();
                throw new Exception(
                    $"No se pueden actualizar datos en la factura fiscal {facturaFiscalDoc.number} porque está está {facturaFiscalDoc.DocumentState.name}");
            }

            // Actualizamos las facturas comerciales
            foreach (var facturaFiscalDoc in facturasFiscalesDoc)
            {
                if (facturaFiscalDoc.Invoice == null) continue; // Si invoice es null, no actualizamos nada

                UpdateInvoiceExteriorData(facturaFiscalDoc, invoice, salesQuotationExterior);

                #region Verificamos si existe alguna factura comercial relacionada a la factura fiscal

                var facturasComercialesDoc = db.Document
                    .Where(e => e.id_documentOrigen == facturaFiscalDoc.id
                        && e.id_documentType == idTipoDocFactComercial && e.id_documentState != idEstadoAnulado);

                if (facturasComercialesDoc != null)
                {
                    foreach (var facturaComercialDoc in facturasComercialesDoc)
                    {
                        UpdateInvoiceComercialData(facturaComercialDoc, invoice, salesQuotationExterior);
                    }
                }

                #endregion Verificamos si existe alguna factura comercial relacionada a la factura fiscal
            }
        }

        private void UpdateInvoiceExteriorData(Document facturaFiscalDoc,
            Invoice invoice, SalesQuotationExterior salesQuotationExterior)
        {
            facturaFiscalDoc.Invoice.InvoiceExterior.id_consignee = salesQuotationExterior.id_consignee; // consignatario
            facturaFiscalDoc.Invoice.id_buyer = invoice.id_buyer; // Cliente del exterior
            facturaFiscalDoc.Invoice.InvoiceExterior.id_notifier = salesQuotationExterior.id_notifier; // Notificador
            facturaFiscalDoc.Invoice.InvoiceExterior.purchaseOrder = salesQuotationExterior.purchaseOrder; // Orden de pedido
            facturaFiscalDoc.Invoice.InvoiceExterior.transport = salesQuotationExterior.transport; // Transporte
            facturaFiscalDoc.Invoice.InvoiceExterior.idVendor = salesQuotationExterior.idVendor; // Vendedor
            facturaFiscalDoc.Invoice.InvoiceExterior.id_termsNegotiation = salesQuotationExterior.id_termsNegotiation; //Termino de Negociación
            facturaFiscalDoc.Invoice.InvoiceExterior.id_PaymentMethod = salesQuotationExterior.id_PaymentMethod; // Forma de Pago
            facturaFiscalDoc.Invoice.InvoiceExterior.id_PaymentTerm = salesQuotationExterior.id_PaymentTerm; // Plazo de Pago
            facturaFiscalDoc.Invoice.InvoiceExterior.id_bank = salesQuotationExterior.id_bank; //Banco Beneficiario
            facturaFiscalDoc.Invoice.InvoiceExterior.valueSubscribed = salesQuotationExterior.valueSubscribed; //Banco Beneficiario
            facturaFiscalDoc.Invoice.InvoiceExterior.dateShipment = salesQuotationExterior.dateShipment; // Fecha de Embarque
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portShipment = salesQuotationExterior.id_portShipment; // Puerto de Embarque
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portDischarge = salesQuotationExterior.id_portDischarge; // Puerto Descarga
            facturaFiscalDoc.Invoice.InvoiceExterior.id_portDestination = salesQuotationExterior.id_portDestination; //Puerto Destino
            facturaFiscalDoc.Invoice.InvoiceExterior.temperatureInstruction = salesQuotationExterior.temperatureInstruction; //Instrucciones de Temperatura
            facturaFiscalDoc.Invoice.InvoiceExterior.temperatureInstrucDate = salesQuotationExterior.temperatureInstrucDate; //Fecha Emisión Instrucciones de Temperatura
            facturaFiscalDoc.Invoice.InvoiceExterior.id_BankTransfer = salesQuotationExterior.id_BankTransfer; //Banco de Transferencia
            facturaFiscalDoc.Invoice.InvoiceExterior.totalBoxes = salesQuotationExterior.totalBoxes; //Total Cartones
            facturaFiscalDoc.Invoice.InvoiceExterior.PO = salesQuotationExterior.PO; // PO
            facturaFiscalDoc.Invoice.InvoiceExterior.noContrato = salesQuotationExterior.noContrato; // Contrato
            facturaFiscalDoc.Invoice.InvoiceExterior.valueInternationalFreight = 0;
            facturaFiscalDoc.Invoice.InvoiceExterior.valueInternationalInsurance = 0;
            facturaFiscalDoc.Invoice.InvoiceExterior.valueCustomsExpenditures = 0;
            facturaFiscalDoc.Invoice.InvoiceExterior.valueTransportationExpenses = 0;


            facturaFiscalDoc.dateUpdate = DateTime.Today;
            facturaFiscalDoc.id_userUpdate = this.ActiveUserId;

            var invoiceFactura = facturaFiscalDoc.Invoice;
            var invoiceProforma = salesQuotationExterior.Invoice;

            //detalle
            invoiceFactura.addBulkDetailProfFact(facturaFiscalDoc.id_documentOrigen, invoiceProforma.InvoiceDetail.ToList(), ActiveUser);

            invoiceFactura.saveWeight(db);

            db.Document.Attach(facturaFiscalDoc);
            db.Entry(facturaFiscalDoc).State = EntityState.Modified;
        }

        private void UpdateInvoiceComercialData(Document facturaComercialDoc,
            Invoice invoice, SalesQuotationExterior salesQuotationExterior)
        {
            var invoiceFacComercial = facturaComercialDoc.InvoiceCommercial;
            invoiceFacComercial.id_Consignee = salesQuotationExterior.id_consignee; // consignatario
            invoiceFacComercial.id_ForeignCustomer = invoice.id_buyer; // Cliente del exterior
            invoiceFacComercial.id_Notifier = salesQuotationExterior.id_notifier; // Notificador
            invoiceFacComercial.purchaseOrder = salesQuotationExterior.purchaseOrder; // Orden de pedido
            invoiceFacComercial.idVendor = salesQuotationExterior.idVendor; // Vendedor
            invoiceFacComercial.id_termsNegotiation = salesQuotationExterior.id_termsNegotiation;// Término de Negociación
            invoiceFacComercial.id_PaymentMethod = salesQuotationExterior.id_PaymentMethod; // Forma de Pago
            invoiceFacComercial.id_PaymentTerm = salesQuotationExterior.id_PaymentTerm; // Plazo de Pago
            invoiceFacComercial.dateShipment = salesQuotationExterior.dateShipment; //  Fecha de Embarque
            invoiceFacComercial.id_portShipment = salesQuotationExterior.id_portShipment; // Puerto de Embarque
            invoiceFacComercial.id_portDischarge = salesQuotationExterior.id_portDischarge; // Puerto Descarga
            invoiceFacComercial.id_portDestination = salesQuotationExterior.id_portDestination; //Puerto Destino
            invoiceFacComercial.id_BankTransfer = salesQuotationExterior.id_BankTransfer; //Banco de Transferencia
            invoiceFacComercial.id_addressCustomer = salesQuotationExterior.id_addressCustomer; //Información de cliente del exterior

            facturaComercialDoc.dateUpdate = DateTime.Today;
            facturaComercialDoc.id_userUpdate = this.ActiveUserId;

            InvoiceCommercial invoiceFacturaCommercial = facturaComercialDoc.InvoiceCommercial;
            var invoiceProforma = salesQuotationExterior.Invoice;

            //detalle
            invoiceFacturaCommercial.addBulkDetailProfFactCom(invoiceFacturaCommercial.Document.id_documentOrigen, invoiceProforma.InvoiceDetail.ToList(), ActiveUser);
;
            db.InvoiceCommercial.Attach(invoiceFacComercial);
            db.Entry(invoiceFacComercial).State = EntityState.Modified;
        }

        #endregion Métodos de actualización de facturas / proformas

        #region {RA} Invoice Edit

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditSalesQuotationExterior(int id, int[] requestDetails)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);

            if (invoice == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("131"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == "01");
                tbsysInvoiceMode invoiceMode = db.tbsysInvoiceMode.FirstOrDefault(r => r.isManual && r.isActive);
                tbsysInvoiceType invoiceType = db.tbsysInvoiceType.FirstOrDefault(r => r.code.Equals("PRO"));

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
                    SalesQuotationExterior = new SalesQuotationExterior
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
                var valInvFact = db.Setting.FirstOrDefault(e => e.code == "INVFACT")?.value == "SI";
                var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                if (valInvFact && puedeModDatosProforma)
                {

                    #region Verificación de facturas Fiscales relacionadas
                    var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
                    var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;

                    var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;
                    var documentOrigin = db.Document.FirstOrDefault(e => e.id_documentOrigen == id);
                    List<Document> facturasFiscales = new List<Document>();
                    if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "70"))
                    {
                        facturasFiscales = db.Document.Where(e => e.id_documentOrigen == documentOrigin.id &&
                                                   e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoAnulado))
                                               .ToList();
                    }
                    else
                    {
                        facturasFiscales = db.Document.Where(e => e.id_documentOrigen == id &&
                                                   e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoAnulado))
                                               .ToList();
                    }


                    var idsFacturaFiscales = facturasFiscales.Select(e => e.id).ToList();

                    if (facturasFiscales.Any())
                    {
                        foreach (var facturasFiscal in facturasFiscales)
                        {
                            //Verifico si existe egreso Relacionado
                            var lstInvoiceInventory = db.InventoryMove.FirstOrDefault(w => w.id_Invoice != null && w.Document.DocumentState.code != "05" && w.id_Invoice == facturasFiscal.id);
                            if (lstInvoiceInventory != null)
                            {
                                ViewBag.movimiento = true;
                                ViewBag.naturalSequential = "No se puede agregar porque está realacionado a un movimiento de inventario " + lstInvoiceInventory.natureSequential;
                                break;
                            }
                            else
                            {
                                ViewBag.movimiento = false;
                            }
                        }

                    }
                    #endregion

                }
                invoice.calculateTotales();
                calculateTotalesSalesQuotationExterior(invoice);
                invoice.saveWeight(db);

                // SET DETALLE DE FACTURAS RELACIONADAS
                var listaDocument1 = new List<Document>();
                this.SetDetalleFacturasRelacionadas(invoice.id, listaDocument1);

                invoice.Document.Document1 = listaDocument1;
            }

            SetSalesQuotationExterior(invoice);
            FillViewBagSolicitante(invoice);

            return PartialView("Edit", invoice);
        }

        private void SetDetalleFacturasRelacionadas(int idDocument, List<Document> facturasRelacionadas)
        {
            var documents = db.Document
                .Where(e => e.id_documentOrigen == idDocument)
                .ToList();

            foreach (var document in documents)
            {
                if(!facturasRelacionadas.Any(e => e.id == document.id)) facturasRelacionadas.Add(document);

                SetDetalleFacturasRelacionadas(document.id, facturasRelacionadas);
            }
        }

        private void GetDocumentOrigins(List<Document> documents, int idDocument)
        {
            var documentsOrigin = db.Document
                .Where(e => e.id_documentOrigen == idDocument
                    && e.DocumentState.code != "05");
        }

        private void FillViewBagSolicitante(Invoice invoice)
        {
            if (invoice.id != 0)
                ViewBag.Solicitante = db.User.FirstOrDefault(u => u.id == invoice.SalesQuotationExterior.id_userCreate)
                        ?.Employee.Person.fullname_businessName ?? "";

            else
                ViewBag.Solicitante = ActiveUser?.Employee.Person.fullname_businessName ?? ActiveUser?.username ?? "";
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceCopy(int id)
        {
            Invoice invoice = db.Invoice.FirstOrDefault(o => o.id == id);

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("131"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code.Equals("01"));

            if (invoice != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
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
                            sequential = GetDocumentSequential(documentType.id),
                            number = GetDocumentNumber(documentType.id)
                        };

                        var buyer = db.Person.FirstOrDefault(a => a.id == invoice.id_buyer && !a.isActive);
                        if(buyer != null)
                        {
                            throw new Exception($"El Cliente del Exterior {buyer.fullname_businessName} esta en estado inactivo.");
                        }

                        if (invoice.SalesQuotationExterior == null) invoice.SalesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(r => r.id == id);

                        invoice.SalesQuotationExterior.pendingBoxes = invoice.SalesQuotationExterior.totalBoxes;
                        invoice.SalesQuotationExterior.usedBoxes = 0;

                        if (invoice.InvoiceDetail.Count() == 0) invoice.InvoiceDetail = db.InvoiceDetail.Where(r => r.id_invoice == id).ToList();

                        foreach (var item in invoice.InvoiceDetail)
                        {
                            var itemdb = db.Item.FirstOrDefault(a => a.id == item.id_item && item.isActive && !a.isActive);
                            if (itemdb != null)
                            {
                                throw new Exception($"El Producto {itemdb.masterCode} esta en estado inactivo.");
                            }
                            item.proformaPendingNumBoxes = item.numBoxes;
                            item.proformaUsedNumBoxes = 0;
                            var aItemWeightConversionFreezen = db.ItemWeightConversionFreezen.FirstOrDefault(fod => fod.id_Item == item.id_item);
                            item.netWeight = aItemWeightConversionFreezen.itemWeightNetWeight;
                        }

                        invoice.calculateTotales();
                        calculateTotalesSalesQuotationExterior(invoice);
                        //invoice.ViewWeight();
                        invoice.saveWeight(db);
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        invoice.Document.PendingDocument(ActiveUser);
                    }
                }
               
            }

            SetSalesQuotationExterior(invoice);

            return PartialView("Edit", invoice);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InvoiceDuplicate(int id, int iteracion)
        {
            // Búsqueda de proforma en la DB
            Invoice invoice = db.Invoice.FirstOrDefault(o => o.id == id);
            // Búsqueda de documentos en base a proforma
            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code.Equals("131"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code.Equals("01"));

            // Búsqueda de proformas de exterior
            //if (invoice.SalesQuotationExterior == null) invoice.SalesQuotationExterior = db.SalesQuotationExterior.FirstOrDefault(r => r.id == id);

            // Si la transaccion existe
            if (invoice != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < iteracion; i++)
                        {
                            #region InvoiceInfo
                            // Retorno de nuevo Modelo
                            Invoice _editInvoice = GetInvoiceInfo(invoice);
                            #endregion

                            #region Document Info
                            // Función para la información de Documento
                            if (invoice.Document != null)
                            {
                                _editInvoice.Document = GetDocumentInfo(documentType, documentState, invoice.Document); 
                            }
                            #endregion

                            // Función para la información de proforma de exterior
                            if (invoice.SalesQuotationExterior != null)
                            {
                                _editInvoice.SalesQuotationExterior = GetSalesQuotationExteriorInfo(invoice); 
                            }

                            // Peso Proforma del Exterior
                            var invoiceExteriorWeightList = new List<InvoiceExteriorWeight>();
                            if (invoice.InvoiceExteriorWeight != null)
                            {
                                foreach (var invoiceExteriorW in invoice.InvoiceExteriorWeight)
                                {
                                    invoiceExteriorWeightList.Add(new InvoiceExteriorWeight()
                                    {
                                        id_invoice = invoiceExteriorW.id_invoice,
                                        id_metricUnit = invoiceExteriorW.id_metricUnit,
                                        id_WeightType = invoiceExteriorW.id_WeightType,
                                        //Invoice = invoiceExteriorW.Invoice,
                                        isActive = invoiceExteriorW.isActive,
                                        MetricUnit = invoiceExteriorW.MetricUnit,
                                        peso = invoiceExteriorW.peso,
                                        WeightType = invoiceExteriorW.WeightType,
                                    });
                                }
                            }

                            // Le asigno al subModelo
                            _editInvoice.InvoiceExteriorWeight = invoiceExteriorWeightList;

                            // Recorrido de términos de Pago del exterior
                            var invoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>();
                            if (invoice.InvoiceExteriorPaymentTerm != null)
                            {
                                foreach (var invoiceExteriorPT in invoice.InvoiceExteriorPaymentTerm)
                                {
                                    invoiceExteriorPaymentTerm.Add(new InvoiceExteriorPaymentTerm()
                                    {
                                        dueDate = invoiceExteriorPT.dueDate,
                                        idInvoiceExterior = 0,
                                        orderPayment = invoiceExteriorPT.orderPayment,
                                        porcentaje = invoiceExteriorPT.porcentaje,
                                        valuePayment = invoiceExteriorPT.valuePayment,
                                        //Invoice = currentInvoice,
                                    });
                                }
                            }

                            // Le asigno el submodelo
                            _editInvoice.InvoiceExteriorPaymentTerm = invoiceExteriorPaymentTerm;

                            // Recorrido de Documento de proforma del exterior
                            var invoiceSaleQuotationExteriorDocument = new List<SalesQuotationExteriorDocument>();
                            if (invoice.SalesQuotationExterior.SalesQuotationExteriorDocument != null)
                            {
                                foreach (var item in invoice.SalesQuotationExterior.SalesQuotationExteriorDocument)
                                {
                                    invoiceSaleQuotationExteriorDocument.Add(new SalesQuotationExteriorDocument()
                                    {
                                        attachment = item.attachment,
                                        descriptionDocument = item.descriptionDocument,
                                        guid = item.guid,
                                        id_salesQuotationExterior = item.id_salesQuotationExterior,
                                        referenceDocument = item.referenceDocument,
                                        SalesQuotationExterior = item.SalesQuotationExterior,
                                        url = item.url,
                                    });
                                }
                            }

                            // Le asigno al submodelo
                            _editInvoice.SalesQuotationExterior.SalesQuotationExteriorDocument = invoiceSaleQuotationExteriorDocument;

                            //if (invoice.InvoiceDetail.Count() == 0)
                            //{                                
                            //    invoice.InvoiceDetail = db.InvoiceDetail.Where(r => r.id_invoice == id).ToList();
                            //}

                            // Recorrido de detalles

                            var invoiceDetail = new List<InvoiceDetail>();
                            if (invoice.InvoiceDetail != null)
                            {
                                foreach (var item in invoice.InvoiceDetail)
                                {
                                    var aItemWeightConversionFreezen = db.ItemWeightConversionFreezen.FirstOrDefault(fod => fod.id_Item == item.id_item);
                                    // Obtener detalles por método
                                    var detail = GetInvoiceDetailInfo(item, aItemWeightConversionFreezen);
                                    // Agregar detalle
                                    invoiceDetail.Add(detail);
                                } 
                            }

                            // Le asgino al submodelo
                            _editInvoice.InvoiceDetail = invoiceDetail;

                            _editInvoice.calculateTotales();
                            calculateTotalesSalesQuotationExterior(_editInvoice);

                            //Actualiza Secuencial
                            if (documentType != null)
                            {
                                documentType.currentNumber++;
                                db.DocumentType.Attach(documentType);
                                db.Entry(documentType).State = EntityState.Modified;
                            }

                            _editInvoice.Document.sequential = GetDocumentSequential(_editInvoice.Document.id_documentType);

                            db.Invoice.Add(_editInvoice);
                            db.SaveChanges();
                            SetSalesQuotationExterior(_editInvoice);
                        }
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage($"Proforma del Exterior duplicada exitosamente");
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = ErrorMessage(e.Message);
                        trans.Rollback();

                        invoice.Document.accessKey = null;
                        invoice.Document.number = null;
                        invoice.Document.sequential = 0;
                        invoice.Document.PendingDocument(ActiveUser);
                    }
                }
            }

            return PartialView("Edit", invoice);
        }

        #region Métodos GetDuplicateInfo
        private Invoice GetInvoiceInfo(Invoice invoiceInfo)
        {
            return new Invoice()
            {
                SalesOrder = invoiceInfo.SalesOrder,
                Person = invoiceInfo.Person,
                id_buyer = invoiceInfo.id_buyer,
                id_InvoiceMode = invoiceInfo.id_InvoiceMode,
                id_InvoiceType = invoiceInfo.id_InvoiceType,
                id_remissionGuide = invoiceInfo.id_remissionGuide,
                id_saleOrder = invoiceInfo.id_saleOrder,
                id_seller = invoiceInfo.id_seller,
                IVA = invoiceInfo.IVA,
                //Person1 = invoiceInfo.Person1,
                //RemissionGuide = invoiceInfo.RemissionGuide,
                subTotal = invoiceInfo.subTotal,
                subTotalExentIVA = invoiceInfo.subTotalExentIVA,
                subtotalIVA = invoiceInfo.subtotalIVA,
                subTotalIVA0 = invoiceInfo.subTotalIVA0,
                subTotalIVA0ProformaTruncate = invoiceInfo.subTotalIVA0ProformaTruncate,
                subTotalIVA0Truncate = invoiceInfo.subTotalIVA0Truncate,
                subTotalNoObjectIVA = invoiceInfo.subTotalNoObjectIVA,
                subtotalNoTaxes = invoiceInfo.subtotalNoTaxes,
                subtotalNoTaxesTruncate = invoiceInfo.subtotalNoTaxesTruncate,
                subTotalTruncate = invoiceInfo.subTotalTruncate,
                tip = invoiceInfo.tip,
                totalDiscount = invoiceInfo.totalDiscount,
                totalDiscountTruncate = invoiceInfo.totalDiscountTruncate,
                totalValue = invoiceInfo.totalValue,
                totalValueProformaTruncate = invoiceInfo.totalValueProformaTruncate,
                totalValueTruncate = invoiceInfo.totalValueTruncate,
                valueCustomsExpendituresTruncate = invoiceInfo.valueCustomsExpendituresTruncate,
                valueICE = invoiceInfo.valueICE,
                valueInternationalFreightTruncate = invoiceInfo.valueInternationalFreightTruncate,
                valueInternationalInsuranceTruncate = invoiceInfo.valueInternationalInsuranceTruncate,
                valueIRBPNR = invoiceInfo.valueIRBPNR,
                valuetotalCIFTruncate = invoiceInfo.valuetotalCIFTruncate,
                valueTotalFOBTruncate = invoiceInfo.valueTotalFOBTruncate,
                valuetotalProformaTruncate = invoiceInfo.valuetotalProformaTruncate,
                valueTransportationExpensesTruncate = invoiceInfo.valueTransportationExpensesTruncate,

            };
        }

        private Document GetDocumentInfo(DocumentType documentType, DocumentState documentState, Document document)
        {
            #region Seleccion punto de Emisión
            int id_ep = 0;
            if (TempData["id_ep"] != null)
            {
                id_ep = (int)TempData["id_ep"];
            }
            id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);
            #endregion

            return new Document()
            {
                id_documentType = documentType?.id ?? 0,
                DocumentType = documentType,
                id_documentState = documentState?.id ?? 0,
                DocumentState = documentState,
                emissionDate = DateTime.Now,
                id_emissionPoint = id_ep,
                id_userCreate = ActiveUser.id,
                dateCreate = DateTime.Now,
                id_userUpdate = ActiveUser.id,
                dateUpdate = DateTime.Now,
                sequential = GetDocumentSequential(documentType.id),
                number = GetDocumentNumber(documentType.id, id_ep), 
                description = document.description,
                id_documentTransactionState = document.id_documentTransactionState,
            };
        }

        private InvoiceDetail GetInvoiceDetailInfo(InvoiceDetail item, ItemWeightConversionFreezen itemWeightConversionFreezen)
        {
            return new InvoiceDetail()
            {
                amount = item.amount,
                amountDisplay = item.amountDisplay,
                amountInvoiceDisplay = item.amountInvoiceDisplay,
                amountproforma = item.amountproforma,
                amountProformaDisplay = item.amountProformaDisplay,
                auxCode_Inf = item.auxCode_Inf,
                cantidad_DetailOperation = item.cantidad_DetailOperation,
                codeMetricUnitOrigin_Inf = item.codeMetricUnitOrigin_Inf,
                codeMetricUnit_Inf = item.codeMetricUnit_Inf,
                codePresentation = item.codePresentation,
                dateCreate = item.dateCreate,
                dateUpdate = item.dateUpdate,
                db_DetailOperation = item.db_DetailOperation,
                description = item.description,
                descriptionAuxCode = item.descriptionAuxCode,
                descriptionCustomer = item.descriptionCustomer,
                discount = item.discount,
                expensesProforma = item.expensesProforma,
                factor_DetailOperation = item.factor_DetailOperation,
                foreignName_Inf = item.foreignName_Inf,
                hasGlaze_DetailOperation = item.hasGlaze_DetailOperation,
                idCompany = item.idCompany,
                idItem_DetailOperation = item.idItem_DetailOperation,
                id_amountInvoice = item.id_amountInvoice,
                id_attribute1 = item.id_attribute1,
                id_invoice = item.id_invoice,
                id_item = item.id_item,
                id_itemMarked = item.id_itemMarked,
                id_metricUnit = item.id_metricUnit,
                id_metricUnitInvoiceDetail = item.id_metricUnitInvoiceDetail,
                id_MetricUnitInvoice_DetailOperation = item.id_MetricUnitInvoice_DetailOperation,
                id_MetricUnit_DetailOperation = item.id_MetricUnit_DetailOperation,
                id_tariffHeadingDetail = item.id_tariffHeadingDetail,
                id_userCreate = item.id_userCreate,
                id_userUpdate = item.id_userUpdate,
                //InvoiceDetailsTaxes = item.InvoiceDetailsTaxes,
                isActive = item.isActive,
                Item = item.Item,
                Item1 = item.Item1,
                iva = item.iva,
                iva0 = item.iva0,
                ivaExented = item.ivaExented,
                ivaNoObject = item.ivaNoObject,
                masterCode = item.masterCode,
                masterCode_Inf = item.masterCode_Inf,
                MetricUnit = item.MetricUnit,
                MetricUnit1 = item.MetricUnit1,
                numBoxes = item.numBoxes,
                netWeight = itemWeightConversionFreezen.itemWeightNetWeight,
                percentageKiloProforma = item.percentageKiloProforma,
                pesoBasic_DetailOperation = item.pesoBasic_DetailOperation,
                pesoProformaTotal_DetailOperation = item.pesoProformaTotal_DetailOperation,
                pesoProforma_DetailOperation = item.pesoProforma_DetailOperation,
                pesoTotal_DetailOperation = item.pesoTotal_DetailOperation,
                peso_DetailOperation = item.peso_DetailOperation,
                precio_DetailOperation = item.precio_DetailOperation,
                presentationMaximum = item.presentationMaximum,
                presentationMinimum = item.presentationMinimum,
                proformaNumBoxesPlusMinus = item.proformaNumBoxesPlusMinus,
                proformaPendingDiscount = item.proformaPendingDiscount,
                proformaPorcVariationPlusMinus = item.proformaPorcVariationPlusMinus,
                proformaUsedDiscount = item.proformaUsedDiscount,
                proformaPendingNumBoxes = item.numBoxes,
                proformaUsedNumBoxes = 0,
                proformaWeight = item.proformaWeight,
                //SalesOrderDetailSalesQuotationExterior = item.SalesOrderDetailSalesQuotationExterior,
                subTotalIVA0Proforma = item.subTotalIVA0Proforma,
                TariffHeading = item.TariffHeading,
                total = item.total,
                totalKiloProforma = item.totalKiloProforma,
                totalPriceWithoutTax = item.totalPriceWithoutTax,
                totalProforma = item.totalProforma,
                total_DetailOperation = item.total_DetailOperation,
                unitPrice = item.unitPrice,
                unitPriceProforma = item.unitPriceProforma,
                valueICE = item.valueICE,
                valueIRBPNR = item.valueIRBPNR,
                weightBoxUM = item.weightBoxUM,
            };
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult GetInfoBank(int id_BankTransfer)
        {
            TempData.Keep("salesQuotationExterior");
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
                TempData.Keep("salesQuotationExterior");
            }

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }
        private SalesQuotationExterior GetSalesQuotationExteriorInfo(Invoice invoice)
        {
            return new SalesQuotationExterior()
            {
                idTipoTemperatura = invoice.SalesQuotationExterior.idTipoTemperatura,
                idVendor = invoice.SalesQuotationExterior.idVendor,
                id_addressCustomer = invoice.SalesQuotationExterior.id_addressCustomer,
                id_bank = invoice.SalesQuotationExterior.id_bank,
                id_capacityContainer = invoice.SalesQuotationExterior.id_capacityContainer,
                id_consignee = invoice.SalesQuotationExterior.id_consignee,
                id_metricUnitInvoice = invoice.SalesQuotationExterior.id_metricUnitInvoice,
                id_notifier = invoice.SalesQuotationExterior.id_notifier,
                id_PaymentMethod = invoice.SalesQuotationExterior.id_PaymentMethod,
                id_PaymentTerm = invoice.SalesQuotationExterior.id_PaymentTerm,
                id_personContact = invoice.SalesQuotationExterior.id_personContact,
                id_personContactConsignatario = invoice.SalesQuotationExterior.id_personContactConsignatario,
                id_portDestination = invoice.SalesQuotationExterior.id_portDestination,
                id_portDischarge = invoice.SalesQuotationExterior.id_portDischarge,
                id_portShipment = invoice.SalesQuotationExterior.id_portShipment,
                id_termsNegotiation = invoice.SalesQuotationExterior.id_termsNegotiation,
                id_portTerminal = invoice.SalesQuotationExterior.id_portTerminal,
                id_userCreate = invoice.SalesQuotationExterior.id_userCreate,
                id_userUpdate = invoice.SalesQuotationExterior.id_userUpdate,
                BoxCardAndBank = invoice.SalesQuotationExterior.BoxCardAndBank,
                CapacityContainer = invoice.SalesQuotationExterior.CapacityContainer,
                ColourGrade = invoice.SalesQuotationExterior.ColourGrade,
                ContainerDetails = invoice.SalesQuotationExterior.ContainerDetails,
                dateCreate = invoice.SalesQuotationExterior.dateCreate,
                dateShipment = invoice.SalesQuotationExterior.dateShipment,
                dateUpdate = invoice.SalesQuotationExterior.dateUpdate,
                direccion = invoice.SalesQuotationExterior.direccion,
                email = invoice.SalesQuotationExterior.email,
                //ForeignCustomerIdentification = invoice.SalesQuotationExterior.ForeignCustomerIdentification,
                //Invoice = invoice.SalesQuotationExterior.Invoice,
                MetricUnit = invoice.SalesQuotationExterior.MetricUnit,
                numBoxesPlusMinus = invoice.SalesQuotationExterior.numBoxesPlusMinus,
                pendingBoxes = invoice.SalesQuotationExterior.totalBoxes,
                numeroContenedores = invoice.SalesQuotationExterior.numeroContenedores,
                PackingDetails = invoice.SalesQuotationExterior.PackingDetails,
                //PaymentMethod = invoice.SalesQuotationExterior.PaymentMethod,
                //PaymentTerm = invoice.SalesQuotationExterior.PaymentTerm,
                //Person = invoice.SalesQuotationExterior.Person,
                //Person1 = invoice.SalesQuotationExterior.Person1,
                //Person2 = invoice.SalesQuotationExterior.Person2,
                //Person3 = invoice.SalesQuotationExterior.Person3,
                //Person4 = invoice.SalesQuotationExterior.Person4,
                //Port = invoice.SalesQuotationExterior.Port,
                //Port1 = invoice.SalesQuotationExterior.Port1,
                //Port2 = invoice.SalesQuotationExterior.Port2,
                //Port3 = invoice.SalesQuotationExterior.Port3,
                Product = invoice.SalesQuotationExterior.Product,
                purchaseOrder = invoice.SalesQuotationExterior.purchaseOrder,
                //SalesOrderDetailSalesQuotationExterior = invoice.SalesQuotationExterior.SalesOrderDetailSalesQuotationExterior,
                //SalesQuotationExteriorDocument = invoice.SalesQuotationExterior.SalesQuotationExteriorDocument,
                Shipment_date = invoice.SalesQuotationExterior.Shipment_date,
                strDateShipment = invoice.SalesQuotationExterior.strDateShipment,
                temperatureInstrucDate = invoice.SalesQuotationExterior.temperatureInstrucDate,
                temperatureInstruction = invoice.SalesQuotationExterior.temperatureInstruction,
                //TermsNegotiation = invoice.SalesQuotationExterior.TermsNegotiation,
                totalBoxes = invoice.SalesQuotationExterior.totalBoxes,
                transport = invoice.SalesQuotationExterior.transport,
                trip = invoice.SalesQuotationExterior.trip,
                usedBoxes = invoice.SalesQuotationExterior.usedBoxes,
                valueCustomsExpenditures = invoice.SalesQuotationExterior.valueCustomsExpenditures,
                valueInternationalFreight = invoice.SalesQuotationExterior.valueInternationalFreight,
                valueInternationalInsurance = invoice.SalesQuotationExterior.valueInternationalInsurance,
                valueSubscribed = invoice.SalesQuotationExterior.valueSubscribed,
                valuetotalCIF = invoice.SalesQuotationExterior.valuetotalCIF,
                valueTotalFOB = invoice.SalesQuotationExterior.valueTotalFOB,
                valueTransportationExpenses = invoice.SalesQuotationExterior.valueTransportationExpenses,
                vessel = invoice.SalesQuotationExterior.vessel,
                id_BankTransfer = invoice.SalesQuotationExterior.id_BankTransfer,
                PO = invoice.SalesQuotationExterior.PO,
                noContrato = invoice.SalesQuotationExterior.noContrato,
            };
        } 
        #endregion

        [HttpPost]
        public ActionResult GetPaymentTerm()
        {
            Invoice invoice = ObtainInvoice(0);
            return PartialView("_TabSalesQuotationPaymentTerm", invoice);
        }

        [HttpPost]
        public ActionResult InvoiceExternalTotales(decimal valueTransportationExpenses,
                                                    decimal valueCustomsExpenditures,
                                                    decimal valueInternationalInsurance,
                                                    decimal valueInternationalFreight)
        {
            Invoice invoice = ObtainInvoice(0);

            invoice.SalesQuotationExterior = invoice.SalesQuotationExterior ?? new SalesQuotationExterior();
            invoice.SalesQuotationExterior.valueTransportationExpenses = valueTransportationExpenses;
            invoice.SalesQuotationExterior.valueCustomsExpenditures = valueCustomsExpenditures;
            invoice.SalesQuotationExterior.valueInternationalInsurance = valueInternationalInsurance;
            invoice.SalesQuotationExterior.valueInternationalFreight = valueInternationalFreight;

            string[] configDec = new string[2];
            invoice.valueTransportationExpensesTruncate = valueTransportationExpenses.ToAdvanceDecimal(configDec, out configDec);  //(decimal)(Math.Truncate(valueTransportationExpenses * 100) / 100);
            invoice.valueCustomsExpendituresTruncate = valueCustomsExpenditures.ToAdvanceDecimal(configDec, out configDec); //(decimal)(Math.Truncate(valueCustomsExpenditures * 100) / 100);
            invoice.valueInternationalInsuranceTruncate = valueInternationalInsurance.ToAdvanceDecimal(configDec, out configDec); //(decimal)(Math.Truncate(valueInternationalInsurance * 100) / 100);
            invoice.valueInternationalFreightTruncate = valueInternationalFreight.ToAdvanceDecimal(configDec, out configDec);  //(decimal)(Math.Truncate(valueInternationalFreight * 100) / 100);

            invoice.calculateTotales();
            calculateTotalesSalesQuotationExterior(invoice);

            return PartialView("_TabDetailsExternalTotales", invoice);
        }

        private void calculateTotalesSalesQuotationExterior(Invoice invoice)
        {
            string[] configDec = new string[2];
            if (invoice.SalesQuotationExterior == null) return;

            if (invoice.InvoiceDetail == null || invoice.InvoiceDetail?.Count() == 0)

            {
                invoice.SalesQuotationExterior.valueTotalFOB = 0;
                invoice.valueTotalFOBTruncate = 0;
            }
            else
            {
                invoice.SalesQuotationExterior.valueTotalFOB = invoice.totalValue;
                invoice.valueTotalFOBTruncate = invoice.totalValueTruncate;
                // error en la etyiqueta es CFR
            }

            invoice.SalesQuotationExterior.valuetotalCIF = (invoice.SalesQuotationExterior.valueTotalFOB +
                                                    invoice.SalesQuotationExterior.valueCustomsExpenditures +
                                                     invoice.SalesQuotationExterior.valueInternationalFreight +
                                                     invoice.SalesQuotationExterior.valueInternationalInsurance +
                                                     invoice.SalesQuotationExterior.valueTransportationExpenses);

            invoice.valuetotalCIFTruncate = (invoice.valueTotalFOBTruncate +
                                                    invoice.SalesQuotationExterior.valueCustomsExpenditures +
                                                     invoice.SalesQuotationExterior.valueInternationalFreight +
                                                     invoice.SalesQuotationExterior.valueInternationalInsurance +
                                                     invoice.SalesQuotationExterior.valueTransportationExpenses); ;
        }

        [HttpPost]
        public ActionResult InvoiceExternalWeight()
        {
            Invoice invoice = ObtainInvoice(0);

            if (invoice != null)
            {
                invoice.saveWeight(db);
                invoice.calculateTotalBoxes_SalesQuotationExterior();
            }

            return PartialView("_TabDetailsInfoAdicional", invoice);
        }

        #endregion {RA} Invoice Edit

        #region Busqueda

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorResults(ParametersSeek _parameters)
        {
            TempData["ParametersSeek"] = _parameters;
            TempData.Keep("ParametersSeek");

            List<Invoice> modelFX = null;

            modelFX = SeekSalesQuotationExterior(_parameters);
            TempData["modelFX"] = modelFX;
            TempData.Keep("modelFX");

            return PartialView("_ResultsPartial", modelFX.OrderByDescending(o => o.id).ToList());
        }

        #endregion Busqueda

        #region Paginacion

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_invoice)
        {
            //TempData.Keep("salesQuotationExterior");
            TempData.Keep("modelFX");
            TempData.Keep("ParametersSeek");

            List<Invoice> invoices = (List<Invoice>)TempData["modelFX"];

            int index = invoices.Where(d => d.tbsysInvoiceType.code.Equals("PRO")).OrderByDescending(r => r.id).ToList()
                .FindIndex(r => r.id == id_invoice);

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
            TempData.Keep("ParametersSeek");
            List<Invoice> invoices = (List<Invoice>)TempData["modelFX"];

            Invoice invoice = invoices.Where(d => d.tbsysInvoiceType.code.Equals("PRO")).OrderByDescending(p => p.id).Take(page).ToList().Last();
            if (invoice != null)
            {
                SetSalesQuotationExterior(invoice);
            }
            invoice = (invoice == null) ? new Invoice() : invoice;

            TempData.Keep("modelFX");

            return PartialView("_EditForm", invoice);
        }

        #endregion Paginacion

        #region InvoiceDetail

        [ValidateInput(false)]
        public ActionResult SalesQuotationExteriorDetail(int? id_invoice, int? id_Buyer, bool busquedaNombre,
                                                        string nameItemFilter, int? sizeBegin, int? sizeEnd,
                                                        int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
                                                        int? id_group, int? id_subgroup, int? id_size,
                                                        int? id_trademark, int? id_trademarkModel, int? id_color,
                                                        string nameCodigoItemFilter)
        {
            Invoice invoice = ObtainInvoice(id_invoice);

            var model = invoice.InvoiceDetail?.ToList() ?? new List<InvoiceDetail>();

                var aInvoiceDetail = db.InvoiceDetail.Where(w => w.isActive && w.id_invoice == invoice.id).ToList();
                foreach (var item in aInvoiceDetail)
                {
                    var aModel = model.FirstOrDefault(fod => fod.id == item.id);
                    var aUsedNumBoxes = item.proformaUsedNumBoxes - aModel.proformaUsedNumBoxes;
                    var aPendingNumBoxes = aModel.proformaPendingNumBoxes - aUsedNumBoxes;
                    var aUsedNumBoxes2 = aModel.proformaUsedNumBoxes + aUsedNumBoxes;
                    aModel.proformaPendingNumBoxes = aPendingNumBoxes < 0 ? 0 : aPendingNumBoxes;
                    aModel.proformaUsedNumBoxes = aUsedNumBoxes2 < 0 ? 0 : aUsedNumBoxes2;
                }

                ExludeItemByEditRow(Request.Params["__DXCallbackArgument"], model, invoice);

                TempData.Keep("id_Items");
                TempData.Keep("amountDetail");
             
                model = invoice.InvoiceDetail?.Where(r => r.isActive).ToList();
                this.ViewBag.Id_buyer = id_Buyer;
                this.ViewBag.busquedaNombre = busquedaNombre;
                this.ViewBag.nameItemFilter = nameItemFilter;
                this.ViewBag.sizeBegin = sizeBegin;
                this.ViewBag.sizeEnd = sizeEnd;
                this.ViewBag.id_inventoryLine = id_inventoryLine;
                this.ViewBag.id_itemType = id_itemType;
                this.ViewBag.id_itemTypeCategory = id_itemTypeCategory;
                this.ViewBag.id_group = id_group;
                this.ViewBag.id_subgroup = id_subgroup;
                this.ViewBag.id_size = id_size;
                this.ViewBag.id_trademark = id_trademark;
                this.ViewBag.id_trademarkModel = id_trademarkModel;
                this.ViewBag.id_color = id_color;
                this.ViewBag.nameCodigoItemFilter = nameCodigoItemFilter;

            return PartialView("_TabDetailsGridViewProduct", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorDetailAddNew(int id_invoice, InvoiceDetail invoiceDetail)
        {
            Invoice invoice = ObtainInvoice(id_invoice);
            amoutInfoTransmit amountData = ((amoutInfoTransmit)TempData["amountDetail"]) ?? CalculaCantidadInt(invoiceDetail.id_item, (int)invoiceDetail.numBoxes, (int)invoiceDetail.id_metricUnitInvoiceDetail);
            if (ModelState.IsValid)
            {
                try
                {
                    Random rnd = new Random();
                    int? newId = rnd.Next(-9999999, -999);

                    invoiceDetail.addNew((int)newId, ActiveUser);
                    invoiceDetail.amount = amountData.cantidadItem;
                    invoiceDetail.id_amountInvoice = amountData.cantidadFactura;
                    invoiceDetail.id_metricUnitInvoiceDetail = (invoiceDetail.id_metricUnitInvoiceDetail == 999) ? invoiceDetail.id_metricUnit : invoiceDetail.id_metricUnitInvoiceDetail;

                    List<InvoiceDetail> _invoiceDetailList = new List<InvoiceDetail>();
                    _invoiceDetailList.Add(invoiceDetail);
                    invoice.addBulkDetail(_invoiceDetailList, ActiveUser);
                    invoice.calculateTotales();

                    TempData["id_Items"] = invoice.getId_Items(null);
                    TempData.Keep("id_Items");

                    //TempData["salesQuotationExterior"] = invoice;

                    TempData.Remove("amountDetail");
                }
                catch (Exception e)
                {
                    TempData.Keep("amountDetail");
                    ViewData["EditError"] = e.Message;
                }
                finally
                {
                    //TempData.Keep("salesQuotationExterior");
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            var model = invoice.InvoiceDetail.Where(r => r.isActive).ToList();// corregir modelo  isActive
            if (model != null)
            {
                model = model.ToList();
            }
            else
            {
                model = new List<InvoiceDetail>();
            }

            return PartialView("_TabDetailsGridViewProduct", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorDetailUpdate(InvoiceDetail invoiceDetail)
        {
            List<InvoiceDetail> model = new List<InvoiceDetail>();
            amoutInfoTransmit amountData = ((amoutInfoTransmit)TempData["amountDetail"]) ?? CalculaCantidadInt(invoiceDetail.id_item, (int)invoiceDetail.numBoxes, (int)invoiceDetail.id_metricUnitInvoiceDetail);

            try
            {
                Invoice invoice = ObtainInvoice(invoiceDetail.id_invoice);

                //model = InvoiceDetailUpdateDelete(invoice, invoiceDetail.id, false, amountData);

                var modelinvoiceDetail = invoice.InvoiceDetail.FirstOrDefault(i => i.id == invoiceDetail.id);
                if (modelinvoiceDetail != null)
                {
                    if (amountData != null)
                    {
                        modelinvoiceDetail.amount = amountData.cantidadItem;
                        modelinvoiceDetail.id_amountInvoice = amountData.cantidadFactura;
                        modelinvoiceDetail.id_metricUnitInvoiceDetail = (modelinvoiceDetail.id_metricUnitInvoiceDetail == 999) ? modelinvoiceDetail.id_metricUnit : modelinvoiceDetail.id_metricUnitInvoiceDetail;
                    }

                    modelinvoiceDetail.amountproforma = invoiceDetail.amountproforma;
                    modelinvoiceDetail.descriptionCustomer = invoiceDetail.descriptionCustomer;
                    modelinvoiceDetail.totalProforma = invoiceDetail.totalProforma;
                    modelinvoiceDetail.discount = invoiceDetail.discount;
                    modelinvoiceDetail.unitPrice = invoiceDetail.unitPrice;
                    modelinvoiceDetail.unitPriceProforma = invoiceDetail.unitPriceProforma;
                    modelinvoiceDetail.netWeight = invoiceDetail.netWeight;
                    modelinvoiceDetail.proformaWeight = invoiceDetail.proformaWeight;
                    modelinvoiceDetail.valueICE = invoiceDetail.valueICE;
                    modelinvoiceDetail.valueIRBPNR = invoiceDetail.valueIRBPNR;
                    modelinvoiceDetail.numBoxes = invoiceDetail.numBoxes;
                    modelinvoiceDetail.proformaUsedNumBoxes = invoiceDetail.proformaUsedNumBoxes;
                    modelinvoiceDetail.proformaPendingNumBoxes = invoiceDetail.proformaPendingNumBoxes;
                    modelinvoiceDetail.proformaNumBoxesPlusMinus = invoiceDetail.proformaNumBoxesPlusMinus;
                    modelinvoiceDetail.proformaPorcVariationPlusMinus = invoiceDetail.proformaPorcVariationPlusMinus;
                    modelinvoiceDetail.amountDisplay = invoiceDetail.amountDisplay;
                    modelinvoiceDetail.amountInvoiceDisplay = invoiceDetail.amountInvoiceDisplay;
                    modelinvoiceDetail.amountProformaDisplay = invoiceDetail.amountProformaDisplay;
                    modelinvoiceDetail.id_item = invoiceDetail.id_item;
                    modelinvoiceDetail.id_itemMarked = invoiceDetail.id_itemMarked;
                    modelinvoiceDetail.id_userUpdate = ActiveUser.id;
                    modelinvoiceDetail.dateUpdate = DateTime.Now;
                    modelinvoiceDetail.calculateTotal();
                }

                model = invoice.InvoiceDetail?.Where(x => x.isActive).ToList() ?? new List<InvoiceDetail>();
                model = (model.Count() != 0) ? model : new List<InvoiceDetail>();

                invoice.calculateTotales();

                TempData["id_Items"] = invoice.getId_Items(null);
                TempData.Keep("id_Items");
                TempData.Remove("amountDetail");
            }
            catch (Exception e)
            {
                TempData.Keep("amountDetail");
                ViewData["EditError"] = e.Message;
            }

            return PartialView("_TabDetailsGridViewProduct", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorDetailDelete(int id_invoice, int id)
        {
            List<InvoiceDetail> model = new List<InvoiceDetail>();
            try
            {
                Invoice invoice = ObtainInvoice(id_invoice);
                var modelinvoiceDetail = invoice.InvoiceDetail.FirstOrDefault(i => i.id == id);
                var puedeModDatosProforma = db.Setting.FirstOrDefault(e => e.code == "MODINFP")?.value == "SI";
                if (puedeModDatosProforma)
                {
                    var idEstadoPendiente = db.DocumentState.FirstOrDefault(e => e.code == "01")?.id;
                    var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05")?.id;

                    var idTipoDocFactComercial = db.DocumentType.FirstOrDefault(r => r.code.Equals("70"))?.id;
                    var idTipoDocFactFiscal = db.DocumentType.FirstOrDefault(r => r.code.Equals("07"))?.id;

                    #region Verificación de facturas Fiscales relacionadas
                    var facturasFiscales = db.Document.Where(e => e.id_documentOrigen == id_invoice &&
                           e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoPendiente
                           && e.id_documentState != idEstadoAnulado))
                       .ToList();
                    var idsFacturaFiscales = facturasFiscales.Select(e => e.id).ToList();

                    if (facturasFiscales.Any())
                    {
                        var detallesFacturaFiscal = db.InvoiceDetail
                            .Where(e => idsFacturaFiscales.Contains(e.id_invoice) && e.isActive && e.id_item == modelinvoiceDetail.id_item)
                            .ToList();

                        if (detallesFacturaFiscal.Any())
                        {
                            throw new Exception($"El Producto consta en alguna factura fiscal relacionada, no puede eliminar el mismo.");
                        }

                        // Verificamos las facturas comerciales relacionadas
                        var facturasComercialesFF = db.Document.Where(e => idsFacturaFiscales.Contains(e.id_documentOrigen ?? 0) &&
                                e.id_documentType == idTipoDocFactComercial && (e.id_documentState != idEstadoPendiente
                                && e.id_documentState != idEstadoAnulado))
                            .ToList();

                        var idsFacturasComercialesFF = facturasComercialesFF.Select(e => e.id).ToList();
                        if (facturasComercialesFF.Any())
                        {
                            var detallesFacturaComercial = db.InvoiceCommercialDetail
                                .Where(e => idsFacturasComercialesFF.Contains(e.id_invoiceCommercial) && e.isActive &&
                                    e.id_item == modelinvoiceDetail.id_item)
                                .ToList();

                            if (detallesFacturaComercial.Any())
                            {
                                throw new Exception($"El Producto consta en alguna factura comercial relacionada, no puede eliminar el mismo.");
                            }
                        }
                    }
                    #endregion

                    #region Verificación de facturas Comerciales relacionadas
                    var facturasComerciales = db.Document.Where(e => e.id_documentOrigen == id_invoice &&
                           e.id_documentType == idTipoDocFactComercial && (e.id_documentState != idEstadoPendiente
                           && e.id_documentState != idEstadoAnulado))
                       .ToList();
                    var idsFacturasComerciales = facturasComerciales.Select(e => e.id).ToList();

                    if (facturasComerciales.Any())
                    {
                        var detallesFacturaComercial = db.InvoiceCommercialDetail
                            .Where(e => idsFacturasComerciales.Contains(e.id_invoiceCommercial) && e.isActive &&
                                e.id_item == modelinvoiceDetail.id_item)
                            .ToList();

                        if (detallesFacturaComercial.Any())
                        {
                            throw new Exception($"El Producto consta en alguna factura comercial relacionada, no puede eliminar el mismo.");
                        }

                        // Verificamos las facturas fiscales relacionadas
                        var facturasFiscalesFC = db.Document.Where(e => idsFacturasComerciales.Contains(e.id_documentOrigen ?? 0) &&
                            e.id_documentType == idTipoDocFactFiscal && (e.id_documentState != idEstadoPendiente
                            && e.id_documentState != idEstadoAnulado))
                        .ToList();

                        var idsFacturasFiscalesFC = facturasFiscalesFC.Select(e => e.id).ToList();
                        if (facturasFiscalesFC.Any())
                        {
                            var detallesFacturaFiscal = db.InvoiceDetail
                               .Where(e => idsFacturasFiscalesFC.Contains(e.id_invoice) && e.isActive && e.id_item == modelinvoiceDetail.id_item)
                               .ToList();

                            if (detallesFacturaFiscal.Any())
                            {
                                throw new Exception($"El Producto consta en alguna factura fiscal relacionada, no puede eliminar el mismo.");
                            }
                        }                        
                    }
                    #endregion
                }
                else
                {
                    if (modelinvoiceDetail != null && modelinvoiceDetail.proformaUsedNumBoxes > 0)
                    {
                        model = invoice.InvoiceDetail.Where(x => x.isActive).ToList();
                        throw new Exception("No se puede eliminar el detalle por tener Cajas Facturadas");
                    }

                    var dc = invoice.Document;
                    var dc1 = invoice.Document.Document1;

                    bool itsProductOnInvoice = false;
                    string tipoFactura = string.Empty;
                    if ((invoice.Document != null) && (invoice.Document.Document1 != null))
                    {
                        foreach (var document in invoice.Document.Document1)
                        {
                            var codeDocType = document.DocumentType.code;
                            if (codeDocType == "07")
                            {
                                itsProductOnInvoice = document.Invoice.InvoiceDetail
                                    .Any(fod2 => fod2.isActive && fod2.id_item == modelinvoiceDetail.id_item);

                                tipoFactura = "Fiscal";
                            }
                            else if (codeDocType == "70")
                            {
                                itsProductOnInvoice = document.InvoiceCommercial.InvoiceCommercialDetail
                                    .Any(fod2 => fod2.isActive && fod2.id_item == modelinvoiceDetail.id_item);

                                tipoFactura = "Comercial";
                            }

                            if (itsProductOnInvoice) break;
                        }
                    }

                    if (itsProductOnInvoice)
                    {
                        model = invoice.InvoiceDetail.Where(x => x.isActive).ToList();
                        throw new Exception($"El Producto consta en alguna factura {tipoFactura} relacionada, no puede eliminar el mismo.");
                    }
                }
                    
                model = InvoiceDetailUpdateDelete(invoice, id, true, null);
                invoice.calculateTotales();

                TempData["id_Items"] = invoice.getId_Items(null);
                TempData.Keep("id_Items");
            }
            catch (Exception e)
            {
                TempData.Keep("amountDetail");
                ViewData["EditError"] = e.Message;
            }

            return PartialView("_TabDetailsGridViewProduct", model);
        }

        private List<InvoiceDetail> InvoiceDetailUpdateDelete(Invoice invoice, int id_invoiceDetail, bool isDelete, amoutInfoTransmit amountData = null)
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
                            modelinvoiceDetail.id_amountInvoice = amountData.cantidadFactura;
                            modelinvoiceDetail.id_metricUnitInvoiceDetail = (modelinvoiceDetail.id_metricUnitInvoiceDetail == 999) ? modelinvoiceDetail.id_metricUnit : modelinvoiceDetail.id_metricUnitInvoiceDetail;
                        }

                        if (isDelete) modelinvoiceDetail.isActive = false;
                        List<InvoiceDetail> _invoiceDetailList = new List<InvoiceDetail>();
                        _invoiceDetailList.Add(modelinvoiceDetail);
                        invoice.addBulkDetail(_invoiceDetailList, ActiveUser);

                        //this.UpdateModel(modelinvoiceDetail);
                    }

                    //TempData["salesQuotationExterior"] = invoice;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData.Keep("salesQuotationExterior");
            TempData.Keep("id_Items");

            List<InvoiceDetail> model = invoice.InvoiceDetail?.Where(x => x.isActive).ToList() ?? new List<InvoiceDetail>();
            model = (model.Count() != 0) ? model : new List<InvoiceDetail>();

            return model;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemCombobox(int? id_Buyer, bool busquedaNombre,
            string nameItemFilter, int? sizeBegin, int? sizeEnd,
            int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
            int? id_group, int? id_subgroup, int? id_size,
            int? id_trademark, int? id_trademarkModel, int? id_color,
            string nameCodigoItemFilter)
        {
            int? id_company = (int?)ViewData["id_company"];
            //List<int> id_items = (List<int>)ViewData["id_items"];

            MVCxColumnComboBoxProperties p = CreateComboBoxColumnProperties(id_company, id_Buyer, busquedaNombre,
                nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
                id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter);
            TempData.Keep("amountDetail");
            TempData.Keep("id_Items");
            this.ViewBag.busquedaNombre = busquedaNombre;
            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        public static MVCxColumnComboBoxProperties CreateComboBoxColumnProperties(int? id_company, int? id_Buyer, bool busquedaNombre,
                                                                                    string nameItemFilter, int? sizeBegin, int? sizeEnd,
                                                                                    int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
                                                                                    int? id_group, int? id_subgroup, int? id_size,
                                                                                    int? id_trademark, int? id_trademarkModel, int? id_color,
                                                                                    string nameCodigoItemFilter)
        {
            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "SalesQuotationExterior", Action = "LoadItemCombobox" };
            p.ClientInstanceName = "id_item";
            p.ValueField = "id";
            p.TextFormatString = (busquedaNombre != true) ? "{0} | {1} | {2}" : "{0} | {2} | {1}";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 20;
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            p.Columns.Add("masterCode", "Código.", 20);//, Unit.Percentage(50));
            p.Columns.Add("name", "Nombre del Producto", 80);
            p.Columns.Add("foreignName", "Nombre Extranjero", 80);
            p.Columns.Add("auxCode", "Cod.Aux.", 40);
            p.Columns.Add("Presentation.MetricUnit.code", "U.M.", 10);
            p.Columns.Add("ItemWeightConversionFreezen.itemWeightNetWeight", "PesoNeto", 20);
            p.Columns.Add("ItemWeightConversionFreezen.weightWithGlaze", "PesoProforma", 20);

            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.ClientSideEvents.SelectedIndexChanged = "ItemCombo_SelectedIndexChanged";
            p.ClientSideEvents.BeginCallback = "ItemCombo_BeginCallback";
            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
            p.ClientSideEvents.Validation = "ItemComboValidation";
            p.ClientSideEvents.Init = "ItemComboInit";
            p.BindList(DXPANACEASOFT.DataProviders.DataProviderItem.SalesItemsBuyerByCompany(id_company, id_Buyer, null, nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
                id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter));
            return p;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemMarkedCombobox()
        {
            int? id_company = (int?)ViewData["id_company"];

            MVCxColumnComboBoxProperties p = CreateComboBoxColumnMarkedProperties(id_company);
            TempData.Keep("amountDetail");
            TempData.Keep("id_Items");
            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        public static MVCxColumnComboBoxProperties CreateComboBoxColumnMarkedProperties(int? id_company)
        {
            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "SalesQuotationExterior", Action = "LoadItemMarkedCombobox" };
            p.ClientInstanceName = "id_itemMarked";
            p.ValueField = "id";
            p.TextFormatString = "{0} | {1} | {2}";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 20;
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            p.Columns.Add("masterCode", "Código.", 80);
            p.Columns.Add("name", "Nombre del Producto", 200);
            p.Columns.Add("foreignName", "Descripcion", 200);

            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
            p.ClientSideEvents.Validation = "ItemMarkedComboValidation";
            p.ClientSideEvents.Init = "ItemMarkedComboInit";
            p.BindList(DXPANACEASOFT.DataProviders.DataProviderItem.SalesItemsByCompany(id_company, null));
            return p;
        }

        [HttpPost, ValidateInput(false)]
        public void SalesQuotationExteriorDetailsDeleteSeleted(int[] ids)
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
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
        }

        #endregion InvoiceDetail

        #region InvoicesRelations

        [HttpPost, ValidateInput(false)]
        public ActionResult PartialInvoiceRelationList()
        {
            var model = new List<Document>();
            var invoice = GetSalesQuotationExterior();
            if (invoice != null)
            {
                model = db.Document.Where(s => s.id_documentOrigen == invoice.id).ToList();
            }

            return PartialView("_InvoicesRelationView", model);
        }

        public ActionResult InvoiceRelationList()
        {
            var model = new List<Document>();
            var invoice = GetSalesQuotationExterior();
            if (invoice != null)
            {
                model = db.Document.Where(s => s.id_documentOrigen == invoice.id).ToList();
            }

            return PartialView("_InvoicesRelationView", model);
        }

        #endregion InvoicesRelations

        #region Clases Aux.

        public class amoutInfoTransmit
        {
            public bool isError { get; set; }
            public string ErrorMessage { get; set; }
            public decimal cantidadItem { get; set; }
            public decimal cantidadFactura { get; set; }
            public string cantidadDisplay { get; set; }
            public string cantidadInvoiceDisplay { get; set; }
        }

        public class ParametersSeek
        {
            public int? id_documentState { get; set; }
            public string number { get; set; }
            public string reference { get; set; }
            public string purchaseOrder { get; set; }
            public string fechaEmisionDesde { get; set; }
            public string fechaEmisionHasta { get; set; }
            public int? id_customer { get; set; }
            public int? id_consignee { get; set; }
            public int? id_seller { get; set; }
            public List<int> items { get; set; }
        }

        #endregion Clases Aux.

        #region Auxiliar

        private Invoice ObtainInvoice(int? id_invoice)
        {
            Invoice invoice = GetSalesQuotationExterior();

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
            catch //(Exception e)
            {
                TempData.Keep("id_Items");
            }
        }

        private List<Invoice> SeekSalesQuotationExterior(ParametersSeek parameterToSeek)
        {
            var modelFx = db.Invoice.Where(r => r.SalesQuotationExterior != null);

            var modelTemp = new List<Invoice>();

            if (!string.IsNullOrEmpty(parameterToSeek.number))
            {
                modelTemp = modelFx.Where(r =>
                    r.Document.number.Contains(parameterToSeek.number)).ToList();
            }
            else
            {
                if (parameterToSeek.fechaEmisionDesde != null)
                {
                    var fromStr = parameterToSeek.fechaEmisionDesde + " 00:00:00 AM";
                    var dateFrom = DateTime.ParseExact(fromStr, "dd/MM/yyyy hh:mm:ss tt",
                        System.Globalization.CultureInfo.InvariantCulture);

                    modelFx = modelFx.Where(r =>
                        r.Document.emissionDate != null && r.Document.emissionDate.CompareTo(dateFrom) >= 0);
                }

                if (parameterToSeek.fechaEmisionHasta != null)
                {
                    var toStr = parameterToSeek.fechaEmisionHasta + " 11:59:59 PM";
                    var dateTo = DateTime.ParseExact(toStr, "dd/MM/yyyy hh:mm:ss tt",
                        System.Globalization.CultureInfo.InvariantCulture);

                    modelFx = modelFx.Where(r =>
                        r.Document.emissionDate != null && r.Document.emissionDate.CompareTo(dateTo) <= 0);
                }

                if (parameterToSeek.id_documentState != null && parameterToSeek.id_documentState != 0)
                {
                    modelFx = modelFx.Where(r =>
                        r.Document.id_documentState == parameterToSeek.id_documentState);
                }

                if (parameterToSeek.id_customer != null && parameterToSeek.id_customer != 0)
                {
                    modelFx = modelFx.Where(r =>
                        r.Person.Customer.id == parameterToSeek.id_customer);
                }

                if (parameterToSeek.id_consignee != null && parameterToSeek.id_consignee != 0)
                {
                    modelFx = modelFx.Where(r =>
                        r.SalesQuotationExterior.id_consignee == parameterToSeek.id_consignee);
                }

                if (parameterToSeek.id_seller != null && parameterToSeek.id_seller != 0)
                {
                    modelFx = modelFx.Where(r =>
                        r.id_seller == parameterToSeek.id_seller);
                }

                if (!string.IsNullOrEmpty(parameterToSeek.reference))
                {
                    modelFx = modelFx.Where(r =>
                        r.Document.reference.Contains(parameterToSeek.reference));
                }

                if (!string.IsNullOrEmpty(parameterToSeek.purchaseOrder))
                {
                    modelFx = modelFx.Where(r =>
                        r.SalesQuotationExterior.purchaseOrder.Contains(parameterToSeek.purchaseOrder));
                }

                if (parameterToSeek.items == null || parameterToSeek.items.Count <= 0)
                    return modelFx.ToList();

                if (parameterToSeek.items.Count == 1 && parameterToSeek.items[0] == 0)
                    return modelFx.ToList();

                foreach (var idItem in parameterToSeek.items)
                {
                    modelTemp.AddRange(modelFx.Where(r =>
                        r.InvoiceDetail.FirstOrDefault(d => d.id_item == idItem) != null).ToList());
                }
            }
            return modelTemp;
        }

        #endregion Auxiliar

        #region SALES QUOTATION EXTERIOR ATTACHMENT

        #region SALES QUOTATION EXTERIOR ATTACHED DOCUMENTS

        [ValidateInput(false)]
        public ActionResult SalesQuotationExteriorAttachedDocumentsPartial()
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var salesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;

            var model = salesQuotationExterior.SalesQuotationExteriorDocument;
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            return PartialView("_SalesQuotationExteriorAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.SalesQuotationExteriorDocument item)
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var salesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;

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
                            var salesQuotationExteriorDocumentDetailAux = salesQuotationExterior.
                                                    SalesQuotationExteriorDocument.
                                                    FirstOrDefault(fod => fod.attachment == item.attachment);
                            if (salesQuotationExteriorDocumentDetailAux != null)
                            {
                                throw new Exception("No se puede repetir el Documento Adjunto: " + item.attachment + ", en el detalle de los Documentos Adjunto.");
                            }
                        }
                    }
                    item.id = salesQuotationExterior.SalesQuotationExteriorDocument.Count() > 0 ? salesQuotationExterior.SalesQuotationExteriorDocument.Max(pld => pld.id) + 1 : 1;
                    salesQuotationExterior.SalesQuotationExteriorDocument.Add(item);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            var model = salesQuotationExterior.SalesQuotationExteriorDocument;
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            return PartialView("_SalesQuotationExteriorAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.SalesQuotationExteriorDocument item)
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var salesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = salesQuotationExterior.SalesQuotationExteriorDocument.FirstOrDefault(i => i.id == item.id);
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
                                var salesQuotationExteriorDocumentDetailAux = salesQuotationExterior.
                                                      SalesQuotationExteriorDocument.
                                                      FirstOrDefault(fod => fod.attachment == item.attachment);
                                if (salesQuotationExteriorDocumentDetailAux != null)
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
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            var model = salesQuotationExterior.SalesQuotationExteriorDocument;
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            return PartialView("_SalesQuotationExteriorAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationExteriorAttachedDocumentsPartialDelete(System.Int32 id)
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var salesQuotationExterior = aSalesQuotationExterior.SalesQuotationExterior;

            try
            {
                var item = salesQuotationExterior.SalesQuotationExteriorDocument.FirstOrDefault(it => it.id == id);
                if (item != null)
                    salesQuotationExterior.SalesQuotationExteriorDocument.Remove(item);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            var model = salesQuotationExterior.SalesQuotationExteriorDocument;
            TempData.Keep("id_Items");
            TempData.Keep("amountDetail");

            return PartialView("_SalesQuotationExteriorAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion SALES QUOTATION EXTERIOR ATTACHED DOCUMENTS

        private void UpdateAttachment(SalesQuotationExterior salesQuotationExterior)
        {
            List<SalesQuotationExteriorDocument> salesQuotationExteriorDocument = salesQuotationExterior.SalesQuotationExteriorDocument.ToList() ?? new List<SalesQuotationExteriorDocument>();
            foreach (var item in salesQuotationExteriorDocument)
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

                        item.guid = FileUploadHelper.FileUploadProcessAttachment("/SalesQuotationExterior/" + salesQuotationExterior.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
                        item.url = urlAux;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
                    }
                }
            }
        }

        private void DeleteAttachment(SalesQuotationExteriorDocument salesQuotationExteriorDocument)
        {
            if (salesQuotationExteriorDocument.url != FileUploadHelper.UploadDirectoryDefaultTemp)
            {
                try
                {
                    FileUploadHelper.CleanUpUploadedFiles(salesQuotationExteriorDocument.url, salesQuotationExteriorDocument.guid);
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
                var aSalesQuotationExterior = GetSalesQuotationExterior();
                List<SalesQuotationExteriorDocument> salesQuotationExteriorDocument = aSalesQuotationExterior.SalesQuotationExterior.SalesQuotationExteriorDocument.ToList() ?? new List<SalesQuotationExteriorDocument>();
                var salesQuotationExteriorDocumentAux = salesQuotationExteriorDocument.FirstOrDefault(fod => fod.id == id);
                if (salesQuotationExteriorDocumentAux != null)
                {
                    // Carga el contenido guardado en el temp
                    string nameAttachment;
                    string typeContentAttachment;
                    string guidAux = salesQuotationExteriorDocumentAux.guid;
                    string urlAux = salesQuotationExteriorDocumentAux.url;
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
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var salesQuotationExteriorDocumentDetailAux = aSalesQuotationExterior.SalesQuotationExterior.
                                                      SalesQuotationExteriorDocument.
                                                      FirstOrDefault(fod => fod.attachment == attachmentNameNew);
            if (salesQuotationExteriorDocumentDetailAux != null)
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

        #endregion SALES QUOTATION EXTERIOR ATTACHMENT

        #region Ajax-Request

        [HttpPost]
        public JsonResult CalculaCantidadCartones(int id_item, int numCajas, int id_metricUnitInvoice, decimal pesoProforma, decimal pesoNeto)
        {
            var JsonReturn = CalculaCantidadIntCartones(id_item, numCajas, id_metricUnitInvoice, pesoProforma, pesoNeto);

            TempData["amountDetail"] = JsonReturn;
            TempData.Keep("amountDetail");
            TempData.Keep("id_Items");

            return Json(JsonReturn, JsonRequestBehavior.AllowGet);
        }

        private amoutInfoTransmit CalculaCantidadIntCartones(int id_item,
                                                      int numCajas,
                                                      int id_metricUnitInvoice,
                                                      decimal pesoProforma, decimal pesoNeto)
        {
            amoutInfoTransmit CalculaCantidadIntReturn = null;
            decimal _cantidad = 0;
            decimal _cantidadInvoice = 0;
            string _cantidadDisplay = "";
            string _cantidadInvoiceDisplay = "";

            CalculaCantidadIntReturn = new amoutInfoTransmit { isError = true, ErrorMessage = "", cantidadItem = _cantidad, cantidadFactura = _cantidadInvoice, cantidadDisplay = _cantidadDisplay, cantidadInvoiceDisplay = _cantidadInvoiceDisplay };
            //decimal factor = 1;

            Invoice invoice = GetSalesQuotationExterior();//(TempData["salesQuotationExterior"] as Invoice);
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

                invoiceDetail.id_MetricUnitInvoice_DetailOperation = id_metricUnitInvoice;

                invoiceDetail.pesoBasic_DetailOperation = pesoNeto != 0 ? (decimal)invoiceDetail.numBoxes * pesoNeto : (decimal)invoiceDetail.numBoxes;
                invoiceDetail.pesoTotal_DetailOperation = pesoProforma != 0 ? (decimal)invoiceDetail.numBoxes * pesoProforma : (decimal)invoiceDetail.numBoxes;

                _cantidadDisplay = invoiceDetail.pesoBasic_DetailOperation.ToString("N2") + " " + _metricUnitOrigen.code;
                _cantidadInvoiceDisplay = invoiceDetail.pesoTotal_DetailOperation.ToString("N2") + " " + _metricUnitDestino.code;

                CalculaCantidadIntReturn.isError = false;
                CalculaCantidadIntReturn.ErrorMessage = "";
                CalculaCantidadIntReturn.cantidadItem = invoiceDetail.pesoBasic_DetailOperation;
                CalculaCantidadIntReturn.cantidadFactura = invoiceDetail.pesoTotal_DetailOperation;
                CalculaCantidadIntReturn.cantidadDisplay = _cantidadDisplay;
                CalculaCantidadIntReturn.cantidadInvoiceDisplay = _cantidadInvoiceDisplay;
            }
            catch //(Exception e)
            {
            }

            return CalculaCantidadIntReturn;
        }

        [HttpPost]
        public JsonResult CalculaCantidad(int id_item, int numCajas, int id_metricUnitInvoice)
        {
            var JsonReturn = CalculaCantidadInt(id_item, numCajas, id_metricUnitInvoice);

            TempData["amountDetail"] = JsonReturn;
            TempData.Keep("amountDetail");
            TempData.Keep("id_Items");

            return Json(JsonReturn, JsonRequestBehavior.AllowGet);
        }

        private amoutInfoTransmit CalculaCantidadInt(int id_item,
                                                      int numCajas,
                                                      int id_metricUnitInvoice)
        {
            amoutInfoTransmit CalculaCantidadIntReturn = null;
            decimal _cantidad = 0;
            decimal _cantidadInvoice = 0;
            string _cantidadDisplay = "";
            string _cantidadInvoiceDisplay = "";

            CalculaCantidadIntReturn = new amoutInfoTransmit { isError = true, ErrorMessage = "", cantidadItem = _cantidad, cantidadFactura = _cantidadInvoice, cantidadDisplay = _cantidadDisplay, cantidadInvoiceDisplay = _cantidadInvoiceDisplay };

            Invoice invoice = GetSalesQuotationExterior();//(TempData["salesQuotationExterior"] as Invoice);
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
                //invoiceDetail.unitPrice = 0;
                invoiceDetail.id_MetricUnitInvoice_DetailOperation = id_metricUnitInvoice;

                invoiceDetail.CalculateDetailInvoiceCommercialDetail();

                _cantidadDisplay = invoiceDetail.pesoBasic_DetailOperation.ToString("N2") + " " + _metricUnitOrigen.code;
                _cantidadInvoiceDisplay = invoiceDetail.pesoTotal_DetailOperation.ToString("N2") + " " + _metricUnitDestino.code;

                CalculaCantidadIntReturn.isError = false;
                CalculaCantidadIntReturn.ErrorMessage = "";
                CalculaCantidadIntReturn.cantidadItem = invoiceDetail.pesoBasic_DetailOperation;
                CalculaCantidadIntReturn.cantidadFactura = invoiceDetail.pesoTotal_DetailOperation;
                CalculaCantidadIntReturn.cantidadDisplay = _cantidadDisplay;
                CalculaCantidadIntReturn.cantidadInvoiceDisplay = _cantidadInvoiceDisplay;
            }
            catch //(Exception e)
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

                    invoice.SalesQuotationExterior.PaymentMethod = paymentMethod;
                    invoice.SalesQuotationExterior.id_PaymentMethod = (int)id_paymentMethod;
                }
                else
                {
                    invoice.SalesQuotationExterior.PaymentMethod = null;
                    invoice.SalesQuotationExterior.id_PaymentMethod = 0;
                }

                jresult = new { error = false, msgerr = "" };
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
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
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnSave = true,
                btnApprove = false,
                btnAutorize = false,
                btnAnnul = false,
                btnRevert = false,
                btnClose = false,
                btnPrint = true,
                btnExportExcel = true,
                btnUpdateData = true,
                btnDuplicate = true,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            Invoice invoice = db.Invoice.FirstOrDefault(r => r.id == id);
            string state = invoice.Document.DocumentState.code;

            if (state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnSave = true,
                    btnApprove = true,
                    btnAutorize = false,
                    btnAnnul = true,
                    btnRevert = false,
                    btnClose = false,
                    btnPrint = true,
                    btnExportExcel = true,
                    btnUpdateData = false,
                    btnDuplicate = true,
                };
            }
            else if (state == "03") // APROBADA
            {
                actions = new
                {
                    btnSave = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnAnnul = false,
                    btnRevert = true,
                    btnClose = true,
                    btnPrint = true,
                    btnExportExcel = true,
                    btnUpdateData = true,
                    btnDuplicate = true,
                };
            }
            else if (state == "05") // ANULADA
            {
                actions = new
                {
                    btnSave = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnAnnul = false,
                    btnRevert = false,
                    btnClose = false,
                    btnPrint = false,
                    btnExportExcel = false,
                    btnUpdateData = false,
                    btnDuplicate = false,
                };
            }
            else if (state == "04"/* || state == "09"*/) // CERRADA
            {
                actions = new
                {
                    btnSave = false,
                    btnApprove = false,
                    btnAutorize = false,
                    btnAnnul = false,
                    btnRevert = true,
                    btnClose = false,
                    btnPrint = true,
                    btnExportExcel = true,
                    btnUpdateData = true,
                    btnDuplicate = false,
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
            }
            catch (Exception e)
            {
                jresult = new { error = true, msgerr = e.Message };
            }
            finally
            {
            }

            return Json(jresult, JsonRequestBehavior.AllowGet);
        }

        #endregion Ajax-Request

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
        public JsonResult PrintSalesQuotationExteriorReport(int id_inv)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_inv;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RPFM";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        //-----------------------------------------------------------------------------------------------
        public JsonResult PrintSalesQuotationExteriorISFTempReport(int id, string codeReport)
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

        //--------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------
        public JsonResult PrintSalesQuotationExteriorSaleContractReport(int id, string codeReport)
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

        //--------------------------------------------------------------------------------

        [HttpPost, ValidateInput(false)]
        public JsonResult SalesQuotationExteriorReportFilterPartial(int id_invoice)
        {
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
        public JsonResult SalesQuotationExteriorExporExcel(int id_invoice)
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
            _repMod.codeReport = "RPEXEL";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;
            _repMod.nameReport = "Proforma";

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;

            List<ResultSalesQuotationExterior> modelAux = new List<ResultSalesQuotationExterior>();
            modelAux = db.Database.SqlQuery<ResultSalesQuotationExterior>
                    ("exec par_ProformasReport @id",
                    new SqlParameter("id", paramLst[0].Valor)
                    ).ToList();

            TempData["modelSalesQuotationExterior"] = modelAux;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult SalesQuotationExteriorPaymentTermViewDetailsPartial(
            int? idPaymentTerm, DateTime? emissionDate, decimal? invoiceTotal, bool? canEditPaymentTerm,
            List<InvoiceExteriorPaymentTerm> currentPaymentTermDetails)
        {
            ICollection<InvoiceExteriorPaymentTerm> invoiceExteriorPaymentTerm = null;

            // Recuperamos la Proforma actual, y nos aseguramos de que tiene la lista de plazos
            var invoice = this.ObtainInvoice(0);
            if (invoice == null)
                return this.PartialView("_TabSalesQuotationExteriorPaymentTermDetails",
                new List<InvoiceExteriorPaymentTerm>());

            try
            {
                invoiceExteriorPaymentTerm = invoice.InvoiceExteriorPaymentTerm;

                if (invoiceExteriorPaymentTerm == null)
                {
                    invoiceExteriorPaymentTerm = new List<InvoiceExteriorPaymentTerm>();
                    invoice.InvoiceExteriorPaymentTerm = invoice.InvoiceExteriorPaymentTerm;
                }

                // Removemos los detalles anteriores del elemento
                invoiceExteriorPaymentTerm.Clear();

                // Verificamos el monto de la Proforma
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
                invoice.InvoiceExteriorPaymentTerm = invoiceExteriorPaymentTerm;
            }

            this.ViewBag.CanEditPaymentTerm = canEditPaymentTerm;

            return this.PartialView("_TabSalesQuotationExteriorPaymentTermDetails",
                invoiceExteriorPaymentTerm);
        }

        private void PrepareInvoiceToSave(Invoice invoice, string jsonPaymentTermDetails)
        {
            var paymentTermDetails = Newtonsoft.Json.JsonConvert
                .DeserializeObject<InvoiceExteriorPaymentTerm[]>(jsonPaymentTermDetails);

            if (paymentTermDetails.Length == 0)
                paymentTermDetails = invoice.InvoiceExteriorPaymentTerm.ToArray();

            // Preparamos los detalles de edición y el documento recibido
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

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductOnInvoice(int id_item)
        {
            var result = new { itsProductOnInvoice = false };

            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var itsProductOnInvoice = (aSalesQuotationExterior.Document != null &&
                                       aSalesQuotationExterior.Document.Document1 != null)
                                       ? (aSalesQuotationExterior.Document.Document1.FirstOrDefault(fod => fod.DocumentState.code != "01" && fod.DocumentState.code != "02" && fod.DocumentState.code != "05" &&
                                                                                                           fod.Invoice.InvoiceDetail.FirstOrDefault(fod2 => fod2.isActive && fod2.id_item == id_item) != null) == null) : true;
            result = new { itsProductOnInvoice };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItem(int? id_itemNew)
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var aInvoiceDetail = aSalesQuotationExterior.InvoiceDetail.FirstOrDefault(fod => fod.id_item == id_itemNew && fod.isActive);
            if (aInvoiceDetail != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name + ",  en los detalles de la Proforma"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemMarked(int? id_itemNew)
        {
            var aSalesQuotationExterior = GetSalesQuotationExterior();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var aInvoiceDetail = aSalesQuotationExterior.InvoiceDetail.FirstOrDefault(fod => fod.id_itemMarked == id_itemNew && fod.isActive);
            if (aInvoiceDetail != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item Marcado: " + itemAux.name + ",  en los detalles de la Proforma"
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private async Task notify(Document document, string purchaseOrder)
        {
            string codeDocumentType = "131";
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            try
            {

                var idDocumentType = (db.DocumentType.FirstOrDefault(r => r.code == codeDocumentType)?.id??0);
                var config = EmailNotifyDocumentType.GetOneByIdDocumentType(idDocumentType);
                if (config == null) throw new Exception($"No esta configurado el tipo de documento con código {codeDocumentType}, para ser notificado");

                var destinariosPersons = EmailNotifyDocumentTypePerson.GetAllByEmailNotifyId(config.id);
                if((destinariosPersons?.Length??0) ==0 ) throw new Exception($"No están configurados los destinatarios");

                int[] personIds = destinariosPersons.Select(r => r.id_PersonReceiver).ToArray();
                Person[] persons = db.Person.Where(r => personIds.Contains(r.id)).ToArray();
                string destinariosEmails = persons
                                                .Where(r => !string.IsNullOrEmpty(r.email))
                                                .Select(r => r.email.Trim())
                                                .Aggregate((i, j) => $"{i};{j}");

                var nameUser = db.User.FirstOrDefault(r => r.id == document.id_userUpdate)?.Employee?.Person?.fullname_businessName;
                var fechaMoficacion = document.dateUpdate.ToDateFormat();
                var horaModificacion = document.dateUpdate.ToTimeFormat();

                string mailer = ConfigurationManager.AppSettings["correoDefaultDesde"];
                string host = ConfigurationManager.AppSettings["smtpHost"];
                int puerto = Int32.Parse(ConfigurationManager.AppSettings["puertoHost"]);
                string passwordSMTP = clsEncriptacion1.LeadnjirSimple.Desencriptar(ConfigurationManager.AppSettings["contrasenaCorreoDefault"]);

                string subject = $"Documento modificado: Proforma # {document.number}, OP # {purchaseOrder}";
                string body = $"Estimad@s <br>"+
                              $"El Documento: Proforma # {document.number}, OP # {purchaseOrder}, ha sido modificado por el usuario {nameUser}, Fecha: {fechaMoficacion}, Hora: {horaModificacion}";


                var resp = await clsCorreoElectronico.EnviarCorreoElectronicoAsync(destinariosEmails,
                                                                                    mailer,
                                                                                    subject,
                                                                                    host,
                                                                                    puerto,
                                                                                    passwordSMTP,
                                                                                    body,
                                                                                    ';',
                                                                                    rutalog: rutaLog);
                if (resp == "OK")
                {
                    LogInfo($"Envío notificación OK Proforma:{document.number}", DateTime.Now);
                }
                
            }
            catch (Exception e)
            {
                FullLog(e);
            }
        
        }
    }
}