using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.ModelExtension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Utilitarios.Logs;
using XmlFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Factura;

namespace DXPANACEASOFT.Models
{
    public partial class Invoice
    {
        public decimal subTotalIVA0Truncate { get; set; }
        public decimal subTotalTruncate { get; set; }
        public decimal totalDiscountTruncate { get; set; }
        public decimal totalValueTruncate { get; set; }
        public decimal subtotalNoTaxesTruncate { get; set; }
        public decimal valueTotalFOBTruncate { get; set; }
        public decimal valueInternationalFreightTruncate { get; set; }
        public decimal valueInternationalInsuranceTruncate { get; set; }
        public decimal valueCustomsExpendituresTruncate { get; set; }
        public decimal valueTransportationExpensesTruncate { get; set; }
        public decimal valuetotalCIFTruncate { get; set; }
        public decimal valuetotalProformaTruncate { get; set; }
        public decimal totalValueProformaTruncate { get; set; }
        public decimal subTotalIVA0ProformaTruncate { get; set; }

        public List<InvoiceWeightView> InvoiceWeightList { get; set; }

        private DBContext db = new DBContext();

        public void addBulkDetail(List<InvoiceDetail> adderInvoiceDetail, User ActiveUser)
        {
            InvoiceDetail _invoiceDetail = null;
            if (this.InvoiceDetail == null) this.InvoiceDetail = new List<InvoiceDetail>();


            foreach (var DetailInv in adderInvoiceDetail)
            {
                _invoiceDetail = this.InvoiceDetail.FirstOrDefault(r => r.id == DetailInv.id);

                Item item = db.Item.FirstOrDefault(r => r.id == DetailInv.id_item);
                if (_invoiceDetail == null)
                {
                    _invoiceDetail = new InvoiceDetail
                    {
                        id = DetailInv.id,
                        amount = DetailInv.amount,
                        id_amountInvoice = DetailInv.id_amountInvoice,
                        amountproforma = (DetailInv.amountproforma == 0) ? DetailInv.numBoxes * DetailInv.proformaWeight : DetailInv.amountproforma,
                        totalProforma = DetailInv.totalProforma,
                        description = item.description,
                        descriptionCustomer = DetailInv.descriptionCustomer,
                        descriptionAuxCode = item.auxCode,
                        masterCode = item.masterCode,
                        codePresentation = item.Presentation.code,
                        presentationMinimum = item.Presentation.minimum,
                        presentationMaximum = item.Presentation.maximum,
                        discount = DetailInv.discount,
                        id_item = DetailInv.id_item,
                        id_itemMarked = DetailInv.id_itemMarked ?? DetailInv.id_item,
                        id_metricUnit = DetailInv.id_metricUnit,
                        id_metricUnitInvoiceDetail = (DetailInv.id_metricUnitInvoiceDetail == 999) ? DetailInv.id_metricUnit : DetailInv.id_metricUnitInvoiceDetail,
                        iva = DetailInv.iva,
                        iva0 = DetailInv.iva0,
                        ivaExented = DetailInv.ivaExented,
                        ivaNoObject = DetailInv.ivaNoObject,
                        unitPrice = DetailInv.unitPrice,
                        unitPriceProforma = DetailInv.unitPriceProforma,
                        netWeight = DetailInv.netWeight,
                        proformaWeight = DetailInv.proformaWeight,
                        valueICE = DetailInv.valueICE,
                        valueIRBPNR = DetailInv.valueIRBPNR,
                        numBoxes = DetailInv.numBoxes,
                        proformaUsedNumBoxes = DetailInv.proformaUsedNumBoxes,
                        proformaUsedDiscount = 0,
                        proformaPendingNumBoxes = DetailInv.proformaPendingNumBoxes,
                        proformaPendingDiscount = DetailInv.discount,
                        proformaNumBoxesPlusMinus = DetailInv.proformaNumBoxesPlusMinus,
                        proformaPorcVariationPlusMinus = DetailInv.proformaPorcVariationPlusMinus,
                        amountDisplay = DetailInv.amountDisplay,
                        amountInvoiceDisplay = DetailInv.amountInvoiceDisplay,
                        amountProformaDisplay = DetailInv.amountProformaDisplay,
                        id_tariffHeadingDetail = DetailInv.id_tariffHeadingDetail,
                        isActive = DetailInv.isActive,
                        id_userCreate = ActiveUser.id,
                        id_userUpdate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        dateUpdate = DateTime.Now
                    };

                    _invoiceDetail.calculateTotal();
                    this.InvoiceDetail.Add(_invoiceDetail);
                }
                else
                {
                    _invoiceDetail.amount = DetailInv.amount;
                    _invoiceDetail.id_amountInvoice = DetailInv.id_amountInvoice;
                    _invoiceDetail.amountproforma = (DetailInv.amountproforma == 0) ? DetailInv.numBoxes * DetailInv.proformaWeight : DetailInv.amountproforma;
                    _invoiceDetail.totalProforma = DetailInv.totalProforma;
                    _invoiceDetail.description = item.description;
                    _invoiceDetail.descriptionCustomer = DetailInv.descriptionCustomer;
                    _invoiceDetail.descriptionAuxCode = item.auxCode;
                    _invoiceDetail.masterCode = item.masterCode;
                    _invoiceDetail.codePresentation = item.Presentation.code;
                    _invoiceDetail.presentationMinimum = item.Presentation.minimum;
                    _invoiceDetail.presentationMaximum = item.Presentation.maximum;
                    _invoiceDetail.discount = DetailInv.discount;
                    _invoiceDetail.id_item = DetailInv.id_item;
                    _invoiceDetail.id_itemMarked = DetailInv.id_itemMarked ?? DetailInv.id_item;
                    _invoiceDetail.id_metricUnit = DetailInv.id_metricUnit;
                    _invoiceDetail.id_metricUnitInvoiceDetail = (DetailInv.id_metricUnitInvoiceDetail == 999) ? DetailInv.id_metricUnit : DetailInv.id_metricUnitInvoiceDetail;
                    _invoiceDetail.iva = DetailInv.iva;
                    _invoiceDetail.iva0 = DetailInv.iva0;
                    _invoiceDetail.ivaExented = DetailInv.ivaExented;
                    _invoiceDetail.ivaNoObject = DetailInv.ivaNoObject;
                    _invoiceDetail.unitPrice = DetailInv.unitPrice;
                    _invoiceDetail.unitPriceProforma = DetailInv.unitPriceProforma;
                    _invoiceDetail.netWeight = DetailInv.netWeight;
                    _invoiceDetail.proformaWeight = DetailInv.proformaWeight;
                    _invoiceDetail.valueICE = DetailInv.valueICE;
                    _invoiceDetail.valueIRBPNR = DetailInv.valueIRBPNR;
                    _invoiceDetail.numBoxes = DetailInv.numBoxes;
                    _invoiceDetail.proformaUsedNumBoxes = DetailInv.proformaUsedNumBoxes;
                    _invoiceDetail.proformaPendingNumBoxes = DetailInv.proformaPendingNumBoxes;
                    _invoiceDetail.proformaNumBoxesPlusMinus = DetailInv.proformaNumBoxesPlusMinus;
                    _invoiceDetail.proformaPorcVariationPlusMinus = DetailInv.proformaPorcVariationPlusMinus;
                    _invoiceDetail.amountDisplay = DetailInv.amountDisplay;
                    _invoiceDetail.amountInvoiceDisplay = DetailInv.amountInvoiceDisplay;
                    _invoiceDetail.amountProformaDisplay = DetailInv.amountProformaDisplay;
                    _invoiceDetail.id_tariffHeadingDetail = DetailInv.id_tariffHeadingDetail;
                    _invoiceDetail.isActive = DetailInv.isActive;
                    _invoiceDetail.id_userUpdate = ActiveUser.id;
                    _invoiceDetail.dateUpdate = DateTime.Now;
                    _invoiceDetail.calculateTotal();
                }
            }
        }

        public void addBulkDetailProfFact(int? id_documentOrigin, List<InvoiceDetail> InvoiceDetailProforma, User ActiveUser)
        {
            InvoiceDetail _invoiceDetail = null;
            List<InvoiceDetail> invoiceDetail = null;
            if (this.InvoiceDetail == null) this.InvoiceDetail = new List<InvoiceDetail>();
            Document document = new Document();
            var invDetCom = InvoiceDetailProforma.OrderByDescending(x => !x.isActive);
            foreach (var DetailInv in invDetCom)
            {
                var documentOrigin = db.Document.FirstOrDefault(e => e.id == id_documentOrigin);
                if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "131"))
                {
                    _invoiceDetail = this.InvoiceDetail.FirstOrDefault(r => r?.Invoice?.Document?.id_documentOrigen == DetailInv.id_invoice &&
                                                                        r?.Invoice?.Document?.DocumentState.code != "05" &&
                                                                        r.id_item == DetailInv.id_item &&
                                                                        r.isActive);
                }
                else if ((documentOrigin != null) && (documentOrigin?.DocumentType?.code == "70"))
                {
                    document = db.Document.FirstOrDefault(e => e.id == documentOrigin.id && e.DocumentState.code != "05"); 
                    _invoiceDetail = document != null
                        ? this.InvoiceDetail.FirstOrDefault(r => r?.Invoice?.Document?.id_documentOrigen == document.id &&
                                                                        r?.Invoice?.Document?.DocumentState.code != "05" &&
                                                                        r.id_item == DetailInv.id_item &&
                                                                        r.isActive)
                        : null;
                }

                Item item = db.Item.FirstOrDefault(r => r.id == DetailInv.id_item);
                if (_invoiceDetail == null && DetailInv.isActive)
                {
                    _invoiceDetail = new InvoiceDetail
                    {
                        id = DetailInv.id,
                        amount = DetailInv.amount,
                        id_amountInvoice = DetailInv.id_amountInvoice,
                        amountproforma = DetailInv.amount,
                        totalProforma = DetailInv.total,
                        description = item.description,
                        descriptionCustomer = DetailInv.descriptionCustomer,
                        descriptionAuxCode = item.auxCode,
                        masterCode = item.masterCode,
                        codePresentation = item.Presentation.code,
                        presentationMinimum = item.Presentation.minimum,
                        presentationMaximum = item.Presentation.maximum,
                        discount = DetailInv.discount,
                        id_item = DetailInv.id_item,
                        id_itemMarked = DetailInv.id_itemMarked ?? DetailInv.id_item,
                        id_metricUnit = DetailInv.id_metricUnit,
                        id_metricUnitInvoiceDetail = (DetailInv.id_metricUnitInvoiceDetail == 999) ? DetailInv.id_metricUnit : DetailInv.id_metricUnitInvoiceDetail,
                        iva = DetailInv.iva,
                        iva0 = DetailInv.iva0,
                        ivaExented = DetailInv.ivaExented,
                        ivaNoObject = DetailInv.ivaNoObject,
                        unitPrice = DetailInv.unitPrice,
                        unitPriceProforma = DetailInv.unitPrice,
                        netWeight = DetailInv.netWeight,
                        proformaWeight = DetailInv.proformaWeight,
                        valueICE = DetailInv.valueICE,
                        valueIRBPNR = DetailInv.valueIRBPNR,
                        numBoxes = DetailInv.numBoxes,
                        proformaUsedNumBoxes = DetailInv.proformaUsedNumBoxes,
                        proformaUsedDiscount = 0,
                        proformaPendingNumBoxes = DetailInv.proformaPendingNumBoxes,
                        proformaPendingDiscount = DetailInv.discount,
                        proformaNumBoxesPlusMinus = DetailInv.proformaNumBoxesPlusMinus,
                        proformaPorcVariationPlusMinus = DetailInv.proformaPorcVariationPlusMinus,
                        amountDisplay = DetailInv.amountDisplay,
                        amountInvoiceDisplay = DetailInv.amountInvoiceDisplay,
                        amountProformaDisplay = DetailInv.amountProformaDisplay,
                        id_tariffHeadingDetail = (document != null && document.InvoiceCommercial != null && DetailInv.id_tariffHeadingDetail == null) ? document.InvoiceCommercial.id_tariffHeading : DetailInv.id_tariffHeadingDetail,
                        isActive = DetailInv.isActive,
                        id_userCreate = ActiveUser.id,
                        id_userUpdate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        dateUpdate = DateTime.Now
                    };

                    _invoiceDetail.calculateTotal();
                    this.InvoiceDetail.Add(_invoiceDetail);
                }
                else if (_invoiceDetail != null)
                {
                    _invoiceDetail.amount = DetailInv.amount;
                    _invoiceDetail.id_amountInvoice = DetailInv.id_amountInvoice;
                    _invoiceDetail.amountproforma = DetailInv.amount;
                    _invoiceDetail.totalProforma = DetailInv.total;
                    _invoiceDetail.description = item.description;
                    _invoiceDetail.descriptionCustomer = DetailInv.descriptionCustomer;
                    _invoiceDetail.descriptionAuxCode = item.auxCode;
                    _invoiceDetail.masterCode = item.masterCode;
                    _invoiceDetail.codePresentation = item.Presentation.code;
                    _invoiceDetail.presentationMinimum = item.Presentation.minimum;
                    _invoiceDetail.presentationMaximum = item.Presentation.maximum;
                    _invoiceDetail.discount = DetailInv.discount;
                    _invoiceDetail.id_item = DetailInv.id_item;
                    _invoiceDetail.id_itemMarked = DetailInv.id_itemMarked ?? DetailInv.id_item;
                    _invoiceDetail.id_metricUnit = DetailInv.id_metricUnit;
                    _invoiceDetail.id_metricUnitInvoiceDetail = (DetailInv.id_metricUnitInvoiceDetail == 999) ? DetailInv.id_metricUnit : DetailInv.id_metricUnitInvoiceDetail;
                    _invoiceDetail.iva = DetailInv.iva;
                    _invoiceDetail.iva0 = DetailInv.iva0;
                    _invoiceDetail.ivaExented = DetailInv.ivaExented;
                    _invoiceDetail.ivaNoObject = DetailInv.ivaNoObject;
                    _invoiceDetail.unitPrice = DetailInv.unitPrice;
                    _invoiceDetail.unitPriceProforma = DetailInv.unitPrice;
                    _invoiceDetail.netWeight = DetailInv.netWeight;
                    _invoiceDetail.proformaWeight = DetailInv.proformaWeight;
                    _invoiceDetail.valueICE = DetailInv.valueICE;
                    _invoiceDetail.valueIRBPNR = DetailInv.valueIRBPNR;
                    _invoiceDetail.numBoxes = DetailInv.numBoxes;
                    _invoiceDetail.proformaUsedNumBoxes = DetailInv.proformaUsedNumBoxes;
                    _invoiceDetail.proformaPendingNumBoxes = DetailInv.proformaPendingNumBoxes;
                    _invoiceDetail.proformaNumBoxesPlusMinus = DetailInv.proformaNumBoxesPlusMinus;
                    _invoiceDetail.proformaPorcVariationPlusMinus = DetailInv.proformaPorcVariationPlusMinus;
                    _invoiceDetail.amountDisplay = DetailInv.amountDisplay;
                    _invoiceDetail.amountInvoiceDisplay = DetailInv.amountInvoiceDisplay;
                    _invoiceDetail.amountProformaDisplay = DetailInv.amountProformaDisplay;
                    _invoiceDetail.id_tariffHeadingDetail = _invoiceDetail.id_tariffHeadingDetail;
                    _invoiceDetail.isActive = DetailInv.isActive;
                    _invoiceDetail.id_userUpdate = ActiveUser.id;
                    _invoiceDetail.dateUpdate = DateTime.Now;
                    _invoiceDetail.calculateTotal();
                }
            }

            var itemsProformas = InvoiceDetailProforma.Select(i => i.id_item);
            invoiceDetail = this.InvoiceDetail.Where(r => r?.Invoice?.Document?.id_documentOrigen == id_documentOrigin &&
                                                                                  r?.Invoice?.Document?.DocumentState.code != "05" &&
                                                                                  r.isActive && !itemsProformas.Contains(r.id_item)
                                                                                  ).ToList();
            foreach (var DetailInv in invoiceDetail)
            {
                DetailInv.isActive = false;
                DetailInv.id_userUpdate = ActiveUser.id;
                DetailInv.dateUpdate = DateTime.Now;
                DetailInv.calculateTotal();
            }
        }

        public void calculateTotales(string dconfig = "FXGEN")
        {
            this.subTotalExentIVA = 0;
            this.subtotalIVA = 0;
            this.IVA = 0;
            this.subTotalNoObjectIVA = 0;
            this.valueICE = 0;
            this.valueIRBPNR = 0;

            if (this.InvoiceDetail.Count() == 0)
            {
                this.subTotal = 0;
                this.subTotalIVA0 = 0;
                this.subtotalNoTaxes = 0;
                this.totalDiscount = 0;
                this.totalValue = 0;

                this.subTotalTruncate = 0;
                this.subTotalIVA0Truncate = 0;
                this.subtotalNoTaxesTruncate = 0;
                this.totalDiscountTruncate = 0;
                this.totalValueTruncate = 0;
                this.totalValueProformaTruncate = 0;
                this.subTotalIVA0ProformaTruncate = 0;
            }
            else
            {
                var i = this.InvoiceDetail.Where(r => r.isActive).Select(r =>
                                                      new
                                                      {
                                                          r.id_amountInvoice,
                                                          r.numBoxes,
                                                          r.unitPrice,
                                                          r.discount,
                                                          r.totalPriceWithoutTax,
                                                          r.total,
                                                          subtotal = Decimal.Round((r.totalPriceWithoutTax), 2),
                                                          subtotalProforma = r.amountproforma.HasValue ? Decimal.Round(((decimal)(r.id_amountInvoice ?? 0) * (decimal)(r.unitPriceProforma)), 2) : 0m,
                                                      }).ToList();

                this.subTotalIVA0 = (decimal)i.Sum(r => (r.subtotal));
                this.subTotal = this.subTotalIVA0;
                this.subtotalNoTaxes = this.subTotalIVA0;
                this.totalDiscount = i.Sum(r => r.discount);
                this.totalValue = (this.subTotal) - (this.totalDiscount);

                string[] configDec = new string[2];
                this.subTotalIVA0Truncate = Decimal.Round(i.Sum(r => ((decimal)r.subtotal)), 2);
                this.subTotalTruncate = this.subTotalIVA0Truncate;
                this.subtotalNoTaxesTruncate = this.subTotalIVA0Truncate;
                this.totalDiscountTruncate = i.Sum(r => r.discount.ToAdvanceDecimal(configDec, out configDec, dconfig));
                //this.totalValueTruncate = (this.subTotalIVA0Truncate) - (this.totalDiscountTruncate);
                this.totalValueTruncate = (this.subTotalIVA0Truncate) - (this.totalDiscountTruncate);
                this.subTotalIVA0ProformaTruncate = i.Sum(r => ((decimal)r.subtotalProforma).ToAdvanceDecimal(null, out configDec, dconfig));
                //this.subTotalIVA0ProformaTruncate = this.subTotalIVA0ProformaTruncate - this.valueInternationalFreightTruncate;
                this.totalValueProformaTruncate = (this.SalesQuotationExterior != null) ? (this.SalesQuotationExterior.valuetotalCIF) : (this.subTotalIVA0ProformaTruncate - this.totalDiscountTruncate);
            }
        }

        public List<int> getId_Items(int? exclude)
        {
            List<int> returnId_Items = new List<int>();

            if (this.InvoiceDetail?.Count() == 0) return returnId_Items;

            if (exclude == null)
            {
                returnId_Items = this.InvoiceDetail.Where(r => r.isActive).Select(s => s.id_item).ToList();
            }
            else
            {
                returnId_Items = this.InvoiceDetail.Where(r => r.isActive && r.id_item != exclude).Select(s => s.id_item).ToList();
            }

            return returnId_Items;
        }

        public void calculateTotalBoxes()
        {
            if (this.InvoiceExterior == null) return;
            if (this.InvoiceDetail == null)
            {
                this.InvoiceExterior.totalBoxes = 0;
                return;
            }
            if (this.InvoiceDetail.Count == 0)
            {
                this.InvoiceExterior.totalBoxes = 0;
                return;
            }

            List<InvoiceDetail> _invoiceDetail = this.InvoiceDetail.ToList();

            int totalBoxes = _invoiceDetail.Where(w => w.isActive).Sum(r => r.numBoxes).Value;

            this.InvoiceExterior.totalBoxes = totalBoxes;
        }

        public void calculateTotalBoxes_SalesQuotationExterior()
        {
            if (this.SalesQuotationExterior == null) return;
            if (this.InvoiceDetail == null)
            {
                this.SalesQuotationExterior.totalBoxes = 0;
                this.SalesQuotationExterior.usedBoxes = 0;
                this.SalesQuotationExterior.pendingBoxes = 0;
                this.SalesQuotationExterior.numBoxesPlusMinus = 0;
                return;
            }
            if (this.InvoiceDetail.Count == 0)
            {
                this.SalesQuotationExterior.totalBoxes = 0;
                this.SalesQuotationExterior.usedBoxes = 0;
                this.SalesQuotationExterior.pendingBoxes = 0;
                this.SalesQuotationExterior.numBoxesPlusMinus = 0;
                return;
            }

            var _invoiceDetail = this.InvoiceDetail.ToList();

            var totalBoxes = _invoiceDetail.Where(w => w.isActive).Sum(r => r.numBoxes);
            this.SalesQuotationExterior.totalBoxes = totalBoxes.Value;

            var pendingBoxes = _invoiceDetail.Where(w => w.isActive).Sum(r => r.proformaPendingNumBoxes);
            this.SalesQuotationExterior.pendingBoxes = pendingBoxes;
            var usedBoxes = _invoiceDetail.Where(w => w.isActive).Sum(r => r.proformaUsedNumBoxes);
            this.SalesQuotationExterior.usedBoxes = usedBoxes;
            var numBoxesPlusMinus = _invoiceDetail.Where(w => w.isActive).Sum(r => r.proformaNumBoxesPlusMinus);
            this.SalesQuotationExterior.numBoxesPlusMinus = numBoxesPlusMinus;
        }

        public void calculateTotalesInvoiceExterior()
        {
            string[] configDec = new string[2];
            if (this.InvoiceExterior == null) return;

            if (this.InvoiceDetail == null || this.InvoiceDetail?.Count() == 0)

            {
                this.InvoiceExterior.valueTotalFOB = 0;
                this.valueTotalFOBTruncate = 0;
                this.valuetotalProformaTruncate = 0;
            }
            else
            {
                this.InvoiceExterior.valueTotalFOB = this.totalValue;
                this.valueTotalFOBTruncate = this.totalValueTruncate;
                this.valuetotalProformaTruncate = this.totalValueProformaTruncate;
            }

            this.InvoiceExterior.valuetotalCIF = (this.InvoiceExterior.valueTotalFOB +
                                                    this.InvoiceExterior.valueCustomsExpenditures +
                                                     this.InvoiceExterior.valueInternationalFreight +
                                                     this.InvoiceExterior.valueInternationalInsurance +
                                                     this.InvoiceExterior.valueTransportationExpenses);

            this.valuetotalCIFTruncate = (this.valueTotalFOBTruncate +
                                                    this.InvoiceExterior.valueCustomsExpenditures +
                                                     this.InvoiceExterior.valueInternationalFreight +
                                                     this.InvoiceExterior.valueInternationalInsurance +
                                                     this.InvoiceExterior.valueTransportationExpenses); ;
        }

        public void ValidateInfo()
        {
            int id_documentState = this.Document.id_documentState;

            String documentStateCode = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "00";

            List<tbsysDocumentDocumentStateControlsState> controlState = db.tbsysDocumentDocumentStateControlsState.Where(r => r.DocumentState.code == documentStateCode && r.DocumentType.code == "07").ToList();

            List<tbsysDocumentDocumentStateControlsState> controlStateError =
                controlState.Where(r =>
                                    (r.controlName == "id_consignee" && r.isRequired && (this.InvoiceExterior.id_consignee == 0 || this.InvoiceExterior.id_consignee == null)) ||
                                    //(r.controlName == "id_notifier" && r.isRequired && (this.InvoiceExterior.id_notifier == 0 || this.InvoiceExterior.id_notifier == null)) ||
                                    (r.controlName == "purchaseOrder" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.purchaseOrder)) ||
                                    (r.controlName == "daeNumber" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.daeNumber)) ||
                                    (r.controlName == "daeNumber2" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.daeNumber2)) ||
                                    (r.controlName == "daeNumber3" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.daeNumber3)) ||
                                    (r.controlName == "daeNumber4" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.daeNumber4)) ||
                                    (r.controlName == "numberRemissionGuide" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.numberRemissionGuide)) ||
                                    (r.controlName == "BLNumber" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.BLNumber)) ||
                                    (r.controlName == "id_capacityContainer" && r.isRequired && this.InvoiceExterior.id_capacityContainer == 0) ||
                                    (r.controlName == "numeroContenedores" && r.isRequired && this.InvoiceExterior.numeroContenedores == 0) ||
                                    (r.controlName == "dateShipment" && r.isRequired && this.InvoiceExterior.dateShipment == null) ||
                                    (r.controlName == "id_PaymentMethod" && r.isRequired && (this.InvoiceExterior.id_PaymentMethod == 0 || this.InvoiceExterior.id_PaymentMethod == null)) ||
                                    (r.controlName == "id_PaymentTerm" && r.isRequired && (this.InvoiceExterior.id_PaymentTerm == 0 || this.InvoiceExterior.id_PaymentTerm == null)) ||
                                    (r.controlName == "id_portDestination" && r.isRequired && (this.InvoiceExterior.id_portDestination == 0 || this.InvoiceExterior.id_portDestination == null)) ||
                                    (r.controlName == "id_portDischarge" && r.isRequired && (this.InvoiceExterior.id_portDischarge == 0 || this.InvoiceExterior.id_portDischarge == null)) ||
                                    (r.controlName == "id_portShipment" && r.isRequired && (this.InvoiceExterior.id_portShipment == 0 || this.InvoiceExterior.id_portShipment == null)) ||
                                    (r.controlName == "id_shippingAgency" && r.isRequired && (this.InvoiceExterior.id_shippingAgency == 0 || this.InvoiceExterior.id_shippingAgency == null)) ||
                                    (r.controlName == "id_ShippingLine" && r.isRequired && (this.InvoiceExterior.id_ShippingLine == 0 || this.InvoiceExterior.id_ShippingLine == null)) ||
                                    //(r.controlName == "id_tariffHeading" && r.isRequired && (this.InvoiceExterior.id_tariffHeading == 0 || this.InvoiceExterior.id_tariffHeading == null) ) ||
                                    (r.controlName == "id_termsNegotiation" && r.isRequired && (this.InvoiceExterior.id_termsNegotiation == 0 || this.InvoiceExterior.id_termsNegotiation == null)) ||
                                    (r.controlName == "shipName" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.shipName)) ||
                                    (r.controlName == "shipNumberTrip" && r.isRequired && string.IsNullOrEmpty(this.InvoiceExterior.shipNumberTrip)) ||
                                    (r.controlName == "gvInvoiceDetail" && r.isRequired && this.InvoiceDetail.Count() == 0)
                               ).ToList();

            // en la validacion si es requerido id_termsNegotiation entonces se validan los campos
            if (controlStateError.Count() > 0)
            {
                string delimiter = ",";
                string msgError = controlStateError.Select(s => s.messageError).Aggregate((i, j) => i + delimiter + j);

                Exception error = new Exception(msgError);
                error.Data.Add("source", "modelDocumentValidation");
                throw error;
            }
        }

        public void ValidateInfo_SalesQuotationExterior()
        {
            int id_documentState = this.Document.id_documentState;

            String documentStateCode = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "00";

            List<tbsysDocumentDocumentStateControlsState> controlState = db.tbsysDocumentDocumentStateControlsState.Where(r => r.DocumentState.code == documentStateCode && r.DocumentType.code == "07").ToList();

            List<tbsysDocumentDocumentStateControlsState> controlStateError =
                controlState.Where(r =>
                                    (r.controlName == "id_consignee" && r.isRequired && (this.SalesQuotationExterior.id_consignee == 0 || this.SalesQuotationExterior.id_consignee == null)) ||
                                    (r.controlName == "id_notifier" && r.isRequired && (this.SalesQuotationExterior.id_notifier == 0 || this.SalesQuotationExterior.id_notifier == null)) ||
                                    (r.controlName == "id_addressCustomer" && r.isRequired && (this.SalesQuotationExterior.id_addressCustomer == 0)) ||
                                    (r.controlName == "dateShipment" && r.isRequired && this.SalesQuotationExterior.dateShipment == null) ||
                                    (r.controlName == "id_PaymentMethod" && r.isRequired && (this.SalesQuotationExterior.id_PaymentMethod == 0 || this.SalesQuotationExterior.id_PaymentMethod == null)) ||
                                    (r.controlName == "id_PaymentTerm" && r.isRequired && (this.SalesQuotationExterior.id_PaymentTerm == 0 || this.SalesQuotationExterior.id_PaymentTerm == null)) ||
                                    //(r.controlName == "id_bank" && r.isRequired && (this.SalesQuotationExterior.id_bank == 0 || this.SalesQuotationExterior.id_bank == null)) ||
                                    (r.controlName == "id_portDestination" && r.isRequired && (this.SalesQuotationExterior.id_portDestination == 0 || this.SalesQuotationExterior.id_portDestination == null)) ||
                                    (r.controlName == "id_portDischarge" && r.isRequired && (this.SalesQuotationExterior.id_portDischarge == 0 || this.SalesQuotationExterior.id_portDischarge == null)) ||
                                    (r.controlName == "id_termsNegotiation" && r.isRequired && (this.SalesQuotationExterior.id_termsNegotiation == 0 || this.SalesQuotationExterior.id_termsNegotiation == null)) ||
                                    (r.controlName == "id_portTerminal" && r.isRequired && (this.SalesQuotationExterior.id_portTerminal == 0 || this.SalesQuotationExterior.id_portTerminal == null)) ||
                                    (r.controlName == "gvInvoiceDetail" && r.isRequired && this.InvoiceDetail.Count() == 0)
                               ).ToList();

            if (controlStateError.Count() > 0)
            {
                string delimiter = ",";
                string msgError = controlStateError.Select(s => s.messageError).Aggregate((i, j) => i + delimiter + j);

                Exception error = new Exception(msgError);
                error.Data.Add("source", "modelDocumentValidation");
                throw error;
            }
        }

        public void ValidateStateChange(string codeNewState)
        {
            string codeState = this.Document?.DocumentState?.code ?? "01";
            Boolean err = false;
            string msgError = "No puede realizar esta acción con el presente documento";
            string msg = "";
            switch (codeState)
            {
                case "01":
                    switch (codeNewState)
                    {
                        case "06":
                            err = true;
                            break;
                    }
                    break;

                case "02":
                    switch (codeNewState)
                    {
                        case "01":
                        case "02":
                        case "06":
                            err = true;
                            break;
                    }
                    break;

                case "03":
                    switch (codeNewState)
                    {
                        case "01":
                        case "02":
                        case "03":
                            err = true;
                            break;
                    }

                    break;

                case "05":
                    switch (codeNewState)
                    {
                        case "01":
                        case "02":
                        case "03":
                        case "05":
                        case "06":
                        case "09":
                            err = true;
                            break;
                    }
                    break;

                case "06":
                    switch (codeNewState)
                    {
                        case "01":
                        case "02":
                        case "03":
                        case "05":
                        case "06":
                            err = true;
                            break;
                    }
                    break;

                default:

                    break;
            }
            if (err)
            {
                msg = msgError;
                Exception error = new Exception(msg);
                error.Data.Add("source", "validateChangeStatus");
                throw error;
            }
        }

        public void fillAmountDetails()
        {
            if (this.InvoiceDetail == null) return;
            if (this.InvoiceDetail.Count() == 0) return;

            foreach (InvoiceDetail invDetail in InvoiceDetail)
            {
                invDetail.fillAmount();
            }
        }

        // Corregir el precio Unitario
        public void ConversionMetricUnitCorrectUnitPrice(int id_MetricUnitInvoice)
        {
            this.InvoiceDetail.Where(r => r.isActive).ToList().ForEach(f => f.ConversionDetailMetricUnitCorrectUnitPrice(id_MetricUnitInvoice));
        }

        // Corregir el precio Unitario
        public void ConversionMetricUnitCorrectTotalValue(int id_MetricUnitInvoice)
        {
            this.InvoiceDetail.Where(r => r.isActive).ToList().ForEach(f => f.ConversionDetailMetricUnitCorrectTotalValue(id_MetricUnitInvoice));
        }

        public void ViewWeight()
        {
            int countActiveWeight = this.InvoiceExteriorWeight?.Where(r => (Boolean)r.isActive).Count() ?? 0;
            if (countActiveWeight == 0) this.InvoiceWeightList = new List<InvoiceWeightView>();

            string[] arWeightType = new string[] { "NET", "BRT" };
            var InvoiceWeightViewA = this.InvoiceExteriorWeight
                                                .Where(r => (Boolean)r.isActive && arWeightType.Contains(r.WeightType.code))
                                                .Select(r => new InvoiceWeightView
                                                {
                                                    peso = Decimal.Round((Decimal)r.peso, 2).ToString(),
                                                    codeUnidadMedida = r.MetricUnit.code,
                                                    namePeso = r.WeightType.name
                                                })
                                                .ToList();

            var InvoiceWeightViewB = this.InvoiceExteriorWeight
                                                .Where(r => (Boolean)r.isActive && r.WeightType.code == "GLS")
                                                .Select(r => new InvoiceWeightView
                                                {
                                                    peso = Decimal.Round((Decimal)r.peso, 2).ToString(),
                                                    codeUnidadMedida = r.MetricUnit.code,
                                                    namePeso = r.WeightType.name
                                                })
                                                .ToList();

            var InvoiceWeightViewC = InvoiceWeightViewA.Union(InvoiceWeightViewB);

            var InvoiceWeightViewD = this.InvoiceExteriorWeight
                                                .Where(r => (Boolean)r.isActive && r.WeightType.code == "PSP")
                                                .Select(r => new InvoiceWeightView
                                                {
                                                    peso = Decimal.Round((Decimal)r.peso, 2).ToString(),
                                                    codeUnidadMedida = r.MetricUnit.code,
                                                    namePeso = r.WeightType.name
                                                })
                                                .ToList();

            var InvoiceWeightViewE = InvoiceWeightViewC.Union(InvoiceWeightViewD);

            this.InvoiceWeightList = InvoiceWeightViewE.ToList();
        }

        public void saveWeight(DBContext db)
        {
            decimal totalGrossWeightKilo = 0;
            decimal totalGrossWeightPound = 0;
            decimal totalNetWeightKilo = 0;
            decimal totalNetWeightPound = 0;
            decimal totalGlazeWeightKilo = 0;
            decimal totalGlazeWeightPound = 0;
            decimal totalProfWeightKilo = 0.00M;
            decimal totalProfWeightPound = 0.00M;

            try
            {
                List<WeightType> weightTypeList = db.WeightType
                                                            .Where(r => r.isActive)
                                                            .ToList();

                List<MetricUnit> metricUnitList = db.MetricUnit
                                                                .Where(r => r.isActive && r.MetricType.code == "PES01")
                                                                .ToList();

                int idMetricUnitKilos = 0;
                int idMetricUnitLibras = 0;
                MetricUnit metricUnitKg = metricUnitList.FirstOrDefault(r => r.code == "Kg");
                MetricUnit metricUnitLbs = metricUnitList.FirstOrDefault(r => r.code == "Lbs");
                idMetricUnitKilos = metricUnitKg?.id ?? 0;
                idMetricUnitLibras = metricUnitLbs?.id ?? 0;

                int countWithGlaze = 0;
                string itemDescription = "";

                if (this.InvoiceDetail != null && this.InvoiceDetail?.Count() > 0)
                {
                    this.InvoiceExteriorWeight.ToList().ForEach(r => { r.isActive = false; });

                    var items = this.InvoiceDetail
                                            .Where(r => r.isActive)
                                            .Select(s =>
                                                            new
                                                            {
                                                                s.id_item,
                                                                s.id_metricUnit,
                                                                s.numBoxes,
                                                                s.presentationMaximum,
                                                                s.presentationMinimum,
                                                                isActive = true,
                                                                s.proformaWeight
                                                            }
                                                        )
                                                .ToList();

                    int[] idsItems = items.Select(r => r.id_item).ToArray();

                    var itemConversionInfoList = db.ItemWeightConversionFreezen
                                                            .Where(r => idsItems.Contains(r.id_Item))
                                                            .ToList();

                    Dictionary<int, string> itemDescripcionList = db.Item
                                                                    .Where(r => idsItems.Contains(r.id))
                                                                    .ToDictionary(r => r.id, r => r.masterCode);

                    foreach (var item in items)
                    {
                        ItemWeightConversionFreezen itemConversionInfo = itemConversionInfoList
                                                                                 .First(r => r.id_Item == item.id_item);

                        string codeMetricUnit = metricUnitList.First(r => r.id == item.id_metricUnit)?.code;

                        itemDescription = "";
                        itemDescripcionList.TryGetValue(item.id_item, out itemDescription);

                        // validar que la unidad de medida sea consistente
                        if (itemConversionInfo.id_MetricUnit != item.id_metricUnit)
                        {
                            throw new Exception("La unidad de medida del item: " + itemDescription + " no es consistente con la unidad de la conversión.");
                        }

                        decimal validacionPeso = ((item.presentationMaximum * item.presentationMinimum) / 2);
                        if (validacionPeso >= itemConversionInfo.itemWeightNetWeight)
                        {
                            throw new Exception("El peso neto  de la conversion no es consistente con el peso de la presentacion del item: " + itemDescription);
                        }

                        decimal factorLibras = itemConversionInfo?.conversionToPounds ?? 0;
                        decimal factorKilos = itemConversionInfo?.conversionToKilos ?? 0;

                        // validar que los factores esten correctos
                        if ((codeMetricUnit == "Lbs" && factorLibras != 1) || (codeMetricUnit == "Lbs" && factorKilos == 1))
                        {
                            throw new Exception("La conversion en Libras para el item: " + itemDescription + " no es correcta.");
                        }

                        if ((codeMetricUnit == "Kg" && factorKilos != 1) || (codeMetricUnit == "Kg" && factorLibras == 1))
                        {
                            throw new Exception("La conversion en Kilogramos para el item: " + itemDescription + " no es correcta.");
                        }

                        if (itemConversionInfo.weightWithGlaze != 0)
                        {
                            totalGlazeWeightKilo += (decimal)(factorKilos * item.numBoxes * itemConversionInfo.weightWithGlaze);
                            totalGlazeWeightPound += (decimal)(factorLibras * item.numBoxes * itemConversionInfo.weightWithGlaze);

                            countWithGlaze++;
                        }

                        totalGrossWeightKilo = totalGrossWeightKilo + (decimal)(factorKilos * item.numBoxes * itemConversionInfo.itemWeightGrossWeight);
                        totalGrossWeightPound = totalGrossWeightPound + (decimal)(factorLibras * item.numBoxes * itemConversionInfo.itemWeightGrossWeight);
                        totalNetWeightKilo = totalNetWeightKilo + (decimal)(factorKilos * item.numBoxes * itemConversionInfo.itemWeightNetWeight);
                        totalNetWeightPound = totalNetWeightPound + (decimal)(factorLibras * item.numBoxes * itemConversionInfo.itemWeightNetWeight);
                        totalProfWeightKilo += codeMetricUnit == "Kg" ? ((decimal)(item.numBoxes * (item.proformaWeight ?? 0.00M)))
                                                                     : ((decimal)(factorKilos * item.numBoxes * (item.proformaWeight ?? 0.00M)));
                        totalProfWeightPound += codeMetricUnit == "Lbs" ? ((decimal)(item.numBoxes * (item.proformaWeight ?? 0.00M)))
                                                                     : ((decimal)(factorLibras * item.numBoxes * (item.proformaWeight ?? 0.00M)));
                    }

                    int idNeto = 0;
                    int idBruto = 0;
                    int idGlaseado = 0;
                    int idPesoProforma = 0;

                    WeightType weightTypeNet = weightTypeList.FirstOrDefault(r => r.code == "NET");
                    WeightType weightTypeBrt = weightTypeList.FirstOrDefault(r => r.code == "BRT");
                    WeightType weightTypeGls = weightTypeList.FirstOrDefault(r => r.code == "GLS");
                    WeightType weightTypePsp = weightTypeList.FirstOrDefault(r => r.code == "PSP");

                    idNeto = weightTypeNet?.id ?? 0;
                    idBruto = weightTypeBrt?.id ?? 0;
                    idGlaseado = weightTypeGls?.id ?? 0;
                    idPesoProforma = weightTypePsp?.id ?? 0;

                    InvoiceExteriorWeight _invoiceExteriorWeightNetoLibras
                                                    = new Models.InvoiceExteriorWeight
                                                    {
                                                        id_metricUnit = idMetricUnitLibras,
                                                        id_WeightType = idNeto,
                                                        peso = totalNetWeightPound,
                                                        isActive = true,
                                                        WeightType = weightTypeNet,
                                                        MetricUnit = metricUnitLbs
                                                    };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightNetoLibras);

                    InvoiceExteriorWeight _invoiceExteriorWeightNetoKilos
                                                    = new Models.InvoiceExteriorWeight
                                                    {
                                                        id_metricUnit = idMetricUnitKilos,
                                                        id_WeightType = idNeto,
                                                        peso = totalNetWeightKilo,
                                                        isActive = true,
                                                        WeightType = weightTypeNet,
                                                        MetricUnit = metricUnitKg
                                                    };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightNetoKilos);

                    InvoiceExteriorWeight _invoiceExteriorWeightBrutoLibras
                                                    = new Models.InvoiceExteriorWeight
                                                    {
                                                        id_metricUnit = idMetricUnitLibras,
                                                        id_WeightType = idBruto,
                                                        peso = totalGrossWeightPound,
                                                        isActive = true,
                                                        WeightType = weightTypeBrt,
                                                        MetricUnit = metricUnitLbs
                                                    };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightBrutoLibras);

                    InvoiceExteriorWeight _invoiceExteriorWeightBrutoKilos
                                                   = new Models.InvoiceExteriorWeight
                                                   {
                                                       id_metricUnit = idMetricUnitKilos,
                                                       id_WeightType = idBruto,
                                                       peso = totalGrossWeightKilo,
                                                       isActive = true,
                                                       WeightType = weightTypeBrt,
                                                       MetricUnit = metricUnitKg
                                                   };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightBrutoKilos);

                    if (countWithGlaze > 0)
                    {
                        if (totalGlazeWeightPound != 0)
                        {
                            InvoiceExteriorWeight _invoiceExteriorWeightGlaseoLibras
                                                       = new Models.InvoiceExteriorWeight
                                                       {
                                                           id_metricUnit = idMetricUnitLibras,
                                                           id_WeightType = idGlaseado,
                                                           peso = totalGlazeWeightPound,
                                                           isActive = true,
                                                           WeightType = weightTypeGls,
                                                           MetricUnit = metricUnitLbs
                                                       };

                            this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightGlaseoLibras);
                        }

                        if (totalGlazeWeightKilo != 0 && countWithGlaze > 0)
                        {
                            InvoiceExteriorWeight _invoiceExteriorWeightGlaseoKilos
                                                       = new Models.InvoiceExteriorWeight
                                                       {
                                                           id_metricUnit = idMetricUnitKilos,
                                                           id_WeightType = idGlaseado,
                                                           peso = totalGlazeWeightKilo,
                                                           isActive = true,
                                                           WeightType = weightTypeGls,
                                                           MetricUnit = metricUnitKg
                                                       };

                            this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightGlaseoKilos);
                        }
                    }

                    InvoiceExteriorWeight _invoiceExteriorWeightPesoProformaLibras
                                                    = new Models.InvoiceExteriorWeight
                                                    {
                                                        id_metricUnit = idMetricUnitLibras,
                                                        id_WeightType = idPesoProforma,
                                                        peso = totalProfWeightPound,
                                                        isActive = true,
                                                        WeightType = weightTypePsp,
                                                        MetricUnit = metricUnitLbs
                                                    };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightPesoProformaLibras);

                    InvoiceExteriorWeight _invoiceExteriorWeightPesoProformaKilos
                                                   = new Models.InvoiceExteriorWeight
                                                   {
                                                       id_metricUnit = idMetricUnitKilos,
                                                       id_WeightType = idPesoProforma,
                                                       peso = totalProfWeightKilo,
                                                       isActive = true,
                                                       WeightType = weightTypePsp,
                                                       MetricUnit = metricUnitKg
                                                   };

                    this.InvoiceExteriorWeight.Add(_invoiceExteriorWeightPesoProformaKilos);

                    this.ViewWeight();
                }
            }
            catch (Exception e)
            {
                string ruta = ConfigurationManager.AppSettings["rutalog"];
                MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "Factura Exterior", "saveWeight");
                throw e;
            }
        }

        public string generateInvoicePartial()
        {
            string returnCallIdentity = null;

            try
            {
                // Numero de contenedores
                int numeroContenedores = this.InvoiceExterior.numeroContenedores;
                if (numeroContenedores <= 1) return null;

                decimal subtTotal0 = 0;
                decimal subtTotalWithOutTax = 0;
                decimal subtTotalFob = 0;

                subtTotal0 = this.subTotalIVA0 / numeroContenedores;
                subtTotalWithOutTax = this.subtotalNoTaxes / numeroContenedores;
                subtTotalFob = this.InvoiceExterior.valueTotalFOB / numeroContenedores;
            }
            catch
            {
            }

            return returnCallIdentity;
        }

        public string GetTariffHeadingDescription()
        {
            string _tariffHeadingDescription = "";
            int?[] idsTariffHeading;
            try
            {
                idsTariffHeading = InvoiceDetail
                    .Where(r => r.isActive && r.id_tariffHeadingDetail.HasValue && r.id_tariffHeadingDetail != 0)
                    .Select(r => (r.id_tariffHeadingDetail))
                    .Distinct()
                    .ToArray();

                if (idsTariffHeading != null && idsTariffHeading.Count() > 0)
                {
                    this.InvoiceExterior.tariffHeadingDescription = db.TariffHeading.ToArray()
                        .Where(r => idsTariffHeading.Contains(r.id))
                        .Select(r => new
                        {
                            description = r.code
                        })
                        .Select(s => s.description)
                        .Aggregate((i, j) => i + Environment.NewLine + j);
                }
                else
                {
                    this.InvoiceExterior.tariffHeadingDescription = null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return _tariffHeadingDescription;
        }

        public XmlDocument GenerateXML(int company)
        {
            XmlDocument xmlout1 = new XmlDocument();

            int? id_documentState = this.Document.id_documentState;
            string code_documentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "";

            if (code_documentState != "09") return null;
            if (this.InvoiceExterior == null) return null;
            if (this.InvoiceDetail.Count() == 0) return null;
            if (this.Document?.DocumentType?.isElectronic == false) return null;

            ElectronicDocument _electronicDocument = this.Document.ElectronicDocument ?? new ElectronicDocument();
            ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == company && e.sriCode.Equals("01"));
            if (electronicState == null) return null;

            _electronicDocument = new ElectronicDocument
            {
                id_electronicDocumentState = electronicState.id
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            XmlFactura xmlFactura = DB2XML.Factura2Xml(this);
            xmlout1 = DB2XML.SerializeToXml(xmlFactura, ns);

            String xml = xmlout1.OuterXml;
            xml = xml.Replace(@"p2:nil=""true""", "");
            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            xml = xml.Replace("<infoAdicional  />", "");

            xmlout1.LoadXml(xml);

            _electronicDocument.xml = xmlout1.OuterXml;

            this.Document.ElectronicDocument = _electronicDocument;

            return xmlout1;
        }
    }

    public partial class InvoiceDetail
    {
        public decimal percentageKiloProforma { get; set; }
        public decimal totalKiloProforma { get; set; }
        public decimal subTotalIVA0Proforma { get; set; }
        public decimal expensesProforma { get; set; }
    }
}
