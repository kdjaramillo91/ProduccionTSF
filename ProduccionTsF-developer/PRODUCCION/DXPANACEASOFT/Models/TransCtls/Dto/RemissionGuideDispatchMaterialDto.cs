using System;

namespace DXPANACEASOFT.Models.Dto
{
    public class RemissionGuideDispatchMaterialDto
    {
        public int id { get; set; }
        public int id_item { get; set; }
        public int id_remisionGuide { get; set; }
        public decimal quantityRequiredForPurchase { get; set; }
        public decimal sourceExitQuantity { get; set; }
        public decimal sendedDestinationQuantity { get; set; }
        public decimal arrivalDestinationQuantity { get; set; }
        public decimal arrivalGoodCondition { get; set; }
        public decimal arrivalBadCondition { get; set; }
        public bool manual { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<int> id_warehouse { get; set; }
        public Nullable<int> id_warehouselocation { get; set; }
        public decimal amountConsumed { get; set; }
        public decimal sendedAdjustmentQuantity { get; set; }
        public decimal stealQuantity { get; set; }
        public decimal transferQuantity { get; set; }
    }

}