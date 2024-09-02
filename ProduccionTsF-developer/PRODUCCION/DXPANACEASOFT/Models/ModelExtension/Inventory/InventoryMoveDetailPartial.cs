using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class InventoryMoveDetail
    {
        public bool isNew { get; set; }
        public bool toRemove { get; set; }
        public decimal QuantityPending { get; set; }
        
        public string SecureInternalNumber 
        { 
            get 
            {
                string _secureInternalNumber = string.Empty;
                Lot _lot = this.Lot;
                if (_lot != null)
                {
                    if (!string.IsNullOrWhiteSpace(_lot.internalNumber) )
                    {
                        _secureInternalNumber = _lot.internalNumber + " / " + _lot.number;
                    }
                    else
                    {
                        ProductionLot _productionLot = _lot.ProductionLot;
                        if (_productionLot != null)
                        {
                            if (!string.IsNullOrWhiteSpace(_productionLot.internalNumber) )
                            {
                                _secureInternalNumber = _productionLot.internalNumber + " / " + _productionLot.number;
                            }
                        }
                    }
                }
                    return _secureInternalNumber;

            } }
    }
}