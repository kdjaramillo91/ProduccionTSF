
// VALIDATIONSid_customer
//var Message = "Ok";

function OnNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabHeader");
}

function OnPriceListCalendarPriceListValidation(s, e) {
    //var codeDocumentType = $("#codeDocumentType").val();
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabHeader");
}

function ComboBoxCalendarPriceList_Init(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    s.SetEnabled(codeDocumentType != "19" && codeDocumentType != "21");
}

function ComboBoxCalendarPriceList_SelectedIndexChanged(s, e) {

    var data = {
        id_calendarPriceList: s.GetValue()
    };

    $.ajax({
        url: "PriceList/GetDatesOfCalendar",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateStartEndDate(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //if (result !== null && result !== undefined) {
            UpdateStartEndDate(result);
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

function UpdateStartEndDate(result) {
    if (result !== null && result !== undefined) {
        //UpdateTabHeader
        //startDate.SetDate(result.startDate);
        //endDate.SetDate(result.endDate);
        //id_calendarPriceList.SetSelectedItem(result.id_calendarPriceList);
        ////UpdateTabDetails
        //list_idInventaryLineFilter.SetValue(result.list_idInventaryLineFilter);
        //list_idItemTypeFilter.SetValue(result.list_idItemTypeFilter);
        //list_idItemGroupFilter.SetValue(result.list_idItemGroupFilter);
        //list_filterShow.SetValue(result.list_filterShow);
        //list_idInventaryLineFilter.ClearTokenCollection();
    } else {
        //UpdateTabHeader
        //startDate.SetDate(null);
        //endDate.SetDate(null);
        //id_calendarPriceList.SetSelectedItem(null);
        ////UpdateTabDetails
        //list_idInventaryLineFilter.ClearTokenCollection();
        //list_idItemTypeFilter.ClearTokenCollection();
        //list_idItemGroupFilter.ClearTokenCollection();
        //list_filterShow.ClearTokenCollection();
        //list_idInventaryLineFilter.ClearTokenCollection();
    }
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