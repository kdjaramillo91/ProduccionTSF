
// VALIDATIONS
//var Message = "Ok";

function OnPriceListProviderValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "19") {//COTIZACIÓN - COMPRA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabPurchase");

}

function OnPriceListGroupProviderValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "19") {//COTIZACIÓN - COMPRA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabPurchase");

}

function OnPriceListPriceListBasePurchaseValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "19") {//COTIZACIÓN - COMPRA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabPurchase");

}

function PriceListByGroupProvider_CheckedChanged(s, e) {

    if (s.GetChecked()) {
        id_groupPersonByRolProvider.SetEnabled(true);
        id_provider.SetValue(null);
        id_provider.SetEnabled(false);
        //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

        //if (!valid) {
        //    UpdateTabImage({ isValid: false }, "tabPurchaseOrder");
        //}


    } else {

        id_groupPersonByRolProvider.SetEnabled(false);
        id_groupPersonByRolProvider.SetValue(null);
        id_provider.SetEnabled(true);

    }
}

//function UpdateProductionLotPriceLists(priceLists) {

//    for (var i = 0; i < id_priceList.GetItemCount() ; i++) {
//        var priceList = id_priceList.GetItem(i);
//        var into = false;
//        for (var j = 0; j < priceLists.length; j++) {
//            if (priceList.value == priceLists[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_priceList.RemoveItem(i);
//            i -= 1;
//        }
//    }


//    for (var i = 0; i < priceLists.length; i++) {
//        var priceList = id_priceList.FindItemByValue(priceLists[i].id);
//        if (priceList == null) id_priceList.AddItem(priceLists[i].name, priceLists[i].id);
//    }

//}

//function ComboPriceList_Init(s, e) {
//    var data = {
//        id_priceList: s.GetValue(),
//        id_provider: id_provider.GetValue()
//    };

//    $.ajax({
//        url: "ProductionLotReception/ProductionLotReceptionPriceListData",
//        type: "post",
//        data: data,
//        async: false,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//            id_priceList.ClearItems();
//        },
//        beforeSend: function () {
//            //showLoading();
//        },
//        success: function (result) {
//            if (result !== null && result !== undefined) {
//                UpdateProductionLotPriceLists(result.priceLists);
//            }
//            else {
//                id_priceList.ClearItems();
//            }
//        },
//        complete: function () {
//            //hideLoading();
//        }
//    });
//}

function UpdateEnabled(enabled) {
    id_calendarPriceList.SetEnabled(enabled);
    inventoryLines.SetEnabled(enabled);
    itemTypes.SetEnabled(enabled);
    itemGroups.SetEnabled(enabled);
    filterShows.SetEnabled(enabled);
}

function ComboPriceListBasePurchase_SelectedIndexChanged(s, e) {
    var id_priceListBaseAux = s.GetValue();
    var data = {
        id_priceListBase: id_priceListBaseAux
    };

    var enabled = id_priceListBaseAux == 0 || id_priceListBaseAux == null || id_priceListBaseAux == "0";
    UpdateEnabled(enabled);

    $.ajax({
        url: "PriceList/UpdatePriceListDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateTabHeaderAndTabDetailsPurchase(null);
            UpdateEnabled(true);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //if (result !== null && result !== undefined) {
            UpdateTabHeaderAndTabDetailsPurchase(result);
            //}
            //else {
            //    UpdateTabPurchaseAndTabSale("");
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
    //gvProductionLotReceptionEditFormPaymentsDetail.UnselectRows();
    //gvProductionLotReceptionEditFormPaymentsDetail.PerformCallback();
    //UpdateProductionLotPaymentDetails(s.GetValue());
    //OnPriceListValidation(s, e);
    //console.log("s.GetValue() : ");
    //console.log(s.GetValue());
}

function UpdateTabHeaderAndTabDetailsPurchase(result) {
    if (result !== null && result !== undefined) {
        //UpdateTabHeader
        //startDate.SetDate(result.startDate);
        //endDate.SetDate(result.endDate);
        id_calendarPriceList.SetValue(result.id_calendarPriceList == 0 ? null : result.id_calendarPriceList);
        //console.log("result.id_calendarPriceList: ");
        //console.log(result.id_calendarPriceList);
        //UpdateTabDetails
        inventoryLines.SetValue(result.list_idInventaryLineFilter);
        itemTypes.SetValue(result.list_idItemTypeFilter);
        itemGroups.SetValue(result.list_idItemGroupFilter);
        filterShows.SetValue(result.list_filterShow);
        //list_idInventaryLineFilter.ClearTokenCollection();
    } else {
        //UpdateTabHeader
        //startDate.SetDate(null);
        //endDate.SetDate(null);
        id_calendarPriceList.SetValue(null);
        //UpdateTabDetails
        inventoryLines.ClearTokenCollection();
        itemTypes.ClearTokenCollection();
        itemGroups.ClearTokenCollection();
        filterShows.ClearTokenCollection();
        UpdateEnabled(true);
        //list_idInventaryLineFilter.ClearTokenCollection();
    }
    gvPriceListDetails.UnselectRows();
    gvPriceListDetails.PerformCallback();
}

function init() {
    //var id_priceListAux = $("#id_priceListAux").val();
    //var codeState = $("#codeState").val();
    //if (codeState == "07")//PENDIENTE DE PAGO
    //{
    //    UpdateProductionLotPaymentDetails(id_priceListAux);
    //}
    
}

$(function () {

    init();
});