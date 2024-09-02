
// VALIDATIONSid_customer
//var Message = "Ok";

function OnPriceListCustomerValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "21") {//COTIZACIÓN - VENTA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabSale");
}

function OnPriceListGroupCustomerValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "21") {//COTIZACIÓN - VENTA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabSale");
}

function PriceListByGroupCustomer_CheckedChanged(s, e) {

    if (s.GetChecked()) {
        id_groupPersonByRolCustomer.SetEnabled(true);
        id_customer.SetValue(null);
        id_customer.SetEnabled(false);

    } else {

        id_groupPersonByRolCustomer.SetEnabled(false);
        id_groupPersonByRolCustomer.SetValue(null);
        id_customer.SetEnabled(true);

    }
}

function OnPriceListPriceListBaseSaleValidation(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null && codeDocumentType == "21") {//COTIZACIÓN - VENTA
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabSale");

}
function UpdateEnabled(enabled) {
    id_calendarPriceList.SetEnabled(enabled);
    inventoryLines.SetEnabled(enabled);
    itemTypes.SetEnabled(enabled);
    itemGroups.SetEnabled(enabled);
    filterShows.SetEnabled(enabled);
}
function ComboPriceListBaseSale_SelectedIndexChanged(s, e) {

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
            UpdateTabHeaderAndTabDetailsSale(null);
            UpdateEnabled(true);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //if (result !== null && result !== undefined) {
            UpdateTabHeaderAndTabDetailsSale(result);
            //}
            //else {
            //    UpdateTabPurchaseAndTabSale("");
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function UpdateTabHeaderAndTabDetailsSale(result) {
    if (result !== null && result !== undefined) {
        //UpdateTabHeader
        //startDate.SetDate(result.startDate);
        //endDate.SetDate(result.endDate);
        id_calendarPriceList.SetValue(result.id_calendarPriceList == 0 ? null : result.id_calendarPriceList);
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
        //list_idInventaryLineFilter.ClearTokenCollection();
        UpdateEnabled(true);

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