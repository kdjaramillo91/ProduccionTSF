
// VALIDATIONS
var Message = "Ok";

function OnPriceListValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        //UpdateProductionLotPaymentDetails(s.GetValue());
        if (Message != "Ok") {
            e.isValid = false;
            e.errorText = Message;
        }
    }
}

function UpdateProductionLotPriceLists(priceLists) {

    for (var i = 0; i < id_priceList.GetItemCount(); i++) {
        var priceList = id_priceList.GetItem(i);
        var into = false;
        for (var j = 0; j < priceLists.length; j++) {
            if (priceList.value === priceLists[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_priceList.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < priceLists.length; i++) {
        var priceList = id_priceList.FindItemByValue(priceLists[i].id);
        if (priceList == null) id_priceList.AddItem(priceLists[i].name, priceLists[i].id);
    }

}

function ComboPriceList_Init(s, e) {
    var data = {
        id_priceList: s.GetValue(),
        id_provider: id_provider.GetValue()
    };

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionPriceListData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                UpdateProductionLotPriceLists(result.priceLists);
            }
            else {
                id_priceList.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}
function OnPriceListInit(s, e) {
    if (s.GetValue() != null) {
            //UpdateProductionLotPaymentDetails(s.GetValue(), true);
        var codeState = $("#codeState").val();
        if (codeState === "06" || codeState === "07")//06-CERRADO ó 07-PENDIENTE DE PAGO
        {
            UpdateProductionLotPaymentDetails(s.GetValue(), true);
        }
    }
}


function OnCopackingTariffInit(s, e) {
    if (s.GetValue() != null) {
        //UpdateProductionLotPaymentDetails(s.GetValue(), true);
        var codeState = $("#codeState").val();
        if (codeState === "06" || codeState === "07")//06-CERRADO ó 07-PENDIENTE DE PAGO
        {
            UpdateProductionLotPaymentDetails(id_priceList.GetValue(), true);
        }
    }
}

function ComboPriceList_SelectedIndexChanged(s, e) {
    UpdateProductionLotPaymentDetails(s.GetValue(), true);
}

function ComboCopackingTariff_SelectedIndexChanged(s, e) {
    UpdateProductionLotPaymentDetails(s.GetValue(), true);
}

function ProductionLotPriceListPayment_BeginCallback(s, e) {
    // 
    e.customArgs["liquidationPayDateYear"] = liquidationPaymentDate.GetDate().getFullYear();
    e.customArgs["liquidationPayDateMonth"] = liquidationPaymentDate.GetDate().getMonth();
    e.customArgs["liquidationPayDateDay"] = liquidationPaymentDate.GetDate().getDate();
}
function OnLiquidationPayDateChanged(s, e) {
    id_priceList.PerformCallback();
}

function UpdateProductionLotPaymentDetails(id_priceList, updatePrice) {
    var data = {
        id_priceList: id_priceList
        //updatePrice: updatePrice
    };
    $.ajax({
        url: "ProductionLotReception/UpdateProductionLotPaymentDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {

                //console.log(result);
                Message = result.Message;

                if (updatePrice) {
                    gvProductionLotReceptionEditFormPaymentsDetail.PerformCallback();
                }
                //gvProductionLotReceptionEditFormPaymentsDetail.UnselectRows();

                //totalQuantityRecivedNet.SetValue(result.totalQuantityRecivedNet);
                //totalQuantityLiquidationAdjust.SetValue(result.totalQuantityLiquidationAdjust);
                //performance.SetValue(result.performance);


                //Message = Message,


            }
        },
        complete: function () {
            //hideLoading();
        }
    });

}
function OnGridViewSummaryPaymentDetailsInit(s, e) {
    gvProductionLotReceptionSummaryPaymentsDetail.PerformCallback();
}
function init() {
    var id_priceListAux = $("#id_priceListAux").val();
    var codeState = $("#codeState").val();
    if (codeState === "06" || codeState === "07")//06-CERRADO ó 07-PENDIENTE DE PAGO
    {
        UpdateProductionLotPaymentDetails(id_priceListAux, false);
    }

}



$(function () {

    init();
});