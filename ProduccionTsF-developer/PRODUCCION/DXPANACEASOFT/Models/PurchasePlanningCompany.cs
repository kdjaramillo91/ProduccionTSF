using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class PurchasePlanningCompany
    {
        public string number { get; set; }
        public string state { get; set; }
        public string emissionDate { get; set; }
        public string period { get; set; }
        public string personPlanning { get; set; }
        public string description { get; set; }
        public List<int> list_id_purchasePlanning { get; set; }
        public List<PurchasePlanningDetailReport> listPurchasePlanningDetail { get; set; }
        public Company company { get; set; }

    }
}