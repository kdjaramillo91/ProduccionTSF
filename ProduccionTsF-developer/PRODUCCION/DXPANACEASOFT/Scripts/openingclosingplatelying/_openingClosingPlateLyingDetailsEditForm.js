

// EDITOR'S EVENTS

function UpdateTitlePanel() {
    var gv = gvOpeningClosingPlateLyingEditFormDetail;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    $("#lblInfoDetails").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsDetails", gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionDetails", gv.GetSelectedRowCount() > 0);
    }
}

function GetSelectedFilteredRowCount() {
    return gvOpeningClosingPlateLyingEditFormDetail.cpFilteredRowCountWithoutPage + gvOpeningClosingPlateLyingEditFormDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewInitDetail(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChangedDetail(s, e) {
    UpdateTitlePanel();

    gvOpeningClosingPlateLyingEditFormDetail.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);

}

function GetSelectedFieldValuesCallbackDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallbackDetail(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallbackDetail(s, e) {
    UpdateTitlePanel();

}

function gvEditClearSelectionDetail() {
    gvOpeningClosingPlateLyingEditFormDetail.UnselectRows();
}

function gvEditSelectAllRowsDetail() {
    gvOpeningClosingPlateLyingEditFormDetail.SelectRows();
}

