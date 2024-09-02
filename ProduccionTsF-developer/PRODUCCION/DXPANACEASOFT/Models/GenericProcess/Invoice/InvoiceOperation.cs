using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DXPANACEASOFT.Models.GenericProcess
{
    public abstract class InvoiceOperation
    {


    }

    public abstract class InvoiceDetailOperation
    {

        internal DBContext db_DetailOperation { get; set; }

        internal int idCompany { get; set; }
        internal Boolean? hasGlaze_DetailOperation { get; set; } 
        internal int idItem_DetailOperation { get; set; }

        internal int? id_MetricUnitInvoice_DetailOperation { get; set; }

        internal int id_MetricUnit_DetailOperation { get; set; }
        internal decimal peso_DetailOperation { get; set; }
        internal decimal pesoBasic_DetailOperation { get; set; }
        internal decimal pesoProforma_DetailOperation { get; set; }
        internal decimal pesoProformaTotal_DetailOperation { get; set; }
        internal decimal pesoTotal_DetailOperation { get; set; }
        internal decimal precio_DetailOperation { get; set; }

        internal decimal factor_DetailOperation { get; set; }

        internal decimal cantidad_DetailOperation { get; set; }

        internal decimal total_DetailOperation { get; set; }

        // Propiedades Informativas Auxiliares
        internal string auxCode_Inf { get; set; }
        internal string masterCode_Inf { get; set; }
        internal string foreignName_Inf { get; set; }
        internal string codeMetricUnit_Inf { get; set; }
        public string codeMetricUnitOrigin_Inf { get; set; }



        public void CalculateDetailInvoiceDetailOperation()
        {

            Item item;
            db_DetailOperation = new DBContext();            
            MetricUnit metricUnitInvoice;
            string codeMetricUnit = "";
            string codeMetricUnitOrigin = "";
            decimal metricUnitConversion = 0; 

            try
            {
                  
                item = db_DetailOperation.Item.FirstOrDefault(i => i.id == this.idItem_DetailOperation);
                this.cantidad_DetailOperation = decimal.Round(Convert.ToDecimal(this.cantidad_DetailOperation), 2);
                this.precio_DetailOperation = decimal.Round(Convert.ToDecimal( this.precio_DetailOperation ), 6);
                 

                if (item == null)
                {
                    throw new Exception("No existe Información de Item con id:" + this.idItem_DetailOperation);
                }

                this.auxCode_Inf = item.auxCode;
                this.masterCode_Inf = item.masterCode;
                this.foreignName_Inf = item.foreignName;

                // 1.- Obtener de ItemWeightConversionFreezen, peso con Glaseo, Peso Neto y Unidad Medida, Conversion Kilos ,Conversion Libras
                // 2.- Si peso factura es gramos buscar en MetricUnitConverter caso contrario usar la conversion de ItemWeightConversionFreezen
                

                ItemWeightConversionFreezen _itemWeightConversionFreezen = db_DetailOperation
                                                                                    .ItemWeightConversionFreezen
                                                                                    .FirstOrDefault(r => r.id_Item == this.idItem_DetailOperation);

                if (_itemWeightConversionFreezen == null)
                {
                    throw new Exception("No existe Información de Conversión para el Item :" + item.masterCode);
                }


                metricUnitInvoice = db_DetailOperation
                                            .MetricUnit
                                            .FirstOrDefault(r => r.id == id_MetricUnitInvoice_DetailOperation);

                //if (metricUnitInvoice == null)
                //{
                //    throw new Exception("No existe Información de Unidad de Medida con ID: " + id_MetricUnitInvoice_DetailOperation);
                //}

                MetricUnit metricUnitOrigin = db_DetailOperation
                                                        .MetricUnit
                                                        .FirstOrDefault(r => r.id == _itemWeightConversionFreezen.id_MetricUnit);

                if (metricUnitOrigin == null)
                {
                    throw new Exception("No existe Información de Unidad de Medida con ID: " + _itemWeightConversionFreezen.id_MetricUnit);
                }


                codeMetricUnit = metricUnitInvoice?.code ?? "";
                codeMetricUnitOrigin = metricUnitOrigin?.code ?? "";

                this.codeMetricUnitOrigin_Inf = codeMetricUnitOrigin;
                if ((Boolean) hasGlaze_DetailOperation)
                {

                    this.peso_DetailOperation = (_itemWeightConversionFreezen.weightWithGlaze == 0) ? _itemWeightConversionFreezen.itemWeightNetWeight.Value :
                                                        (_itemWeightConversionFreezen.weightWithGlaze != _itemWeightConversionFreezen.itemWeightNetWeight) ? _itemWeightConversionFreezen.weightWithGlaze.Value : _itemWeightConversionFreezen.itemWeightNetWeight.Value;
                    //this.peso_DetailOperation = (_itemWeightConversionFreezen.weightWithGlaze == 0) ? _itemWeightConversionFreezen.itemWeightNetWeight :
                    //                                    (_itemWeightConversionFreezen.weightWithGlaze != _itemWeightConversionFreezen.itemWeightNetWeight) ? _itemWeightConversionFreezen.weightWithGlaze : _itemWeightConversionFreezen.itemWeightNetWeight;
                }
                else
                {
                    //this.peso_DetailOperation  = _itemWeightConversionFreezen.itemWeightNetWeight;
                    this.peso_DetailOperation = _itemWeightConversionFreezen.itemWeightNetWeight.Value;
                }
                 
                this.pesoBasic_DetailOperation = decimal.Round(this.cantidad_DetailOperation * this.peso_DetailOperation, 2);
                

                if (  !( id_MetricUnitInvoice_DetailOperation == 0 ||                    
                         id_MetricUnitInvoice_DetailOperation == 999 ||
                         id_MetricUnitInvoice_DetailOperation == null
                        )
                   )
                {



                    int? id_metricUnitAux = _itemWeightConversionFreezen.id_MetricUnit;
                    this.factor_DetailOperation = (id_metricUnitAux == id_MetricUnitInvoice_DetailOperation) ? 1 : 0;

                    if (this.factor_DetailOperation == 0)
                    {


                        if (codeMetricUnitOrigin == "Gr" || codeMetricUnit == "Gr")
                        {

                            metricUnitConversion = db_DetailOperation
                                                        .MetricUnitConversion
                                                        .FirstOrDefault(muc => muc.id_company == this.idCompany &&
                                                                                                muc.id_metricOrigin == id_metricUnitAux &&
                                                                                                muc.id_metricDestiny == id_MetricUnitInvoice_DetailOperation)?.factor ?? 0;
                        }
                        else
                        {

                            if (codeMetricUnit == "Kg")
                            {

                                //metricUnitConversion = _itemWeightConversionFreezen.conversionToPounds;
                                metricUnitConversion = _itemWeightConversionFreezen.conversionToPounds ?? 1;
                            }
                            else if (codeMetricUnit == "Lbs")
                            {

                                //metricUnitConversion = _itemWeightConversionFreezen.conversionToKilos;
                                metricUnitConversion = _itemWeightConversionFreezen.conversionToKilos ?? 1;
                            }

                        }



                        
                        
                    }


                    //this.factor_DetailOperation = metricUnitConversion;
                    this.codeMetricUnit_Inf = codeMetricUnit;

                }
                else
                {
                    this.factor_DetailOperation = 1;
                    this.codeMetricUnit_Inf = codeMetricUnitOrigin;

                }


                this.pesoTotal_DetailOperation = decimal.Round((this.pesoBasic_DetailOperation * this.factor_DetailOperation), 2);

                this.total_DetailOperation = decimal.Round(this.pesoTotal_DetailOperation * this.precio_DetailOperation, 2);

                this.pesoProformaTotal_DetailOperation = decimal.Round((this.pesoProforma_DetailOperation * this.cantidad_DetailOperation), 2);

            }
            catch (Exception e)
            {
                throw e;

            }

          

        }



    }
    

}