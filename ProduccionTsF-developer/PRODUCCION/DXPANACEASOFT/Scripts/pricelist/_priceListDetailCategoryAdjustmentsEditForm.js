
//Validations
var maxValueAdjustment = 0;

function OnAdjustmentValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    else {
        var adjustmentAux = e.value === null ? 0 : e.value;
        var strMaxValueAdjustmentAux = maxValueAdjustment == 0 ? "0.00" : "-$" + maxValueAdjustment.toString();
        var valueAux = maxValueAdjustment + adjustmentAux;
        if (valueAux < 0) {
            e.isValid = false;
            e.errorText = "El valor de Ajuste no puede ser menor que (" + strMaxValueAdjustmentAux + ")";
        };
    }
}

// EDITOR'S EVENTS

function OnAdjustmentInit(s, e) {
    
    $.ajax({
        url: "PriceList/InitAdjustment",
        type: "post",
        data: null,
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
                maxValueAdjustment = result.maxValueAdjustment;
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS
//var customCommand = "";

//function PriceListDetailsOnGridViewInit(s, e) {
//    PriceListDetailsUpdateTitlePanel();
//    s.PerformCallback();
//    //gvPriceListDetails.PerformCallback();
//    //console.log("Se Ejecuto PriceListDetailsOnGridViewInit");
//}

//function PriceListDetailsOnGridViewSelectionChanged(s, e) {
//    PriceListDetailsUpdateTitlePanel();
//    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackItemsDetail);
//}

//function PriceListDetailsOnGridViewBeginCallback(s, e) {
//    customCommand = e.command;
//    PriceListDetailsUpdateTitlePanel();
//}

//function PriceListDetailsOnGridViewEndCallback(s, e) {
//    PriceListDetailsUpdateTitlePanel();
//    if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
//        s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
//    }
//}

//function PriceListDetailsSelectAllRows() {
//    gvPriceListDetails.SelectRows();
//}

//function PriceListDetailsClearSelection() {
//    gvPriceListDetails.UnselectRows();
//}

//function PriceListDetailsUpdateTitlePanel() {
//    var selectedFilteredRowCount = PriceListDetailsGetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gvPriceListDetails.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gvPriceListDetails.GetSelectedRowCount() - PriceListDetailsGetSelectedFilteredRowCount();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";
//    $("#lblInfo").html(text);

//    //if ($("#selectAllMode").val() != "AllPages") {
//    SetElementVisibility("lnkSelectAllRows", gvPriceListDetails.GetSelectedRowCount() > 0 && gvPriceListDetails.cpVisibleRowCount > selectedFilteredRowCount);
//    SetElementVisibility("lnkClearSelection", gvPriceListDetails.GetSelectedRowCount() > 0);
//    //}
//    //var columns = gvPriceListDetails.columns;
//    ////console.log("columns: ");
//    ////console.log(columns);
//    ////console.log("columns['1']: ");
//    ////console.log(columns[1]);
//    //columns[1].Visible = (false);
//    //btnRemoveDetail.SetEnabled(gvPriceListDetails.GetSelectedRowCount() > 0);
//}

//function SetElementVisibility(id, visible) {
//    var $element = $("#" + id);
//    visible ? $element.show() : $element.hide();
//}

//function PriceListDetailsGetSelectedFilteredRowCount() {
//    return gvPriceListDetails.cpFilteredRowCountWithoutPage + gvPriceListDetails.GetSelectedKeysOnPage().length;
//}

//function GetSelectedFieldValuesCallbackItemsDetail(values) {
//    var selectedRows = [];
//    for (var i = 0; i < values.length; i++) {
//        selectedRows.push(values[i]);
//    }
//}




function init() {
    //var id_priceListAux = $("#id_priceListAux").val();
    //var codeState = $("#codeState").val();
    //if (codeState == "07")//PENDIENTE DE PAGO
    //{
    //    UpdateProductionLotPaymentDetails(id_priceListAux);
    //}
    //var columns = gvPriceListDetails.Columns;
    //console.log(columns);
    //col0.SetColVisible(false);
    //try{
    //    gvPriceListDetails.UnselectRows();
    //    gvPriceListDetails.PerformCallback();
    //}
    //catch(e){
    //    //No se puede refrescar aun
    //}
}

$(function () {

    init();
});