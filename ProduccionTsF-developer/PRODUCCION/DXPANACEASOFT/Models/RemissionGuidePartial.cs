


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class RemissionGuide
    {
        private DBContext db = new DBContext();
        public DateTime? emissionDateRG { get; set; }

        public bool? isManual { get; set; }

        public string liquidationDocument { get; set; }

        public string invoiceNumber { get; set; }

        public decimal totalLiquidation { get; set; }

        public void getLiquidationInformation()
        {
            var detailLiquidationFreight = db.LiquidationFreightDetail
                                                .FirstOrDefault(fod => fod.id_remisionGuide == this.id);

            if (detailLiquidationFreight != null)
            {
                this.liquidationDocument = db.Document.FirstOrDefault(fod => fod.id == detailLiquidationFreight.id_LiquidationFreight)?.sequential.ToString();
                this.invoiceNumber = detailLiquidationFreight?.LiquidationFreight?.InvoiceNumber ?? "";
                this.totalLiquidation = (detailLiquidationFreight.pricetotal) - (detailLiquidationFreight.pricesavance);
            }
        }

    }
}