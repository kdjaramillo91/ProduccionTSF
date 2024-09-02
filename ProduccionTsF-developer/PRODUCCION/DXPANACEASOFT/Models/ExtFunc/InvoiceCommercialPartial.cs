using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Data.Details;
using DXPANACEASOFT.Models.GenericProcess;
using DXPANACEASOFT.Models.ModelExtension;

namespace DXPANACEASOFT.Models
{
    public partial class InvoiceCommercial: InvoiceOperation
    {

        private DBContext db = new DBContext();

        public SecurityControlInfo BLNumber_CSI { get; set; }
        public SecurityControlInfo btnUpdateFoot_CSI { get; set; }
        public SecurityControlInfo daeNumber_CSI { get; set; }
        public SecurityControlInfo daeNumber2_CSI { get; set; }
        public SecurityControlInfo daeNumber3_CSI { get; set; }
        public SecurityControlInfo daeNumber4_CSI { get; set; }
        public SecurityControlInfo dateShipment_CSI { get; set; }
        public SecurityControlInfo description_CSI { get; set; }
        public SecurityControlInfo glazePercentage_CSI { get; set; }
        public SecurityControlInfo gvFormContainer_CSI { get; set; }
        public SecurityControlInfo gvInvoiceCommercialDetail_CSI { get; set; }
        public SecurityControlInfo gvInvoiceDetail_CSI { get; set; }
        public SecurityControlInfo id_buyer_CSI { get; set; }
        public SecurityControlInfo id_capacityContainer_CSI { get; set; }
        public SecurityControlInfo id_CityDelivery_CSI { get; set; }
        public SecurityControlInfo id_Consignee_CSI { get; set; }
        public SecurityControlInfo id_documentInvoice_CSI { get; set; }
        public SecurityControlInfo id_ForeignCustomer_CSI { get; set; }
        public SecurityControlInfo id_Language_CSI { get; set; }
        public SecurityControlInfo id_metricUnitInvoice_CSI { get; set; }
        public SecurityControlInfo id_Notifier_CSI { get; set; }
        public SecurityControlInfo id_Notifier2_CSI { get; set; }
        public SecurityControlInfo id_PaymentMethod_CSI { get; set; }
        public SecurityControlInfo id_PaymentTerm_CSI { get; set; }
        public SecurityControlInfo id_portDestination_CSI { get; set; }
        public SecurityControlInfo id_portDischarge_CSI { get; set; }
        public SecurityControlInfo id_portShipment_CSI { get; set; }
        public SecurityControlInfo id_shippingAgency_CSI { get; set; }
        public SecurityControlInfo id_shippingLine_CSI { get; set; }
        public SecurityControlInfo id_tariffHeading_CSI { get; set; }
        public SecurityControlInfo id_termsNegotiation_CSI { get; set; }
        public SecurityControlInfo idPortfolioFinancing_CSI { get; set; }
        public SecurityControlInfo letterCredit_CSI { get; set; }
        public SecurityControlInfo numberRemissionGuide_CSI { get; set; }
        public SecurityControlInfo numeroContenedores_CSI { get; set; }
        public SecurityControlInfo purchaseOrder_CSI { get; set; }
        public SecurityControlInfo shipName_CSI { get; set; }
        public SecurityControlInfo shipNumberTrip_CSI { get; set; }
        public SecurityControlInfo valueCustomsExpenditures_CSI { get; set; }
        public SecurityControlInfo valueInternationalFreight_CSI { get; set; }
        public SecurityControlInfo valueInternationalInsurance_CSI { get; set; }
        public SecurityControlInfo valueTransportationExpenses_CSI { get; set; }

        public SecurityControlInfo idVendor_CSI { get; set; }
        public SecurityControlInfo etaDate_CSI { get; set; }
        public SecurityControlInfo blDate_CSI { get; set; }
        public SecurityControlInfo seals_CSI { get; set; }
        public SecurityControlInfo containers_CSI { get; set; }
  

        public void ValidateInfo(string codeStatus)
        {

            

            List<tbsysDocumentDocumentStateControlsState> controlState = db.tbsysDocumentDocumentStateControlsState
                .Where(r => r.DocumentState.code == codeStatus && r.DocumentType.code == "70")
                .ToList();

            List<tbsysDocumentDocumentStateControlsState> controlStateError =
                controlState.Where(r =>
                                    (r.controlName == "id_Consignee" && r.isRequired && (this.id_Consignee == 0 || this.id_Consignee == null)) ||
                                    (r.controlName == "id_Notifier" && r.isRequired && (this.id_Notifier == 0 || this.id_Notifier == null)) ||
                                    (r.controlName == "daeNumber" && r.isRequired && string.IsNullOrEmpty(this.daeNumber)) ||
                                    (r.controlName == "daeNumber2" && r.isRequired && string.IsNullOrEmpty(this.daeNumber2)) ||
                                    (r.controlName == "daeNumber3" && r.isRequired && string.IsNullOrEmpty(this.daeNumber3)) ||
                                    (r.controlName == "daeNumber4" && r.isRequired && string.IsNullOrEmpty(this.daeNumber4)) ||
                                    (r.controlName == "BLNumber" && r.isRequired && string.IsNullOrEmpty(this.BLNumber)) ||
                                    (r.controlName == "id_capacityContainer" && r.isRequired && this.id_capacityContainer == 0) ||
                                    (r.controlName == "numeroContenedores" && r.isRequired && this.numeroContenedores == 0) ||
                                    (r.controlName == "dateShipment" && r.isRequired && this.dateShipment == null) ||
                                    (r.controlName == "id_PaymentMethod" && r.isRequired && (this.id_PaymentMethod == 0 || this.id_PaymentMethod == null)) ||
                                    (r.controlName == "id_PaymentTerm" && r.isRequired && (this.id_PaymentTerm == 0 || this.id_PaymentTerm == null)) ||
                                    (r.controlName == "id_portDestination" && r.isRequired && (this.id_portDestination == 0 || this.id_portDestination == null)) ||
                                    (r.controlName == "id_portDischarge" && r.isRequired && (this.id_portDischarge == 0 || this.id_portDischarge == null)) ||
                                    (r.controlName == "id_portShipment" && r.isRequired && (this.id_portShipment == 0 || this.id_portShipment == null)) ||
                                    (r.controlName == "id_shippingAgency" && r.isRequired && (this.id_shippingAgency == 0 || this.id_shippingAgency == null)) ||
                                    (r.controlName == "id_shippingLine" && r.isRequired && (this.id_shippingLine == 0 || this.id_shippingLine == null)) ||
                                    (r.controlName == "id_tariffHeading" && r.isRequired && (this.id_tariffHeading == 0 || this.id_tariffHeading == null)) ||
                                    (r.controlName == "id_termsNegotiation" && r.isRequired && (this.id_termsNegotiation == 0 || this.id_termsNegotiation == null)) ||
                                    (r.controlName == "shipName" && r.isRequired && string.IsNullOrEmpty(this.shipName)) ||
                                    (r.controlName == "shipNumberTrip" && r.isRequired && string.IsNullOrEmpty(this.shipNumberTrip)) ||
                                    (r.controlName == "gvInvoiceCommercialDetail" && r.isRequired && this.InvoiceCommercialDetail.Count() == 0) ||
                                    //(r.controlName == "gvFormContainer" && r.isRequired && this.InvoiceCommercialContainer.Count() == 0) ||
                                    (r.controlName == "idPortfolioFinancing" && r.isRequired && (this.idPortfolioFinancing == 0 || this.idPortfolioFinancing == null)) ||
                                    (r.controlName == "idVendor" && r.isRequired && (this.idVendor == 0 || this.idVendor == null)) ||
                                    (r.controlName == "etaDate" && r.isRequired && this.etaDate == null) ||
                                    (r.controlName == "seals" && r.isRequired && this.seals == null) ||
                                    (r.controlName == "containers" && r.isRequired && this.containers == null) ||
                                    (r.controlName == "blDate" && r.isRequired && this.blDate == null)
                               ).ToList();

                    // Validaciones Manuales
                    tbsysDocumentDocumentStateControlsState errorValidation = new tbsysDocumentDocumentStateControlsState();
                    // validacion si forma de Pago es Transferecia requerir banco
                    PaymentMethod _paymentMethod = db.PaymentMethod?.FirstOrDefault(r => r.id == id_PaymentMethod);
                    if (_paymentMethod!= null && _paymentMethod.code == "TR"  )
                    {
                        if (this.id_BankTransfer == null || this.id_BankTransfer == 0)
                        {
                            errorValidation = new tbsysDocumentDocumentStateControlsState
                            {
                                messageError = "Seleccione Banco de Transferencia, si Forma de Pago: Transferencia"
                            };
                            controlStateError.Add(errorValidation);
                         }           

                    }
            

        // Validacion Terminos de Negociacion
        TermsNegotiation _termsNegotiation = db.TermsNegotiation.FirstOrDefault(r => r.id == this.id_termsNegotiation);
                if(_termsNegotiation != null && _termsNegotiation.code == "FOBFLET")
                {

                    if (this.valueTotalFreight == 0 )
                    {
                        errorValidation = new tbsysDocumentDocumentStateControlsState
                        {
                            messageError = "Ingrese Flete, si Términos de Negociacion: FOB+FLETE "
                        };
                        controlStateError.Add(errorValidation);
                    }
                }

            // en la validacion si es requerido id_termsNegotiation entonces se validan los campos 
            if (controlStateError.Count() > 0)
            {
                string delimiter = ",";
                string msgError = controlStateError.Select(s => s.messageError).Aggregate((i, j) => i + delimiter + j);

                Exception error = new Exception("Por favor agregar: "  + msgError);
                error.Data.Add("source", "modelDocumentValidation");
                throw error;

            }
        }
        public List<int> getIds_Items(int? exclude)
        {
            List<int> returnId_Items = new List<int>();

            if (this.InvoiceCommercialDetail?.Count() == 0) return returnId_Items;

            if (exclude == null)
            {
                returnId_Items = this.InvoiceCommercialDetail.Where(r => r.isActive).Select(s => s.id_item).ToList();
            }
            else
            {
                returnId_Items = this.InvoiceCommercialDetail.Where(r => r.isActive && r.id_item != exclude).Select(s => s.id_item).ToList();
            }


            return returnId_Items;

        }
        public void ConversionMetricUnitCorrectTotalValue(int id_MetricUnitInvoice, int idCompany)
        {
            this.id_metricUnitInvoice = id_MetricUnitInvoice;
            this.InvoiceCommercialDetail.Where(r => r.isActive).ToList().ForEach(f => f.ConversionDetailMetricUnitCorrectTotalValue(id_MetricUnitInvoice, idCompany));
        }
        public void addBulkDetailProfFactCom(int? id_documentOrigin, List<InvoiceDetail> InvoiceDetailProforma, User ActiveUser)
        {
            InvoiceCommercialDetail _invoiceDetail = null;
            InvoiceCommercialDetail _invoiceDetailEliminar = null;
            List<InvoiceCommercialDetail> invoiceDetail = null;
            InvoiceCommercial _invoiceCommercial = null;
            if (this.InvoiceCommercialDetail == null) this.InvoiceCommercialDetail = new List<InvoiceCommercialDetail>();
            var invDetCom = InvoiceDetailProforma.OrderByDescending(x => !x.isActive);
            foreach (var DetailInv in invDetCom)
            {
                
                var documentOrigin = db.Document.FirstOrDefault(e => e.id == id_documentOrigin);
                if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "131"))
                {
                    _invoiceCommercial = this.InvoiceCommercialDetail.FirstOrDefault(r => r.InvoiceCommercial.Document.id_documentOrigen == DetailInv.id_invoice &&
                                                                                     r.InvoiceCommercial.Document.DocumentState.code != "05").InvoiceCommercial;

                    _invoiceDetail = this.InvoiceCommercialDetail.FirstOrDefault(r => r?.InvoiceCommercial?.Document?.id_documentOrigen == DetailInv.id_invoice &&
                                                                                  r?.InvoiceCommercial?.Document?.DocumentState.code != "05" &&
                                                                                  r.id_itemOrigen == DetailInv.id_item &&
                                                                                  r.isActive);
                    if(!DetailInv.isActive)
                    {
                        _invoiceDetailEliminar = this.InvoiceCommercialDetail.FirstOrDefault(r => r?.InvoiceCommercial?.Document?.id_documentOrigen == DetailInv.id_invoice &&
                                                                                  r?.InvoiceCommercial?.Document?.DocumentState.code != "05" &&
                                                                                  r.id_itemOrigen == DetailInv.id_item &&
                                                                                  r.isActive != DetailInv.isActive);
                    }
                    

                }
                else if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "07"))
                {
                    _invoiceCommercial = this.InvoiceCommercialDetail.FirstOrDefault(r => r.InvoiceCommercial.Document.id_documentOrigen == documentOrigin.id &&
                                                                                          r.InvoiceCommercial.Document.DocumentState.code != "05").InvoiceCommercial;
                    _invoiceDetail = this.InvoiceCommercialDetail.FirstOrDefault(r => r?.InvoiceCommercial?.Document?.id_documentOrigen == documentOrigin.id &&
                                                                                  r?.InvoiceCommercial?.Document?.DocumentState.code != "05" &&
                                                                                  r.id_itemOrigen == DetailInv.id_item &&
                                                                                  r.isActive);
                }


                Item item = db.Item.FirstOrDefault(r => r.id == DetailInv.id_item);
                if (_invoiceDetail == null && DetailInv.isActive)
                {
                    _invoiceDetail = new InvoiceCommercialDetail
                    {
                        id = DetailInv.id,
                        id_itemOrigen = DetailInv.id_item,
                        codePresentationOrigen = item.Presentation.code,
                        presentationMinimumOrigen = item.Presentation.minimum,
                        presentationMaximumOrigen = item.Presentation.maximum,
                        numBoxesOrigen = DetailInv.numBoxes.Value,
                        id_metricUnitOrigen = DetailInv.id_metricUnit.Value,
                        amountOrigen = DetailInv.amount,
                        unitPriceOrigen = DetailInv.unitPrice,
                        totalOrigen = DetailInv.total,

                        id_item = DetailInv.id_item,
                        codePresentation = item.Presentation.code,
                        presentationMinimum = item.Presentation.minimum,
                        presentationMaximum = item.Presentation.maximum,
                        numBoxes = DetailInv.numBoxes.Value,
                        id_metricUnit = DetailInv.id_metricUnit.Value,
                        amount = DetailInv.amount,
                        amountInvoice = DetailInv.id_amountInvoice.Value,
                        unitPrice = DetailInv.unitPrice,
                        total = DetailInv.total,
                        glazePercentageDetail = 0,
                        isActive = DetailInv.isActive,
                        id_userCreate = ActiveUser.id,
                        id_userUpdate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        dateUpdate = DateTime.Now,
                        weightBox = null,
                        factorMetricUnit = null,
                        id_itemMarked = DetailInv.id_itemMarked,
                        discount = DetailInv.discount
                    };
 
                    this.InvoiceCommercialDetail.Add(_invoiceDetail);
                    UpdateInvoiceCommercialTotals(_invoiceCommercial);
                }
                else if (_invoiceDetail != null)
                {
                    _invoiceDetail.id_itemOrigen = DetailInv.id_item;
                    _invoiceDetail.codePresentationOrigen = item.Presentation.code;
                    _invoiceDetail.presentationMinimumOrigen = item.Presentation.minimum;
                    _invoiceDetail.presentationMaximumOrigen = item.Presentation.maximum;
                    _invoiceDetail.numBoxesOrigen = DetailInv.numBoxes.Value;
                    _invoiceDetail.id_metricUnitOrigen = DetailInv.id_metricUnit.Value;
                    _invoiceDetail.amountOrigen = DetailInv.amount;
                    _invoiceDetail.unitPriceOrigen = DetailInv.unitPrice;
                    _invoiceDetail.totalOrigen = DetailInv.total;

                    _invoiceDetail.id_item = DetailInv.id_item;
                    _invoiceDetail.codePresentation = item.Presentation.code;
                    _invoiceDetail.presentationMinimum = item.Presentation.minimum;
                    _invoiceDetail.presentationMaximum = item.Presentation.maximum;
                    _invoiceDetail.numBoxes = DetailInv.numBoxes.Value;
                    _invoiceDetail.id_metricUnit = DetailInv.id_metricUnit.Value;
                    _invoiceDetail.amount = DetailInv.amount;
                    _invoiceDetail.amountInvoice = DetailInv.id_amountInvoice.Value;
                    _invoiceDetail.unitPrice = DetailInv.unitPrice;
                    _invoiceDetail.total = DetailInv.total;
                    _invoiceDetail.glazePercentageDetail = 0;
                    _invoiceDetail.isActive = DetailInv.isActive;
                    _invoiceDetail.id_userUpdate = ActiveUser.id;
                    _invoiceDetail.dateUpdate = DateTime.Now;
                    _invoiceDetail.weightBox = null;
                    _invoiceDetail.factorMetricUnit = null;
                    _invoiceDetail.id_itemMarked = DetailInv.id_itemMarked;
                    _invoiceDetail.discount = DetailInv.discount;
                    UpdateInvoiceCommercialTotals(_invoiceDetail.InvoiceCommercial);
                }
                if (_invoiceDetailEliminar != null) 
                {

                    _invoiceDetailEliminar.isActive = false;
                    _invoiceDetailEliminar.id_userUpdate = ActiveUser.id;
                    _invoiceDetailEliminar.dateUpdate = DateTime.Now;
                    UpdateInvoiceCommercialTotals(_invoiceDetailEliminar.InvoiceCommercial);
                }
            }


            var itemsProformas = InvoiceDetailProforma.Select(i => i.id_item);
            invoiceDetail = this.InvoiceCommercialDetail.Where(r => r?.InvoiceCommercial?.Document?.id_documentOrigen == id_documentOrigin &&
                                                                                  r?.InvoiceCommercial?.Document?.DocumentState.code != "05" &&
                                                                                  r.isActive && !itemsProformas.Contains(r.id_item)
                                                                                  ).ToList();
            foreach (var DetailInv in invoiceDetail)
            {
                DetailInv.isActive = false;
                DetailInv.id_userUpdate = ActiveUser.id;
                DetailInv.dateUpdate = DateTime.Now;
                UpdateInvoiceCommercialTotals(DetailInv.InvoiceCommercial);
            }



        }
        public void UpdateInvoiceCommercialTotals(InvoiceCommercial invoiceCommercial)
        {
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
            }

            subtotal = (subtotal - invoiceCommercial.valueDiscount);

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
    }
}