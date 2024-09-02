

// EDITOR'S EVENTS

function UpdateTitlePanel() {
    var gv = gvProductionLotDailyCloseEditFormProductionLotsDetail;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    $("#lblInfoProductionLots").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsProductionLots", gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionProductionLots", gv.GetSelectedRowCount() > 0);
    }
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotDailyCloseEditFormProductionLotsDetail.cpFilteredRowCountWithoutPage + gvProductionLotDailyCloseEditFormProductionLotsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewInitProductionLotDetail(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChangedProductionLotDetail(s, e) {
    UpdateTitlePanel();

    gvProductionLotDailyCloseEditFormProductionLotsDetail.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackProductionLotDetail);

}

function GetSelectedFieldValuesCallbackProductionLotDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallbackProductionLotDetail(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallbackProductionLotDetail(s, e) {
    UpdateTitlePanel();

}

function gvEditClearSelectionProductionLot() {
    gvProductionLotDailyCloseEditFormProductionLotsDetail.UnselectRows();
}

function gvEditSelectAllRowsProductionLot() {
    gvProductionLotDailyCloseEditFormProductionLotsDetail.SelectRows();
}

