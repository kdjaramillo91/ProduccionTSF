using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class RemissionGuideRiver
    {
        private DBContext db = new DBContext();

        public string liquidationDocument { get; set; }

        public string invoiceNumber { get; set; }

        public decimal totalLiquidation { get; set; }

        public bool requiredLogistic { get; set; }

        public void getLiquidationInformation()
        {
            //Inserto requiredLogistic
            this.requiredLogistic = this.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            var detailLiquidationFreight = db.LiquidationFreightRiverDetail
                                                .FirstOrDefault(fod => fod.id_remisionGuideRiver == this.id && fod.LiquidationFreightRiver.Document.DocumentState.code != "05");

            if (detailLiquidationFreight != null)
            {
                this.liquidationDocument = db.Document.FirstOrDefault(fod => fod.id == detailLiquidationFreight.id_LiquidationFreightRiver)?.sequential.ToString();
                this.invoiceNumber = detailLiquidationFreight?.LiquidationFreightRiver?.InvoiceNumber ?? "";
                this.totalLiquidation = (detailLiquidationFreight.pricetotal) - (detailLiquidationFreight.pricesavance);
            }
        }

    }
}