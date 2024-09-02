using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.GenericProcess;


namespace DXPANACEASOFT.Models 
{
    public partial class InvoiceCommercialDetail : InvoiceDetailOperation
    {
        

        public string weightBoxUM { get; set; }

        private DBContext db = new DBContext();
        public void ConversionDetailMetricUnitCorrectTotalValue(int id_MetricUnitInvoice, int idCompany)
        {

            /* Validar si la nueva unidad de medida == a la anterior */
            /* Obtener Pesos */
            /* validar si es Gramos Buscar conversion en la tabla de conversion de pesos */
            /* caso contrario usar la conversion en pesos */
            //decimal xfactorUnit = 0;
            decimal pesoGeneric = 0;
            decimal amount = 0;
            decimal metricUnitConversion = 0;
            decimal amountInvoice = 0;

            int id_MetricUnitInvoiceDetail = (int)this.id_metricUnit;

            


            // Obtener informacion de Conversion 
            ItemWeightConversionFreezen _itemWeightConversionFreezen = db.ItemWeightConversionFreezen
                                                                                    .FirstOrDefault(r => r.id_Item == this.id_item);
            
            if (_itemWeightConversionFreezen == null) return;

            // Obtener Peso en Undida de Medida Item
            //pesoGeneric = (_itemWeightConversionFreezen.weightWithGlaze == 0) ? _itemWeightConversionFreezen.itemWeightNetWeight :
            //                                            (_itemWeightConversionFreezen.weightWithGlaze != _itemWeightConversionFreezen.itemWeightNetWeight) ? _itemWeightConversionFreezen.weightWithGlaze : _itemWeightConversionFreezen.itemWeightNetWeight;
            pesoGeneric = (_itemWeightConversionFreezen.weightWithGlaze == 0) ? _itemWeightConversionFreezen.itemWeightNetWeight.Value :
                                                        (_itemWeightConversionFreezen.weightWithGlaze != _itemWeightConversionFreezen.itemWeightNetWeight) ? _itemWeightConversionFreezen.weightWithGlaze.Value : _itemWeightConversionFreezen.itemWeightNetWeight.Value;

            amount = this.numBoxes * pesoGeneric;
            amount = decimal.Round(amount, 2);

            if(id_MetricUnitInvoice == 0 || id_MetricUnitInvoice == 999)
            {
                this.amount = amount;
                this.amountInvoice = amount;
                this.total = this.amount * this.unitPrice;
                return;
            }


            MetricUnit metricUnitInvoiceSource = db.MetricUnit.FirstOrDefault(r => r.id == _itemWeightConversionFreezen.id_MetricUnit );
            MetricUnit metricUnitInvoiceDestiny = db.MetricUnit.FirstOrDefault(r => r.id == id_MetricUnitInvoice);


            if (metricUnitInvoiceSource == null || metricUnitInvoiceDestiny == null) return;

            string codeMetricUnit = metricUnitInvoiceSource.code;
            string codeDestinyMetricUnit = metricUnitInvoiceDestiny.code;

            if (codeMetricUnit == "Gr" || codeDestinyMetricUnit == "Gr")
            {

                metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == idCompany &&
                                                                                    muc.id_metricOrigin == id_MetricUnitInvoiceDetail &&
                                                                                    muc.id_metricDestiny == id_MetricUnitInvoice)?.factor ?? 0;
            }
            else
            {

                if (codeDestinyMetricUnit == "Kg")
                {

                    //metricUnitConversion = _itemWeightConversionFreezen.conversionToPounds;
                    metricUnitConversion = _itemWeightConversionFreezen.conversionToPounds ?? 1;
                }
                else if (codeDestinyMetricUnit == "Lbs")
                {

                    //metricUnitConversion = _itemWeightConversionFreezen.conversionToKilos;
                    metricUnitConversion = _itemWeightConversionFreezen.conversionToKilos ?? 1;
                }

            }


            amountInvoice = decimal.Round((amount * metricUnitConversion), 2);


            this.MetricUnit1 = metricUnitInvoiceDestiny;
            this.id_metricUnit= id_MetricUnitInvoice;
            this.amountInvoice = amountInvoice;
            this.total = decimal.Round(amountInvoice * this.unitPrice, 2); ;
            
        }


        public Boolean CalculateDetailInvoiceCommercialDetail()
        {
            this.db_DetailOperation = this.db;
            this.hasGlaze_DetailOperation = (this.hasGlaze_DetailOperation) ?? this.InvoiceCommercial?.hasGlaze ?? false;
            this.idItem_DetailOperation = this.id_item;
            this.cantidad_DetailOperation = this.numBoxes;
            this.precio_DetailOperation = this.unitPrice;
            this.idCompany = (this.idCompany == 0) ?  (this.InvoiceCommercial?.Document?.EmissionPoint?.id_company ?? 0) :0;

            this.CalculateDetailInvoiceDetailOperation();

            this.weightBox = this.peso_DetailOperation;
            this.weightBoxUM = this.codeMetricUnitOrigin_Inf;
            this.factorMetricUnit = this.factor_DetailOperation;
            this.total = this.total_DetailOperation;
            this.totalOrigen = this.total_DetailOperation;
            this.amountInvoice = this.pesoTotal_DetailOperation;
            this.amount = this.pesoBasic_DetailOperation;
            this.amountOrigen = this.pesoBasic_DetailOperation;

            return true;
        }

       
    }



    public static class InvoiceCommercialDetailExtension
    {

 
        public static IEnumerable<InvoiceCommercialDetail> CalulateInvoiceDetail(this IEnumerable<InvoiceCommercialDetail> thisObj)
        {
            
            thisObj.All(r => r.CalculateDetailInvoiceCommercialDetail());
            return thisObj;
        }


        public static IEnumerable<InvoiceCommercialDetail> CalulateInvoiceDetail(this IEnumerable<InvoiceCommercialDetail> thisObj, int? idItem)
        {
            
            if(idItem != null)
            {
                thisObj
                    .FirstOrDefault(r=> r.id_item  == idItem)
                    .CalculateDetailInvoiceCommercialDetail();

            }
            else
            {
                thisObj//.AsParallel()
                    .All(r => r.CalculateDetailInvoiceCommercialDetail());
            }
            
            return thisObj;
        }

    }
}