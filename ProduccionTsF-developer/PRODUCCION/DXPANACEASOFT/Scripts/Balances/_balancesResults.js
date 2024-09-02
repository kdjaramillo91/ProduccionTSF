
// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

function Print(s, e) {
    $.ajax({
        url: "Kardex/KardexReport",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {

}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvKardexDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvKardexDetails.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvKardexDetails.GetSelectedRowCount() > 0 && gvKardexDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvKardexDetails.GetSelectedRowCount() > 0);
    //}

}

function GetSelectedFilteredRowCount() {
    return gvKardexDetails.cpFilteredRowCountWithoutPage + gvKardexDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvKardexDetails.SelectRows();
}

function UnSelectRows() {
    gvKardexDetails.UnselectRows();
}

//function OnClickEditInventoryMove(s, e) {
//    var data = {
//        id: gvKardexDetails.GetRowKey(e.visibleIndex)
//    };
//    showPage("InventoryMove/InventoryMoveEditFormPartial", data);
//}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});