
function GenerateInventoryMoveEntry(s, e) {
    var data = {
        id: 0,
        code: "04",//Ingreso x Orden de Compra
        ordersDetails: []
    };
    for (var i = 0; i < selectedPurchaseOrdersDetailsRows.length; i++) {
        data.ordersDetails.push(selectedPurchaseOrdersDetailsRows[i]);
    }

    showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
}

// SELECTIONS

function OnGridViewPurchaseOrdersDetailsInit(s, e) {
    UpdateTitlePanelOrdersDetails();
}

function OnGridViewPurchaseOrdersDetailsSelectionChanged(s, e) {
    UpdateTitlePanelOrdersDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

var selectedPurchaseOrdersDetailsRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
    selectedPurchaseOrdersDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedPurchaseOrdersDetailsRows.push(values[i]);
    }
}

function OnGridViewPurchaseOrdersDetailsEndCallback(s, e) {
    UpdateTitlePanelOrdersDetails();
}

function UpdateTitlePanelOrdersDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredOrdersDetailsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrdersDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrdersDetails.GetSelectedRowCount() - GetSelectedFilteredOrdersDetailsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    console.log(gvPurchaseOrdersDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrdersDetails.GetSelectedRowCount() > 0 && gvPurchaseOrdersDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrdersDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateMove.SetEnabled(gvPurchaseOrdersDetails.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrdersDetailsRowCount() {
    return gvPurchaseOrdersDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrdersDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewPurchaseOrdersDetailsClearSelection() {
    gvPurchaseOrdersDetails.UnselectRows();
}

function GridViewPurchaseOrdersDetailsSelectAllRow() {
    gvPurchaseOrdersDetails.SelectRows();
}

function inti() {
    
}

$(function () {

});