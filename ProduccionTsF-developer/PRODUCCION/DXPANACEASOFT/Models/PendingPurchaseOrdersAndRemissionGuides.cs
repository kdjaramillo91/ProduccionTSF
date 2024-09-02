using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class PendingPurchaseOrdersAndRemissionGuides
    {
        public int id { get; set; }//representa un identificador en el conjunto de Resultado de Orden y Guis Pendiente de Recepcionar

        public int id_purchaseOrderDetail { get; set; }
        public PurchaseOrderDetail purchaseOrderDetail { get; set; }

        public int? id_remissionGuideDetail { get; set; }
        public RemissionGuideDetail remissionGuideDetail { get; set; }

        public string metricUnit { get; set; }
        public decimal quantityPendingOrder { get; set; }
        public decimal quantityPendingGuide { get; set; }

        public int id_provider { get; set; }
        public Provider provider { get; set; }

        public int id_buyer { get; set; }
        public Person buyer { get; set; }

        public bool withPrice { get; set; }
    }
}