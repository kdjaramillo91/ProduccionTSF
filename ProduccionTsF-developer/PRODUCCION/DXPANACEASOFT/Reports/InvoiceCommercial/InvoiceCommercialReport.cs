using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace DXPANACEASOFT.Reports.InvoiceCommercial
{
    public partial class InvoiceCommercialReport : DevExpress.XtraReports.UI.XtraReport
    {
        public InvoiceCommercialReport()
        {
            InitializeComponent();
        }

        private void ReportHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }
    }
}
