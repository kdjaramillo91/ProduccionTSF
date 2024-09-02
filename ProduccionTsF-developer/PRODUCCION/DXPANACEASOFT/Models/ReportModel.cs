using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.XtraReports.Parameters;

using DXPANACEASOFT.Reports.ProductionLotPay;

namespace DXPANACEASOFT.Models
{
    public class ReportModel
    {
        public string ReportName
        {
            get;
            set;
        }

        public string ReportDescription
        {
            get;
            set;
        }

        public  List<ReportParameter>  ListReportParameter
        {
            get;
            set;
        }


        public XtraReport CreateReport()
        {
            String path = "DXPANACEASOFT.Reports.";
            switch (ReportName)
            {
                case "Country":
                    //      XtraReport RemissionGuide =  new RemissionGuideReport();
                    XtraReport Country = new CountryReport();

                  
                    AddParameter(ref Country);
                    


                    return Country;

                case "PurchaseOrdersReport":


                    XtraReport PurchaseOrdersReport = new PurchaseOrdersReport(); // (XtraReport)Activator.CreateInstance(Type.GetType(path));
              

                    AddParameter(ref PurchaseOrdersReport);
                    PurchaseOrdersReport.FillDataSource();
                    return PurchaseOrdersReport;

                case "PurchaseRequestReport":

                    
                    XtraReport PurchaseRequestReport = new PurchaseRequestReport(); // (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref PurchaseRequestReport);
                    PurchaseRequestReport.FillDataSource();
                    return PurchaseRequestReport;
                case "RemisionGuideReportUnitPreint":

                    path += "Logistics.RemisionGuideReportUnitPreint";
                    XtraReport RemisionGuideReportUnit = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref RemisionGuideReportUnit);
                    RemisionGuideReportUnit.FillDataSource();

                    return RemisionGuideReportUnit;


                case "AdvanceProviderReportList":

                    path += "AdvanceProvider.AdvanceProviderReportList";
                    XtraReport AdvanceProviderReportList = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref AdvanceProviderReportList);
                    AdvanceProviderReportList.FillDataSource();
                    return AdvanceProviderReportList;

                case "LiquidationFreightReport":

                     path += "LiquidationFreight.LiquidationFreightReport";
                    XtraReport LiquidationFreightReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref LiquidationFreightReport);
                    LiquidationFreightReport.FillDataSource();
                    return LiquidationFreightReport;

                case "LiquidationFreightRiverReport":

                    path += "LiquidationFreightRiver.LiquidationFreightRiverReport";
                    XtraReport LiquidationFreightRiverReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref LiquidationFreightRiverReport);
                    LiquidationFreightRiverReport.FillDataSource();
                    return LiquidationFreightRiverReport;

                case "AdvanceProviderReport":

                    path += "AdvanceProvider.AdvanceProviderReport";
                    XtraReport AdvanceProviderReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref AdvanceProviderReport);
                    AdvanceProviderReport.FillDataSource();
                    return AdvanceProviderReport;
                    
                case "RemisionGuideReporthielo":

                    path += "Logistics.RemisionGuideReporthielo";
                    XtraReport RemisionGuideReporthielo = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref RemisionGuideReporthielo);
                    RemisionGuideReporthielo.FillDataSource();

                    return RemisionGuideReporthielo;

                // MIA case "RemisionGuideReportViatico":
                case "RemisionGuideReportViaticoCustomized":

                    path += "Logistics.RemisionGuideReportViaticoCustomized";
                    XtraReport RemisionGuideReportViaticoCustomized = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref RemisionGuideReportViaticoCustomized);
                    RemisionGuideReportViaticoCustomized.FillDataSource();

                    return RemisionGuideReportViaticoCustomized;


                case "RemisionGuideReportViaticoPersona":

                    path += "Logistics.RemisionGuideReportViaticoPersona";
                    XtraReport RemisionGuideReportViaticoPersona = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref RemisionGuideReportViaticoPersona);
                    RemisionGuideReportViaticoPersona.FillDataSource();
                    return RemisionGuideReportViaticoPersona;


                //case "RemisionGuideReportMaterialdespacho":
                case "RemisionGuideReportMaterialdespachoDetailWH":

                    path += "Logistics.RemisionGuideReportMaterialdespachoDetailWH";
                    XtraReport RemisionGuideReportMaterialdespachoDetailWH = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref RemisionGuideReportMaterialdespachoDetailWH);

                    RemisionGuideReportMaterialdespachoDetailWH.FillDataSource();
                    return RemisionGuideReportMaterialdespachoDetailWH;

                case "InvoiceExteriorReport":

                    //path += "InvoiceExterior.InvoiceExteriorReport";
                    //XtraReport InvoiceExteriorReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    //AddParameter(ref InvoiceExteriorReport);

                    //InvoiceExteriorReport.FillDataSource();
                    //return InvoiceExteriorReport;

                    XtraReport invoiceExteriorReport = new InvoiceExteriorReport(); // (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref invoiceExteriorReport);
                    invoiceExteriorReport.FillDataSource();
                    return invoiceExteriorReport;

                case "InvoiceCommercialReport":

                    path += "InvoiceCommercial.InvoiceCommercialReport";
                    XtraReport InvoiceCommercialReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref InvoiceCommercialReport);

                    InvoiceCommercialReport.FillDataSource();
                    return InvoiceCommercialReport;


                case "ProductionLotPaymentReport":

                    //path += "InvoiceExterior.InvoiceExteriorReport";
                    //XtraReport InvoiceExteriorReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    //AddParameter(ref InvoiceExteriorReport);

                    //InvoiceExteriorReport.FillDataSource();
                    //return InvoiceExteriorReport;

                    XtraReport productionLotPaymentReport = new ProductionLotPaymentReport(); // (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref productionLotPaymentReport);
                    productionLotPaymentReport.FillDataSource();
                    return productionLotPaymentReport;
                case "ProductionLotPaymentEndReport":

                    //path += "InvoiceExterior.InvoiceExteriorReport";
                    //XtraReport InvoiceExteriorReport = (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    //AddParameter(ref InvoiceExteriorReport);

                    //InvoiceExteriorReport.FillDataSource();
                    //return InvoiceExteriorReport;

                    XtraReport productionLotPaymentEndReport = new ProductionLotPaymentEndReport(); // (XtraReport)Activator.CreateInstance(Type.GetType(path));
                    AddParameter(ref productionLotPaymentEndReport);
                    productionLotPaymentEndReport.FillDataSource();
                    return productionLotPaymentEndReport;

                case "RemissionGuide":
                         XtraReport RemissionGuide =  new RemissionGuideReport();
                                    AddParameter(ref RemissionGuide);
                    return RemissionGuide;
                case "Products":
                    return null;
                // return new ProductsReport();
                case "Orders":
                    return null;
                    //return new OrdersReport();
            }
            return null;
        }

        public void AddParameter(ref XtraReport xreport)
        {
            if (xreport != null && ListReportParameter !=null) { 
            foreach (ReportParameter vParameter in ListReportParameter)
            {
              if (vParameter != null) { 
                if (!string.IsNullOrEmpty(vParameter.Name) && !string.IsNullOrEmpty(vParameter.Value))
                {
                  for (int item= 0; item < xreport.Parameters.Count; item++)
                            {
                                if(xreport.Parameters[item].Name == vParameter.Name)
                                { 
                                    xreport.Parameters[item].Value= vParameter.Value;
                                    break;
                                }
                            }
                }

            }
                }

            }

        }
    }



    
}