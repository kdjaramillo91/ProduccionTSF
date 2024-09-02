using DXPANACEASOFT.Models.GenericProcess;
using System;
using System.Linq;

namespace DXPANACEASOFT.Models
{
    public partial class InvoiceDetail : InvoiceDetailOperation
    {
        private DBContext db = new DBContext();
        public int? id_attribute1 { get; set; }
        public string amountDisplay { get; set; }
        public string amountInvoiceDisplay { get; set; }
        public string amountProformaDisplay { get; set; }
        public string weightBoxUM { get; set; }

        public decimal totalPriceWithoutTaxLessDiscount { get => (this.totalPriceWithoutTax - this.discount) * (this.id_amountInvoice??0); }

        public void addNew(int newId, User ActiveUser)
        {
            Item item = db.Item.FirstOrDefault(r => r.id == this.id_item);

            this.id = newId;
            // complete item data
            this.description = item.description;
            this.descriptionAuxCode = item.auxCode;
            this.id_metricUnit = item.Presentation.id_metricUnit;
            this.codePresentation = item.Presentation.code;
            this.presentationMinimum = item.Presentation.minimum;
            this.presentationMaximum = item.Presentation.maximum;

            // complete value data
            this.iva = 0;
            this.iva0 = 0;
            this.ivaNoObject = 0;
            this.ivaExented = 0;
            this.valueICE = 0;
            this.valueIRBPNR = 0;
            this.total = this.totalPriceWithoutTax;

            // complete log data
            this.dateCreate = DateTime.Now;
            this.dateUpdate = DateTime.Now;
            this.id_userCreate = ActiveUser.id;
            this.id_userUpdate = ActiveUser.id;
            this.isActive = true;
        }

        public void calculateTax()
        {
        }

        public void fillAmount()
        {
            MetricUnit metricUnitItem = db.MetricUnit.FirstOrDefault(r => r.id == this.id_metricUnit);
            MetricUnit metricUnitInvoice = db.MetricUnit.FirstOrDefault(r => r.id == this.id_metricUnitInvoiceDetail);

            string amoutItem = Convert.ToString(this.amount) + ((metricUnitItem != null) ? metricUnitItem.code : "");
            string amoutInvoice = Convert.ToString(this.id_amountInvoice) + ((metricUnitInvoice != null) ? metricUnitInvoice.code : "");

            this.amountDisplay = amoutItem;
            this.amountInvoiceDisplay = amoutInvoice;
        }

        public void calculateTotal()
        {
            decimal totalValue = (decimal)(this.id_amountInvoice * this.unitPrice);
            this.totalPriceWithoutTax = totalValue;
            this.total = this.totalPriceWithoutTax;
        }

        // summary:
        // Convierte La unidad de Medida del item por al nueva de la factua y aplica el cambio hacia el precio unitario
        public void ConversionDetailMetricUnitCorrectUnitPrice(int id_MetricUnitInvoice)
        {
            MetricUnitConversion metricUnitConversion = new MetricUnitConversion();

            int id_MetricUnitOrigin = (int)this.id_metricUnit;
            if (id_MetricUnitInvoice == 999)
            {
                id_MetricUnitInvoice = id_MetricUnitOrigin;
            }

            decimal xfactorUnit = 0;

            if (id_MetricUnitOrigin != id_MetricUnitInvoice)
            {
                metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(r => r.id_metricOrigin == id_MetricUnitOrigin && r.id_metricDestiny == id_MetricUnitInvoice);
                if (metricUnitConversion == null) return;
                xfactorUnit = metricUnitConversion.factor;
            }
            else
            {
                xfactorUnit = 1;
            }

            decimal amountInvoice = (((this.presentationMinimum * xfactorUnit) * this.presentationMaximum) * (decimal)this.numBoxes);
            decimal newUnitPrice = (this.totalPriceWithoutTax + this.discount) / amountInvoice;

            this.id_metricUnitInvoiceDetail = id_MetricUnitInvoice;
            this.id_amountInvoice = amountInvoice;
            this.unitPrice = newUnitPrice;
            this.fillAmount();
        }

        // summary:
        // Convierte La unidad de Medida del item por al nueva de la factua y aplica el cambio hacia el total
        public void ConversionDetailMetricUnitCorrectTotalValue(int id_MetricUnitInvoice)
        {
            MetricUnitConversion metricUnitConversion = new MetricUnitConversion();
            int id_MetricUnitOrigin = (int)this.id_metricUnit;
            if (id_MetricUnitInvoice == 999)
            {
                id_MetricUnitInvoice = id_MetricUnitOrigin;
            }

            decimal xfactorUnit = 0;
            if (id_MetricUnitOrigin != id_MetricUnitInvoice)
            {
                metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(r => r.id_metricOrigin == id_MetricUnitOrigin && r.id_metricDestiny == id_MetricUnitInvoice);
                if (metricUnitConversion == null) return;
                xfactorUnit = metricUnitConversion.factor;
            }
            else
            {
                xfactorUnit = 1;
            }

            decimal amountInvoice = ((decimal)this.proformaWeight * (decimal)this.numBoxes);
            decimal newTotal = (amountInvoice * this.unitPrice) - this.discount;

            this.id_metricUnitInvoiceDetail = id_MetricUnitInvoice;
            this.id_amountInvoice = amountInvoice;
            this.total = newTotal;
            this.totalPriceWithoutTax = newTotal;
            this.fillAmount();
        }

        public void Calculation()
        {
            decimal _cantidadInvoice = 0;

            decimal factor = 1;

            Item item = db.Item.FirstOrDefault(r => r.id == this.id_item);
            MetricUnit _metricUnitOrigen = db.Presentation.FirstOrDefault(r => r.id == item.id_presentation).MetricUnit;
            MetricUnit _metricUnitDestino = db.MetricUnit.FirstOrDefault(r => r.id == this.id_metricUnitInvoiceDetail);

            // item
            if (this.id_metricUnitInvoiceDetail == 0)
            {
                factor = 1;
            }
            else if (_metricUnitOrigen.id != this.id_metricUnitInvoiceDetail)
            {
                MetricUnitConversion metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(r => (r.id_metricOrigin == _metricUnitOrigen.id) && (r.id_metricDestiny == this.id_metricUnitInvoiceDetail));
                factor = metricUnitConversion.factor;
            }

            decimal cMinimo = item?.Presentation.minimum ?? 0;
            decimal cMaximo = item?.Presentation.maximum ?? 0;

            _cantidadInvoice = (cMinimo * cMaximo * (int)this.numBoxes * factor);
            this.id_amountInvoice = _cantidadInvoice;
            this.totalPriceWithoutTax = ((decimal)this.amountproforma * this.unitPrice) - this.discount;
            this.total = this.totalPriceWithoutTax;
        }

        public Boolean CalculateDetailInvoiceCommercialDetail()
        {
            this.db_DetailOperation = this.db;
            this.hasGlaze_DetailOperation = false;
            this.idItem_DetailOperation = this.id_item;
            this.cantidad_DetailOperation = (decimal)this.numBoxes;
            this.pesoProforma_DetailOperation = (decimal)(this.proformaWeight ?? 0.00M);
            this.precio_DetailOperation = this.unitPrice;
            this.idCompany = (this.idCompany == 0) ? (this.Invoice?.Document?.EmissionPoint?.id_company ?? 0) : 0;

            this.CalculateDetailInvoiceDetailOperation();

            this.weightBoxUM = this.codeMetricUnitOrigin_Inf;
            this.total = this.total_DetailOperation;
            this.amount = this.pesoBasic_DetailOperation;
            this.id_amountInvoice = this.pesoProformaTotal_DetailOperation;

            return true;
        }
    }
}